using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitsPanelBattle : MonoBehaviour {

	private const int 
		unitsNo = 12,  //correlate with MenuUnitBase.cs
		unitGroupsNo = 4,
		unitZ = -3;

	public UILabel[] 
		availableUnitsNo = new UILabel[unitsNo],				//battle = comes from existingUnitsNo in own map
		deployedUnitsNo = new UILabel[unitsNo];					//deployed = comes from battleUnitsNo

	public UISprite[] existingUnitsPics = new UISprite[unitsNo];

	public GameObject[] 
		minusBt = new GameObject[unitsNo],
		UnitPrefabs = new GameObject[unitsNo],
		UnitGroupBt = new GameObject[unitGroupsNo];

	public GameObject 
		spawnPointStar0, spawnPointStarI, spawnPointStarII, spawnPointStarIII,			//prefabs for the stars you click on the edge of the map
		deployBt;																		//we need to deactivate this button at the end of the battle		

	private GameObject GroupUnits;

	private List<GameObject> 
		tempList = new List<GameObject> (),
		starList = new List<GameObject> ();

	private List<Vector2> spawnPointList = new List<Vector2> ();

	public Vector3 spawnPoint = new Vector3(0, 0, unitZ);//cycles in the list and spreads the units on the map

	private Component statsBattle, battleProc, soundFx, relay, messenger;

	void Start () {

		GroupUnits = GameObject.Find("GroupUnits");

		statsBattle = GameObject.Find ("StatsBattle").GetComponent<StatsBattle> ();
		battleProc = GameObject.Find ("BattleProc").GetComponent<BattleProc> ();
		relay = GameObject.Find ("Relay").GetComponent<Relay> ();
		messenger = GameObject.Find ("Messenger").GetComponent<Messenger> ();
		soundFx = GameObject.Find ("SoundFX").GetComponent<SoundFX> ();

		UpdateMinusButtons();
		StartCoroutine (UpdateExistingUnits());
	}

	private void Delay()//to prevent button commands from interfering with sensitive areas/buttons underneath
	{
		((Relay)relay).DelayInput();
	}

	public void RecordSpawnPoint()
	{
		if (!((BattleProc)battleProc).networkLoadReady||((Relay)relay).delay)
						return;

		Vector3 pos = new Vector3(0,0,0);
		Vector3 gridPos = new Vector3(0,0,0);

		// Generate a plane that intersects the transform's position with an upwards normal.
		Plane playerPlane = new Plane(Vector3.back, new Vector3(0, 0, 0));//transform.position + 

		// Generate a ray from the cursor position

		Ray RayCast;

		if (Input.touchCount > 0)
		RayCast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
		else
		RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

		// Determine the point where the cursor ray intersects the plane.
		float HitDist = 0;
		
		// If the ray is parallel to the plane, Raycast will return false.
		if (playerPlane.Raycast(RayCast, out HitDist))//playerPlane.Raycast
		{
			// Get the point along the ray that hits the calculated distance.
			Vector3 RayHitPoint = RayCast.GetPoint(HitDist);
						
			int indexCell = GridManager.instance.GetGridIndex(RayHitPoint);
			
			int col = GridManager.instance.GetColumn(indexCell);
			int row = GridManager.instance.GetRow(indexCell);
				
			if(row==0||row==31||col==0||col==31)
			{
				if(!GridManager.instance.nodes[row,col].isObstacle)
				{
					gridPos = GridManager.instance.nodes[row,col].position;
					pos = new Vector3(gridPos.x,gridPos.y,unitZ);//(RayHitPoint.x,RayHitPoint.y,unitZ);
					CreateStar (gridPos);
				}
			}		
		}
	}

	private void CreateStar(Vector2 gridPos)
	{
		((SoundFX)soundFx).SoldierPlace();
		int currentGroup = ((BattleProc)battleProc).instantiationGroupIndex;
		
		switch (currentGroup) 
		{
		case -1:
			GameObject StarO = (GameObject)Instantiate (spawnPointStar0, new Vector3(gridPos.x,gridPos.y,unitZ), Quaternion.identity);
			break;
		case 0:
			GameObject StarI = (GameObject)Instantiate (spawnPointStarI, new Vector3(gridPos.x,gridPos.y,unitZ), Quaternion.identity);	
			break;
		case 1:
			GameObject StarII = (GameObject)Instantiate (spawnPointStarII, new Vector3(gridPos.x,gridPos.y,unitZ), Quaternion.identity);	
			break;
		case 2:
			GameObject StarIII = (GameObject)Instantiate (spawnPointStarIII, new Vector3(gridPos.x,gridPos.y,unitZ), Quaternion.identity);	
			break;	
		case 3:
			((Messenger)messenger).DisplayMessage("You already deployed all 4 squads.");
		break;
		}

		spawnPointList.Add(new Vector2(gridPos.x,gridPos.y));

		GameObject[] stars = GameObject.FindGameObjectsWithTag ("Star");
		
		for (int i = 0; i < stars.Length; i++) 		
		{
			if(stars[i].GetComponent<Selector>().isSelected)
			{
				starList.Add(stars[i]);
				stars[i].GetComponent<Selector>().isSelected = false;
				break;
			}
		}
	}

	private void DestroyStars()
	{
		for (int i = 0; i < starList.Count; i++) 
		{
			((Star)starList[i].GetComponent("Star")).die = true;
		}
		starList.Clear ();
		spawnPointList.Clear ();
	}

	public void Commit0()	{Commit (0);}
	public void Commit1()	{Commit (1);}
	public void Commit2()	{Commit (2);}
	public void Commit3()	{Commit (3);}
	public void Commit4()	{Commit (4);}
	public void Commit5()	{Commit (5);}
	public void Commit6()	{Commit (6);}
	public void Commit7()	{Commit (7);}
	public void Commit8()	{Commit (8);}
	public void Commit9()	{Commit (9);}
	public void Commit10() {Commit (10);}
	public void Commit11() {Commit (11);}

	public void Cancel0(){Cancel (0);}
	public void Cancel1(){Cancel (1);}
	public void Cancel2(){Cancel (2);}
	public void Cancel3(){Cancel (3);}
	public void Cancel4(){Cancel (4);}
	public void Cancel5(){Cancel (5);}
	public void Cancel6(){Cancel (6);}
	public void Cancel7(){Cancel (7);}
	public void Cancel8(){Cancel (8);}
	public void Cancel9(){Cancel (9);}
	public void Cancel10(){Cancel (10);}
	public void Cancel11(){Cancel (11);}

	public void ActivateDeployBt()
	{
		if(!deployBt.activeSelf)
			deployBt.SetActive(true);
	}

	public void DeactivateDeployBt()
	{
		if(deployBt.activeSelf)
		deployBt.SetActive(false);
	}

	private void UpdateMinusButtons()
	{
		for (int i = 0; i < unitsNo; i++) 
		{
			if (((StatsBattle)statsBattle).deployedUnits [i] > 0) 
			{
				minusBt[i].SetActive(true);
			}
			else
				minusBt[i].SetActive(false);
		}
	}

	private void Commit(int i)
	{
		Delay ();//brief delay to prevent stars from appearing under the menus
		if (((StatsBattle)statsBattle).availableUnits [i] > 0) 
		{
			if (((StatsBattle)statsBattle).deployedUnits [i] == 0) 
			{
				minusBt[i].SetActive(true);
			}
			((StatsBattle)statsBattle).availableUnits [i]--; 
			((StatsBattle)statsBattle).deployedUnits [i]++;
		} 
		else 
		{
			return;
		}

		UpdateUnits ();
	}

	private void Cancel(int i)
	{
		Delay ();//brief delay to prevent stars from appearing under the menus
		if (((StatsBattle)statsBattle).deployedUnits [i] > 0) 
		{
			if (((StatsBattle)statsBattle).deployedUnits [i] == 1) 
			{
				minusBt[i].SetActive(false);
			}
			((StatsBattle)statsBattle).deployedUnits [i]--;
			((StatsBattle)statsBattle).availableUnits [i]++;
		} 
		else 
		{
			return;
		}

		UpdateUnits ();
	}

	public void UpdateStats()
	{
		StartCoroutine (UpdateExistingUnits());//cannot send command directly, takes a while to update from stats
	}

	private IEnumerator UpdateExistingUnits()//building reselect
	{
		yield return new WaitForSeconds(0.25f);
		UpdateUnits ();
	}

	private void UpdateUnits()
	{
		for (int i = 0; i < availableUnitsNo.Length; i++) 
		{
			if(((StatsBattle)statsBattle).availableUnits[i]>0 )
			{
				availableUnitsNo[i].text =  ((StatsBattle)statsBattle).availableUnits[i].ToString();
				((UISprite)existingUnitsPics[i]).color = new Color(255,255,255);
			}

			else
			{
				availableUnitsNo[i].text = " ";
			}

			if(((StatsBattle)statsBattle).deployedUnits[i]>0)
			{
				deployedUnitsNo[i].text = ((StatsBattle)statsBattle).deployedUnits[i].ToString();
				((UISprite)existingUnitsPics[i]).color = new Color(255,255,255);
			}
			else
			{
				deployedUnitsNo[i].text = " ";
			}

			if(((StatsBattle)statsBattle).availableUnits[i] == 0 && ((StatsBattle)statsBattle).deployedUnits[i] == 0)
			{
				((UISprite)existingUnitsPics[i]).color = new Color(0,0,0);
			}
		}
		UpdateMinusButtons ();
	}

	public void DeployUnits()
	{
		Delay ();//brief delay to prevent stars from appearing under the menus or select building target at the same time
		if (starList.Count == 0)
		{
			((Messenger)messenger).DisplayMessage("Select location on the edge of the map.");
			return;//insert message
		}
		if (((BattleProc)battleProc).instantiationGroupIndex >= 3)	//already deployed all 4 groups
		{
			((Messenger)messenger).DisplayMessage("You already deployed all 4 squads.");
			return;
		}
		if(!((BattleProc)battleProc).networkLoadReady)						//map not loaded yet - don't deploy
		{
			((Messenger)messenger).DisplayMessage("Map is not loaded or internet connection failed.");
			return;//insert messages
		}
		bool someUnitSelected = false;

		for (int i = 0; i < unitsNo; i++) 
		{
			if(((StatsBattle)statsBattle).deployedUnits [i] != 0)
			{
				someUnitSelected = true;
				break;
			}
		}

		if (!someUnitSelected) return;										//user has not selected any unit to deploy

		((BattleProc)battleProc).instantiationGroupIndex++;

		if(((BattleProc)battleProc).instantiationGroupIndex!=0)
		((BattleProc)battleProc).selectedGroupIndex++;

		switch (((BattleProc)battleProc).instantiationGroupIndex) 			//manages the last deployed group
		{
		case 0:
			tempList=((BattleProc)battleProc).GroupO;
			((BattleProc)battleProc).Select0();
			UnitGroupBt[0].SetActive(true);
			break;
		case 1:
			tempList=((BattleProc)battleProc).GroupI;
			((BattleProc)battleProc).Select1();
			UnitGroupBt[1].SetActive(true);
			break;
		case 2:
			tempList=((BattleProc)battleProc).GroupII;
			((BattleProc)battleProc).Select2();
			UnitGroupBt[2].SetActive(true);
			break;
		case 3:
			tempList=((BattleProc)battleProc).GroupIII;
			((BattleProc)battleProc).Select3();
			UnitGroupBt[3].SetActive(true);
			break;
		}

		int spawnIndex = 0;

		for (int i = 0; i < unitsNo; i++) 
		{
			int index = ((StatsBattle)statsBattle).deployedUnits [i];
			if(index>0)
			{
				float speedModifier = 0.2f;								//puts some distance between units while walking
				for (int j = 0; j < index; j++) 
				{
					spawnPoint = spawnPointList[spawnIndex];
					if(spawnIndex<spawnPointList.Count-1)			//distributes the units to all spawn points
						spawnIndex++;
					else
						spawnIndex=0;

					InstantiateUnit(i, speedModifier);					
					speedModifier += 0.2f;
				}
			}
		}

		UpdateInterface();

		((BattleProc)battleProc).unitPosition[((BattleProc)battleProc).selectedGroupIndex] = spawnPointList[0];
		((BattleProc)battleProc).FindClosestBuilding ();
		DestroyStars ();
	}

	private void InstantiateUnit(int index, float speedModifier)
	{
		switch (index) 
		{
		case 0:
			GameObject Wizard = (GameObject)Instantiate (UnitPrefabs [0], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Wizard", speedModifier);
		break;

		case 1:
			GameObject Chicken = (GameObject)Instantiate (UnitPrefabs [1], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Chicken", speedModifier);
			break;

		case 2:
			GameObject ClockTree = (GameObject)Instantiate (UnitPrefabs [2], spawnPoint, Quaternion.identity);	
			ProcessUnit ("ClockTree", speedModifier);
		break;

		case 3:
			GameObject Mine = (GameObject)Instantiate (UnitPrefabs [3], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Mine", speedModifier);
			break;

		case 4:
			GameObject Gargoyle = (GameObject)Instantiate (UnitPrefabs [4], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Gargoyle", speedModifier);
			break;

		case 5:
			GameObject Colossus = (GameObject)Instantiate (UnitPrefabs [5], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Colossus", speedModifier);
			break;

		case 6:
			GameObject Monster = (GameObject)Instantiate (UnitPrefabs [6], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Monster", speedModifier);
			break;

		case 7:
			GameObject Porcupiner = (GameObject)Instantiate (UnitPrefabs [7], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Porcupiner", speedModifier);
			break;

		case 8:
			GameObject Jelly = (GameObject)Instantiate (UnitPrefabs [8], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Jelly", speedModifier);
			break;

		case 9:
			GameObject Wisp = (GameObject)Instantiate (UnitPrefabs [9], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Wisp", speedModifier);
			break;

		case 10:
			GameObject Phoenix = (GameObject)Instantiate (UnitPrefabs [10], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Phoenix", speedModifier);
			break;

		case 11:
			GameObject Dragon = (GameObject)Instantiate (UnitPrefabs [11], spawnPoint, Quaternion.identity);	
			ProcessUnit ("Dragon", speedModifier);
			break;
		}
	}

	private void ProcessUnit(string unitType, float speedModifier)
	{
		unitType = "Dobbit";//regardless of selected unit, we will instantiate a dobit; 
		//remove this to process different units 

		GameObject[] units = GameObject.FindGameObjectsWithTag(unitType);				
		for (int i = 0; i < units.Length; i++) 
		{
			if(((Selector)units[i].GetComponent("Selector")).isSelected)
			{
				units[i].transform.parent = GroupUnits.transform;
				units[i].GetComponent<FighterController>().speed += speedModifier;
				units[i].GetComponent<FighterController>().assignedToGroup = ((BattleProc)battleProc).selectedGroupIndex;

				tempList.Add(units[i]);
				((BattleProc)battleProc).DeployedUnits.Add(units[i]);
				((BattleProc)battleProc).instantiationUnitIndex++;
				units[i].GetComponent<Selector>().index = ((BattleProc)battleProc).instantiationUnitIndex;
				((Selector)units[i].GetComponent("Selector")).isSelected = false;
				break;
			}
		}
	}

	private void UpdateInterface()
	{
		for (int i = 0; i < unitsNo; i++) 
		{
			((StatsBattle)statsBattle).deployedUnits [i] = 0;
		}
		UpdateUnits();
	}
}
