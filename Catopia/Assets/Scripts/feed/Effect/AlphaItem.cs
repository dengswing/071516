using DG.Tweening;
using UnityEngine;

/// <summary>
/// 透明
/// </summary>
public class AlphaItem : MonoBehaviour
{
    public float duration = 2f;
    public float startTime = 0f;
    public float startAlpha = 0f;
    public float endAlpha = 1f;

    float sTime;
    CanvasGroup canvas;
    bool isDoing;

    void Start()
    {
        canvas = gameObject.GetComponent<CanvasGroup>();
    }
    void StartDoing()
    {
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, startAlpha, 0);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, endAlpha, duration);
    }

    void OnDisable()
    {
        isDoing = false;
        sTime = 0;
    }

    void Update()
    {
        sTime += Time.deltaTime;
        if (sTime > startTime + duration + 10)
        {
            sTime = 0;
            //enabled = false;
            return;
        }

        if (!isDoing && sTime > startTime)
        {
            isDoing = true;
            StartDoing();
        }
    }
}
