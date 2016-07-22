using UnityEngine;

/// <summary>
/// 吃东西结果控制
/// </summary>
public class EatResultManager : MonoBehaviour
{
    public float duration = 1f;

    protected bool isStartHide;
    protected float iTime;

    public virtual void Show()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        CatGameTools.UIGameObjectAlpha(gameObject, 0, 1, duration);
    }
    public virtual void Hide()
    {
        CatGameTools.UIGameObjectAlpha(gameObject, 1, 0, duration);
        isStartHide = true;
    }

    void OnDisable()
    {
        isStartHide = false;
        iTime = 0;
    }

    void Update()
    {
        if (isStartHide)
        {
            iTime += Time.deltaTime;
            if (iTime > duration)
            {
                if (gameObject.activeSelf) gameObject.SetActive(false);
            }
        }
    }
}
