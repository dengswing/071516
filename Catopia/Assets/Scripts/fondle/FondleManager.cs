using Catopia;
using System;
using UnityEngine;

/// <summary>
/// 抚摸管理
/// </summary>
public class FondleManager : SingleInstance<FondleManager>
{
    public Transform scoreEffect;
    public Transform yummyEffect;
    public Transform yuckEffect;
    public Transform yawnEffect;
    public Transform handEffect;

    public FondleSensitivity catSensitivity;
    public FondleCatStart startCatFondle;


    CatActionManager actionManager;
    ExpressionManager expressionManager;
    DialogueManager dialogueManager;
    FeedScoreManager feedScoreManager;
    YummyEffectManager yummyManager;
    YawnEffectManager yawmManager;
    YuckEffectManager yuckManager;
    EatResultManager eatManager;
    HandManager handManager;

    CAT_STATE resultState;
    CAT_STATE changeState;

    void Awake()
    {
        actionManager = transform.GetComponentInChildren<CatActionManager>();
        expressionManager = transform.GetComponentInChildren<ExpressionManager>();
        dialogueManager = transform.GetComponentInChildren<DialogueManager>();

        catSensitivity += CatSensitivityHandler;
    }

    void Start()
    {
        feedScoreManager = scoreEffect.GetComponentInChildren<FeedScoreManager>();
        yummyManager = yummyEffect.GetComponent<YummyEffectManager>();
        yuckManager = yuckEffect.GetComponent<YuckEffectManager>();
        yawmManager = yawnEffect.GetComponent<YawnEffectManager>();
        handManager = handEffect.GetComponent<HandManager>();

        //开始逻辑
        StartFlow();
    }

    void CatSensitivityHandler(string state, Action callBack)
    {//抚摸触发
       // actionManager.FondleAction(state, callBack);

        ExpressionState exState = (UnityEngine.Random.Range(0, 2) == 0 ? ExpressionState.E : ExpressionState.F);
        expressionManager.StartExpression(exState, ()=> {
            expressionManager.Hide();

            callBack();
        });
    }

    void StartFlow()
    {
        Debug.Log("StartFlow");
        dialogueManager.StartDailogue(CatGameConst.Dialogue_Fondle_A, DailogueFinish);
    }

    void DailogueFinish()
    {
        if (startCatFondle != null) startCatFondle(CatZoomState.ZoomIn, () =>
        {
            handManager.Show(FondleFinish);
        });
    }

    void FondleFinish(CAT_STATE resultState)
    {//抚摸完成
        handManager.Hide();
        if (startCatFondle != null) startCatFondle(CatZoomState.ZoomOut, () =>
        {
            ShowResult(resultState);
        });
    }

    void ShowResult(CAT_STATE resultState)
    { //显示结果
        actionManager.Feed(resultState, null);
        var info = string.Empty;
        switch (resultState)
        {
            case CAT_STATE.Yuck:
                changeState = CAT_STATE.YuckChange;
                eatManager = yuckManager;
                info = CatGameConst.Dialogue_Fondle_B;
                break;
            case CAT_STATE.Yummy:
                changeState = CAT_STATE.YummyChange;

                eatManager = yummyManager;
                info = CatGameConst.Dialogue_Fondle_C;
                break;
            case CAT_STATE.Yawm:
                changeState = CAT_STATE.YummyChange;
                info = CatGameConst.Dialogue_Fondle_C;
                eatManager = yawmManager;
                break;
            default:
                break;
        }

        eatManager.Show(); //显示结果
        dialogueManager.StartDailogue(info, DalogueFinishB); //显示对话        
    }


    void DalogueFinishB()
    {
        scoreEffect.gameObject.SetActive(true); //激活成绩
        feedScoreManager.AddListener(ScoreFinish, ScoreStartHide);
    }

    void ScoreFinish()
    {
        actionManager.Feed(changeState, null);

        ScoreEffectDisable();

        StartFlow();
    }

    void ScoreStartHide()
    {
        expressionManager.Hide(); //隐藏表情
        eatManager.Hide();    //隐藏结果
    }

    #region disable

    void ScoreEffectDisable()
    {
        scoreEffect.gameObject.SetActive(false); //禁用结果
    }

    #endregion
}
