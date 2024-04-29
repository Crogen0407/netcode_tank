using DG.Tweening;
using UnityEngine;

public class GunRecoilFeedback : Feedback
{
    [SerializeField] private Transform _targetTrm;
    [SerializeField] private float _recoilPower = 0.2f;

    private float _originYPos;
    private void Awake()
    {
        _originYPos = _targetTrm.localPosition.y;
    }

    public override void CompleteFeedback()
    {
        _targetTrm.DOComplete(); //트윈이 자동 종료
        Vector3 pos = _targetTrm.localPosition;
        pos.y = _originYPos;
        _targetTrm.localPosition = pos;
    }

    public override void CreateFeedback()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_targetTrm.DOLocalMoveY(_originYPos - _recoilPower, 0.1f));
        seq.Append(_targetTrm.DOLocalMoveY(_originYPos, 0.1f));

        //_targetTrm.DOLocalMoveY(current - _recoilPower, 0.1f)
        //    .SetLoops(1, LoopType.Yoyo);
    }
}
