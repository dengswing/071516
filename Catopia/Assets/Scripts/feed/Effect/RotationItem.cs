using UnityEngine;

/// <summary>
/// 旋转
/// </summary>
public class RotationItem : MonoBehaviour
{
    public float speed = 3f;
    public float startTime = 0f;

    private RectTransform rect;
    private float sTime;
    private float index;
    private float duration = 5f;

    void Start()
    {
        rect = gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        sTime += Time.deltaTime;
        if (sTime > startTime)
        {
            index -= duration;
            rect.Rotate(new Vector3(0f, 0f, index), speed);
        }
    }
}
