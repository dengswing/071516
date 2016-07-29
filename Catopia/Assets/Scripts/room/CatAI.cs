using System.Collections.Generic;
using UnityEngine;

public class CatAI : MonoBehaviour
{
    //移动速度;
    public float playerMoveSpeed = 1f;
    public float turningSpeed = 5;
    public List<GameObject> moveTargetList;
    public bool mouseClickTest = false;

    private AStarPlayer player;
    private int index;

    void Start()
    {
        player = GetComponent<AStarPlayer>();
        player.TurningSpeed = turningSpeed;
        player.PlayerMoveSpeed = playerMoveSpeed;
        ExecuteAI();
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

    private void ExecuteAI()
    {
        if (moveTargetList == null || moveTargetList.Count <= 0) return;
        AILoop(index);
    }

    private void AILoop(int index)
    {
        GameObject con = moveTargetList[index];

        var buildingEvent = CheckBuildingEvent(con);

        Vector3 position = con.transform.position;
        if (buildingEvent != null)
        {
            //  Vector2 globalMousePos = RectTransformUtility.w(Camera.main, tTrsF.position);
            position = buildingEvent.hot.position;
        }

        player.MoveTarget(position, () =>
        {
            if (buildingEvent != null)
            {
                PlayerEquipement(con, buildingEvent);
                return; //展示退出
            }

            index += 1;
            var lenth = moveTargetList.Count;
            if (index > lenth) index = 0;
            AILoop(index);
        });
    }

    private BuildingEvent CheckBuildingEvent(GameObject con)
    {
        return con.GetComponent<BuildingEvent>();
    }

    private void PlayerEquipement(GameObject con, BuildingEvent buildingEvent)
    {
        if (buildingEvent == null) return;
        player.RotationTarget(con.transform.eulerAngles, () =>
        {
            buildingEvent.PlayerEquipment();
            player.PlayerEquipment();
        });
    }
}
