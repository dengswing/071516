using UnityEngine;
using System.Collections;


public class FighterController : MonoBehaviour {  //controlls all soldiers

	public FighterPath path;
	public float speed = 30.0f, mass = 5.0f, unitZ = -3 ;

	private float curSpeed, pathLength;

	private Vector3 targetPoint; 
	private Vector2 velocity;

	public Vector3 targetCenter;

	private string direction = "N";

	public enum DobbitState{PathWalk, Attack, Idle, Disperse}
	public DobbitState currentState = DobbitState.Idle;
	private DobbitAnimController dobbitAnimController;	

	private bool 
		pause = false, 
		breakAction;

	public int 
		assignedToGroup = -1,//keeps track of which group this unit is assigned to. 
		life = 100;

	private tk2dUIProgressBar healthBar;

	private int 
		maxLife = 100,//for progress bar percentage
		curPathIndex;
	 
	void Start () {
				dobbitAnimController = GetComponent<DobbitAnimController> ();				
				path = GetComponent<FighterPath> ();
				healthBar = GetComponentInChildren<tk2dUIProgressBar> ();

				pathLength = path.Length;
				curPathIndex = 0;
						
				velocity = transform.forward;//get the current velocity 

				//InvokeRepeating("UpdateZDepth",0.5f,0.5f);//moved to centralized in battleproc
				//InvokeRepeating ("SetDirection", 0.5f, 0.5f);//safety check - for proper direction; moved to centralized in battleproc
	}

	public void Hit()
	{
		if (life >= 25) 
		{
			life -= 25;
			((tk2dUIProgressBar)healthBar).Value = (float)life / (float)maxLife;

			if(life == 0)
			Destroy (this.gameObject);//, 0.5f
		} 			
	}

	public void UpdateDirectionZ()
	{
		SetDirection ();
		UpdateZDepth ();
	}

	private void UpdateZDepth()//slight adjustment necessary for units that are close together
	{
		Vector3 pos = transform.position;

		float correctiony = 10 / (pos.y + 2905);//ex: fg 10 = 0.1   bg20 = 0.05  
		//all y values must be positive, so we add the grid origin y 2805 +100 to avoid divide by 0; 
		//otherwise depth glitches around y 0

		if(Mathf.Abs(correctiony)<1)
		{
			transform.position = new Vector3(pos.x, pos.y, unitZ - correctiony);//targetPoint.z		   
		}
	}		

	private void UpdatePath()
	{
		pause = true;
		path.UpdatePath();
		pathLength = path.Length;

		curPathIndex = 0;
		pause = false;
	}

	void FixedUpdate () {

		if (!pause) 
		{
			switch (currentState) 
			{
				case DobbitState.PathWalk:
				PathWalk ();
				break;	
			}					
		}
	}

	private void PathWalk()
	{
		curSpeed = speed * 0.016f;//Time.deltaTime creates a speed hickup - replaced with constant

		//to go straight to AITarget targetPoint = path.GetEndPoint();
		targetPoint = (Vector2)path.GetPoint(curPathIndex);

		//if radius to path point reached, move to next point
		if (Vector2.Distance(transform.position, targetPoint) <
		    path.Radius) {

			if (curPathIndex < pathLength - 1) curPathIndex++;	
			else
			{
				currentState = DobbitState.Attack;
				this.GetComponent<Shooter> ().shoot = true;
			}

			StartCoroutine("ChangeAnimation");
		}

		velocity += Steer((Vector2)targetPoint);

		transform.position += new Vector3(velocity.x, velocity.y, 0);//velocity.z velocity z=0!!! 
	}

