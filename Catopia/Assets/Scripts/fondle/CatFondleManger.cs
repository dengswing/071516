using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 抚摸猫管理
/// </summary>
public class CatFondleManger : MonoBehaviour
{
    public GameObject hand;
    public List<GameObject> sensitivityList;
    public MusicNoteManger musicNote;
    public int randomMin = 1;
    public int randomMax = 3;

    private HandCircleEffect handEffect;
    private List<GameObject> currentSensitivityList;
    private bool _isShowParticle;

    void Start()
    {

    }

    public bool isShowParticle
    {
        get { return _isShowParticle; }
    }

    /// <summary>
    /// 显示
    /// </summary>
    public void Show()
    {
        EasyTouch.On_SwipeEnd += On_SwipeEnd;
        EasyTouch.On_SwipeStart += On_SwipeStart;
        EasyTouch.On_Swipe += On_Swipe;
        EasyTouch.On_SimpleTap += On_SimpleTap;

        RandomSensitivity();
    }

    /// <summary>
    /// 随机部位
    /// </summary>
    public void RandomSite()
    {
        RandomSensitivity();
    }

    void OnDestroy()
    {

    }

    void OnEnable()
    {
        Show();
    }

    void OnDisable()
    {
        if (hand.activeSelf) hand.SetActive(false);
        EasyTouch.On_SwipeEnd -= On_SwipeEnd;
        EasyTouch.On_SwipeStart -= On_SwipeStart;
        EasyTouch.On_Swipe -= On_Swipe;
        EasyTouch.On_SimpleTap -= On_SimpleTap;
    }
    void On_SimpleTap(Gesture gesture)
    {
        Debug.Log("click");
       DetectionSensitivity(gesture.GetCurrentFirstPickedUIElement());
    }

    void On_SwipeEnd(Gesture gesture)
    {
        Debug.Log("end");
        if (hand.activeSelf) hand.SetActive(false);
        musicNote.Cancel();
    }

    private void RandomSensitivity()
    {
        currentSensitivityList = CatGameTools.RandomContent<GameObject>(sensitivityList, randomMin, randomMax);
    }

    private void DetectionSensitivity(GameObject obj)
    {
        if (obj == null) return;
        var layer = LayerMask.NameToLayer("UIFondle");
        if (obj.layer != layer) return;

        var length = sensitivityList.Count;
        for (int i = 0; i < length; i++)
        {
            if (sensitivityList[i] == obj)
            {
                //触发事件
                var index = currentSensitivityList.IndexOf(obj);
                // Debug.Log("act==>" + index);
                _isShowParticle = true;
                musicNote.Show(obj.name, obj.transform.localPosition, (index != -1));
                break;
            }
        }
    }

    private void On_SwipeStart(Gesture gesture)
    {
        Debug.Log("start");
        if (!hand.activeSelf) hand.SetActive(true);
    }

    // 滑动中效果，比如划过就画出一条线。
    private void On_Swipe(Gesture gesture)
    {
        Vector2 globalMousePos = RectTransformUtility.WorldToScreenPoint(gesture.pickedCamera, gesture.position);
        Canvas canvas = DragImage.FindInParents<Canvas>(hand);
        Vector2 globalPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(hand.transform.parent.transform as RectTransform, globalMousePos, canvas.worldCamera, out globalPos);

        var x1 = hand.transform.localPosition.x;
        var x2 = globalPos.x;

        var y1 = hand.transform.localPosition.y;
        var y2 = globalPos.y;
        var gap = 5;

        if (Mathf.Abs(x1 - x2) <= gap && Mathf.Abs(y1 - y2) <= gap)
        { //限制太密集
            return;
        }

        _isShowParticle = false;
        hand.transform.localPosition = globalPos;
        DetectionSensitivity(gesture.GetCurrentFirstPickedUIElement());

       // Debug.Log("obj==>" + gesture.GetCurrentFirstPickedUIElement() + "|" + gesture.GetCurrentPickedObject() + "|" + gesture.pickedObject + "|" + gesture.pickedUIElement);
    }
}