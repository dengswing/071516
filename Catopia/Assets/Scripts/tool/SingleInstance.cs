using UnityEngine;

public class SingleInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T sInstance;

    private static bool sIsQuitting = false;

    private static object sLock = new object();

    public static T Instance
    {
        get
        {
            if (sIsQuitting && Application.isPlaying)
            {
#if UNITY_EDITOR
                Debug.LogWarning("application is quitting");
#endif
                return null;
            }

            lock (sLock)
            {
                if (sInstance == null)
                {
                    sInstance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        return sInstance;
                    }

                    if (sInstance == null)
                    {
                        GameObject singleton = new GameObject();
                        sInstance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).Name;
                        DontDestroyOnLoad(singleton);
                    }
                }

                return sInstance;
            }
        }
    }

    public void OnDestroy()
    {
        if (Application.isPlaying)
        {
#if UNITY_EDITOR
            Debug.LogWarningFormat("singleton [{0}] destroy", this.ToString());
#endif
            sIsQuitting = true;
        }
    }
}

