using UnityEngine;
using DG.Tweening;

/// <summary>
/// 大小坐标透明组合
/// </summary>
public class ScaleMoveAlphaGroup : MonoBehaviour
{
    public bool isAutoTouch = true;
    public float startTime = 0f;
    public float duration = 2f;
    public Vector3 startScaleVector = new Vector3(1f, 1f, 1f);
    public Vector3 endScaleVector = new Vector3(1f, 1f, 1f);
    public float startAlpha = 0f;
    public float endAlpha = 1f;
    public Vector3 startPositionVector = new Vector3(0f, 0f, 0f);
    public Vector3 endPositionVector = new Vector3(0f, 0f, 0f);

    protected float sTime = 0f;
    protected bool isDoing;

    /// <summary>
    /// 设置启动
    /// </summary>
    public bool SetStart
    {
        set { isAutoTouch = value; }
    }

    void Awake()
    {

    }

    void Start()
    {

    }

    protected void StartDoing()
    {
        CatGameTools.UIGameObjectScale(gameObject, startScaleVector, endScaleVector, duration);
        CatGameTools.UIGameObjectAlpha(gameObject, startAlpha, endAlpha, duration);

        if (!startPositionVector.Equals(endPositionVector))
        {
            CatGameTools.UIGameObjectPosition(gameObject, startPositionVector, endPositionVector, duration);
        }
    }

    void Update()
    {
        if (!isAutoTouch) return;

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
