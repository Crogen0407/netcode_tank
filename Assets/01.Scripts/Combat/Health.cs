using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System;

public class Health : NetworkBehaviour
{
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>();
    public int maxHealth;

    public event Action OnDieEvent;
    public event Action OnHealthChangedEvent;

    private bool _isDead;

    private void Awake()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        //주의사항2. NetworkVariable은 서버만 건드릴 수 있다. 클라는 값의 변경을 받기만
        if (IsClient)
        {
            currentHealth.OnValueChanged += HandleHealthValueChanged;
        }

        if (IsServer == false) return;
        currentHealth.Value = maxHealth; //처음 시작시 최대체력으로 넣어준다.
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            currentHealth.OnValueChanged -= HandleHealthValueChanged;
        }
    }

    public float GetNormalizedHealth()
    {
        return (float)currentHealth.Value / maxHealth;
    }
    
    private void HandleHealthValueChanged(int previousvalue, int newvalue)
    {
        OnHealthChangedEvent?.Invoke();
    }

    //이 녀석은 서버만 실행하는 매서드야
    private void ModifyHealth(int value)
    {
        if (_isDead) return;
        currentHealth.Value = Mathf.Clamp(currentHealth.Value + value, 0, maxHealth);

        if (currentHealth.Value == 0)
        {
            OnDieEvent?.Invoke();
            _isDead = true;
        }
    }

    public void TakeDamage(int damageValue)
    {
        ModifyHealth(-damageValue);
    }

    public void RestoreHealth(int healValue)
    {
        ModifyHealth(healValue);
    }
}
