using Catopia;
using UnityEngine;

/// <summary>
/// 喂食商店管理
/// </summary>
public class FeedItemManager : MonoBehaviour
{
    public int useless = 10;
    private int[] itemCount = new int[] { 4, 2, 10 };
    private UIMultiScroller itemList;
    private FeedClickDelegate itemClick;

    /// <summary>
    /// 显示商品
    /// </summary>
    /// <param name="index"></param>
    public void ShowItem(int index)
    {
        CreateItem(index);
    }

    /// <summary>
    /// 喂食侦听
    /// </summary>
    /// <param name="callBack"></param>
    public void AddListener(FeedClickDelegate callBack)
    {
        itemClick = callBack;
    }

    void Awake()
    {
       
    }

    void Start()
    {
        itemList = transform.GetComponent<UIMultiScroller>();
        CreateItem();
    }

    void CreateItem(int index = 1)
    {
        if (index <= 0 || index > itemCount.Length) index = 1;
        itemList.AddListenerItemClick(ItemClickHandler);
        itemList.Reset();
        itemList.DataCount = itemCount[index - 1] + useless;
        itemList.StartShow();
    }

    void ItemClickHandler(int index)
    {
        var eatState = EAT_STATE.Yummy;
        switch (index)
        {
            case 0:
                eatState = EAT_STATE.Yuck;
                break;
            case 1:
                eatState = EAT_STATE.Yawm;
                break;
            case 2:
                eatState = EAT_STATE.Yummy;
                break;
            default:
                break;
        }

        Debug.Log("===>" + index + "|" + eatState);
        if (itemClick != null) itemClick(eatState);
    }
}
