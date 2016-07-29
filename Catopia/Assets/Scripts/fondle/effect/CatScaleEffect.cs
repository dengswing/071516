using DG.Tweening;
using System;
using UnityEngine;

public enum CatZoomState
{
    Normal,
    ZoomIn,
    ZoomOut,
}

/// <summary>
/// 猫位移
/// </summary>
public class CatScaleEffect : MonoBehaviour
{
    public GameObject bg;
    public Vector2 startPosition;
    public Vector2 endPosition;
    public Vector2 startScale;
    public Vector2 endScale;
    public float duration = 1f;

    private FondleManager fondleManager;
    private Action zoomFinish;
    private float iTime;
    private CatZoomState iState = CatZoomState.Normal;
    private RectTransform rect;
    private CanvasGroup canvas;

    void Start()
    {
        fondleManager = FondleManager.Instance;
        fondleManager.startCatFondle += FondleHandler;

        rect = gameObject.GetComponent<RectTransform>();
        canvas = bg.GetComponent<CanvasGroup>();
    }

    private void FondleHandler(CatZoomState state, Action callBack)
    {
        zoomFinish = callBack;
        iState = state;
        if (state == CatZoomState.ZoomIn)
            ZoomIn();
        else if (state == CatZoomState.ZoomOut)
            ZoomOut();
    }

    public void ZoomIn()
    {
        iState = CatZoomState.ZoomIn;

        rect.DOKill();
        rect.DOLocalMove(startPosition, 0f);
        rect.DOLocalMove(endPosition, duration).SetEase(Ease.OutQuart);
        rect.DOScale(startScale, 0f);
        rect.DOScale(endScale, duration).SetEase(Ease.OutQuart);

        CatGameTools.UIGameObjectAlphaBack(canvas, .5f, duration);
    }

    public void ZoomOut()
    {
        iState = CatZoomState.ZoomOut;

        rect.DOKill();
        rect.DOLocalMove(endPosition, 0f);
        rect.DOLocalMove(startPosition, duration).SetEase(Ease.OutQuart);
        rect.DOScale(endScale, 0f);
        rect.DOScale(startScale, duration).SetEase(Ease.OutQuart);

        CatGameTools.UIGameObjectAlphaBack(canvas, 1f, duration);
    }

    void Update()
    {
        if (iState == CatZoomState.Normal) return;
        iTime += Time.deltaTime;
        if (iTime >= duration)
        {
            if (zoomFinish != null) zoomFinish();
            zoomFinish = null;
            iTime = 0;
            iState = CatZoomState.Normal;
        }
    }
}
