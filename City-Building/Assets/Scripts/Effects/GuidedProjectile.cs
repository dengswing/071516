using UnityEngine;
using System.Collections;

public class GuidedProjectile : MonoBehaviour { 		//colored projectiles coming from attacking units towards the center of the building

	//instead of processing who's firing them, 4 prefabs would be better- too many projectiles, wasted performance

	public int assignedToGroup;//keeps track of which unit group fires this
	public GameObject sparks;

	public float 
		projectileSpeed;
		//lifeTime = 3.0f;

	//private float lifeCounter = 0;

	public Vector2 velocity, target;

	void Start () 
	{
		target = GameObject.Find ("BattleProc").GetComponent<BattleProc> ().targetCenter[assignedToGroup];
		transform.parent = GameObject.Find("GroupEffects").transform;
	}

	void FixedUpdate () {
		/*
		lifeCounter += Time.deltaTime;
		if (lifeCounter > lifeTime) 
		{
			Explode ();
		}
		*/
		FireSeq ();	
	}

	private void Explode()
	{
		GameObject Sparks = (GameObject)Instantiate(sparks, transform.position, Quaternion.identity);	
		Destroy (gameObject);
	}

	private void FireSeq()
	{			
		velocity += Steer(target);
	}

	private Vector2 Steer(Vector2 target)	
	{	
		Vector2 pos = new Vector2 (transform.position.x, transform.position.y);
		Vector2 desiredVelocity = (target - pos);
		float dist = desiredVelocity.magnitude;//square root of (x*x+y*y+z*z)

		desiredVelocity.Normalize();

		if (dist < 10) Explode ();

		desiredVelocity *= projectileSpeed;

		Vector2 acceleration = desiredVelocity - velocity;		
		transform.position += new Vector3(velocity.x, velocity.y, 0); //velocity.z  velocity z=0!!!

		return acceleration;	
	}
}