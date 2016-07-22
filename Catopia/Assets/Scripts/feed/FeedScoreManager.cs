using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 喂食成绩控制
/// </summary>
public class FeedScoreManager : MonoBehaviour
{
    public float startTime = 2f;
    public float duration = 1f;

    Action startBack;
    Action finishBack;
    float sTime;
    Slider score;
    Text progressText;
    int max = 100;
    float current = 0;
    float index = 80;

    public void AddListener(Action callback, Action startback)
    {
        finishBack = callback;
        this.startBack = startback;
    }

    void Awake()
    {

    }

    void Start()
    {
        score = transform.GetComponentInChildren<Slider>();
        progressText = score.GetComponentInChildren<Text>();
        progressText.text = string.Empty;
    }


    void OnDisable()
    {
        index = 80;
        sTime = 0;
        current = 0;
        progressText.text = string.Empty;
        score.value = 0;
    }


    void Update()
    {
        sTime += Time.deltaTime;

        if (score.value >= max)
        {
            if (startBack != null)
            {
                sTime = 0;
                CatGameTools.UIGameObjectAlpha(gameObject, 1, 0, duration); //成绩开始隐藏
                startBack();
            }
            startBack = null;

            if (finishBack != null && sTime >= duration + 1)
            {
                finishBack();
                finishBack = null;
            }
            return;
        }

        if (sTime > startTime)
        {
            index = index / 2;
            if (index <= 1) index = 0.15f;
            current += index;
            score.value = current;
            progressText.text = string.Format("{0} %", (int)score.value);
        }
    }
}
