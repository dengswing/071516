using UnityEngine;
using System.Collections;

public class ShowSelected : MonoBehaviour {
	
	public Shader selectedShader;
	public Color outterColor;

	
	private Color myColor ;
	private Shader myShader;
	private bool Selected = false;
	
	// Use this for initialization
	void Start () {
		myColor = GetComponent<Renderer>().material.color;
		myShader = GetComponent<Renderer>().material.shader;
		selectedShader = Shader.Find("Hidden/RimLightSpce");
		if(!selectedShader)
		{
			enabled = false;
			return;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	  void OnMouseEnter() {
        //renderer.material.color = Color.black;
		GetComponent<Renderer>().material.shader = selectedShader;
		GetComponent<Renderer>().material.SetColor("_RimColor",outterColor);
    }
	void OnMouseExit(){
		GetComponent<Renderer>().material.color = myColor;
		GetComponent<Renderer>().material.shader = myShader;
	}
	void OnMouseDown(){
		Selected  = !Selected;
		gameObject.layer= Selected ? 8 : 0;
	}
}
