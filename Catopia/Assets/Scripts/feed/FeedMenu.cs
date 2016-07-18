using UnityEngine;
using UnityEngine.UI;

public class FeedMenu : MonoBehaviour
{
    public GameObject btnPrefab;
    private string[] btnName = new string[] { "A", "B", "C" };
    public RectTransform _content;

    void Awake()
    {
        CreateMenu();
    }

    void Start()
    {

    }

    void CreateMenu()
    {
        FeedButton btn;
        foreach (var item in btnName)
        {
            btn = GameTools.AddChild(_content, btnPrefab).GetComponent<FeedButton>();
            btn.BtnTitle = item;
        }
    }
}
