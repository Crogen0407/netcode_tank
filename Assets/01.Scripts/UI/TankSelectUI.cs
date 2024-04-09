using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TankSelectUI : NetworkBehaviour
{
    [SerializeField] private Image _tankImage;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Button[] _colorButtons;
    [SerializeField] private Button _readyBtn;
    [SerializeField] private Image _statusImage;


    [SerializeField] private Sprite _readySprite, _notReadySprite, _readyBtnSprite, _notReadyBtnSprite;

    public bool isReady;
    public Color selectedColor;

    private TextMeshProUGUI _readyBtnText;
    private Image _readyBtnAttachedImage;
    private NetworkVariable<FixedString32Bytes> playerName;

    private void Awake()
    {
        _readyBtnText = _readyBtn.GetComponentInChildren<TextMeshProUGUI>();
        
        playerName = new NetworkVariable<FixedString32Bytes>();

        playerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes previousvalue, FixedString32Bytes newvalue)
    {
        _nameText.text = newvalue.ToString();
    }

    public override void OnNetworkSpawn()
    {
        isReady = false;
        SetReadyStatusVisual();
        
        if (IsOwner == false) return;
        _readyBtn.onClick.AddListener(HandleReadyBtnClick);

        foreach (Button button in _colorButtons)
        {
            button.onClick.AddListener(() =>
            {
                SetTankColor(button.image.color);
            });
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner == false) return;
        _readyBtn.onClick.RemoveListener(HandleReadyBtnClick);
        foreach (Button button in _colorButtons)
        {
            button.onClick.RemoveAllListeners();
        }
    }

    private void SetTankColor(Color color)
    {
        
    }

    private void SetReadyStatusVisual()
    {
        if (isReady)
        {
            _statusImage.sprite = _readySprite;
            _readyBtn.image.sprite = _readyBtnSprite;
            _readyBtnText.text = "준비 완료";
        }
        else
        {
            _statusImage.sprite = _notReadySprite;
            _readyBtn.image.sprite = _notReadyBtnSprite;
            _readyBtnText.text = "준비";
        }
    }

    #region Only Owner execution area

    private void HandleReadyBtnClick()
    {
        isReady = !isReady;
        SetReadyStatusVisual();
        //서버에게 내가 변경되었음을 알려줘야 하겠지
        SetReadyClaimToServerRpc(isReady);
    }

    #endregion
    
    #region Only server excution area

    public void SetTankName(string name)
    {
        playerName.Value = name;
    }
    
    [ServerRpc]
    private void SetReadyClaimToServerRpc(bool value)
    {
        isReady = value; //이때 isReady는
        SetReadyClientRpc(isReady);
    }
    #endregion

    #region Only Client execution area
    [ClientRpc]
    private void SetReadyClientRpc(bool value)
    {
        if (IsOwner) return;
        isReady = value;
        SetReadyStatusVisual();
    }
    #endregion
}