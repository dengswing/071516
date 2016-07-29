using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 手的管理
/// </summary>
public class HandManager : MonoBehaviour
{
    public GameObject starParticle;
    public GameObject hand;
    public GameObject handInit;
    public float duration = 2f;

    private HandCircleEffect handEffect;
    private CatFondleManger catFondle;
    private TimeManager timeManager;
    private Action<CAT_STATE> timeFinishBack;
    private FondleManager fondleManager;

    private float iTime;
    void Awake()
    {
        handEffect = handInit.GetComponent<HandCircleEffect>();
        catFondle = gameObject.GetComponent<CatFondleManger>();
        timeManager = gameObject.GetComponent<TimeManager>();

        fondleManager = FondleManager.Instance;
        fondleManager.catSensitivity += CatSensitivityHandler;
        iTime = duration;
        Update();
    }

    public void Show(Action<CAT_STATE> finishBack)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        handEffect.StartInit(HandInitFinish);
        if (!starParticle.activeSelf) starParticle.SetActive(true);
        timeFinishBack = finishBack;
    }

    public void Hide()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
        if (starParticle.activeSelf) starParticle.SetActive(false);

        catFondle.enabled = false;
        iTime = duration;
        Update();
    }

    void CatSensitivityHandler(string state, Action callBack)
    {//抚摸触发
        catFondle.RandomSite();
    }

    void Update()
    {
        iTime += Time.deltaTime;

        if (iTime < duration)
        {
            return;
        }

        iTime = 0;
        if (catFondle.enabled)
        {
            if (catFondle.isShowParticle) starParticle.transform.position = hand.transform.position;
        }
        else
        {
            starParticle.transform.position = handInit.transform.position;
        }
    }

    private void HandInitFinish()
    {
        catFondle.enabled = true;
        timeManager.StartTime(TimeHandler);
    }

    private void TimeHandler()
    {
        CAT_STATE state;
        var sorce = catFondle.musicNote.musiceNoteCount;
        if (sorce >= 6)
        {
            state = CAT_STATE.Yummy;
        }
        else if (sorce >= 4)
        {
            state = CAT_STATE.Yawm;
        }
        else
        {
            state = CAT_STATE.Yuck;
        }

        if (timeFinishBack != null) timeFinishBack(state);
    }
}
