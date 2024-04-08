using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanelUI : MonoBehaviour
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private LobbyUI _lobbyUIPrefab;
    [SerializeField] private Button _closeButton;
    
    [SerializeField] private float _spacing = 30f;
    
    private List<LobbyUI> _lobbyUIList;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _lobbyUIList = new List<LobbyUI>();
        _closeButton.onClick.AddListener(ClosePanel);
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        float screenHeight = Screen.height;
        _rectTransform.anchoredPosition = new Vector2(0, screenHeight);
    }

    public void OpenPanel()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(_rectTransform.DOAnchorPosY(0, 0.8f));
        seq.Join(_canvasGroup.DOFade(1f, 0.8f));
        seq.AppendCallback(() =>
        {
            _canvasGroup.interactable = true;
        });
    }
    
    public void ClosePanel()
    {
        float screenHeight = Screen.height;
        Sequence seq = DOTween.Sequence();
        _canvasGroup.interactable = false;
        seq.Append(_rectTransform.DOAnchorPosY(screenHeight, 0.8f));
        seq.Join(_canvasGroup.DOFade(0f, 0.8f));
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            CreateLobbyUI();
        }
    }

    private void CreateLobbyUI()
    {
        LobbyUI ui = Instantiate(_lobbyUIPrefab, _scrollRect.content);

        //여기에 로비 정보 셋팅 부분이 들어갈 예정
        _lobbyUIList.Add(ui);
        
        float offset = _spacing;
        
        for (int i = 0; i < _lobbyUIList.Count; i++)
        {
            _lobbyUIList[i].Rect.anchoredPosition = new Vector2(0, -offset);
            offset += _lobbyUIList[i].Rect.sizeDelta.y + _spacing;
        }

        Vector2 contentSize = _scrollRect.content.sizeDelta;
        contentSize.y = offset;
        _scrollRect.content.sizeDelta = contentSize;
    }
}
