using System;
using System.Collections;
using System.Collections.Generic;
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

    public override void CreateFeedback()
    {
        _targetTrm.DOComplete();
        Vector3 pos = _targetTrm.localPosition;
        pos.y = _originYPos;
        _targetTrm.localPosition = pos;
    }

    public override void CompleteFeedback()
    {
        float current = _targetTrm.localPosition.y;

        Sequence seq = DOTween.Sequence();
        seq.Append(_targetTrm.DOLocalMoveY(current - _recoilPower, 0.1f));
        seq.Append(_targetTrm.DOLocalMoveY(current, 0.1f));
        
        
    }
}
