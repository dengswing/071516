using System;
using UnityEngine;

public class UIMultiScrollIndex : MonoBehaviour
{
    protected Action<UIMultiScrollIndex> clickBack;

    private UIMultiScroller _scroller;
    private int _index;

    void Awake()
    {

    }

    void Start()
    {

    }

    public virtual void AddListener(Action<UIMultiScrollIndex> callBack)
    {
        clickBack = callBack;
    }

    public virtual int Index
    {
        get { return _index; }
        set
        {
            _index = value;
            transform.localPosition = _scroller.GetPosition(_index);
            gameObject.name = string.Format("{0}_{1}", transform.name, _index);
        }
    }

    public UIMultiScroller Scroller
    {
        set { _scroller = value; }
    }
}
