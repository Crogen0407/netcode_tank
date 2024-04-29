using UnityEngine;

public class DealDamageOnContact : MonoBehaviour
{
    [SerializeField] private int _damage = 10;

    public void SetDamage(int damage)
    {
        _damage = damage;
    }

    //여기다가 나랑 부딛혔을 때 해당 오브젝트의 Health를 받아다가
    //있으면 TakeDamage를 호출하게 해보자.

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody is null) return;


        if (collision.attachedRigidbody.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(_damage);
        }
    }
}
