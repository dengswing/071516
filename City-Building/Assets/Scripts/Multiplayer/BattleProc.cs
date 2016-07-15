using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BattleProc : MonoBehaviour {

	private const int unitGroupsNo = 4;//the maximum number of groups
	private float 
		elapsedTime = 0.0f, 
		intervalTime = 1.0f,	// interval between attack updates
		updateOTimer,			// used to scatter the updates for units - timers
		updateITimer,
		updateIITimer,
		updateIIITimer; 
		
	//status variables:
	public bool networkLoadReady = false;
	private bool 
		mapIsEmpty = false, 
		battleOver = false,
		allDestroyed = false,
		updatePathMaster,			// used to scatter the updates for units - bools
		updatePathO,
		updatePathI,
		updatePathII,
		updatePathIII;

	//numeric data:
	private int 
		smokeZ = -10,	// z-depths
		fireZ = -11,
		rubbleZ = 0,
		targetZ = -11,	//to avoid gioded projectiles from changing Z and going under sprites
		allLootGold, 	// total gold/mana on map - passed to StatsBattle to properly dimension the gold/mana loot progress bars.
		allLootMana,
		gain,			// the loot fraction, based on total building value and damage procentage;
		updateOCounter,	// used to scatter the updates for units - cycles through the unit lists
		updateICounter,
		updateIICounter,
		updateIIICounter;
	
	public int 
		instantiationGroupIndex = -1,	//keeps track of total number of groups- must not exceed 4
		instantiationUnitIndex = -1,	//each unit will have a unique ID- necessary when killed - not necessarily in order
		selectedGroupIndex = 0;			//0 when initialized, reset to -1 afterwards 

	//interface elements:
	public UILabel goldNoLb, manaNoLb, buildingsDestroyedNoLb, unitsLostNoLb, unitsRecoveredNoLb;//missioncomplete panel labels

	public GameObject[] 		
		InterfaceElements = new GameObject[6], 
		MenuPanels = new GameObject[3],// UnitsBattlePanel, OptionsPanel, GhostHelper
		SelectedGroupLb = new GameObject[unitGroupsNo],//the red/blue labels alternating when manually selecting groups
		UnselectedGroupLb = new GameObject[unitGroupsNo],
		grassTarget = new GameObject[unitGroupsNo];//each group heads for its assigned grass target

	public GameObject 
		MissionCompletePanel, //activated at battle end
		NavigationButtons, // moved at the corner of the panel above 
		SmokePf, ExplosionPf, FirePf, RubblePf, GravePf, GainGoldPf,  GainManaPf,//prefabs
		EffectsGroup;//parent instantiated effect prefabs

	public List<int> 
		BuildingValues = new List<int>(),//holds loot values for each building; 0 when completely destroyed
		BuildingsHealth = new List<int>();//100, 100, 100 - you can make this private - useful to increase health to study unit behavior 

	//lists and arrays for buildings and units:
	public List<bool> 
		BuildingGoldBased = new List<bool>(),
		DamageEffectsRunning = new List<bool> ();//0,0,0... initially then 1,1,1...

	public int[] 
		targetBuildingIndex = new int[unitGroupsNo],
		currentDamage = new int[unitGroupsNo],//if 5 units attack a building, the building health (100) is decreased by 5 each second
		surroundIndex = new int[unitGroupsNo];//for each separate group, increments an index to "surround" the buildings - occupy the aiTargets in sequence - 0,1,2, etc 

	private GameObject[] grassPatches;//all grass patches on the map - found by label
	public Vector3[] targetCenter = new Vector3[unitGroupsNo];
	public Vector2[] unitPosition = new Vector2[unitGroupsNo];// the position of the group is set by the position of element 0

	public bool[] 
		pauseAttack = new bool[unitGroupsNo],//necessary to avoid errors untill the units state has changed
		updateTarget = new bool[unitGroupsNo],
		userSelect = new bool[unitGroupsNo];

	public List<GameObject> 
		Buildings = new List<GameObject>(),//buildings and cannons, instantiated and passed at LoadMap
		Cannons = new List<GameObject>(),
		DamageBars = new List<GameObject>(),

		DeployedUnits = new List<GameObject>(),//all units deployed
		selectedList =  new List<GameObject>(),//currently selected from the groups below 
		GroupO = new List<GameObject>(), 
		GroupI = new List<GameObject>(), 
		GroupII = new List<GameObject>(), 
		GroupIII = new List<GameObject>(); 

	public List<Vector2> 
		grassTargets = new List<Vector2>(),// distance, grassIndex
		aiTargetsFree = new List<Vector2>(),//##make private - to see if the list is sorted properly
	 	aiTargetVectorsCurrent = new List<Vector2>(),//List of current target vectors - will be passed to each group; aiTargets are the green squares around the building - units go there to attack
		aiTargetVectorsO = new List<Vector2>(),
		aiTargetVectorsI = new List<Vector2>(),
		aiTargetVectorsII = new List<Vector2>(),
		aiTargetVectorsIII = new List<Vector2>();

	private Component statsBattle, transData, saveLoadBattle, soundFx, relay;//components needed to pass info

	// Use this for initialization
	void Start () {

		statsBattle = GameObject.Find ("StatsBattle").GetComponent<StatsBattle> ();
		transData = GameObject.Find ("TransData").GetComponent<TransData> ();
		saveLoadBattle = GameObject.Find ("SaveLoadBattle").GetComponent<SaveLoadBattle> ();
		soundFx = GameObject.Find ("SoundFX").GetComponent<SoundFX> ();
		relay = GameObject.Find ("Relay").GetComponent<Relay> ();

		InvokeRepeating ("SetDirection", 0.5f, 0.5f);//makes sure the players are all facing the correct direction while walking (centralizes this function)
	}

	public void Select0()	
	{selectedList = GroupO; selectedGroupIndex = 0; ResetGroupLabels (0); UpdateSelectStars ();}
	public void UserSelect0(){	userSelect [0] = true;	Delay ();}

	public void Select1() 
	{selectedList = GroupI; selectedGroupIndex = 1; ResetGroupLabels (1); UpdateSelectStars ();}
	public void UserSelect1(){ userSelect [1] = true;	Delay ();}

	public void Select2()	
	{selectedList = GroupII; selectedGroupIndex = 2; ResetGroupLabels (2); UpdateSelectStars ();}
	public void UserSelect2(){	userSelect [2] = true;	Delay ();}

	public void Select3()	
	{selectedList = GroupIII; selectedGroupIndex = 3; ResetGroupLabels (3); UpdateSelectStars ();}
	public void UserSelect3(){	userSelect [3] = true;	Delay ();}

	private void Delay()
	{
		((Relay)relay).DelayInput ();
	}

	private void ResetGroupLabels(int index)
	{
		for (int i = 0; i < unitGroupsNo; i++) 
		{
			SelectedGroupLb[i].SetActive(false);
			UnselectedGroupLb[i].SetActive(true);
			userSelect[i] = false;
		}

		UnselectedGroupLb[index].SetActive(false);
		SelectedGroupLb[index].SetActive(true);
	}

	//IEnumerator GetFirstBuilding()
	private void GetFirstBuilding()
	{
		//yield return new WaitForSeconds(1.0f);
		FindClosestBuilding();

		for (int i = 0; i < updateTarget.Length; i++) 
		{
			updateTarget[i] = true;
		}
	}

	public void NetworkLoadReady()
	{	
		PrepareBuildings ();
		GetFirstBuilding ();
		networkLoadReady = true;
	}

	private void PrepareBuildings()
	{
		BuildingsHealth.Clear ();

		for (int i = 0; i < Buildings.Count; i++) 
		{
			PrepareLoot(Buildings[i].GetComponent<BuildingSelector>().buildingType);		//adds the buildings half-value from TransData 

			BuildingsHealth.Add(100);//100 default
			DamageEffectsRunning.Add(false);
		}

		((StatsBattle)statsBattle).maxStorageGold = allLootGold;
		((StatsBattle)statsBattle).maxStorageMana = allLootMana;
	}

	private void PrepareLoot(string buidingType)
	{
		int i = 0;
		switch (buidingType) 
		{
		case "Academy": i=0; break;			
		case "Barrel": i=1;	break;			
		case "Chessboard": i=2;	break;			
		case "Classroom": i=3; break;			
		case "Forge": i=4; break;			
		case "Generator": i=2; break;			
		case "Globe": i=5; break;			
		case "Summon": i=6;	break;			
		case "Toolhouse": i=7; break;			
		case "Vault": i=8; break;			
		case "Workshop": i=9; break;
		} 

		bool goldBased = ((TransData)transData).buildingGoldBased [i];
		int value = ((TransData)transData).buildingValues [i];

		BuildingValues.Add(value);
		BuildingGoldBased.Add(goldBased);

		if(goldBased) 
			allLootGold += value;
		else 
			allLootMana += value;
	}

	private void SetDirection()//updates all units directions and zdepth
	{
		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			DeployedUnits[i].GetComponent<FighterController>().UpdateDirectionZ();
		}
	}

	public void FindSpecificBuilding() 		//user has tapped on a building
	{
		aiTargetsFree.Clear();
		
		grassPatches = GameObject.FindGameObjectsWithTag("Grass");

		for (int i = 0; i < grassPatches.Length; i++) 
		{
			if(grassPatches[i].GetComponent<GrassSelector>().grassIndex == targetBuildingIndex[selectedGroupIndex])
			{
				grassTarget[selectedGroupIndex] = grassPatches[i];
				break;
			}
		}
			
		SelectTargetGrid();
	}

	public int FindClosestGroup(Vector2 newTargetPos)		// finds the closest group to the new target building
	{	
		if (instantiationGroupIndex == -1)					//give up if there are no units deployed
						return -1;

		List<Vector2> closeGroups = new List<Vector2> ();
		closeGroups.Clear ();

		switch (instantiationGroupIndex) 
		{
		case 0:

			if(GroupO.Count>0)
			{
				closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));//calculates position based on first unit
				Select0();
			}
			break;
		case 1:
			bool alive1O = false, alive1I = false;

			if(GroupO.Count>0)
			{
				closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));
				alive1O = true;
			}
			if(GroupI.Count>0)
			{
				closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupI[0].transform.position),1));
				alive1I = true;
			}

			if(alive1I) Select1();
			else if(alive1O) Select0();
			break;
		case 2:
			bool alive2O = false, alive2I = false, alive2II = false;

			if(GroupO.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));
			alive2O = true;
			}
			if(GroupI.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupI[0].transform.position),1));
				alive2I = true;
			}
			if(GroupII.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupII[0].transform.position),2));
				alive2II = true;
			}
			if(alive2II) Select2();
			else if(alive2I) Select1();
			else if(alive2O) Select0();
			break;
		case 3:
			bool alive3O = false, alive3I = false, alive3II = false, alive3III = false;
	
			if(GroupO.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupO[0].transform.position),0));
				alive3O = true;
			}
			if(GroupI.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupI[0].transform.position),1));
				alive3I = true;
			}
			if(GroupII.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupII[0].transform.position),2));
				alive3II = true;
			}
			if(GroupIII.Count>0)
			{
			closeGroups.Add(new Vector2(Vector2.Distance(newTargetPos,GroupIII[0].transform.position),3));
				alive3III = true;			
			}
			if(alive3III) Select3();
			else if(alive3II) Select2();
			else if(alive3I) Select1();
			else if(alive3O) Select0();

			break;
		}

		closeGroups.Sort(delegate (Vector2 d1,Vector2 d2){return d1.x.CompareTo(d2.x);});
		return (int)closeGroups [0].y;//returns the closest group index

	}

	public void FindClosestBuilding()		//auto-select next target
	{	
		grassTargets.Clear();
		aiTargetsFree.Clear();

		grassPatches = GameObject.FindGameObjectsWithTag("Grass");

		if (grassPatches.Length == 0) //nothing was found on the map
		{
			mapIsEmpty = true;
			if(!battleOver)
			StartCoroutine("MissionComplete");
			return;
		}

		SearchAliveBuildings ();

		grassTargets.Sort(delegate (Vector2 d1,Vector2 d2)
		{
			return d1.x.CompareTo(d2.x); 
		});

		grassTarget[selectedGroupIndex] = grassPatches[(int)grassTargets[0].y];
		targetBuildingIndex[selectedGroupIndex] = grassPatches[(int)grassTargets[0].y].GetComponent<GrassSelector>().grassIndex;

		targetCenter[selectedGroupIndex] = grassTarget [selectedGroupIndex].transform.position;

		SelectTargetGrid();
	}

	private void StopCannons()
	{
		for (int i = 0; i < Cannons.Count; i++) 
		{
			Cannons[i].GetComponent<CannonController>().fire = false;
		}
	}

	private int GetCannonListIndex(int index)//in case buildings do not start from 0, the index must be translated - buildingIndex 16 becomes list index 1
	{
		int listIndex = 0;
		for (int i = 0; i < Cannons.Count; i++) 
		{
			if(Cannons[i].GetComponent<Selector>().index == index)
			{
				listIndex = i;
				break;
			}
		}
		return listIndex;
	}

	private int GetBuildingListIndex(int index)
	{
		int listIndex = 0;
		for (int i = 0; i < Buildings.Count; i++) 
		{
			if(Buildings[i].GetComponent<BuildingSelector>().buildingIndex == index)
			{
				listIndex = i;
				break;
			}
		}
		return listIndex;
	}

	private void UpdateCannons()
	{
		List<int> keepFiring = new List<int> ();
		keepFiring.Clear ();
		int index = 0;		//this will retrieve the actual cannon index associated with the building index

		for (int i = 0; i <= instantiationGroupIndex; i++) 
		{
			index = GetCannonListIndex(targetBuildingIndex[i]);

			switch (i) 
			{
			case 0:
			if(GroupO.Count>0)
			{				
				Cannons[index].GetComponent<CannonController>().fire = true;
				Cannons[index].GetComponent<CannonController>().target = GroupO[0].transform.position;				
				keepFiring.Add(targetBuildingIndex[i]);
			}
			else
				Cannons[index].GetComponent<CannonController>().fire = false;
			break;

			case 1:
			if(GroupI.Count>0)
			{
				Cannons[index].GetComponent<CannonController>().fire = true;
				Cannons[index].GetComponent<CannonController>().target = GroupI[0].transform.position;			
				keepFiring.Add(targetBuildingIndex[i]);
			}
			else
			{
				bool disableCannon = true;
				for (int j= 0; j < keepFiring.Count; j++) 
				{
					if(targetBuildingIndex[i]==keepFiring[j])
					{
						disableCannon = false;
						break;
					}
				}
				if(disableCannon)
				Cannons[index].GetComponent<CannonController>().fire = false;
			}
			break;

			case 2:
			if(GroupII.Count>0)
			{
				Cannons[index].GetComponent<CannonController>().fire = true;
				Cannons[index].GetComponent<CannonController>().target = GroupII[0].transform.position;			
				keepFiring.Add(targetBuildingIndex[i]);
			}
			else
			{
				bool disableCannon = true;
				for (int j= 0; j < keepFiring.Count; j++) 
				{
					if(targetBuildingIndex[i]==keepFiring[j])
					{
						disableCannon = false;
						break;
					}						
				}
				if(disableCannon)
				Cannons[index].GetComponent<CannonController>().fire = false;
			}
			break;

			case 3:
			if(GroupIII.Count>0)
			{
				Cannons[index].GetComponent<CannonController>().fire = true;
				Cannons[index].GetComponent<CannonController>().target = GroupIII[0].transform.position;		
			}	
			else
			{					
				bool disableCannon = true;
				for (int j= 0; j < keepFiring.Count; j++) 
				{
					if(targetBuildingIndex[i]==keepFiring[j])
					{
						disableCannon = false;
						break;
					}						
				}
				if(disableCannon)
				Cannons[index].GetComponent<CannonController>().fire = false;
			}
			break;				
			}

		}
	}

	public void KillUnit(int groupIndex, int unitIndex)
	{	
		switch (groupIndex) 
		{
		case 0:
			for (int i = 0; i < GroupO.Count; i++) 
			{
				if(GroupO[i].GetComponent<Selector>().index==unitIndex)
				{
					((SoundFX)soundFx).SoldierDie();
					GroupO.RemoveAt(i);
					RemoveFromDeployed(unitIndex);
					break;
				}
			}		
			break;

		case 1:
			for (int i = 0; i < GroupI.Count; i++) 
			{
				if(GroupI[i].GetComponent<Selector>().index==unitIndex)
				{
					((SoundFX)soundFx).SoldierDie();
					GroupI.RemoveAt(i);
					RemoveFromDeployed(unitIndex);
					break;
				}
			}
			break;

		case 2:
			for (int i = 0; i < GroupII.Count; i++) 
			{
				if(GroupII[i].GetComponent<Selector>().index==unitIndex)
				{
					((SoundFX)soundFx).SoldierDie();
					GroupII.RemoveAt(i);
					RemoveFromDeployed(unitIndex);
					break;
				}
			}
			break;

		case 3:
			for (int i = 0; i < GroupIII.Count; i++) 
			{
				if(GroupIII[i].GetComponent<Selector>().index==unitIndex)
				{
					((SoundFX)soundFx).SoldierDie();
					GroupIII.RemoveAt(i);
					RemoveFromDeployed(unitIndex);
					break;
				}
			}
			break;
		}
	}

	private void RemoveFromDeployed(int unitIndex)
	{
		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i].GetComponent<Selector>().index==unitIndex)
			{
				DeployedUnits.RemoveAt(i);
				((StatsBattle)statsBattle).unitsLost++;
				break;
			}
		}
	}

	private void SearchAliveBuildings()
	{
		allDestroyed = true;

		UpdateGroupPosition (selectedGroupIndex);

		for (int i = 0; i < grassPatches.Length; i++) 
		{
			if(!grassPatches[i].GetComponent<GrassSelector>().isDestroyed)
			{
				grassTargets.Add(new Vector2( Vector2.Distance(grassPatches[i].transform.position, unitPosition[selectedGroupIndex]), i ));
				allDestroyed = false;
			}
		}

		if(allDestroyed)
		{
			mapIsEmpty = true;
			if(!battleOver)
			StartCoroutine("MissionComplete");
		}
	}

	private void SelectTargetGrid()
	{
		StopCannons ();
		UpdateCannons ();

		Transform AITargetsParentObj = grassTarget[selectedGroupIndex].transform.FindChild("AITargets");//AiTargets Parent Object
		
		Transform[] aiTargetsChildren = AITargetsParentObj.GetComponentsInChildren<Transform>();//Transform of all AI green squares sorounding a building, free or not

		//also selects AITargets parent, but it's in the middle
		UpdateGroupPosition (selectedGroupIndex);

		for (int i = 1; i < aiTargetsChildren.Length; i++) // 1 skip parent
		{
			int indexCell = GridManager.instance.GetGridIndex(aiTargetsChildren[i].position);
			int col = GridManager.instance.GetColumn(indexCell);
			int row = GridManager.instance.GetRow(indexCell);

			if(!GridManager.instance.nodes[row,col].isObstacle)			 
			{
				aiTargetsFree.Add(new Vector2( Vector2.Distance(aiTargetsChildren[i].position, unitPosition[selectedGroupIndex]), i ));
			}
		}
		
		aiTargetsFree.Sort(delegate (Vector2 d1,Vector2 d2)
		               {
			return d1.x.CompareTo(d2.x); 
		});

		aiTargetVectorsCurrent.Clear ();

		for (int i = 0; i < aiTargetsFree.Count; i++) 
		{
			aiTargetVectorsCurrent.Add(aiTargetsChildren[(int)aiTargetsFree[i].y].position);
		}

		CopyTargetLists ();
		StartCoroutine(UpdateTargetGroup(selectedGroupIndex));
		pauseAttack[selectedGroupIndex] = false;
	}


	private void CopyTargetLists()
	{
		switch (selectedGroupIndex) 
		{
		case 0:
			aiTargetVectorsO.Clear();
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsO.Add(aiTargetVectorsCurrent[i]);
			}
			break;
		case 1:
			aiTargetVectorsI.Clear();			
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsI.Add(aiTargetVectorsCurrent[i]);
			}
			break;
		case 2:
			aiTargetVectorsII.Clear();			
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsII.Add(aiTargetVectorsCurrent[i]);
			}
			break;
		case 3:
			aiTargetVectorsIII.Clear();			
			for (int i = 0; i < aiTargetVectorsCurrent.Count; i++) 
			{
				aiTargetVectorsIII.Add(aiTargetVectorsCurrent[i]);
			}
			break;		
		}
		ResetSurroundIndex ();
	}

	private void ResetSurroundIndex()
	{
		surroundIndex[selectedGroupIndex] = 0;
	}

	private void UpdatePaths()			//makes sure that not all units update paths at the same time - performance
	{
		if(updatePathO)
		{
			updateOTimer += Time.deltaTime;

			if(updateOTimer > 0.1f)//time between nextunit updatePath
			{
				updateOTimer = 0;
				if(updateOCounter < GroupO.Count)		//counter is an integer that cycles through list.Count
				{		
					if(GroupO[updateOCounter]==null) return; // the unit just died
					
						GroupO[updateOCounter].GetComponent<FighterPathFinder>().FindPath();
						GroupO[updateOCounter].GetComponent<FighterController>().ChangeTarget();
						GroupO[updateOCounter].GetComponent<FighterController>().RevertToPathWalk();

						updateOCounter++;

				}
				else
				{
					ResetUpdatePaths(0);
				}
			}
		}
		else if(updatePathI)
		{
			updateITimer += Time.deltaTime;
			
			if(updateITimer > 0.1f)//time between nextunit updatePath
			{
				updateITimer = 0;
				if(updateICounter < GroupI.Count)		//counter is an integer that cycles through list.Count
				{
					if(GroupI[updateICounter]==null) return;

					GroupI[updateICounter].GetComponent<FighterPathFinder>().FindPath();
					GroupI[updateICounter].GetComponent<FighterController>().ChangeTarget();
					GroupI[updateICounter].GetComponent<FighterController>().RevertToPathWalk();
					
					updateICounter++;
				}
				else
				{
					ResetUpdatePaths(1);
				}
			}
		}
		else if(updatePathII)
		{
			updateIITimer += Time.deltaTime;
			
			if(updateIITimer > 0.1f)//time between nextunit updatePath
			{
				updateIITimer = 0;
				if(updateIICounter < GroupII.Count)		//counter is an integer that cycles through list.Count
				{
					if(GroupII[updateIICounter]==null) return;

					GroupII[updateIICounter].GetComponent<FighterPathFinder>().FindPath();
					GroupII[updateIICounter].GetComponent<FighterController>().ChangeTarget();
					GroupII[updateIICounter].GetComponent<FighterController>().RevertToPathWalk();
					
					updateIICounter++;
				}
				else
				{
					ResetUpdatePaths(2);
				}
			}
		}
		else if(updatePathIII)
		{
			updateIIITimer += Time.deltaTime;
			
			if(updateIIITimer > 0.1f)//time between nextunit updatePath
			{
				updateIIITimer = 0;
				if(updateIIICounter < GroupIII.Count)		//counter is an integer that cycles through list.Count
				{
					if(GroupIII[updateIIICounter]==null) return;

					GroupIII[updateIIICounter].GetComponent<FighterPathFinder>().FindPath();
					GroupIII[updateIIICounter].GetComponent<FighterController>().ChangeTarget();
					GroupIII[updateIIICounter].GetComponent<FighterController>().RevertToPathWalk();
					
					updateIIICounter++;
				}
				else
				{
					ResetUpdatePaths(3);
				}
			}
		}

	}

	private void ResetUpdatePaths(int index)
	{
		switch (index) 
		{	

		case 0:
			updatePathO = false;
			updateOCounter = 0;
			updateOTimer = 0;
		break;

		case 1:
			updatePathI = false;
			updateICounter = 0;
			updateITimer = 0;
		break;

		case 2:
			updatePathII = false;
			updateIICounter = 0;
			updateIITimer = 0;
		break;

		case 3:
			updatePathIII = false;
			updateIIICounter = 0;
			updateIIITimer = 0;
		break;

		}
		if (!updatePathO && !updatePathI && !updatePathII && !updatePathIII)
			updatePathMaster = false;
	}


	// Update is called once per frame
	void Update () 
	{
		if (mapIsEmpty||battleOver) { return; }

		for (int i = 0; i <= instantiationGroupIndex; i++) 
		{
			if (updateTarget [i]) 
			{							
				StartCoroutine (UpdateTargetGroup (i));
				updateTarget [i] = false;
			}
		}

		if(updatePathMaster)
		{
			UpdatePaths();
		}

		//Damage
		elapsedTime += Time.deltaTime;

		if (elapsedTime >= intervalTime) 
		{
					
			if(instantiationGroupIndex==3 && DeployedUnits.Count==0) //all deployed units have died, no more slots to deploy another group
			{
				if(!battleOver)
				StartCoroutine("MissionComplete");
				return;
			}
			elapsedTime = 0.0f;

			UpdateCannons ();

			for (int i = 0; i < currentDamage.Length; i++)	{ currentDamage [i] = 0; }//reset the current damage array of 4

			bool noBuildingUnderAttack = true;

			for (int i = 0; i < DeployedUnits.Count; i++) 
			{
				if (DeployedUnits [i] != null) 
				{
					if (DeployedUnits [i].GetComponent<FighterController> ().currentState == FighterController.DobbitState.Attack) 
					{
						currentDamage [DeployedUnits [i].GetComponent<FighterController> ().assignedToGroup]++;
						noBuildingUnderAttack = false;
					}
				}
			}

			if (BuildingsHealth.Count == 0 || noBuildingUnderAttack) 
					return;//print ("no building under attack + buildingshealth.count=0");}//empty map - no buildings || no building under attack
									
			int[] gainPerBuilding = new int[Buildings.Count];//same count as Buildings
					
			for (int i = 0; i <= instantiationGroupIndex; i++) 
			{
				if (!pauseAttack [i] && currentDamage[i]>0) 
				{
					int k = GetBuildingListIndex(targetBuildingIndex [i]);
					gain = (currentDamage [i]*BuildingValues[k])/100;				
					gainPerBuilding[k] += gain;
					BuildingsHealth [k] -= 1 * currentDamage [i];
					DamageBars[k].GetComponent<UISlider>().value = BuildingsHealth [k]*0.01f;//since full building health was 100
				}
			}

			for (int i = 0; i < gainPerBuilding.Length; i++) 
			{
				if(gainPerBuilding[i]>0)
				{

				Vector2 pos = Buildings [i].transform.position;
						
				if(BuildingGoldBased[i])
				{
					GameObject GainGold = (GameObject)Instantiate (GainGoldPf, new Vector3 (pos.x, pos.y+100, smokeZ), Quaternion.identity);
					((StatsBattle)statsBattle).gold += gainPerBuilding[i];//approximates a bit- the building health might reach -17 at last hit 
				}
				else
				{
					GameObject GainMana = (GameObject)Instantiate (GainManaPf, new Vector3 (pos.x, pos.y+100, smokeZ), Quaternion.identity);
					((StatsBattle)statsBattle).mana +=  gainPerBuilding[i];
				}
												
				GameObject[] gainType = GameObject.FindGameObjectsWithTag("Gain");

				for (int j = 0; j < gainType.Length; j++) 
				{
					if(gainType[j].GetComponent<Selector>().isSelected)
					{
						gainType[j].GetComponentInChildren<tk2dTextMesh>().text = "+ " + gainPerBuilding[i].ToString();
						gainType[j].transform.parent = EffectsGroup.transform;
						gainType[j].GetComponent<Selector>().isSelected = false;
						break;
						}
					}						
				}
			}

			((StatsBattle)statsBattle).UpdateUI();

			for (int k = 0; k < targetBuildingIndex.Length; k++) 
			{
			int buildingIndex = targetBuildingIndex [k];
			int i = GetBuildingListIndex(targetBuildingIndex [k]);
					
			if (!DamageEffectsRunning [i] && BuildingsHealth [i] < 90) //instantiates smoke the first time the building is attacked/below 90 health
			{
				//((SoundFX)SoundFx).BuildingBurn(); //sound gets too clogged, remain silent
				DamageEffectsRunning [i] = true;

				GameObject Smoke = (GameObject)Instantiate (SmokePf, new Vector3 (Buildings [i].transform.position.x, 
				                                                                  Buildings [i].transform.position.y, 
				                                                                  smokeZ), Quaternion.identity);
				
				//this parents the smoke effect to the group; for performance, this can be commented
				GameObject[] instSmoke = GameObject.FindGameObjectsWithTag ("Smoke");
					
				for (int j = 0; j < instSmoke.Length; j++) 
				{
					if (instSmoke [j].GetComponent<Selector> ().isSelected) 
					{
						instSmoke [j].transform.parent = EffectsGroup.transform;					
						instSmoke [j].GetComponent<Selector> ().isSelected = false;
						break;
					}
				}
					
				GameObject Fire = (GameObject)Instantiate (FirePf, new Vector3 (Buildings [i].transform.position.x, 
				                                                              Buildings [i].transform.position.y, 
				                                                              fireZ), Quaternion.identity);

				//this parents the fire effect to the group; for performance, this can be commented
				GameObject[] instFire = GameObject.FindGameObjectsWithTag ("Fire");
					
				for (int j = 0; j < instFire.Length; j++) 
				{
					if (instFire [j].GetComponent<Selector> ().isSelected) 
					{
						instFire [j].transform.parent = EffectsGroup.transform;					
						instFire [j].GetComponent<Selector> ().isSelected = false;
						break;
					}
				}
			}
			
			if (BuildingsHealth [i] < 0 && !grassPatches [i].GetComponent<GrassSelector> ().isDestroyed)//explodes once, then marks the grass as destroyed  
			{		
				((SoundFX)soundFx).BuildingExplode();
				((StatsBattle)statsBattle).buildingsDestroyed++;
				
				DamageBars[i].transform.localScale=new Vector3(0.3f,0.2f,1);
				DamageBars[i].GetComponent<UISlider>().foregroundWidget.color = new Color(0,0,0);
				DamageBars[i].GetComponent<UISlider>().value = 1;

				GameObject Explosion = (GameObject)Instantiate (ExplosionPf, new Vector3 (Buildings [i].transform.position.x, Buildings [i].transform.position.y, smokeZ), Quaternion.identity);
				GameObject Rubble = (GameObject)Instantiate (RubblePf, new Vector3 (Buildings [i].transform.position.x, Buildings [i].transform.position.y, rubbleZ), Quaternion.identity);
				
				//this parents the rubble sprite to the effects group; for performance, this can be commented
				GameObject[] instRubble = GameObject.FindGameObjectsWithTag ("Rubble");
																					
				for (int j = 0; j < instRubble.Length; j++) 
				{
					if (instRubble [j].GetComponent<Selector> ().isSelected) 
					{
						instRubble [j].transform.parent = EffectsGroup.transform;					
						instRubble [j].GetComponent<Selector> ().isSelected = false;
						break;
					}
				}
				
				Buildings [i].transform.FindChild ("Sprites").gameObject.SetActive (false);
				Buildings [i].transform.FindChild ("Button").gameObject.SetActive (false);
				Cannons[i].transform.FindChild("Turret").gameObject.SetActive(false);
											
				if(!Buildings[i].activeSelf)//constructions- the building is disabled
				{												
					Buildings [i].transform.parent.GetComponent<ConstructionSelector>().enabled=false;
					Buildings [i].transform.parent.FindChild ("TimeCounterLb").gameObject.SetActive (false);
					Buildings [i].transform.parent.FindChild ("Dobbit").gameObject.SetActive (false);
				}

				for (int j = 0; j < targetBuildingIndex.Length; j++) 
				{
					if (targetBuildingIndex [j] == buildingIndex) //targetBuildingIndex [k] i
					{
						grassTarget [j].GetComponent<GrassSelector> ().isDestroyed = true;
						break;
					}
				}

				for (int j = 0; j <= instantiationGroupIndex; j++)  //-1 4 4 -1 the index for target building index is the group!!!targetBuildingIndex.Length
				{	
					if (targetBuildingIndex [j] == buildingIndex) //targetBuildingIndex [k] i
					{						
						ProcessUnitGroup (j);
					}
				}					
			}
			}
		}
	}
		

	private void ProcessUnitGroup(int index)
	{
		bool groupDead = true;

		UpdateGroupPosition (index);//necessary to separate since it is called by other methods

		switch (index)
		{
		case 0:
			if(GroupO.Count>0)
			{
				Select0();					
				groupDead = false;
			}
			break;

		case 1:
			if(GroupI.Count>0)
			{
				Select1();					
				groupDead = false;
			}
			break;

		case 2:
			if(GroupII.Count>0)
			{
				Select2();
				groupDead = false;
			}				
			break;

		case 3:
			if(GroupIII.Count>0)
			{
				Select3();
				groupDead = false;
			}								
			break;
		}

		if (groupDead) 		
			return;			

		SearchAliveBuildings ();
		
		if(!allDestroyed)
		{ 
			FindClosestBuilding();
		}
		else
		{
			mapIsEmpty = true;
			if(!battleOver)
			StartCoroutine("MissionComplete");
			return;
		}

		pauseAttack[index] = true;
		updateTarget[index] = true;
	}


	private void UpdateSelectStars()//the little stars above each unit
	{
		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i]!=null)
			{
				DeployedUnits[i].transform.GetChild(0).gameObject.SetActive(false);
			}
		}

		for (int i = 0; i < selectedList.Count; i++) 
		{ 
			selectedList[i].transform.GetChild(0).gameObject.SetActive(true);
		}
	}

	private void UpdateGroupPosition(int index)		//the group position is given by the first alive member for simplification
	{
		switch (index) 
		{
		case 0:
			if(GroupO.Count>0)
			{			
				unitPosition[0] = GroupO[0].transform.position;
			}
			break;
		case 1:
			if(GroupI.Count>0)
			{
				unitPosition[1] = GroupI[0].transform.position;
			}
			break;
		case 2:
			if(GroupII.Count>0)
			{
				unitPosition[2] = GroupII[0].transform.position;
			}
			break;
		case 3:
			if(GroupIII.Count>0)
			{
				unitPosition[3] = GroupIII[0].transform.position;
			}
			break;
		}
	}

	IEnumerator UpdateTargetGroup(int index)
	{	
		yield return new WaitForSeconds (1.5f);//1.5f
		if(!allDestroyed)
		UpdateTarget(index);
	}

	private void UpdateTarget(int index)
	{	
		if (instantiationGroupIndex == -1)
						return;

		updatePathMaster = false; //stop update sequence for all
		StopCoroutine ("ResumeUpdateUnits"); //stop resumeupdate in case it was called

		List<GameObject> currentList = new List<GameObject> ();
		currentList.Clear ();

		UpdateGroupPosition (index);

		switch (index) 
		{
		case 0:
			if(GroupO.Count>0)
			{
				currentList= GroupO;
				updatePathO = true;
				updateOCounter = 0;
			}
			break;
		case 1:
			if(GroupI.Count>0)
			{
				currentList= GroupI;
				updatePathI = true;
				updateICounter = 0;
			}
			break;
		case 2:
			if(GroupII.Count>0)
			{
				currentList= GroupII;
				updatePathII = true;
				updateIICounter = 0;
			}
			break;
		case 3:
			if(GroupIII.Count>0)
			{
				currentList= GroupIII;
				updatePathIII = true;
				updateIIICounter = 0;
			}
			break;
		}

		ResetSurroundIndex (); //this is used to suround a building with units

		for (int i = 0; i < currentList.Count; i++)
		{
			currentList[i].GetComponent<FighterController>().targetCenter = new Vector3(targetCenter[index].x,targetCenter[index].y,targetZ);//selectedGroupIndex
			currentList[i].GetComponent<FighterController>().RevertToIdle();
		}

		StartCoroutine ("ResumeUpdateUnits");
		elapsedTime = 0.0f;
		pauseAttack[index] = false;
	}

	private IEnumerator ResumeUpdateUnits()
	{
		yield return new WaitForSeconds(0.3f);
		updatePathMaster = true;
	}

	private void DisablePanels()
	{
		for (int i = 0; i < MenuPanels.Length; i++) // UnitsBattlePanel, OptionsPanel, GhostHelper
		{
			MenuPanels[i].SetActive(false);//prevents the user from launching menus behind the end battle panel
		}

		InterfaceElements[3].SetActive(false);//deactivate units button
		InterfaceElements[4].SetActive(false);//deactivate options button
		((Relay)relay).pauseInput = true;
	}

	public void Retreat()
	{
		StartCoroutine ("MissionComplete");
	}

	IEnumerator MissionComplete()	
	{
		battleOver = true;
		DisablePanels ();
		yield return new WaitForSeconds(0.5f);
		StopCannons ();

		for (int i = 0; i < DeployedUnits.Count; i++) 
		{
			if(DeployedUnits[i]!=null)//##??
			{
				DeployedUnits[i].GetComponent<FighterController>().RevertToIdle();
			}
		}

		ActivateEndGame ();

	}

	private void ActivateEndGame()
	{
		for (int i = 0; i < InterfaceElements.Length; i++) 
		{
			InterfaceElements [i].SetActive (false);
		}

		MissionCompletePanel.SetActive (true);

		NavigationButtons.SetActive (false);//normally these buttons are never off; the mission complete panel has its own navig buttons

		//goldLb, manaLb, buildingsDestroyedLb, unitsLostLb, unitsRecoveredLb;

		goldNoLb.text = ((StatsBattle)statsBattle).gold.ToString();
		manaNoLb.text = ((StatsBattle)statsBattle).mana.ToString();
		buildingsDestroyedNoLb.text = ((StatsBattle)statsBattle).buildingsDestroyed.ToString();
		unitsLostNoLb.text = ((StatsBattle)statsBattle).unitsLost.ToString();

		int remainingUnits = 0;

		for (int i = 0; i < ((StatsBattle)statsBattle).availableUnits.Length; i++) 
		{
			remainingUnits += ((StatsBattle)statsBattle).availableUnits[i];
		}

		unitsRecoveredNoLb.text = remainingUnits.ToString();

		((SaveLoadBattle)saveLoadBattle).SaveAttack();

		((TransData)transData).goldGained = (int)((StatsBattle)statsBattle).gold;
		((TransData)transData).manaGained = (int)((StatsBattle)statsBattle).mana;
		((TransData)transData).battleOver = true;//this variable is checked at game.unity load to see if the user is returning from battle or just started the game
		((TransData)transData).tutorialBattleSeen = ((StatsBattle)statsBattle).tutorialBattleSeen;
	}

}