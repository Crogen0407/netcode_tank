using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UGSConnectPanel : MonoBehaviour
{
    [SerializeField] private Button _relayHostBtn;
    [SerializeField] private Button _enterLobbyBtn;

    public UnityEvent OpenLobbyEvent;
    
    private void Awake()
    {
        _relayHostBtn.onClick.AddListener(HandleRelayHostClick);
        _enterLobbyBtn.onClick.AddListener(HandleOpenLobbyClick);
    }

    private void HandleOpenLobbyClick()
    {
        OpenLobbyEvent?.Invoke();
    }
    
    private async void HandleRelayHostClick()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }
}
