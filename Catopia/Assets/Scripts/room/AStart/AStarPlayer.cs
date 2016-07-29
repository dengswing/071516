using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class AStarPlayer : MonoBehaviour
{

    //移动速度;
    public float playerMoveSpeed = 1f;
    public float turningSpeed = 5;
    public List<GameObject> moveTargetList;


    //目标位置;
    Vector3 targetPosition;

    Seeker seeker;

    //计算出来的路线;
    Path path;
    
    //当前点
    int currentWayPoint = 0;

    bool stopMove = true;

    //Player中心点;
    float playerCenterY = 1.0f;

    Animator animator;

    int currentIndex; 

    // Use this for initialization
    void Start()
    {
        seeker = GetComponent<Seeker>();
        playerCenterY = transform.localPosition.y;
        animator = GetComponent<Animator>();
    }

    //寻路结束;
    public void OnPathComplete(Path p)
    {
        Debug.Log("OnPathComplete error = " + p.error);
        if (!p.error)
        {
            currentWayPoint = 0;
            path = p;
            stopMove = false;
            StartMove();
        }

        for (int index = 0; index < path.vectorPath.Count; index++)
        {
            Debug.Log("path.vectorPath[" + index + "]=" + path.vectorPath[index]);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            /* RaycastHit hit;
             if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
             {
                 return;
             }
             if (!hit.transform)
             {
                 return;
             }
             targetPosition = hit.point;// new Vector3(hit.point.x, transform.localPosition.y, hit.point.z);
             */
            targetPosition = moveTargetList[currentIndex].transform.position;
            seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        }
    }

    void FixedUpdate()
    {
        if (path == null || stopMove)
        {
            return;
        }

        //根据Player当前位置和 下一个寻路点的位置，计算方向;
        // Vector3 currentWayPointV = new Vector3(path.vectorPath[currentWayPoint].x, path.vectorPath[currentWayPoint].y + playerCenterY, path.vectorPath[currentWayPoint].z);
        Vector3 currentWayPointV = new Vector3(path.vectorPath[currentWayPoint].x, playerCenterY, path.vectorPath[currentWayPoint].z);
        Vector3 dir = (currentWayPointV - transform.position).normalized;
        RotateTowards(dir);

        //计算这一帧要朝着 dir方向 移动多少距离;
        dir *= playerMoveSpeed * Time.fixedDeltaTime;

        //计算加上这一帧的位移，是不是会超过下一个节点;
        float offset = Vector3.Distance(transform.localPosition, currentWayPointV);

        if (offset < 0.1f)
        {
            transform.localPosition = currentWayPointV;

            currentWayPoint++;

            if (currentWayPoint == path.vectorPath.Count)
            {
                stopMove = true;
                currentWayPoint = 0;
                path = null;
                EndMove();
            }
        }
        else
        {
            if (dir.magnitude > offset)
            {
                Vector3 tmpV3 = dir * (offset / dir.magnitude);
                dir = tmpV3;

                currentWayPoint++;

                if (currentWayPoint == path.vectorPath.Count)
                {
                    stopMove = true;

                    currentWayPoint = 0;
                    path = null;
                }
            }
            transform.localPosition += dir;
        }
    }

    private void EndMove()
    {
        animator.SetBool("IsWalk", false);
    }

    private void StartMove()
    {
        animator.SetBool("IsWalk", true);
    }


    protected void RotateTowards(Vector3 dir)
    {

        if (dir == Vector3.zero) return;

        Quaternion rot = transform.rotation;
        Quaternion toTarget = Quaternion.LookRotation(dir);

        rot = Quaternion.Slerp(rot, toTarget, turningSpeed * Time.deltaTime);
        Vector3 euler = rot.eulerAngles;
        euler.z = 0;
        euler.x = 0;
        rot = Quaternion.Euler(euler);

        transform.rotation = rot;
    }
}
