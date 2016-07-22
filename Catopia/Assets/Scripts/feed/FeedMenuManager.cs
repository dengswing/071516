using Catopia;
using UnityEngine;

/// <summary>
/// 菜单管理
/// </summary>
public class FeedMenuManager : MonoBehaviour
{
    public GameObject btnPrefab;
    public RectTransform content;

    private string[] btnName;
    private int padding = 10;

    private FeedMenu currentMenu;
    private FeedMenuClickDelegate menuClick;

    /// <summary>
    /// 添加菜单点击委托
    /// </summary>
    /// <param name="callBack"></param>
    public void AddListener(FeedMenuClickDelegate callBack)
    {
        menuClick = callBack;
    }

    void Awake()
    {
        btnName = CatGameConst.MenuBtnName;
        CreateMenu();
    }

    void Start()
    {

    }

    void CreateMenu()
    {
        FeedMenu btn;
        int cellPositionX = 0;
        int length = btnName.Length;
        for (int i = 0; i < length; i++)
        {
            var item = btnName[i];
            btn = CatGameTools.AddChild(content, btnPrefab).GetComponent<FeedMenu>();
            btn.BtnTitle = item;
            btn.BtnPosition = new Vector3(cellPositionX, 0f, 0f);
            btn.AddListener(OnSelectHadnler);
            btn.Index = i + 1;
            cellPositionX = (int)btn.PositionX + padding;

            if (i == 0) currentMenu = btn;
        }

        SelectMenu(currentMenu);
    }

    void SelectMenu(FeedMenu menu)
    {
        if (currentMenu != null) currentMenu.ChangeState();
        currentMenu = menu;
        if (currentMenu != null) currentMenu.ChangeState(true);
    }

    void OnSelectHadnler(FeedMenu menu)
    {
        SelectMenu(menu);
        if (menuClick != null) menuClick(menu.Index);
    }
}
