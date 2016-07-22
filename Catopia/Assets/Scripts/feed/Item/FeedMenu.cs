using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 喂食菜单按钮
/// </summary>
public class FeedMenu : MonoBehaviour
{
    private Text nameTxt;
    private RectTransform rect;
    private Action<FeedMenu> menuClickBack;
    private int _index;
    private Button menuBtn;

    void Awake()
    {
        menuBtn = transform.GetComponent<Button>();
        menuBtn.onClick.AddListener(OnBtnHandler);
        nameTxt = transform.GetComponentInChildren<Text>();
        rect = transform.GetComponent<RectTransform>();
    }

    void Start()
    {

    }

    public string BtnTitle
    {
        set
        {
            nameTxt.text = value;
        }
    }

    public Vector3 BtnPosition
    {
        set
        {
            transform.localPosition = value;
        }
    }

    public float PositionX
    {
        get
        {
            return rect.rect.width + transform.localPosition.x;
        }
    }

    public int Index
    {
        get
        {
            return _index;
        }

        set
        {
            _index = value;
        }
    }

    public void ChangeState(bool isChecked = false)
    {
        int index = (isChecked ? 2 : 1);
        var texture = CatGameTools.GetResources<Texture2D>(string.Format(CatGameConst.ResourcesMenu, index));
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        var img = transform.GetComponent<Image>();
        img.sprite = sprite;

        menuBtn.enabled = !isChecked;
        var txt = transform.GetComponentInChildren<Text>();
        var tColor = Color.red;
        if (menuBtn.enabled)
        {
            tColor = new Color(0.388f, 0.427f, 0.529f);
        }
        else
        {
            tColor = new Color(1, 1, 1);
        }
        
        txt.color = tColor;
    }

    public void AddListener(Action<FeedMenu> callBack)
    {
        menuClickBack = callBack;
    }

    void OnBtnHandler()
    {
        if (menuClickBack != null) menuClickBack(this);
    }
}
