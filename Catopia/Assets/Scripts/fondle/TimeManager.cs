using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 时间的管理
/// </summary>
public class TimeManager : MonoBehaviour
{
    public MusicNoteManger musicNoteManager;
    public float duration = 30f;
    public Text timeText;

    private float iTime;
    private bool isStart;
    private Action callBack;

    void Awake()
    {
        timeText.text = string.Empty;
    }

    public void StartTime(Action callBack)
    {
        this.callBack = callBack;
        isStart = true;
    }

    public void Hide()
    {

    }

    void OnDisable()
    {
        iTime = 0;
        isStart = false;
        timeText.text = string.Empty;
    }

    void Update()
    {
        if (!isStart) return;

        iTime += Time.deltaTime;

        //Debug.Log("|" + iTime + "|" + Math.Floor(iTime));
        var tmpValue = duration - Math.Floor(iTime);
        if (tmpValue < 0) tmpValue = 0;
        timeText.text = tmpValue.ToString();

        if (iTime >= duration && musicNoteManager.isOver)
        {
            isStart = false;
            if (callBack != null) callBack();
            callBack = null;
        }
    }
}
