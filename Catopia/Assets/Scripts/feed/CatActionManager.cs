using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 吃东西的状态
/// </summary>
public enum CAT_STATE
{
    IDEL,
    EAT,
    Yuck, //讨厌
    Yummy, //愉快的
    Yawm, //一般
    YummyChange,
    YuckChange,
}

/// <summary>
/// 猫动作管理
/// </summary>
public class CatActionManager : MonoBehaviour
{
    SpineAnimationManager spineAnimation;
    Action animationBack;
    List<string> actionList;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eatState"></param>
    public void Feed(CAT_STATE eatState, Action callBack)
    {
        animationBack = callBack;

        switch (eatState)
        {
            case CAT_STATE.IDEL:
                actionList = new List<string> { CatGameConst.CatIdle };
                PlayAnimation(actionList);
                break;
            case CAT_STATE.EAT:
                actionList = new List<string> { CatGameConst.CatEat };
                PlayAnimation(actionList);
                break;
            case CAT_STATE.Yuck:
                actionList = new List<string>(CatGameConst.CatYuckList.ToArray());
                PlayAnimation(actionList);
                break;
            case CAT_STATE.Yummy:
            case CAT_STATE.Yawm:
                actionList = new List<string>(CatGameConst.CatYummyList.ToArray());
                PlayAnimation(actionList);
                break;
            case CAT_STATE.YummyChange:
                actionList = new List<string> { CatGameConst.CatYummyChange };
                PlayAnimation(actionList);
                break;
            case CAT_STATE.YuckChange:
                actionList = new List<string> { CatGameConst.CatYuckChange };
                PlayAnimation(actionList);
                break;
            default:
                actionList = new List<string>(CatGameConst.CatYummyList.ToArray());
                PlayAnimation(actionList);
                break;
        }
    }

    public void FondleAction(string name, Action callBack)
    {
        animationBack = () =>
        {
            spineAnimation.PlayAnimation(CatGameConst.CatIdle, true, null);
            if (callBack != null)
            {
                callBack();
                callBack = null;
            }
        };
        actionList = new List<string> { name };
        PlayAnimation(actionList);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Debug.Log("===>>>" + coll.gameObject.name);
    }

    void Start()
    {
        spineAnimation = transform.GetComponent<SpineAnimationManager>();
    }

    void PlayAnimation(List<string> animation, bool isLoop = false)
    {
        if (animation.Count <= 0)
        {
            if (animationBack != null) animationBack();
            animationBack = null;
            return;
        }

        var act = animation[0];
        spineAnimation.PlayAnimation(act, isLoop, () =>
        {
            animation.RemoveAt(0);

            if (animation.Count == 1) isLoop = true; //最后一个一直播放
            PlayAnimation(animation, isLoop);

            if (act == CatGameConst.CatYummyChange || act == CatGameConst.CatYuckChange) spineAnimation.PlayAnimation(CatGameConst.CatIdle, true, null);
        });
    }
}
