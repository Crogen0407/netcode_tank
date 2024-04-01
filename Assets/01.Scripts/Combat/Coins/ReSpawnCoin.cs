using System;
using UnityEngine;

public class ReSpawnCoin : Coin
{
    public event Action<ReSpawnCoin> OnCollected;
    private Vector3 _prevPos;
    public override int Collect()
    {
        if (_alreadyCollected) return 0;

        if (!IsServer)
        {
            SetVisible(false);
            return 0;
        }

        _alreadyCollected = true;
        OnCollected?.Invoke(this);
        isActive.Value = false; //코인을 꺼준다.

        return _coinValue;

    }

    //서버만 실행하는 함수. 클라는 이거 안실행해
    [ContextMenu("ResetCoins")]
    public void ResetCoin()
    {
        _alreadyCollected = false;
        isActive.Value = true; //네트워크 변수를 true로 설정해주는 거지
        SetVisible(true);

        //transform.position = new Vector3(0, 0, 0);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _prevPos = transform.position;

        // if(IsClient)
        //     isActive.OnValueChanged += HandleActiveValueChanged;
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        // if(IsClient)
        //     isActive.OnValueChanged -= HandleActiveValueChanged;
    }

    // private void HandleActiveValueChanged(bool previousvalue, bool newvalue)
    // {
    // }

    private void Update()
    {
        if (IsServer) return;

        if (Vector2.Distance(_prevPos, transform.position) > 0.1f)
        {
            _prevPos = transform.position;
            SetVisible(true);
        }
    }
}
