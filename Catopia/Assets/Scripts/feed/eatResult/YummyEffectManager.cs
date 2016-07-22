using UnityEngine;

/// <summary>
/// 美味动画控制
/// </summary>
public class YummyEffectManager : EatResultManager
{
    public GameObject StarParticle;

    StarEffectManager starEffect;

    public override void Hide()
    {
        base.Hide();
        starEffect.Hide();
    }

    public override void Show()
    {
        base.Show();
        StartDoing();
    }

    void Awake()
    {
        var star = transform.Find("Star");
        starEffect = star.GetComponent<StarEffectManager>();
    }

    void StartDoing()
    {
        starEffect.PlayAnimation();
        StarParticle.SetActive(true);
    }

    void Start()
    {

    }

    void OnDisable()
    {
        isStartHide = false;
        iTime = 0;
        StarParticle.SetActive(false);
    }
    
}
