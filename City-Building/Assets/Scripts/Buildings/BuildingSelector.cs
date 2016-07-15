using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public class BuildingSelector : MonoBehaviour {//attached to each building as an invisible 2dtoolkit button
	
	public bool 
		isSelected = true,
		inConstruction = true,//only for load/save
		goldBased,
		battleMap = false;

	public int 
		buildingIndex = -1,	
		resourceValue;

	public string buildingType;

	private Component buildingCreator, relay, battleProc, soundFX, tween;

	// Use this for initialization
	void Start () {
		tween = GetComponent<BuildingTween> ();
		soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();
		relay = GameObject.Find("Relay").GetComponent<Relay>();

		buildingCreator = GameObject.Find("BuildingCreator").GetComponent<BuildingCreator>();

		if (battleMap) 
		{				
			battleProc =  GameObject.Find("BattleProc").GetComponent<BattleProc>();
		}
	}

	public void ReSelect()
	{
		if(((Relay)relay).delay||((Relay)relay).pauseInput) return;

		((BuildingTween)tween).Tween();
		((SoundFX)soundFX).Click();

		if(!battleMap)
		{
		
		if(!((BuildingCreator)buildingCreator).isReselect &&
			!((Relay)relay).pauseInput)
		{
			isSelected = true;
			
			((BuildingCreator)buildingCreator).isReselect = true;

			switch (buildingType) //sends the reselect commands to BuildingCreator
			{
				case "Academy":
				((BuildingCreator)buildingCreator).OnReselect0();
				break;
				
				case "Barrel":
				((BuildingCreator)buildingCreator).OnReselect1();
				break;
				
				case "Chessboard":
				((BuildingCreator)buildingCreator).OnReselect2();
				break;
				
				case "Classroom":
				((BuildingCreator)buildingCreator).OnReselect3();
				break;
				
				case "Forge":
				((BuildingCreator)buildingCreator).OnReselect4();
				break;
				
				case "Generator":
				((BuildingCreator)buildingCreator).OnReselect5();
				break;
		
				case "Globe":
				((BuildingCreator)buildingCreator).OnReselect6();
				break;
				
				case "Summon":
				((BuildingCreator)buildingCreator).OnReselect7();
				break;			
				
				case "Toolhouse":
				((BuildingCreator)buildingCreator).OnReselect8();
				break;
				
				case "Vault":
				((BuildingCreator)buildingCreator).OnReselect9();
				break;
				
				case "Workshop":
				((BuildingCreator)buildingCreator).OnReselect10();
				break;				
			}
		}
		}
		else //the target select on the battle map
		{
			if(((BattleProc)battleProc).DeployedUnits.Count == 0)return; //ignore if there are no units deployed
	
			int assignedToGroup = -1;
			bool userSelect = false;  //auto or user target select

			for (int i = 0; i <= ((BattleProc)battleProc).instantiationGroupIndex; i++) //((BattleProc)battleProcSc).userSelect.Length
			{			
				if(((BattleProc)battleProc).userSelect[i])
				{
					assignedToGroup = i;
					((BattleProc)battleProc).userSelect[i] = false;
					userSelect = true;
					break;
				}
			}

			if(!userSelect)
			{
				assignedToGroup = ((BattleProc)battleProc).FindClosestGroup(transform.position);//designate a group to attack this building
			}

			if(assignedToGroup == -1) return;

			if(((BattleProc)battleProc).targetBuildingIndex[assignedToGroup] != buildingIndex)//if this building is not already the target of the designated group
			{
				switch (assignedToGroup) 
				{
				case 0:
					((BattleProc)battleProc).Select0();
					break;

				case 1:
					((BattleProc)battleProc).Select1();
					break;

				case 2:
					((BattleProc)battleProc).Select2();
					break;

				case 3:
					((BattleProc)battleProc).Select3();
					break;
				}

				((BattleProc)battleProc).targetBuildingIndex[assignedToGroup] = buildingIndex;	//pass relevant info to BattleProc for this new target building		
				((BattleProc)battleProc).targetCenter[assignedToGroup] = transform.position;
				((BattleProc)battleProc).FindSpecificBuilding();
				((BattleProc)battleProc).updateTarget[assignedToGroup] = true;
				((BattleProc)battleProc).pauseAttack[assignedToGroup] = true;
			}

		}
	}

}
