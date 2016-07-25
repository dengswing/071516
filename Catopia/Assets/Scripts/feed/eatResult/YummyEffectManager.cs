using UnityEngine;
/// <summary>
/// 美味动画控制
/// </summary>
public class YummyEffectManager : EatResultManager
{
    public GameObject star;
    public GameObject launch;
    public GameObject launchTwo;

    StarEffectManager starEffect;
    StarEffectManager launchEffect;
    StarEffectManager launchEffectTwo;

    public override void Hide()
    {
        base.Hide();
        starEffect.Hide();
        launchEffect.Hide();
        launchEffectTwo.Hide();
    }

    public override void Show()
    {
        base.Show();
        StartDoing();
    }

    void Awake()
    {        
        starEffect = star.GetComponent<StarEffectManager>();
        launchEffect = launch.GetComponent<StarEffectManager>();
        launchEffectTwo = launchTwo.GetComponent<StarEffectManager>();
    }

    void StartDoing()
    {
        starEffect.PlayAnimation();
        launchEffect.PlayAnimation();
        launchEffectTwo.Active();
    }

    void Start()
    {

    }

    void OnDisable()
    {
        isStartHide = false;
        iTime = 0;
    }
}
