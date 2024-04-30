using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool isServerProjectile;

    [SerializeField] private int _damage = 10;
    [SerializeField] private GameObject _destroyObj;
    [SerializeField] private float _lifetime;

    private float _currentLifetime;
    private Rigidbody2D _rb2DCompo;
    private Collider2D _collider;

    private void Awake()
    {
        _rb2DCompo = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    public void SetUpProjectile(Collider2D playerCollider, Vector3 dir, float speed, int damage)
    {
        transform.up = dir;
        Physics2D.IgnoreCollision(playerCollider, _collider, true);
        _rb2DCompo.velocity = transform.up * speed;
        _damage = damage;
    }

    private void Update()
    {
        _currentLifetime += Time.deltaTime;
        if (_currentLifetime >= _lifetime)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody is null) return;

        if(isServerProjectile) //서버 발사체만 데미지
        {
            if(collision.attachedRigidbody.TryGetComponent(out Health health))
            {
                health.TakeDamage(_damage);
            }
        }

        if(_destroyObj != null) //파괴 파티클이 존재하면 재생
        {
            Instantiate(_destroyObj, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
}