	public Vector2 Steer(Vector2 target)
	{	
		Vector2 pos = (Vector2)transform.position;
		Vector2 desiredVelocity = (target - pos);
		float dist = desiredVelocity.magnitude;//square root of (x*x+y*y+z*z)

		desiredVelocity.Normalize();//normalized, a vector keeps the same direction but its length is 1.0
			
		if (dist < 10.0f)
			desiredVelocity *= (curSpeed * (dist / 10.0f));//slow down close to target
		else 
			desiredVelocity *= curSpeed;

		Vector2 steeringForce = desiredVelocity - velocity;
		Vector2 acceleration = steeringForce / mass;

		transform.position += new Vector3(velocity.x, velocity.y,0);//velocity.z velocity; z=0 
		//!!! Disregarding z can make character go back and forth below point, unable to "touch" it

		return (acceleration);
	}

	public void ChangeTarget()
	{
		IngameUpdatePath ();
	}

	private void  IngameUpdatePath()
	{
		UpdatePath ();
		StartCoroutine("ChangeAnimation");
	}

	private void ChangeToPathWalk()
	{
		if (currentState != DobbitState.Attack||breakAction) 
		{	
			currentState = DobbitState.PathWalk;
			StartCoroutine ("ChangeAnimation");		
			breakAction = false;
		}
	}

	public void RevertToPathWalk()//called by BattleProc
	{
		breakAction = true;
		this.GetComponent<Shooter> ().shoot = false;
		ChangeToPathWalk ();
	}

	public void RevertToIdle()
	{
		currentState = DobbitState.Idle;
		this.GetComponent<Shooter> ().shoot = false;
		StartCoroutine ("ChangeAnimation");
	}

	IEnumerator ChangeAnimation()
	{
		yield return new WaitForSeconds(0.1f);
		SetDirection ();
	}

	private void SetDirection()
	{
		switch (currentState) {
		
		case DobbitState.PathWalk:
			SpeedToDirection();
			dobbitAnimController.ChangeAnim("Walk");						
			break;		

		case DobbitState.Attack:
			SetRelativeDirection();
			dobbitAnimController.ChangeAnim("Attack");
			break;		

		case DobbitState.Idle:			
			//SetRelativeDirection();
			dobbitAnimController.ChangeAnim("Idle");
			break;

		}		
		
		dobbitAnimController.Turn(direction);
		dobbitAnimController.UpdateCharacterAnimation();

	}

	private void SpeedToDirection()
	{
		if(Mathf.Abs(velocity.x)>0.05f)//high X speed
		{
			if(Mathf.Abs(velocity.y)>0.05f) //high y speed 0.2
			{					
				if(velocity.x>0)
				{
					if(velocity.y>0) direction="NE";
					else direction="SE";
				}
				
				else //if(velocity.x<0)
				{
					if(velocity.y>0) direction="NW";
					else direction="SW";
				}
			}
			else //low y speed
			{
				if(velocity.x>0) direction="E";					
				else direction="W";
			}
			
		}//#########################
		
		else//if(Mathf.Abs(velocity.x<0.2f)) low x speed 
		{
			if(Mathf.Abs(velocity.y)>0.05f) //high y speed
			{
				if(velocity.y>0) direction="N";
				else direction="S";					
			}
			else //low y speed
			{
				if(velocity.x>0)
				{
					if(velocity.y>0) direction="NE";
					else direction="SE";
				}
				
				else //if(velocity.x<0)
				{
					if(velocity.y>0) direction="NW";
					else direction="SW";
				}
			}
		}
	}

	private void SetRelativeDirection()//the unit must face the center of the target
	{
		float xRelativePos = targetCenter.x - transform.position.x ;
		float yRelativePos = targetCenter.y - transform.position.y ;


		if (xRelativePos > 100) 
		{
			if(yRelativePos>100) direction = "NE";
			else if(yRelativePos<-100) direction = "SE";
			else direction = "E";
		}

		else if(xRelativePos < -100) //direction = "W";
		{
			if(yRelativePos>100) direction = "NW";
			else if(yRelativePos<-100) direction = "SW";
			else direction = "W";
		}

		else
		{
			if(yRelativePos>0) direction = "N";
			else direction = "S";
		}
	}
	
}

