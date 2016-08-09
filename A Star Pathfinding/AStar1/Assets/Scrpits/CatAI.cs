using System.Collections.Generic;
using UnityEngine;

public class CatAI : MonoBehaviour
{
    //移动速度;
    public float playerMoveSpeed = 1f;
    public float turningSpeed = 5;
    public List<GameObject> moveTargetList;
    public bool mouseClickTest = false;
    public float jumpTime = 0.5f;
    public float resetTime = 5f;

    private AStarPlayer player;
    private int index;
    private Vector3 initPosition;
    private float iTime;
    private bool isWait;

    void Start()
    {
        player = GetComponent<AStarPlayer>();
        player.TurningSpeed = turningSpeed;
        player.PlayerMoveSpeed = playerMoveSpeed;  
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && mouseClickTest)
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
            player.MoveTarget(hit.point, null);
        }
    }
}
