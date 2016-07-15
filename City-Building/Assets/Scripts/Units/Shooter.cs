using UnityEngine;
using System.Collections;

public class Shooter: MonoBehaviour {		//soldiers shoot at targets using this

	public GameObject[] projectile;
	public int assignedToGroup = 0;

	public float fireRate;
	private float fireCounter = 0;
	private int projectileZ = -11;
	public bool shoot;

	private Component battleProc, fighterController, soundFx;

	void Start () {

		battleProc = GameObject.Find ("BattleProc").GetComponent<BattleProc> ();
		soundFx = GameObject.Find ("SoundFX").GetComponent<SoundFX> ();
		fighterController = GetComponent<FighterController> ();

		assignedToGroup = ((FighterController)fighterController).assignedToGroup;
	}

	void FixedUpdate () {
	
	if(shoot)
		{
			if(((BattleProc)battleProc).pauseAttack[((FighterController)fighterController).assignedToGroup])
				return;

			fireCounter += Time.deltaTime;

			if (fireCounter>fireRate) 
				{
					((SoundFX)soundFx).SoldierFire();
					Instantiate(projectile[assignedToGroup],new Vector3(transform.position.x,transform.position.y, projectileZ),Quaternion.identity);
					fireCounter = 0;
				}
		}
	}

}
