using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager : IDisposable
{
    private Allocation _allocation;
    private const int _maxConnections = 8;
    private string _joinCode;
    public string JoinCode => _joinCode;
    private string _lobbyId; //내가 게임을 만들고, 로비를 만들건데 그 아이디

    public NetworkServer NetServer { get; private set; }

    private void MakeNetworkServer()
    {
        NetServer = new NetworkServer(NetworkManager.Singleton);
    }

    public async Task StartHostAsync()
    {
        try
        {
            _allocation = await Relay.Instance.CreateAllocationAsync(_maxConnections);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }

        try
        {
            _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(_joinCode);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return;
        }

        //실질적으로 서버를 켜야하는데
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        //tcp, udp, tls, dtls
        RelayServerData relayData = new RelayServerData(_allocation, "dtls");
        transport.SetRelayServerData(relayData);

        //로비 정보를 만들어준다.
        try
        {
            CreateLobbyOptions lobbyOption = new CreateLobbyOptions();
            lobbyOption.IsPrivate = false;
            lobbyOption.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode",
                    new DataObject(DataObject.VisibilityOptions.Member, _joinCode)
                }
            };

            string playerName = ClientSingleton.Instance.GameManager.PlayerName;

            Lobby lobby = await Lobbies.Instance.CreateLobbyAsync($"{playerName}'s Lobby", 
                                _maxConnections, lobbyOption);

            _lobbyId = lobby.Id; //만들어진 로비의 아이디를 기록한다.

            //15초간격으로 하트비트 핑 날리기
            HostSingleton.Instance.StartCoroutine(HeartBeatLobby(15));
        }catch(LobbyServiceException ex)
        {
            Debug.LogError(ex);
            return;
        }

        MakeNetworkServer();

        ClientSingleton.Instance.GameManager.SetPayloadData();

        NetServer.OnClientLeft += HandleClientLeft;

        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                SceneNames.GameScene, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    private async void HandleClientLeft(string authID)
    {
        if (_lobbyId == null) return;

        try
        {
            await LobbyService.Instance.RemovePlayerAsync(_lobbyId, authID);
        }catch(LobbyServiceException ex)
        {
            Debug.LogError(ex);
        }
    }

    private IEnumerator HeartBeatLobby(float waitTime)
    {
        var timer = new WaitForSecondsRealtime(waitTime);
        while(true)
        {
            Lobbies.Instance.SendHeartbeatPingAsync(_lobbyId);
            yield return timer;
        }
    }


    public bool StartHostLocalNetwork()
    {
        MakeNetworkServer();
        ClientSingleton.Instance.GameManager.SetPayloadData();
        //여기다 코드 옮겨적으면 된다.
        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(
                SceneNames.GameScene, LoadSceneMode.Single);
            return true;
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
            return false;
        }
        
    }

    public void Dispose()
    {
        Shutdown();
    }

    public async void Shutdown()
    {
        if(HostSingleton.Instance != null)
            HostSingleton.Instance.StopAllCoroutines(); //하트비트 꺼버리고

        if(!string.IsNullOrEmpty(_lobbyId))
        {
            try
            {
                await Lobbies.Instance.DeleteLobbyAsync(_lobbyId);
            }catch(LobbyServiceException ex)
            {
                Debug.LogError(ex);
            }
        }
        NetServer.OnClientLeft -= HandleClientLeft;
        _lobbyId = string.Empty;
        NetServer?.Dispose();
    }
       
}