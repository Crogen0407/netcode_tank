using Unity.Netcode;
using UnityEngine.SceneManagement;

public class NetworkClient
{
    private NetworkManager _networkManager;

    public NetworkClient(NetworkManager manager)
    {
        _networkManager = manager;
        _networkManager.OnClientDisconnectCallback += HandleClientDisconnect;
    }

    private void HandleClientDisconnect(ulong clientID)
    {
        if (clientID != 0 && clientID != _networkManager.LocalClientId) return;
        
        Disconnect();
    }

    public void Disconnect()
    {
        if (SceneManager.GetActiveScene().name != SceneNames.MenuScene)
        {
            SceneManager.LoadScene(SceneNames.MenuScene);
        }

        if (_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown(); //강제 종료
        }
    }
}
