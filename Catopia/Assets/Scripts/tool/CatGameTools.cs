using DG.Tweening;
using UnityEngine;

public static class CatGameTools
{
    static public GameObject AddChild(Transform parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent, false);
            go.layer = parent.gameObject.layer;
        }
        return go;
    }

    static public T GetResources<T>(string path) where T : Object
    {
      //  Debug.Log("GetResources path =>" + Resources.Load(path));
        var go = Resources.Load<T>(path);
        return go;
    }

    static public void UIGameObjectAlpha(GameObject gameObject, float startAlpha, float endAlpha, float duration)
    {
        var canvas = gameObject.GetComponent<CanvasGroup>();
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, startAlpha, 0);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, endAlpha, duration);
    }
    static public void UIGameObjectScale(GameObject gameObject, Vector3 startScaleVector, Vector3 endScaleVector, float duration)
    {
        var rect = gameObject.GetComponent<RectTransform>();
        rect.DOScale(startScaleVector, 0f);
        rect.DOScale(endScaleVector, duration);
    }

    static public void UIGameObjectPosition(GameObject gameObject, Vector3 startPositionVector, Vector3 endPositionVector, float duration)
    {
        var rect = gameObject.GetComponent<RectTransform>();
        rect.DOLocalMove(startPositionVector, 0f);
        rect.DOLocalMove(endPositionVector, duration);
    }
}