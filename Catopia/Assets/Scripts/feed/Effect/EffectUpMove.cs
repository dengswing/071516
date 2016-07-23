using UnityEngine;

/// <summary>
/// 动画移动
/// </summary>
public class EffectUpMove : MonoBehaviour
{
    public float startTime = 0f;
    public float duration = 2f;
    public bool isStartRandom = false;

    Vector3 startPosition;
    Vector3 endPosition;

    float sTime;
    bool isDoing;

    void Start()
    {
        if (isStartRandom) RandomPosition();
    }

    public void SetRandomPosition(bool isFirst = false)
    {
        RandomPosition(isFirst);
    }

    void StartDoing()
    {
        CatGameTools.UIGameObjectPosition(gameObject, startPosition, endPosition, duration);
    }

    void OnDisable()
    {

    }

    void OnDestroy()
    {
        isDoing = true;
    }

    void RandomPosition(bool isFirst = false)
    {
        var screenWidth = Screen.width;
        var screenHeight = Screen.height;
        var x = Random.Range(-screenWidth, screenWidth);

        var height = (transform as RectTransform).rect.height;
        var y = -screenHeight - height;

        if (isFirst) y = Random.Range(-screenHeight, 0);
        Debug.Log("isFirst==>" + isFirst + "|" + y);

        startPosition = new Vector3(x, y, 1);
        transform.localPosition = startPosition;
        endPosition = new Vector3(x, screenHeight, 1);
    }

    void Update()
    {
        sTime += Time.deltaTime;

        if (sTime > startTime + duration)
        {
            RandomPosition();
            sTime = 0;
            isDoing = false;
            return;
        }

        if (!isDoing && sTime > startTime)
        {
            isDoing = true;
            StartDoing();
        }
    }
}
