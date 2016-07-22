using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 讨厌动画控制
/// </summary>
public class YuckEffect : MonoBehaviour
{
    public List<GameObject> yuckAnimationList;
    public int count = 5;
    public float startRandomTime = 5;
    public float endRandomTime = 8;
    public int batch = 1;
    public int spaceTime = 1;

    float sTime;
    int currentCount;
    List<GameObject> effectList;

    public void Hide()
    {

    }

    void Awake()
    {
        effectList = new List<GameObject>();
    }

    void StartDoing(bool isFirst = false)
    {
        sTime = 0;
        var value = count / batch;
        var createCount = value;
        if (currentCount + value > count)
        {
            createCount = count - currentCount;
        }
        currentCount += createCount;
        CreateEffect(createCount, isFirst);

        Debug.Log("create==>" + createCount + "|currentCount=" + currentCount + "|" + count);
    }

    void CreateEffect(int count, bool isFirst = false)
    {
        EffectUpMove upMove;
        int length = yuckAnimationList.Count;
        int index = 0;
        for (int i = 0; i < count; i++)
        {
            var item = yuckAnimationList[index];
            index++;
            if (index >= length) index = 0;
            var con = CatGameTools.AddChild(transform, item);
            upMove = con.GetComponentInChildren<EffectUpMove>();
            upMove.duration += Random.Range(startRandomTime, endRandomTime);
            upMove.SetRandomPosition(isFirst);
            effectList.Add(con);
        }
    }

    void Start()
    {
        StartDoing(true);
    }

    void OnDisable()
    {
        currentCount = 0;
        sTime = 0;

        //while (effectList.Count > 0)
        //{
        //    GameObject.DestroyImmediate(effectList[0]);
        //    effectList.RemoveAt(0);
        //}
    }

    void Update()
    {
        sTime += Time.deltaTime;

        if (currentCount < count && sTime > spaceTime)
        {
            StartDoing();
        }
    }
}
