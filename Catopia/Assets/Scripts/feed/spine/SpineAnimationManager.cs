using UnityEngine;
using System;

/// <summary>
/// spine动画管理
/// </summary>
public class SpineAnimationManager : MonoBehaviour
{
    private SkeletonGraphic skeleton;

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="act"></param>
    /// <param name="loop"></param>
    /// <param name="callBack"></param>
    public void PlayAnimation(string act, bool loop, Action callBack)
    {
        SetSkeleton(act, loop, callBack);
    }

    void Awake()
    {
        skeleton = transform.GetComponentInChildren<SkeletonGraphic>();
    }

    void SetSkeleton(string act, bool loop, Action callBack)
    {
      //  Debug.Log("act: " + act);
        skeleton.startingAnimation = act;
        skeleton.startingLoop = loop;
        var trackEntry = skeleton.Initialize(true);
        trackEntry.End += (state, trackIndex) =>
        {           
            if (callBack != null) callBack();
        };
    }
}
