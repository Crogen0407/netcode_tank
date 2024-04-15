using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    public NetworkManager _networkManager;

    //클라이언트 아이디로 Auth 아이디 알아내는 것
    private Dictionary<ulong, string> _clientToAuthDictionary = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _authToUserDictionary = new Dictionary<string, UserData>();    
    
    public NetworkServer(NetworkManager manager)
    {
        _networkManager = manager;
        _networkManager.ConnectionApprovalCallback += HandleApprovalCheck;

        _networkManager.OnServerStarted += HandleServerStart;
    }

    private void HandleServerStart()
    {
        _networkManager.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void HandleClientDisconnect(ulong clientID)
    {
        //접속 끊으면 2개의 딕셔너리 모두 삭제한다.
        if (_clientToAuthDictionary.TryGetValue(clientID, out string authID))
        {
            _clientToAuthDictionary.Remove(clientID);
            _authToUserDictionary.Remove(authID);
        }
    }

    private void HandleApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        string json = Encoding.UTF8.GetString(request.Payload);
        UserData data = JsonUtility.FromJson<UserData>(json);

        _clientToAuthDictionary[request.ClientNetworkId] = data.userAuthID;
        _authToUserDictionary[data.userAuthID] = data;

        response.CreatePlayerObject = false;
        response.Approved = true;

        HostSingleton.Instance.StartCoroutine(CreatePanelWithDelay(0.5f, request.ClientNetworkId, data.username));
    }

    private IEnumerator CreatePanelWithDelay(float time, ulong clientID, string username)
    {
        yield return new WaitForSeconds(time);
        GameManager.Instance.CreateUIPanel(clientID, username);
    }
    
    public void Dispose()
    {
        if (_networkManager == null) return;

        _networkManager.ConnectionApprovalCallback -= HandleApprovalCheck;
        _networkManager.OnServerStarted -= HandleServerStart;
        _networkManager.OnClientDisconnectCallback -= HandleClientDisconnect;

        if (_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }

    public UserData GetUseDataByClientID(ulong clientID)
    {
        if (_clientToAuthDictionary.TryGetValue(clientID, out string authID))
        {
            if (_authToUserDictionary.TryGetValue(authID, out UserData data))
            {
                return data;
            }
        }
        return null;
    }
}
