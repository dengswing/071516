using UnityEngine;
using UnityEngine.UI;

public class FeedButton : MonoBehaviour
{
    Text nameTxt;

    void Awake()
    {
        var btn = transform.GetComponent<Button>();
        btn.onClick.AddListener(OnBtnHandler);
        nameTxt = transform.GetComponentInChildren<Text>(); 
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
   

    void OnBtnHandler()
    {
        Debug.Log("XXX");
    }
}
