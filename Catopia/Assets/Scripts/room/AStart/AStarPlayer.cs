using Pathfinding;
using System;
using UnityEngine;

public class AStarPlayer : MonoBehaviour
{
    //移动速度;
    float playerMoveSpeed = 1f;
    float turningSpeed = 5;

    //目标位置;
    Vector3 targetPosition = Vector3.zero;

    Seeker seeker;

    //计算出来的路线;
    Path path;

    //当前点
    int currentWayPoint = 0;

    bool stopMove = true;

    //Player中心点;
    float playerCenterY = 1.0f;

    Animator animator;

    Action moveFinish;


    public float PlayerMoveSpeed
    {
        get { return playerMoveSpeed; }
        set { playerMoveSpeed = value; }
    }

    public float TurningSpeed
    {
        get { return turningSpeed; }
        set { turningSpeed = value; }
    }

    // Use this for initialization
    void Awake()
    {
        seeker = GetComponent<Seeker>();
        playerCenterY = transform.localPosition.y;
        animator = GetComponent<Animator>();
    }

    public void PlayerEquipment()
    {
        animator.SetTrigger("IsPlayer");
    }

    public void MoveTarget(Vector3 position, Action callback)
    {
        if (!targetPosition.Equals(Vector3.zero))
        {
            Debug.Log("move");
            return;
        }
        moveFinish = callback;
        targetPosition = position;
        seeker.StartPath(transform.position, targetPosition, OnPathComplete);
    }

    public void RotationTarget(Vector3 rotation, Action callback)
    {
        CatGameTools.UIGameObjectRotationBack(transform, rotation, 0.5f, callback);
        // RotateTowards(rotation);
        Debug.Log("rotation" + rotation);
    }

    //寻路结束;
    private void OnPathComplete(Path p)
    {
        Debug.Log("OnPathComplete error = " + p.error);
        if (!p.error)
        {
            currentWayPoint = 0;
            path = p;
            stopMove = false;
            StartMove();
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
        dir *= PlayerMoveSpeed * Time.fixedDeltaTime;

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
        targetPosition = Vector3.zero;
        if (moveFinish != null) moveFinish();
    }

    private void StartMove()
    {
        animator.SetBool("IsWalk", true);
    }

    private void RotateTowards(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        Quaternion rot = transform.rotation;
        Quaternion toTarget = Quaternion.LookRotation(dir);

        rot = Quaternion.Slerp(rot, toTarget, TurningSpeed * Time.deltaTime);
        Vector3 euler = rot.eulerAngles;
        euler.z = 0;
        euler.x = 0;
        rot = Quaternion.Euler(euler);
        transform.rotation = rot;
    }
}
