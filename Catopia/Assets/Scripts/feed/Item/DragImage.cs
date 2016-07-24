using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragImage : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool dragOnSurfaces = true;
    public ScrollRect ScrollRect;
    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;

    private Action<GameObject> dragEndBack;
    Vector2 onBeginDragPosition;
    bool isDragItem;
    float scrollX;

    void Start()
    {
        if (ScrollRect == null) ScrollRect = FindInParents<ScrollRect>(gameObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onBeginDragPosition = eventData.position;
        scrollX = onBeginDragPosition.x;
        Debug.Log("==>" + onBeginDragPosition);

        ScrollRect.OnBeginDrag(eventData);
    }

    void CreateDraggedIcon(PointerEventData data)
    {
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;

        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        m_DraggingIcon = new GameObject("dragIcon");

        m_DraggingIcon.transform.SetParent(canvas.transform, false);
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.AddComponent<Image>();
        // The icon will be under the cursor.
        // We want it to be ignored by the event system.
        m_DraggingIcon.AddComponent<IgnoreRaycast>();

        image.sprite = GetComponent<Image>().sprite;
        image.SetNativeSize();

        if (dragOnSurfaces)
            m_DraggingPlane = transform as RectTransform;
        else
            m_DraggingPlane = canvas.transform as RectTransform;

        SetDraggedPosition(data);
    }

    public void OnDrag(PointerEventData data)
    {
       
        if (m_DraggingIcon != null)
        {
            SetDraggedPosition(data);
        }
        else
        {
            isDragItem = true;
            Vector2 tempPosition = data.position;
            float scrollMoveX = data.delta.x - scrollX;
            scrollX = tempPosition.x;
            if ((tempPosition.x > onBeginDragPosition.x) &&
               Mathf.Abs(tempPosition.x - onBeginDragPosition.x) < Mathf.Abs(tempPosition.y - onBeginDragPosition.y))
            //data.pointerCurrentRaycast.gameObject == data.pointerPressRaycast.gameObject)
            {
                CreateDraggedIcon(data);
            }
            else if (Mathf.Abs(tempPosition.y - onBeginDragPosition.y) < Mathf.Abs(tempPosition.x - onBeginDragPosition.x))
            {
               // data.scrollDelta = new Vector2(data.delta.x * .2f, 0);
               // data.IsScrolling();
                ScrollRect.OnDrag(data);
                isDragItem = false;               
            }
        }
    }

    public void AddListener(Action<GameObject> dragEndBack)
    {
        this.dragEndBack = dragEndBack;
    }

    private void SetDraggedPosition(PointerEventData data)
    {
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // if (!isDragItem) eventData.scrollDelta = new Vector2(eventData.delta.x * .2f, 0);

        ScrollRect.OnEndDrag(eventData);

        if (m_DraggingIcon != null)
        {
            if (dragEndBack != null) dragEndBack(m_DraggingIcon);
        }
        //Destroy(m_DraggingIcon);
    }

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}