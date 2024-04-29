using TMPro;
using UnityEngine;

public class JoinCodeDisplay : MonoBehaviour
{
    private TextMeshProUGUI _joinCode;

    private void Awake()
    {
        _joinCode = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (HostSingleton.Instance == null) return;
        _joinCode.text = HostSingleton.Instance.GameManager.JoinCode;
    }
}
