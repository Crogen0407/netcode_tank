using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _firePosTrm;
    [SerializeField] private Projectile _serverProjectilePrefab;
    [SerializeField] private Projectile _clientProjectilePrefab;

    [SerializeField] private Collider2D _playerCollider;

    [Header("Setting values")]
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private float _fireCooltime;

    private float _prevFireTime;
    public int damage = 10;
    
    public UnityEvent OnFireEvent;
    
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _playerInput.OnFireEvent += HandleFireEvent;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        _playerInput.OnFireEvent -= HandleFireEvent;
    }

    private void HandleFireEvent()
    {
        if (Time.time < _prevFireTime + _fireCooltime) return;

        if (MapManager.Instance.IsInSafetyZone(transform.position))
        {
            TextManager.Instance.PopUpText("Safety zone!", transform.position, Color.white);
            return;
        }
        

        FireServerRpc(_firePosTrm.position, _firePosTrm.up);
        SpawnDummyProjectile(_firePosTrm.position, _firePosTrm.up);
        _prevFireTime = Time.time;
    }

    private void SpawnDummyProjectile(Vector3 spawnPos, Vector3 dir)
    {
        Projectile projectile = Instantiate(_clientProjectilePrefab, spawnPos, Quaternion.identity);

        projectile.SetUpProjectile(_playerCollider, dir, _projectileSpeed, damage);

        OnFireEvent?.Invoke();
    }

    
    [ServerRpc]
    private void FireServerRpc(Vector3 spawnPos, Vector3 dir)
    {
        Projectile projectile = Instantiate(_serverProjectilePrefab, spawnPos, Quaternion.identity);
        projectile.transform.up = dir;// 위방향으로 회전
        Collider2D projectileCollider = projectile.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(_playerCollider, projectileCollider);
        Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
        rigidbody.velocity = projectile.transform.up * _projectileSpeed;

        //만들어!
        SpawnDummyProjectileClientRpc(spawnPos, dir);
    }

    [ClientRpc]
    private void SpawnDummyProjectileClientRpc(Vector3 spawnPos, Vector3 dir)
    {
        if (IsOwner) return;

        SpawnDummyProjectile(spawnPos, dir);
    }
}
