using UnityEngine;

/// <summary>
/// 喂食管理
/// </summary>
public class FeedManager : MonoBehaviour
{
    FeedItemManager itemManager;
    FeedMenuManager menuManager;
    CatActionManager actionManager;
    FeedShopMove shopMove;
    ExpressionManager expressionManager;
    DialogueManager dialogueManager;
    FeedScoreManager feedScoreManager;
    YummyEffectManager yummyManager;
    YawnEffectManager yawmManager;
    YuckEffectManager yuckManager;
    LoopEffectManager loopEffectManager;

    EatResultManager eatManager;

    EAT_STATE resultState;
    EAT_STATE changeState;
    Transform shop;
    Transform scoreEffect;
    Transform eatLoop;

    void Awake()
    {
        actionManager = transform.GetComponentInChildren<CatActionManager>();
        expressionManager = transform.GetComponentInChildren<ExpressionManager>();
        dialogueManager = transform.GetComponentInChildren<DialogueManager>();
    }

    void Start()
    {
        shop = transform.FindChild("Shop");
        scoreEffect = transform.FindChild("ScoreEffect");

        var yummyEffect = transform.FindChild("YummyEffect");
        var yuckEffect = transform.FindChild("YuckEffect");
        var yawnEffect = transform.FindChild("YawnEffect");

        eatLoop = transform.FindChild("EatLoop");
        itemManager = shop.GetComponentInChildren<FeedItemManager>();
        menuManager = shop.GetComponentInChildren<FeedMenuManager>();
        shopMove = shop.GetComponentInChildren<FeedShopMove>();
        menuManager.AddListener(MenuHandler);
        itemManager.AddListener(EatHandler);

        feedScoreManager = scoreEffect.GetComponentInChildren<FeedScoreManager>();
        loopEffectManager = eatLoop.GetComponent<LoopEffectManager>();

        yummyManager = yummyEffect.GetComponent<YummyEffectManager>();
        yuckManager = yuckEffect.GetComponent<YuckEffectManager>();
        yawmManager = yawnEffect.GetComponent<YawnEffectManager>();

        //开始逻辑
        StartFlow();
    }

    void StartFlow()
    {
        Debug.Log("StartFlow");
        expressionManager.StartExpression(ExpressionState.A, ExpressionFinish);
    }

    void ExpressionFinish()
    {
        dialogueManager.StartDailogue(CatGameConst.Dialogue_A, DailogueFinish);
    }

    void DailogueFinish()
    {
        shop.gameObject.SetActive(true); //激活商店       
    }

    void MenuHandler(int index)
    {
        itemManager.ShowItem(index);
    }

    void EatHandler(EAT_STATE eat)
    { //开始喂食
        resultState = eat;
        shopMove.ItemShowAndHide(false);
        actionManager.Feed(EAT_STATE.EAT, EatFinish);
        expressionManager.Hide(); //隐藏表情
        loopEffectManager.PlayAnimation();
    }

    void EatFinish()
    { //喂食完成

        actionManager.Feed(resultState, null);

        var info = string.Empty;
        ExpressionState expressionIcon = ExpressionState.B;
        switch (resultState)
        {
            case EAT_STATE.Yuck:
                changeState = EAT_STATE.YuckChange;
                expressionIcon = ExpressionState.B;
                eatManager = yuckManager;
                info = CatGameConst.Dialogue_B;
                break;
            case EAT_STATE.Yummy:
                changeState = EAT_STATE.YummyChange;
                expressionIcon = ExpressionState.C;
                eatManager = yummyManager;
                info = CatGameConst.Dialogue_C;
                break;
            case EAT_STATE.Yawm:
                changeState = EAT_STATE.YummyChange;
                expressionIcon = ExpressionState.D;
                info = CatGameConst.Dialogue_C;
                eatManager = yawmManager;
                break;
            default:
                break;
        }

        eatManager.Show(); //显示结果
        dialogueManager.StartDailogue(info, DalogueFinishB); //显示对话

        expressionManager.StartExpression(expressionIcon, null);
        loopEffectManager.Hide(false);
        ShopDisable();
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
    void ShopDisable()
    {
        shop.gameObject.SetActive(false); //禁用商店
    }

    void ScoreEffectDisable()
    {
        scoreEffect.gameObject.SetActive(false); //禁用结果
    }

    #endregion
}
