using DG.Tweening;
using UnityEngine;

/// <summary>
/// 缩放
/// </summary>
public class ScaleItem : MonoBehaviour
{
    public float duration = 2f;
    public float startTime = 0f;
    public Vector3 startScaleVector = new Vector3(1f, 1f, 1f);
    public Vector3 endScaleVector = new Vector3(1f, 1f, 1f);

    float sTime;
    RectTransform rect;
    bool isDoing;

    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
    }
    void StartDoing()
    {
        rect.DOScale(startScaleVector, 0f);
        rect.DOScale(endScaleVector, duration);
    }

    void Update()
    {
        sTime += Time.deltaTime;
        if (sTime > startTime + duration + 10)
        {
            enabled = false;
            return;
        }

        if (!isDoing && sTime > startTime)
        {
            isDoing = true;
            StartDoing();
        }
    }
}
