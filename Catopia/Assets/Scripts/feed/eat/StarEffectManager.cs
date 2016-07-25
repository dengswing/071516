using DG.Tweening;
using UnityEngine;

/// <summary>
/// 星星动画控制
/// </summary>
public class StarEffectManager : MonoBehaviour
{
    public float duration = 0f;
    public float startTime = 0f;
    private SpineAnimationManager spineAnimation;
    CanvasGroup canvas;

    float iTime;
    bool isPlay;
    float sTime;
    bool isDoing;

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnimation(float time=1f)
    {
        isPlay = true;
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        spineAnimation.PlayAnimation(CatGameConst.YummyStar, false, finishBack);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 1, time); //为了防止spine动画闪一下
    }

    public void Active()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }

    public void Hide(bool isActive = true)
    {
        if (!isActive && gameObject.activeSelf) gameObject.SetActive(false);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 0, 1);
        isDoing = false;
    }


    void Awake()
    {
        spineAnimation = transform.GetComponent<SpineAnimationManager>();
        canvas = gameObject.GetComponent<CanvasGroup>();
    }

    void OnDisable()
    {
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 0, 0);
    }

    void Start()
    {

    }

    void Update()
    {
        if (startTime != 0)
        {
            sTime += Time.deltaTime;
            if (!isDoing && sTime > startTime)
            {
                isDoing = true;
                iTime = 0;
                PlayAnimation();
            }
        }

        if (!isPlay)
        {
            iTime += Time.deltaTime;
            if (duration > 0 && iTime > duration)
            {
                PlayAnimation(0.1f);
            }
        }
    }

    void finishBack()
    {
        if (duration > 0)
        {
            OnDisable();
            iTime = 0;
            isPlay = false;
        }
    }
}
