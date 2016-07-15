using UnityEngine;
using System.Collections;

public class ConstructionPath : MonoBehaviour {		//the square path each dobbit is following during construction
	public bool bDebug = true;
	public float Radius = 0.5f;
	public Vector3[] 
		waypoints = new Vector3[4],
		adjWaypoints = new Vector3[4];

	void Start () 
	{
		for (int i = 0; i < adjWaypoints.Length; i++) 
		{
			adjWaypoints[i] = transform.position + waypoints[i];//adjWaypoints are adjusted with object position
		}
	}

	public float Length {
		get {
			return adjWaypoints.Length;
		}
	}
	public Vector3 GetPoint(int index) {
		return adjWaypoints[index];
	}
	
	void OnDrawGizmos() 
	{
		if (!bDebug) return;
		for (int i = 0; i < adjWaypoints.Length; i++) 
		{
			if (i + 1 < adjWaypoints.Length) 
			{
				Debug.DrawLine(adjWaypoints[i], adjWaypoints[i + 1], Color.red);
			} 
		}
	}	
}
