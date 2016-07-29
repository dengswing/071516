using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Mian : MonoBehaviour
{
    public Button fondleBtn;
    public Button fondleBtn2;
    public Button feedBtn;
    public Button feedBtn2;

    // Use this for initialization
    void Start()
    {
        fondleBtn.onClick.AddListener(FondleHandler);
        fondleBtn2.onClick.AddListener(Fondle2Handler);
        feedBtn.onClick.AddListener(FeedHandler);
        feedBtn2.onClick.AddListener(Feed2Handler);
    }

    void FondleHandler()
    {
        SceneManager.LoadScene("fondle");
    }

    void Fondle2Handler()
    {
        SceneManager.LoadScene("fondle_2");
    }

    void FeedHandler()
    {
        SceneManager.LoadScene("feed");
    }

    void Feed2Handler()
    {
        SceneManager.LoadScene("feed_2");
    }
}
