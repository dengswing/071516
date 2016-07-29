using System;
using UnityEngine;

/// <summary>
/// 手效果
/// </summary>
public class HandCircleEffect : MonoBehaviour
{
    public float angle = 0;
    public float radius = 150;
    public float speed = 10f;
    public float x0 = 50f;
    public float y0 = 50f;
    public int guideCount = 2;
    public float duration = 1f;
    public float alpha = 0.3f;

    public Action initBack;
    private int Max_Angle = 360;
    private float radian = 0;
    private bool isInit;
    private int currentCount;
    private float currentAngle;
    private bool isHide;
    private float iTime;
    private CanvasGroup canvas;

    /// <summary>
    /// 开始初始化
    /// </summary>
    public void StartInit(Action back)
    {
        isInit = true;
        initBack = back;
        Show();
    }

    public void Show()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        CatGameTools.UIGameObjectAlphaBack(canvas, 1, 0.05f);
    }
    public void Hide()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }

    void Start()
    {
        currentAngle = angle;
        movePosition();
        canvas = gameObject.GetComponent<CanvasGroup>();
    }

    void OnDisable()
    {
        isInit = false;
        currentCount = 0;
        currentAngle = angle;
        isHide = false;
    }

    void Update()
    {
        if (!isInit)
        {
            return;
        }

        if (currentCount >= guideCount)
        { //结束初始化
            iTime += Time.deltaTime;
            if (iTime > 0.05f)
            {
                Hide();
            }

            CatGameTools.UIGameObjectAlpha(gameObject, canvas.alpha, 0, 0.05f);
            if (initBack != null) initBack();
            initBack = null;
            return;
        }

        if (currentAngle == angle)
        {
            if (canvas.alpha != alpha && canvas.alpha != 1) return; //完全透明之后在执行
            CatGameTools.UIGameObjectAlpha(gameObject, alpha, 1, 0.05f);
        }

        currentAngle += speed;
        movePosition();

        //  Debug.Log("currentAngle==>" + currentAngle + "|" + (Max_Angle + angle) + "|" + isHide);
        if (currentAngle * 1.5 > (Max_Angle + angle) && !isHide)
        {
            isHide = true;
            CatGameTools.UIGameObjectAlpha(gameObject, 1, alpha, duration);
        }

        if (currentAngle > Max_Angle + angle)
        {
            isHide = false;
            currentAngle = angle;
            currentCount += 1;
        }
    }

    private void movePosition()
    {
        radian = currentAngle * Mathf.PI / 180;
        float xx = x0 + radius * Mathf.Sin(radian);
        float yy = y0 + radius * Mathf.Cos(radian);
        transform.localPosition = new Vector3(xx, yy, 0);
    }
}
