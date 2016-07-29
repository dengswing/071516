using UnityEngine;
using System;
using DG.Tweening;

/// <summary>
/// 表情包
/// </summary>
public enum ExpressionState
{
    A,
    B,
    C,
    D,
    E,
    F,
}

/// <summary>
/// 表情动画控制
/// </summary>
public class ExpressionManager : MonoBehaviour
{
    private SpineAnimationManager spineAnimation;
    CanvasGroup canvas;

    /// <summary>
    /// 开始显示表情
    /// </summary>
    /// <param name="info"></param>
    public void StartExpression(ExpressionState state, Action callback)
    {
        ExpressionShow(state, callback);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 1, 0.5f); //为了防止spine动画闪一下
    }

    public void Hide()
    {
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, 0, 0.3f); //为了防止spine动画闪一下
    }

    void ExpressionShow(ExpressionState state, Action callback)
    {
        string expressionA = string.Empty;
        string expressionB = string.Empty;

        switch (state)
        {
            case ExpressionState.A:
                expressionA = CatGameConst.Expression[0];
                expressionB = CatGameConst.Expression[1];
                break;
            case ExpressionState.B:
                expressionA = CatGameConst.Expression[2];
                expressionB = CatGameConst.Expression[3];
                break;
            case ExpressionState.C:
                expressionA = CatGameConst.Expression[4];
                expressionB = CatGameConst.Expression[5];
                break;
            case ExpressionState.D:
                expressionA = CatGameConst.Expression[6];
                expressionB = CatGameConst.Expression[7];
                break;
            case ExpressionState.E:
                expressionA = CatGameConst.Expression[4];
                break;
            case ExpressionState.F:
                expressionA = CatGameConst.Expression[6];
                break;
            default:
                break;
        }

        spineAnimation.PlayAnimation(expressionA, false, () =>
            {
                if (!string.IsNullOrEmpty(expressionB)) spineAnimation.PlayAnimation(expressionB, true, null);
                if (callback != null) callback();
                callback = null;
            });
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
