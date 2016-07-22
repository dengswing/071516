using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// 星星动画控制
/// </summary>
public class StarEffectManager : MonoBehaviour
{
    private SpineAnimationManager spineAnimation;
    CanvasGroup canvas;

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnimation()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        spineAnimation.PlayAnimation(CatGameConst.YummyStar, false, null);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 1, 1); //为了防止spine动画闪一下
    }

    public void Hide(bool isActive = true)
    {
        if (!isActive && gameObject.activeSelf) gameObject.SetActive(false);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 0, 1);
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
}
