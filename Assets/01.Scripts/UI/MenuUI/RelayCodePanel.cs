using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class RelayCodePanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _relayText;
        
    void Awake()
    {
        if (!NetworkManager.Singleton.IsHost)
        {
            gameObject.SetActive(false);
            return;
        }

        _relayText.text = HostSingleton.Instance.GameManager.JoinCode.ToString();
    }
}
