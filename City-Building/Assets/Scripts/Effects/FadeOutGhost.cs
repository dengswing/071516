using UnityEngine;
using System.Collections;

public class FadeOutGhost : MonoBehaviour { // the floating goast going up from each grave

	public float alpha, scale, deltaAlpha, deltaScale, ySpeed;
		
	private tk2dSprite sprite;
	
	void Start () 
	{
		transform.parent = GameObject.Find("GroupEffects").transform;
		sprite = gameObject.GetComponentInChildren<tk2dSprite> ();
	}

	void FixedUpdate () {

		if (alpha > 0)
		{
			scale += deltaScale;
			alpha += deltaAlpha;

			transform.position += new Vector3 (0, ySpeed,0);
			transform.localScale = new Vector3(scale,scale,1);
					
			Color currentSpriteCollor = sprite.color;
			sprite.color = new Color(currentSpriteCollor.r, currentSpriteCollor.g, currentSpriteCollor.b, alpha);
		}
		else
			Destroy(gameObject);
			                                                        
	}
}