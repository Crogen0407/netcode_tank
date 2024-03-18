using Unity.Netcode;
using UnityEngine;
 
public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _turretTrm;

    private void LateUpdate()
    {
        if (!IsOwner) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(_playerInput.AimPosition);

        Vector2 direction = (mousePos - (Vector2)_turretTrm.position).normalized;
        
        _turretTrm.up = direction;
    }
}
