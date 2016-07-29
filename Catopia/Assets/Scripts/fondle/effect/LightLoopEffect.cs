using UnityEngine;
using System;
using DG.Tweening;

public enum LightLoopState
{
    Normal,
    ZoomOut,
    ZoomIn,
}

/// <summary>
/// 光圈效果
/// </summary>
public class LightLoopEffect : MonoBehaviour
{
    private float duration;
    private float waitTime;
    private int touchCount;
    private Action finishBack;
    private bool isPlayer;
    private CanvasGroup canvas;
    private float iTime;
    private int count;

    private bool isStartHide;
    private LightLoopState state = LightLoopState.Normal;
    private RectTransform rect;
    private bool isImmediately;

    void Awake()
    {
        canvas = gameObject.GetComponent<CanvasGroup>();
        rect = gameObject.GetComponent<RectTransform>();
    }

    public void AddListener(Action back)
    {
        finishBack = back;
    }

    public LightLoopState lightLoopState
    {
        get { return state; }
    }

    public int TouchCount
    {
        get { return touchCount; }

        set { touchCount = value; }
    }

    public float WaitTime
    {
        get { return waitTime; }

        set { waitTime = value; }
    }

    public float Duration
    {
        get { return duration; }

        set { duration = value; }
    }

    public void Show()
    {
        if (state == LightLoopState.ZoomIn) return;
        if (isPlayer)
        {
            count += 1;
            return;
        }

        if (!gameObject.activeSelf) gameObject.SetActive(true);
        CatGameTools.UIGameObjectAlpha(gameObject, 0, 1, 0.1f);
        ZoomOut();
    }

    public void Cancel()
    {
        // if (count >= TouchCount) return;
        if (isStartHide || state == LightLoopState.Normal) return;
        count = 0;
        ZoomIn();
    }


    private void Hide()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);

        rect.DOKill();
        state = LightLoopState.Normal;
    }

    void Start()
    {

    }

    void OnDisable()
    {
        isPlayer = false;
        count = 0;
        iTime = 0;
        canvas.alpha = 0;
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        isStartHide = false;
        isImmediately = false;
    }

    /// <summary>
    /// 缩小
    /// </summary>
    void ZoomOut()
    {
        if (isPlayer) return;

        CatGameTools.UIGameObjectAlphaBack(canvas, 1, 0.25f, () =>
        {
            CatGameTools.UIGameObjectAlphaBack(canvas, 0, Duration);            
        });
        CatGameTools.UIGameObjectScaleBack(gameObject.transform as RectTransform, new Vector3(0, 0, 0), Duration);

        isPlayer = true;
        state = LightLoopState.ZoomOut;
    }

    /// <summary>
    ///还原
    /// </summary>
    void ZoomIn()
    {
        iTime = 0;
        CatGameTools.UIGameObjectAlphaBack(canvas, .7f, Duration * 0.15f, () =>
        {
            CatGameTools.UIGameObjectAlphaBack(canvas, 0, 0.05f);
        });
        CatGameTools.UIGameObjectScaleBack(gameObject.transform as RectTransform, new Vector3(1, 1, 1), Duration * 0.15f);

        isPlayer = true;
        isStartHide = true;
        state = LightLoopState.ZoomIn;
    }

    void PlayerFinish()
    {
        isPlayer = false;
        if (finishBack != null) finishBack();
        finishBack = null;

        //isStartHide = true;
       // iTime = 0.5f;
        Hide();
    }

    void Update()
    {
        if (!isPlayer) return;

        iTime += Time.deltaTime;

        if (isImmediately)
        {
            if (iTime >= 0.5f)
            {
                PlayerFinish();
            }
            return;
        }

        if (isStartHide)
        {
            if (iTime >= Duration * 0.16f)
            {
                Hide();
            }
            return;
        }

        if (!isStartHide && iTime >= WaitTime && count < TouchCount)
        {//触发消失
            Debug.Log("触发消失！！" + WaitTime + "|" + count + "|" + TouchCount + "|" + isStartHide);
            Cancel();
            return;
        }

        //if (count >= TouchCount)
        //{
        //    iTime = 0;
        //    CatGameTools.UIGameObjectAlpha(gameObject, 1, 0, 0.5f);
        //    var startScale = gameObject.transform.localScale;
        //    CatGameTools.UIGameObjectScale(gameObject, startScale, new Vector3(0, 0, 0), 0.5f);
        //    isImmediately = true;
        //    Debug.Log("立即触发完成！！" + WaitTime + "|" + count + "|" + TouchCount + "|" + isStartHide);
        //    return;
        //}

        if (iTime >= Duration * 0.6f)
        {
            Debug.Log("触发完成！！" + WaitTime + "|" + count + "|" + TouchCount + "|" + isStartHide);
            if (count < TouchCount)
                Cancel();
            else
                PlayerFinish();
        }
    }
}