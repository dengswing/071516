using DG.Tweening;
using UnityEngine;

/// <summary>
/// 坐标设置
/// </summary>
public class PositionItem : MonoBehaviour
{
    public float duration = 2f;
    public float startTime = 0f;
    public Vector3 startPositionVector = new Vector3(0f, 0f, 0f);
    public Vector3 endPositionVector = new Vector3(0f, 0f, 0f);

    float sTime;
    RectTransform rect;
    bool isDoing;

    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();

        Debug.Log("PositionItem xy="+transform.localPosition);
    }
    void StartDoing()
    {
        if (!startPositionVector.Equals(endPositionVector))
        {
            rect.DOLocalMove(startPositionVector, 0f);
            rect.DOLocalMove(endPositionVector, duration);
        }
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
           // enabled = false;
            return;
        }

        if (!isDoing && sTime > startTime)
        {
            isDoing = true;
            StartDoing();
        }
    }
}
