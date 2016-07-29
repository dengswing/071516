using UnityEngine;
using System;

/// <summary>
/// 音符效果
/// </summary>
public class MusicNoteEffect : MonoBehaviour
{
    private Action<object> finishBack;
    private SpineAnimationManager spineAnimation;
    private bool isPlayer;
    private object param;

    void Awake()
    {
        spineAnimation = transform.GetComponent<SpineAnimationManager>();
    }

    public void AddListener(Action<object> back, object param)
    {
        finishBack = back;
        this.param = param;
    }

    public void Show()
    {
        if (!gameObject.activeSelf) gameObject.SetActive(true);
        PlayerEffect();
        CatGameTools.UIGameObjectAlpha(gameObject, 0, 1, 0.1f);
    }

    public void Hide()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }

    void Start()
    {

    }

    void PlayerEffect()
    {
        if (isPlayer) return;
        spineAnimation.PlayAnimation(CatGameConst.MusicNote, false, PlayerFinish);
        isPlayer = true;
    }

    void PlayerFinish()
    {
        isPlayer = false;
        CatGameTools.UIGameObjectAlpha(gameObject, 1, 0, 0);

        if (finishBack != null) finishBack(param);
        finishBack = null;
    }
}