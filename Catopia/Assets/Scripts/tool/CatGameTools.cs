using DG.Tweening;
using System.Collections.Generic;
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
        var go = Resources.Load<T>(path);
        return go;
    }

    static public void UIGameObjectAlpha(GameObject gameObject, float startAlpha, float endAlpha, float duration)
    {
        var canvas = gameObject.GetComponent<CanvasGroup>();
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, startAlpha, 0);
        DOTween.To(() => canvas.alpha, x => canvas.alpha = x, endAlpha, duration);
    }

    static public void UIGameObjectAlphaBack(CanvasGroup canvas, float endAlpha, float duration, System.Action finishBack = null)
    {
        Tween t = DOTween.To(() => canvas.alpha, x => canvas.alpha = x, endAlpha, duration);
        t.OnComplete(() =>
        {
            if (finishBack != null) finishBack();
            finishBack = null;
        });
    }

    static public void UIGameObjectScale(GameObject gameObject, Vector3 startScaleVector, Vector3 endScaleVector, float duration)
    {
        var rect = gameObject.GetComponent<RectTransform>();
        rect.DOKill();
        rect.DOScale(startScaleVector, 0f);
        rect.DOScale(endScaleVector, duration);
    }

    static public void UIGameObjectScaleBack(RectTransform rect, Vector3 endScaleVector, float duration, System.Action finishBack = null)
    {
        rect.DOKill();
        var t = rect.DOScale(endScaleVector, duration);
        t.OnComplete(() =>
        {
            if (finishBack != null) finishBack();
            finishBack = null;
        });
    }

    static public void UIGameObjectPosition(GameObject gameObject, Vector3 startPositionVector, Vector3 endPositionVector, float duration)
    {
        var rect = gameObject.GetComponent<RectTransform>();
        rect.DOKill();
        rect.DOLocalMove(startPositionVector, 0f);
        rect.DOLocalMove(endPositionVector, duration);
    }

    static public void UIGameObjectPositionBack(RectTransform rect, Vector3 endPositionVector, float duration, System.Action finishBack = null)
    {
        rect.DOKill();
        var t = rect.DOLocalMove(endPositionVector, duration);
        t.OnComplete(() =>
        {
            if (finishBack != null) finishBack();
            finishBack = null;
        });
    }

    static public List<T> RandomContent<T>(List<T> data, int min, int max)
    {
        if (data == null) return null;
        var randomList = new List<T>();
        var tmpList = new List<T>(data.ToArray());
        var length = Random.Range(min, max + 1);
        if (length >= data.Count) length = data.Count;
        for (int i = 0; i < length; i++)
        {
            var index = Random.Range(0, tmpList.Count);
            var value = tmpList[index];
            tmpList.RemoveAt(index);
            randomList.Add(value);
        }

        return randomList;
    }
}