using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FighterPathFinder : MonoBehaviour {  //finds the path to the target


	private Vector2 startPos, endPos;
	public Node startNode { get; set; }
	public Node goalNode { get; set; }

	public ArrayList pathArray;

	private Component battleProc;

	private int selectedTarget = 0;
	private List<Vector2> targetList = new List<Vector2>();

	void Start () 
	{
		battleProc =  GameObject.Find("BattleProc").GetComponent<BattleProc>();
		pathArray = new ArrayList();
	}

	public void FindPath()
	{
		startPos = transform.position;

		int myGroup = GetComponent<FighterController> ().assignedToGroup;
		if(targetList.Count == 0)
		{
			switch (myGroup) 
			{
				case 0:
					targetList =  ((BattleProc)battleProc).aiTargetVectorsO; 
					break;
				case 1:
					targetList =  ((BattleProc)battleProc).aiTargetVectorsI;
					break;
				case 2:
					targetList =  ((BattleProc)battleProc).aiTargetVectorsII;
					break;
				case 3:
					targetList =  ((BattleProc)battleProc).aiTargetVectorsIII;			
					break;			
			}
		}

		GetSurroundIndex(myGroup);//some of the units may have died; when surrounding the building, this unit's order might be different
		endPos = targetList [selectedTarget];// of aiTargets

		//Assign StartNode and Goal Node
		startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(startPos)));
		goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos)));
				
		pathArray = AStar.FindPath(startNode, goalNode);
	}

	private void GetSurroundIndex(int index)
	{
		int currentIndex = ((BattleProc)battleProc).surroundIndex[index];
		selectedTarget = currentIndex;

		if(currentIndex<targetList.Count-1)
		{
			((BattleProc)battleProc).surroundIndex[index]++;
		}
		else
		{
			((BattleProc)battleProc).surroundIndex[index] = 0;
		}
	}

	void OnDrawGizmos()
	{
		if (pathArray == null)
			return;
		
		if (pathArray.Count > 0)
		{
			int index = 1;
			foreach (Node node in pathArray)
			{
				if (index < pathArray.Count)
				{
					Node nextNode = (Node)pathArray[index];
					Debug.DrawLine(node.position, nextNode.position, Color.green);
					index++;
				}
			}
		}
	}

}
