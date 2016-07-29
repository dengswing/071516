using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 喂食成绩控制
/// </summary>
public class FeedScoreManager : MonoBehaviour
{
    public float speed = 2f;
    public float duration = 1f;

    Action startBack;
    Action finishBack;
    float sTime;
    Slider score;
    Text progressText;
    int max = 100;
    float current = 0;
    float index = 80;
	bool isStart;

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

		//StartDoing ();
    }

	void StartDoing()
	{
		int number = 0;
		// 创建一个 Tweener 对象， 另 number的值在 5 秒内变化到 100
		Tween t = DOTween.To(() => number, x => number = x, max, speed).SetEase(Ease.InOutCirc);
		// 给执行 t 变化时，每帧回调一次 UpdateTween 方法
		t.OnUpdate(() => UpdateTween(number));
	}

    private void UpdateTween(int num)
    {
        score.value = num;
        progressText.text = num.ToString();
    }

    void OnDisable()
    {
        index = 80;
        sTime = 0;
        current = 0;
        progressText.text = string.Empty;
        score.value = 0;
		isStart = false;
    }


    void Update()
    {
        sTime += Time.deltaTime;

		if (score.value >= max) {
			if (startBack != null) {
				sTime = 0;
				CatGameTools.UIGameObjectAlpha (gameObject, 1, 0, duration); //成绩开始隐藏
				startBack ();
			}
			startBack = null;

			if (finishBack != null && sTime >= duration + 1) {
				finishBack ();
				finishBack = null;
			}
			return;
		} else if(!isStart) 
		{
			isStart = true;
			StartDoing ();
			
		}
			
    }
}
