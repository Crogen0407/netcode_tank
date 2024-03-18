using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] private float _lifeTime;
    private float _currentLifeTime;

    private void Update()
    {
        _currentLifeTime += Time.deltaTime;
        if (_currentLifeTime >= _lifeTime)
            Destroy(gameObject);
    }
}
