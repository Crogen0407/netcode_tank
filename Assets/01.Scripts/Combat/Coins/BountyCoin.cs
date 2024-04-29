using Cinemachine;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class BountyCoin : Coin
{
    private CinemachineImpulseSource _impulseSource;

    protected override void Awake()
    {
        base.Awake();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public override int Collect()
    {
        if (!IsServer)
        {
            SetVisible(false);
            return 0;
        }

        if (_alreadyCollected) return 0;
        _alreadyCollected = true;

        Destroy(gameObject); //서버가 네트워크 오브젝트 달려있는 녀석을 뽀개면
        //모든 클라이언트에서 대상이 부서진다.
        return _coinValue;
    }

    public void SetScaleAndVisible(float coinScale)
    {
        isActive.Value = true;
        CoinSpawnClientRpc(coinScale);
    }

    [ClientRpc]
    private void CoinSpawnClientRpc(float coinScale)
    {
        Vector3 destination = transform.position;
        //위로 3만큼 올려서
        transform.position = destination + new Vector3(0, 3f, 0);
        transform.localScale = Vector3.one * coinScale;
        SetVisible(true);

        //여기서 닷트윈으로 destination까지 내려주면 된다.
        transform.DOMove(destination, 0.6f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            _impulseSource.GenerateImpulse(0.3f);
        });

    }
}
