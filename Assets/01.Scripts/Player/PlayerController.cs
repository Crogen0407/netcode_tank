using Cinemachine;
using System;
using System.Net.NetworkInformation;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{

    public static event Action<PlayerController> OnPlayerSpawn;
    public static event Action<PlayerController> OnPlayerDespawn;


    [Header("Reference")]
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private TextMeshPro _nameText;
    [SerializeField] private SpriteRenderer _minimapIcon;

    [Header("Setting Values")]
    [SerializeField] private int _ownerCamPriority = 15;
    [SerializeField] private Color _ownerColor;

    //��ũ �ε���
    public NetworkVariable<Color> tankColor;

    public PlayerVisual VisualCompo { get; private set; }
    public PlayerMovement MovementCompo { get; private set; }
    public Health HealthCompo { get; private set; }
    public CoinCollector CoinCompo { get; private set; }

    public NetworkVariable<FixedString32Bytes> playerName;

    private void Awake()
    {
        tankColor = new NetworkVariable<Color>();
        playerName = new NetworkVariable<FixedString32Bytes>();

        VisualCompo = GetComponent<PlayerVisual>();
        MovementCompo = GetComponent<PlayerMovement>();
        HealthCompo = GetComponent<Health>();

        CoinCompo = GetComponent<CoinCollector>();
    }

    public override void OnNetworkSpawn()
    {
        tankColor.OnValueChanged += HandleColorChanged;
        playerName.OnValueChanged += HandleNameChanged;
        if (IsOwner)
        {
            _followCam.Priority = _ownerCamPriority;
            _minimapIcon.color = _ownerColor;
        }

        if(IsServer)
        {
            UserData data = HostSingleton.Instance.GameManager
                                .NetServer.GetUserDataByClientID(OwnerClientId);
            playerName.Value = data.username;
        }
        HandleNameChanged(string.Empty, playerName.Value); //���� �ε��ÿ��� ����ȭ�ϰ�
        OnPlayerSpawn?.Invoke(this); //������ �̺�Ʈ�� �����Ѵ�.
    }

    public override void OnNetworkDespawn()
    {
        tankColor.OnValueChanged -= HandleColorChanged;
        playerName.OnValueChanged -= HandleNameChanged;

        OnPlayerDespawn?.Invoke(this);
    }

    private void HandleNameChanged(FixedString32Bytes previousValue, FixedString32Bytes newValue)
    {
        _nameText.text = newValue.ToString();
    }

    private void HandleColorChanged(Color previousValue, Color newValue)
    {
        VisualCompo.SetTintColor(newValue);
    }

    #region Only Server execution area
    //���߿� SetTankData�� �����Ҳ���.
    public void SetTankData(Color color, int coin)
    {
        tankColor.Value = color;
        CoinCompo.totalCoin.Value = coin;
    }
    #endregion
}
