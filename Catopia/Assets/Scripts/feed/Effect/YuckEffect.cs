using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 讨厌动画控制
/// </summary>
public class YuckEffect : MonoBehaviour
{
    public List<GameObject> groupList;
    public float gap = 800f;
    public float speed = 0.1f;

    int currentIndex;
    bool isDoing;
    int cellPositionY = 0;
    List<GameObject> list = new List<GameObject>();

    public void Hide()
    {

    }

    void Awake()
    {

    }

    void StartDoing()
    {
        var count = groupList.Count;
        for (int i = 0; i < count; i++)
        {
            CreateItem(i);
        }
    }

    void CreateItem(int index)
    {
        GameObject con;
        var item = groupList[index];
        con = CatGameTools.AddChild(transform, item);
        con.transform.localPosition = new Vector3(0, cellPositionY);
        cellPositionY -= (int)gap;
        list.Add(con);
    }

    void Start()
    {
        StartDoing();
        isDoing = true;
    }

    void OnDisable()
    {
		currentIndex = 0;
        list.Clear();
        transform.localPosition = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if (isDoing)
        {
            transform.Translate(0, speed, 0);
            HitItem();
        }
    }

    void HitItem()
    {
        var _content = transform.GetComponent<RectTransform>();
        var index = Mathf.FloorToInt(_content.anchoredPosition.y / gap);
        if (currentIndex != index)
        {
            var con = list[currentIndex];
            con.transform.localPosition = new Vector3(0, cellPositionY);
            cellPositionY -= (int)gap;
            list.Add(con);
            currentIndex = index;
        }
    }
}
