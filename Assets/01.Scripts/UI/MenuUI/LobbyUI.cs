using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _countText;
    [SerializeField] private Button _enterBtn;

    private RectTransform _rectTrm;
    public RectTransform Rect => _rectTrm;

    private Lobby _lobby;

    public void SetRoomTemplate(Lobby lobby)
    {
        _titleText.text = lobby.Name;
        _countText.text = $"{lobby.Players.Count} / {lobby.MaxPlayers}";
        _lobby = lobby;
    }
    
    private void Awake()
    {
        _rectTrm = GetComponent<RectTransform>();
    }
}