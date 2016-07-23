using UnityEngine;
using DG.Tweening;

/// <summary>
/// 移动喂食商店
/// </summary>
public class FeedShopMove : MonoBehaviour
{
    public float duration;
    public float postionY = -375;
    public float startTime = 0f;

    private RectTransform rect;
    private float sTime;
    private bool isDoing;

    /// <summary>
    /// 显示和隐藏食物
    /// </summary>
    /// <param name="isShow"></param>
    public void ItemShowAndHide(bool isShow)
    {
        ShowAndHide(isShow, duration);
    }

    void Awake()
    {
        //  DOTween.Init(true, true, LogBehaviour.ErrorsOnly).SetCapacity(200, 10);
    }

    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
        ShowAndHide(false, 0f);
    }

    void OnDisable()
    {
        isDoing = false;
        sTime = 0;
    }

    void ShowAndHide(bool isShow, float duration)
    {
        if (!isShow)
            rect.DOLocalMoveY(0, duration);
        else
            rect.DOLocalMoveY(postionY, duration);
    }

    void Update()
    {
        sTime += Time.deltaTime;
        if (!isDoing && sTime > startTime)
        {
            isDoing = true;
            ShowAndHide(true, duration);
        }
    }
}
