using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private Allocation _allocation;
    private const int _maxConnections = 8;
    public string joinCode;
    
    public async Task StartHostAsync()
    {
        try
        {
            _allocation = await Relay.Instance.CreateAllocationAsync(_maxConnections);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }

        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return;
        }
        
        //실직적으로 서버를 켜야 하는데
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        //tcp, udp, dtcp, dtls
        RelayServerData relayData = new RelayServerData(_allocation, "dtls");
        transport.SetRelayServerData(relayData);

        if (NetworkManager.Singleton.StartHost())
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneNames.GameScene,
                UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    public bool StartHostLocalNetwork()
    {
        if(NetworkManager.Singleton.StartHost())
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
}