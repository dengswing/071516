using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对话动画控制
/// </summary>
public class DialogueManager : MonoBehaviour
{
    //开始对话
    private GameObject dialogue;
    private Action finishBack;
    DialogueBubble bubble;

    /// <summary>
    /// 开始显示对话
    /// </summary>
    /// <param name="info"></param>
    public void StartDailogue(string info, Action callBack)
    {
        finishBack = callBack;
        DailogueShow(info);
    }

    private void DailogueShow(string info)
    {
        if (dialogue == null)
        {
            var prefab = CatGameTools.GetResources<GameObject>(CatGameConst.ResourcesDialogueBubble);
            dialogue = CatGameTools.AddChild(transform, prefab);          
        }

        var infoText = dialogue.transform.Find("InfoText").GetComponent<Text>();
        infoText.text = info;
        dialogue.SetActive(true);

        bubble = dialogue.transform.GetComponent<DialogueBubble>();
        bubble.AddListener(DialogueDisappear);
    }

    void DialogueDisappear()
    {
        if (finishBack != null) finishBack();
        finishBack = null;
        GameObject.DestroyImmediate(dialogue);
        dialogue = null;
    }

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {
       
    }
}
