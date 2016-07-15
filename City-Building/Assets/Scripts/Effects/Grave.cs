using UnityEngine;
using System.Collections;

public class Grave : MonoBehaviour {

	// adjusts zdepth of vertical plank, so units can appear behind or in front of it
	int unitZ = -3;

	void Start () {	//o stones 1 anchor 2 plank

		Vector3 anchorPos = transform.transform.GetChild (1).position; 
		Vector3 plankPos = transform.GetChild (2).position;

		float correctiony = 10 / (anchorPos.y + 2905);//ex: fg 10 = 0.1   bg20 = 0.05  
		//all y values must be positive, so we add the grid origin y 2805 +100 to avoid divide by 0; 
		//otherwise depth glitches around y 0

		transform.parent = GameObject.Find("GroupEffects").transform;
		
		if(Mathf.Abs(correctiony)<1)
		{
			transform.GetChild(2).position = new Vector3(plankPos.x, plankPos.y, unitZ - correctiony);	   
		}
	}
	
	
}
