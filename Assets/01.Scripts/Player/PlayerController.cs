using Cinemachine;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [Header("Reference")] 
    [SerializeField] private CinemachineVirtualCamera _followCam;
    [SerializeField] private TextMeshPro _nameText;

    [Header("Setting Values")] 
    [SerializeField] private int _ownerCamPriority = 15;
    
    
    //탱그 인덱스
    public NetworkVariable<Color> tankColor;
    
    public PlayerVisual VisualCompo { get; private set; }
    public PlayerMovement MovementCompo { get; private set; }
    public NetworkVariable<FixedString32Bytes> playerName;
    
    private void Awake()
    {
        tankColor = new NetworkVariable<Color>();
        playerName = new NetworkVariable<FixedString32Bytes>();
            
        VisualCompo = GetComponent<PlayerVisual>();
        MovementCompo = GetComponent<PlayerMovement>();
    }

    public override void OnNetworkSpawn()
    {
        tankColor.OnValueChanged += HandleColorChanged;
        playerName.OnValueChanged += HandleUserNameChanged; 
        
        if (IsOwner)
        {
            _followCam.Priority = _ownerCamPriority;
        }

        if (IsServer)
        {
            UserData data = HostSingleton.Instance.GameManager.NetServer.GetUseDataByClientID(OwnerClientId);
            playerName.Value = data.username;
        }
        HandleUserNameChanged(string.Empty, playerName.Value); //최초 로딩시에도 동기화하게
    }

    private void HandleUserNameChanged(FixedString32Bytes previousvalue, FixedString32Bytes newvalue)
    {
        _nameText.text = newvalue.ToString();
    }


    public override void OnNetworkDespawn()
    {
        tankColor.OnValueChanged -= HandleColorChanged;
        playerName.OnValueChanged -= HandleUserNameChanged; 
    }

    private void HandleColorChanged(Color previousvalue, Color newvalue)
    {
        VisualCompo.SetTintColor(newvalue);
    }

    #region Only Server execution area
    //나중에 SetTankData로 변경할거야
    public void SetTankColor(Color color)
    {
        tankColor.Value = color;
    }
    #endregion
}
