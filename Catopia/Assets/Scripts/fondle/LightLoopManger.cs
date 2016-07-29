using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 光圈的管理
/// </summary>
public class LightLoopManger : MonoBehaviour
{
    public int touchCount = 3;
    public float duration = 10f;
    public float waitTime = 3f;

    public GameObject itemPrefab;

    private Dictionary<string, LightLoopEffect> lightLoopList;
    private FondleManager fondleManager;
    private Action finishBack;

    private LightLoopEffect lightLoop;

    void Awake()
    {
        lightLoopList = new Dictionary<string, LightLoopEffect>();
        fondleManager = FondleManager.Instance;
    }

    public void AddListener(Action back)
    {
        finishBack = back;
    }

    public LightLoopState lightLoopState
    {
        get { return (lightLoop ? lightLoop.lightLoopState : LightLoopState.Normal); }
    }

    public void CancleLightLoop()
    {
        lightLoop.Cancel();
    }

    public void Show(string name, Vector2 localPosition)
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        ShowLightLoop(name, localPosition);
    }

    public void Hide()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }

    void OnDisable()
    {
        foreach (var item in lightLoopList)
        {
            GameObject.DestroyImmediate(item.Value);
        }

        lightLoopList.Clear();

        Debug.Log("clear All LightLoopManager");
    }

    void Start()
    {

    }

    void ShowLightLoop(string name, Vector2 position)
    {
        if (!lightLoopList.TryGetValue(name, out lightLoop))
        {
            lightLoop = CatGameTools.AddChild(transform, itemPrefab).GetComponentInChildren<LightLoopEffect>();
            lightLoop.transform.localPosition = position;
            lightLoop.Duration = duration;
            lightLoop.WaitTime = waitTime;
            lightLoop.TouchCount = touchCount;
            lightLoopList[name] = lightLoop;
        }

        lightLoop.AddListener(FinishHandler);
        lightLoop.Show();
    }

    void FinishHandler()
    {
        if (finishBack != null) finishBack();
        finishBack = null;
    }
}