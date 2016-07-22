using System;
using UnityEngine;

/// <summary>
/// 对话管理
/// </summary>
public class DialogueBubble : ScaleMoveAlphaGroup
{
    private bool isClick;
    private Action finishBack;

    public void AddListener(Action callBack)
    {
        finishBack = callBack;
    }

    void Update()
    {
        if (isClick)
        {
            sTime += Time.deltaTime;

            if (sTime > startTime + duration)
            {
                enabled = false;
                if (finishBack != null) finishBack();
                return;
            }

            if (!isDoing && sTime > startTime)
            {
                isDoing = true;
                StartDoing();
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                isClick = true;
            }
        }
    }

}
