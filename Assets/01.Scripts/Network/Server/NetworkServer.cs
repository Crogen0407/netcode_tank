using System.Text;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer
{
    public NetworkManager _networkManager;

    public NetworkServer(NetworkManager manager)
    {
        _networkManager = manager;
        _networkManager.ConnectionApprovalCallback += HandleApprovalCheck;
    }

    private void HandleApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request, 
        NetworkManager.ConnectionApprovalResponse response)
    {
        string json = Encoding.UTF8.GetString(request.Payload);
        UserData data = JsonUtility.FromJson<UserData>(json);
        
        Debug.Log(data.username);

        response.CreatePlayerObject = false;
        response.Approved = true;
    }
}
