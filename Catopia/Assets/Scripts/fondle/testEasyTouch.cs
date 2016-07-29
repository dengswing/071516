using UnityEngine;
using System.Collections.Generic;

public class testEasyTouch : MonoBehaviour
{

    void Start()
    {
        //添加一个手指滑动的事件。
        EasyTouch.On_SwipeEnd += EasyTouch_On_SwipeEnd;
        EasyTouch.On_SwipeStart += On_SwipeStart;
        EasyTouch.On_Swipe += On_Swipe;

    }

    void OnDestroy()
    {

    }

    //当手指滑动结束时在这里
    void EasyTouch_On_SwipeEnd(Gesture gesture)
    {
        Debug.Log("end");
    }

    private void On_SwipeStart(Gesture gesture)
    {
        Time.timeScale = 0.5f;
        Debug.Log("start");
    }

    // 滑动中效果，比如划过就画出一条线。
    private void On_Swipe(Gesture gesture)
    {
        Debug.Log("obj==>" + gesture.GetCurrentFirstPickedUIElement()+"|"+ gesture.GetCurrentPickedObject()+"|"+ gesture.pickedObject+"|"+gesture.pickedUIElement);
    }
}