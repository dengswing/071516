using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using TDTK;


namespace TDTK {
	
	[RequireComponent (typeof (Animator))]
	public class UnitTowerAnimation : MonoBehaviour {

		public UnitTower tower;
		
		//for mecanim
		[HideInInspector] public Animator anim;
		public AnimationClip clipConstruct;
		public AnimationClip clipDeconstruct;
		public AnimationClip clipUpgrade;
		public AnimationClip clipShoot;
        public AnimationClip clipIdle;


        public bool enableShoot=true;
		public bool enableConstruct= true;
        public bool enableDeconstruct= true;
        public bool enableIdle = true;


        public float shootDelay=0;
		
		
		void Start(){
			anim=gameObject.GetComponent<Animator>();
			if(anim!=null){
                if (enableShoot) tower.playShootAnimation = this.PlayShoot;
                if (enableConstruct) tower.playConstructAnimation = this.PlayConstruct;
                if (enableDeconstruct) tower.playDeconstructAnimation = this.PlayDeconstruct;
                if (enableIdle) tower.playIdleAnimation = this.PlayIdle;

                AnimatorOverrideController overrideController = new AnimatorOverrideController();
				overrideController.runtimeAnimatorController = anim.runtimeAnimatorController;
		 
				overrideController["DefaultTowerConstruct"] = clipConstruct!=null ? clipConstruct : null;
				overrideController["DefaultTowerDeconstruct"] = clipDeconstruct!=null ? clipDeconstruct : null;
				overrideController["DefaultTowerShoot"] = clipShoot!=null ? clipShoot : null;
                overrideController["DefaultTowerIdle"] = clipIdle != null ? clipIdle : null;

                anim.runtimeAnimatorController = overrideController;
			}
		}
		
		
		void OnEnable(){
			
		}
		
		
		public float PlayShoot(){
			anim.SetTrigger("Shoot");
			return shootDelay;
		}
		public void PlayConstruct(){
			anim.SetTrigger("Construct");
		}
		public void PlayDeconstruct(){
			anim.SetTrigger("Deconstruct");
		}

        public void PlayIdle(){
            anim.SetTrigger("Idle");
        }

    }
	
}
