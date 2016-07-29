using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 音符的管理
/// </summary>
public class MusicNoteManger : MonoBehaviour
{
    public int randomMin = 1;
    public int randomMax = 3;

    public LightLoopManger lightLoopManager;
    public GameObject itemPrefab;

    private Dictionary<string, MusicNoteData> sensitivitySiteList;
    private FondleManager fondleManager;
    private MusicNoteData currentCatAction;
    private int showMusicNoteCount;
    private bool isTouchNusicNote;

    void Awake()
    {

        sensitivitySiteList = new Dictionary<string, MusicNoteData>();
        fondleManager = FondleManager.Instance;

    }

    public void Cancel()
    {
        if (currentCatAction != null)
        {
            if (lightLoopManager.lightLoopState == LightLoopState.ZoomOut)
            {
                //可以取消
                lightLoopManager.CancleLightLoop();
            }
        }
    }

    public bool isOver
    {
        get { return !isTouchNusicNote && (lightLoopManager.lightLoopState == LightLoopState.Normal); }
    }

    public void Show(string name, Vector2 localPosition, bool isShowNote)
    {
        Debug.Log("musicNote===>123" + name + "|" + isTouchNusicNote);
        if (!isShowNote || isTouchNusicNote) return;

        if (!gameObject.activeSelf) gameObject.SetActive(true);

        MusicNoteData musicNote;
        if (sensitivitySiteList.TryGetValue(name, out musicNote))
        {
            if (musicNote.musicNoteCount <= 0)
            { //没有次数了

                Debug.Log("没有次数了");
                return;
            }
        }
        else
        {
            musicNote = new MusicNoteData();
            musicNote.name = name;
            sensitivitySiteList[name] = musicNote;
            musicNote.musicNoteCount = musicNote.musicNoteMax = Random.Range(randomMin, randomMax + 1);
            musicNote.localPosition = localPosition;
        }

        if (currentCatAction != null && !currentCatAction.Equals(musicNote))
        {//检查是否取消触摸
            if (lightLoopManager.lightLoopState == LightLoopState.ZoomOut)
            {
                //上一个可以取消
                lightLoopManager.CancleLightLoop();
                Debug.Log("取消上一个");
                return;
            }
            else if (lightLoopManager.lightLoopState == LightLoopState.ZoomIn)
            {
                Debug.Log("等上一个取消完成" + lightLoopManager.lightLoopState);
                return;
            }
        }

        //Debug.Log("musicNote===>" + name);
        currentCatAction = musicNote;
        lightLoopManager.AddListener(FinishHandler);
        lightLoopManager.Show(name, localPosition);
    }

    public void Hide()
    {
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示的音符数量
    /// </summary>
    public int musiceNoteCount
    {
        get { return showMusicNoteCount; }
    }

    void OnDisable()
    {
        showMusicNoteCount = 0;
        foreach (var item in sensitivitySiteList)
        {
            GameObject.DestroyImmediate(item.Value.musicNote);
        }

        sensitivitySiteList.Clear();
        currentCatAction = null;
        isTouchNusicNote = false;
    }

    void Start()
    {

    }

    void FinishHandler()
    {
        var musicNote = currentCatAction;
        if (musicNote == null) return;
        isTouchNusicNote = true;
        TouchAction(musicNote);
        ShowMusiceNote(musicNote, musicNote.localPosition);
    }

    void ShowMusiceNote(MusicNoteData musicNote, Vector2 position)
    {
        if (musicNote.musicNote == null)
        {
            musicNote.musicNote = CatGameTools.AddChild(transform, itemPrefab).GetComponent<MusicNoteEffect>();
        }

        musicNote.musicNote.transform.localPosition = position;
        if (musicNote.isTouch) return;

        musicNote.isTouch = true;
        musicNote.musicNote.AddListener(PlayerFinish, musicNote);
        musicNote.musicNote.Show();
        musicNote.musicNoteCount -= 1;
        showMusicNoteCount += 1;
    }

    void TouchAction(MusicNoteData musicNote)
    {
        if (musicNote.isPlay) return;
        musicNote.isPlay = true;
        if (fondleManager.catSensitivity != null) fondleManager.catSensitivity(musicNote.name, () =>
        {
            musicNote.isPlay = false;
            currentCatAction = null;
            isTouchNusicNote = false;
        });
    }

    void PlayerFinish(object musicNote)
    {
        ((MusicNoteData)musicNote).isTouch = false;
      //  isTouchNusicNote = false;
    }

}

class MusicNoteData
{
    public string name;
    public int count;
    public bool isPlay;
    public MusicNoteEffect musicNote;
    public int musicNoteCount;
    public int musicNoteMax;
    public bool isTouch;
    public Vector2 localPosition;
}