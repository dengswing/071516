using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 喂食道具
/// </summary>
public class FeedItem : UIMultiScrollIndex
{
    float time;
    float endTime = 0.5f;
    DragImage image;
    GameObject currentDrag;
    bool isEat;

    void Awake()
    {
        image = transform.GetComponentInChildren<DragImage>();
        image.AddListener(DragEnd);
    }

    void DragEnd(GameObject drag)
    {
        currentDrag = drag;
        var dragTransform = currentDrag.transform;
        var position = dragTransform.localPosition;

        time = 0;
        isEat = (position.y >= 0);

        currentDrag.AddComponent<CanvasGroup>();
        if (isEat)
        {
            DragImageEffect(currentDrag, dragTransform.localPosition, new Vector3(0, 0, position.z), dragTransform.localScale, new Vector3(0.8f, 0.8f, 0.8f));
        }
        else
        {
            Canvas canvas = DragImage.FindInParents<Canvas>(currentDrag);
            Vector2 globalMousePos = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, transform.position);

            Vector2 globalPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dragTransform.parent.transform as RectTransform, globalMousePos, canvas.worldCamera, out globalPos);
            globalPos.x += (dragTransform as RectTransform).rect.width * 0.75f * .5f;
            globalPos.y -= (dragTransform as RectTransform).rect.height * 0.75f * .5f;
            DragImageEffect(currentDrag, dragTransform.localPosition, globalPos, dragTransform.localScale, new Vector3(0.75f, 0.75f, 0.75f));
        }
    }

    void DragImageEffect(GameObject gameObject, Vector3 startPositionVector, Vector3 endPositionVector, Vector3 startScaleVector, Vector3 endScaleVector)
    {
		float duration = endTime;

        CatGameTools.UIGameObjectScale(gameObject, startScaleVector, endScaleVector, duration);
        CatGameTools.UIGameObjectAlpha(gameObject, 1, 0, duration);
        CatGameTools.UIGameObjectPosition(gameObject, startPositionVector, endPositionVector, duration);

		if (isEat && startClickBack != null)
			startClickBack (this);
    }

    void Update()
    {
        time += Time.deltaTime;
        if (currentDrag && time > endTime)
        {
			if (isEat && clickBack != null) clickBack(this);
            GameObject.DestroyImmediate(currentDrag);
            currentDrag = null;
        }
    }

    public override int Index
    {
        get
        {
            return base.Index;
        }

        set
        {
            base.Index = value;
            ChangeProperty(Index);
        }
    }

    void ChangeProperty(int index)
    {
        index++;
        var texture = CatGameTools.GetResources<Texture2D>(string.Format(CatGameConst.ResourcesItem, index));

        var img = transform.FindChild("IconImage").GetComponent<Image>();
        var text = transform.GetComponentInChildren<Text>();
        var bgimg = transform.FindChild("Bg");
        var bgUseless = transform.FindChild("BgUseless");

        if (texture != null)
        {
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));

            img.sprite = sprite;
            text.text = Random.Range(1, 10).ToString();

            img.gameObject.SetActive(true);
            bgimg.gameObject.SetActive(true);
            bgUseless.gameObject.SetActive(false);
        }
        else
        {
            img.gameObject.SetActive(false);
            text.text = string.Empty;
            bgimg.gameObject.SetActive(false);
            bgUseless.gameObject.SetActive(true);
        }
    }
}
