using UnityEngine;
using System.Collections;

public class CatEasyTouch : MonoBehaviour
{
    //头像
    GameObject icon;
    void Start()
    {
       // icon = transform.Find("icon").gameObject;
        //添加一个手指滑动的事件。
        EasyTouch.On_SwipeEnd += EasyTouch_On_SwipeEnd;

        EasyTouch.On_SwipeStart += On_SwipeStart;
        EasyTouch.On_Swipe += On_Swipe;
        EasyTouch.On_SwipeEnd += EasyTouch_On_SwipeEnd;
    }

    void OnDestroy()
    {
        //施放一个手指滑动的事件。
        EasyTouch.On_SwipeEnd -= EasyTouch_On_SwipeEnd;
    }

    //当手指滑动结束时在这里
    void EasyTouch_On_SwipeEnd(Gesture gesture)
    {
        Debug.Log("end");
        //判断手指是否触摸在NGUI的头像中
        if (gesture.GetCurrentFirstPickedUIElement() == icon)
        {
            //输出手指滑动的方向
            Debug.Log("hello");
        }
    }

    private void On_SwipeStart(Gesture gesture)
    {
        Time.timeScale = 0.5f;
        Debug.Log("start");
    }

    // 滑动中效果，比如划过就画出一条线。
    private void On_Swipe(Gesture gesture)
    {
        Debug.Log("progress=>"+gesture.GetCurrentFirstPickedUIElement()+"  :  =>"+gesture.GetCurrentPickedObject());
    }


    //计算出NGUI某个UISprite或者UITexture或者 UILabel 在屏幕中占的矩形位置。
    /* private Rect NGUIObjectToRect(GameObject go)
     {
         Camera camera = NGUITools.FindCameraForLayer(go.layer);
         Bounds bounds = NGUIMath.CalculateAbsoluteWidgetBounds(go.transform);
         Vector3 min = camera.WorldToScreenPoint(bounds.min);
         Vector3 max = camera.WorldToScreenPoint(bounds.max);
         return new Rect(min.x, min.y, max.x - min.x, max.y - min.y);
     }*/

}