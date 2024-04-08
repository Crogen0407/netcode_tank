using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    private JoinAllocation _allocation;
    private string _playerName;
    public string PlayerName => _playerName;
    
    public async Task<bool> InitAsync()
    {
        //UGS 서비스 인증파트가 들어갈 예정입니다.
        await UnityServices.InitializeAsync(); //초기화

        AuthState authState = await UGSAuthWrapper.DoAuth(); //인증이 5회 진행될거고

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GotoMenuScene()
    {
        SceneManager.LoadScene(SceneNames.MenuScene);
    }

    public async Task StartClientWithJoinCode(string joinCode)
    {
        try
        {
            _allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            var relayServerData = new RelayServerData(_allocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
    }
    
    public bool StartClientLocalNetwork()
    {
        return NetworkManager.Singleton.StartClient();
    }

    public void SetPlayerName(string text)
    {
        _playerName = text;
    }
}
