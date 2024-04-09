using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private TankSelectUI _selectUIPrefab;
    [SerializeField] private RectTransform _selectPanelTrm;

    public void CreateUIPanel(ulong clientID, string username)
    {
        TankSelectUI ui = Instantiate(_selectUIPrefab);
        ui.NetworkObject.SpawnAsPlayerObject(clientID);
        ui.transform.SetParent(_selectPanelTrm);
        ui.transform.localScale = Vector3.one;
        
        ui.SetTankName(username);
    }
}
