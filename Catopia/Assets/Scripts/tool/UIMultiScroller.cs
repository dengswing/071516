using UnityEngine;
using System.Collections.Generic;
using System;

public class UIMultiScroller : MonoBehaviour
{
    public enum Arrangement { Horizontal, Vertical, }
    public Arrangement _movement = Arrangement.Horizontal;
    //单行或单列的Item数量
    [Range(1, 20)]
    public int maxPerLine = 5;
    //Item之间的距离
    [Range(0, 20)]
    public int cellPadding = 5;
    //Item的宽高
    public int cellWidth = 500;
    public int cellHeight = 100;
    //默认加载的行数，一般比可显示行数大2~3行
    [Range(0, 20)]
    public int viewCount = 6;
    public GameObject itemPrefab;
    public RectTransform _content;

    private int _index = -1;
    private List<UIMultiScrollIndex> _itemList;
    private int _dataCount;

    private Queue<UIMultiScrollIndex> _unUsedQueue;  //将未显示出来的Item存入未使用队列里面，等待需要使用的时候直接取出

    private Action<int> itemClickBack;
	private Action<int> itemStartBack;

    void Awake()
    {
        _itemList = new List<UIMultiScrollIndex>();
        _unUsedQueue = new Queue<UIMultiScrollIndex>();
    }

    public void OnValueChange()
    {
        int index = GetPosIndex();

      //  Debug.Log("index=" + index + "|_index" + _index);

        if (_index != index && index > -1)
        {
            _index = index;
            for (int i = _itemList.Count; i > 0; i--)
            {
                UIMultiScrollIndex item = _itemList[i - 1];
                if (item.Index < index * maxPerLine || (item.Index >= (index + viewCount) * maxPerLine))
                {
                    _itemList.Remove(item);
                    _unUsedQueue.Enqueue(item);
                }
            }
            for (int i = _index * maxPerLine; i < (_index + viewCount) * maxPerLine; i++)
            {
                if (i < 0) continue;
                if (i > _dataCount - 1) continue;
                bool isOk = false;
                foreach (UIMultiScrollIndex item in _itemList)
                {
                    if (item.Index == i) isOk = true;
                }
                if (isOk) continue;
                CreateItem(i);
            }
        }
    }

    /// <summary>
    /// 清除所有
    /// </summary>
    public void Reset()
    {
        if (_itemList == null) return;
        while (_itemList.Count > 0)
        {
            UIMultiScrollIndex item = _itemList[0];
            GameObject.DestroyImmediate(item.gameObject);
            _itemList.Remove(item);
        }

        _itemList.Clear();
        // _unUsedQueue.Clear();
        _index = -1;
        _content.localPosition = new Vector3(0f, 0f, 0f);
        DataCount = 0;
    }

    /// <summary>
    /// 启动显示
    /// </summary>
    public void StartShow()
    {
        OnValueChange();
    }

    /// <summary>
    /// 提供给外部的方法，添加指定位置的Item
    /// </summary>
    public void AddItem(int index)
    {
        if (index > _dataCount)
        {
            Debug.LogError("添加错误:" + index);
            return;
        }
        AddItemIntoPanel(index);
        DataCount += 1;
    }

    /// <summary>
    /// 提供给外部的方法，删除指定位置的Item
    /// </summary>
    public void DelItem(int index)
    {
        if (index < 0 || index > _dataCount - 1)
        {
            Debug.LogError("删除错误:" + index);
            return;
        }
        DelItemFromPanel(index);
        DataCount -= 1;
    }

    /// <summary>
    /// 添加点击事件侦听
    /// </summary>
    /// <param name="callBack"></param>
	public void AddListenerItemClick(Action<int> callBack,Action<int> startBack)
    {
        itemClickBack = callBack;
		itemStartBack = startBack;
    }

    private void AddItemIntoPanel(int index)
    {
        for (int i = 0; i < _itemList.Count; i++)
        {
            UIMultiScrollIndex item = _itemList[i];
            if (item.Index >= index) item.Index += 1;
        }
        CreateItem(index);
    }

    private void DelItemFromPanel(int index)
    {
        int maxIndex = -1;
        int minIndex = int.MaxValue;
        for (int i = _itemList.Count; i > 0; i--)
        {
            UIMultiScrollIndex item = _itemList[i - 1];
            if (item.Index == index)
            {
                GameObject.Destroy(item.gameObject);
                _itemList.Remove(item);
            }
            if (item.Index > maxIndex)
            {
                maxIndex = item.Index;
            }
            if (item.Index < minIndex)
            {
                minIndex = item.Index;
            }
            if (item.Index > index)
            {
                item.Index -= 1;
            }
        }
        if (maxIndex < DataCount - 1)
        {
            CreateItem(maxIndex);
        }
    }

    private void CreateItem(int index)
    {
        UIMultiScrollIndex itemBase;
        if (_unUsedQueue.Count > 0)
        {
            itemBase = _unUsedQueue.Dequeue();
        }
        else
        {
            itemBase = CatGameTools.AddChild(_content, itemPrefab).GetComponent<UIMultiScrollIndex>();
        }


		itemBase.AddListener(ItemClickHandler,ItemStartHandler);
        itemBase.Scroller = this;
        itemBase.Index = index;
        _itemList.Add(itemBase);
    }

    private void ItemClickHandler(UIMultiScrollIndex item)
    {
        if (item && itemClickBack != null) itemClickBack(item.Index);
    }

	private void ItemStartHandler(UIMultiScrollIndex item)
	{
		if (item && itemStartBack != null) itemStartBack(item.Index);
	}

    private int GetPosIndex()
    {
        switch (_movement)
        {
            case Arrangement.Horizontal:
                return Mathf.FloorToInt(_content.anchoredPosition.x / -(cellWidth + cellPadding));
            case Arrangement.Vertical:
                return Mathf.FloorToInt(_content.anchoredPosition.y / (cellHeight + cellPadding));
        }
        return 0;
    }

    public Vector3 GetPosition(int i)
    {
        switch (_movement)
        {
            case Arrangement.Horizontal:
                return new Vector3(cellWidth * (i / maxPerLine), -(cellHeight + cellPadding) * (i % maxPerLine), 0f);
            case Arrangement.Vertical:
                return new Vector3(cellWidth * (i % maxPerLine), -(cellHeight + cellPadding) * (i / maxPerLine), 0f);
        }
        return Vector3.zero;
    }

    public int DataCount
    {
        get { return _dataCount; }
        set
        {
            _dataCount = value;
            UpdateTotalWidth();
        }
    }

    private void UpdateTotalWidth()
    {
        int lineCount = Mathf.CeilToInt((float)_dataCount / maxPerLine);
        //Debug.Log("_dataCount=" + _dataCount + "|" + maxPerLine);
        switch (_movement)
        {
            case Arrangement.Horizontal:
                _content.sizeDelta = new Vector2(cellWidth * lineCount + cellPadding * (lineCount - 1), _content.sizeDelta.y);
                break;
            case Arrangement.Vertical:
                _content.sizeDelta = new Vector2(_content.sizeDelta.x, cellHeight * lineCount + cellPadding * (lineCount - 1));
                break;
        }
    }
}
