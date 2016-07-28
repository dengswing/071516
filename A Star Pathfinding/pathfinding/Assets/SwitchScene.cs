using UnityEngine;
using System.Collections;

public class SwitchScene : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
    {
        Debug.Log("SwitchScene");
        DontDestroyOnLoad(gameObject);
	}
	
	// Update is called once per frame
	void OnGUI () 
    {
        if (GUI.Button(new Rect(0,0,200,50),"Scene_AIPath"))
        {
            Application.LoadLevel("Scene_AIPath");
        }
        if (GUI.Button(new Rect(210, 0, 200, 50), "Scene_Custom"))
        {
            Application.LoadLevel("Scene_Custom");
        }
	}
}
