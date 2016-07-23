using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 吃东西的状态
/// </summary>
public enum EAT_STATE
{
    IDEL,
    EAT,
    Yuck,
    Yummy,
    Yawm,
    YummyChange,
    YuckChange
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
    public void Feed(EAT_STATE eatState, Action callBack)
    {
        animationBack = callBack;

        switch (eatState)
        {
            case EAT_STATE.IDEL:
                actionList = new List<string> { CatGameConst.CatIdle };
                PlayAnimation(actionList);
                break;
            case EAT_STATE.EAT:
                actionList = new List<string> { CatGameConst.CatEat };
                PlayAnimation(actionList);
                break;
            case EAT_STATE.Yuck:
                actionList = new List<string>(CatGameConst.CatYuckList.ToArray());
                PlayAnimation(actionList);
                break;
            case EAT_STATE.Yummy:
            case EAT_STATE.Yawm:
                actionList = new List<string>(CatGameConst.CatYummyList.ToArray());
                PlayAnimation(actionList);
                break;
            case EAT_STATE.YummyChange:
                actionList = new List<string> { CatGameConst.CatYummyChange };
                PlayAnimation(actionList);
                break;
            case EAT_STATE.YuckChange:
                actionList = new List<string> { CatGameConst.CatYuckChange };
                PlayAnimation(actionList);
                break;
            default:
                actionList = new List<string>(CatGameConst.CatYummyList.ToArray());
                PlayAnimation(actionList);
                break;
        }
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
