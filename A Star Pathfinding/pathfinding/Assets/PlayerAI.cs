using UnityEngine;
using System.Collections;
using Pathfinding.RVO;

namespace Pathfinding
{
    [RequireComponent(typeof(Seeker))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerAI : AIPath
    {
        /** Minimum velocity for moving */
        public float sleepVelocity = 0.4F;

        /** Speed relative to velocity with which to play animations */
        public float animationSpeed = 0.2F;

        /** Effect which will be instantiated when end of path is reached.
         * \see OnTargetReached */
        public GameObject endOfPathEffect;

        public new void Start()
        {
            //Call Start in base script (AIPath)
            base.Start();
        }

        /** Point for the last spawn of #endOfPathEffect */
        protected Vector3 lastTarget;

        public override void OnTargetReached()
        {
            if (endOfPathEffect != null && Vector3.Distance(tr.position, lastTarget) > 1)
            {
                GameObject.Instantiate(endOfPathEffect, tr.position, tr.rotation);
                lastTarget = tr.position;
            }
        }

        public override Vector3 GetFeetPosition()
        {
            return tr.position;
        }

        protected new void Update()
        {

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    return;
                }
                if (!hit.transform)
                {
                    return;
                }
                target.localPosition = hit.point;
            }

            //Get velocity in world-space
            Vector3 velocity;
            if (canMove)
            {
                //Calculate desired velocity
                Vector3 dir = CalculateVelocity(GetFeetPosition());

                //Rotate towards targetDirection (filled in by CalculateVelocity)
                RotateTowards(targetDirection);

                dir.y = 0;
                if (dir.sqrMagnitude > sleepVelocity * sleepVelocity)
                {
                    //If the velocity is large enough, move
                }
                else
                {
                    //Otherwise, just stand still (this ensures gravity is applied)
                    dir = Vector3.zero;
                }

                if (this.rvoController != null)
                {
                    rvoController.Move(dir);
                    velocity = rvoController.velocity;
                }
                else
                    if (navController != null)
                    {
#if FALSE
					navController.SimpleMove (GetFeetPosition(), dir);
#endif
                        velocity = Vector3.zero;
                    }
                    else if (controller != null)
                    {
                        controller.SimpleMove(dir);
                        velocity = controller.velocity;
                    }
                    else
                    {
                        Debug.LogWarning("No NavmeshController or CharacterController attached to GameObject");
                        velocity = Vector3.zero;
                    }
            }
            else
            {
                velocity = Vector3.zero;
            }
        }
    }
}