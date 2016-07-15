using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;

public class BuildingCreator : MonoBehaviour {
	
	private const int noOfBuildings = 11;//number of existing buildings ingame 

	public int buildingIndex = -1;//associates the underlying grass with the building on top, so they can be reselected together

	private int 
		currentSelection = 0,//when a construct building button is pressed, this determines which one
	 	gridx = 256,//necessary to adjust the middle screen "target" to the exact grid X position
		gridy = 181,//necessary to adjust the middle screen "target" to the exact grid Y position
	
		buildingZ = 1,//1//layer depths for building, 3 after parenting; 0 instantiated, 2 after parenting
		padZ = -3,//moving pad
		grassZ = 2;//grass2

	public int[] existingBuildings = new int[noOfBuildings]; // necessary to keep track of each buiding type number and enforce conditions

	private float 
		touchMoveCounter = 0,//drag-moves the buildings in steps
		touchMoveTime = 0.1f;

	public GameObject[] BuildingPrefabs = new GameObject[noOfBuildings];

	public GameObject 
		ConstructionPrefab,//the building "under construction" sand and materials prefab
		BuildingSelectedPanel, //menu that appears when you reselect a finished building - buttons: upgrade, move, ok, cancel	
		DummyObj,// a crosshair that follows the middle of the screen, for placing a new building; position is adjusted to exact grid middle point
		GroupBuildings,// to keep all buildings in one place in the ierarchy, they are parented to this empty object
		MovingPad,// the arrow pad - when created/selected+move, the buildings are parented to this object that can move
		StatsPad,//displays relevant info when a building is reselected
		Grass2x,Grass3x,Grass4x,// the grass prefabs

		StatsCoin,//coin icon appearing on the stats pad
		StatsMana,//mana icon - on the stats pad
		ProductionLabel;//if the building is producing any resource (mana/gold)

	private GameObject 
		selectedBuilding,//current selected building
		selectedGrass,//current selected grass
		selectedConstruction;//current selected "under construction" prefab

	private GameObject[] 
		selectedBuildingType,//necessary to select all buildings of a certain type by tag
		selectedGrassType;// necessary to select all patches of grass of a certain type by tag

	public UILabel[] BuildingPriceLbs = new UILabel[noOfBuildings];//price labels on the construction panel buttons

	public UILabel 
		StatsName,//label with the building name(type)
		StatsDescription,//text description on StatsPad
		StatsGoldProduction,//amount of gold produced per second
		StatsManaProduction;//amount of mana produced per second
		
	public bool 
		isReselect = false,//building is under construction or reselected
		myTown;//stats game obj does not exist on battle mappublic bool myTown;//stats game obj does not exist on battle map

	private bool 
		inCollision = false,// prevents placing the building in inaccessible areas/on top of another building
		pivotCorrection = false,//adjusts the position to match the grid
		displacedonZ = false;

	public TextAsset BuildingsXML;//variables for loading building characteristics from XML
	private List<Dictionary<string,string>> buildings = new List<Dictionary<string,string>>();
	private Dictionary<string,string> dictionary;

	private Component stats, soundFX, transData, cameraController, relay, messenger, mainMenu;

	// Use this for initialization
	void Start () {

		transData = GameObject.Find ("TransData").GetComponent<TransData>();
		relay = GameObject.Find ("Relay").GetComponent<Relay>();
		soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();// //connects to SoundFx - a sound source near the camera
		messenger = GameObject.Find ("Messenger").GetComponent<Messenger> ();
		cameraController  = GameObject.Find("tk2dCamera").GetComponent<CameraController>();//to move building and not scroll map
		mainMenu = GameObject.Find ("UIAnchor").GetComponent<MainMenu> ();

		if(myTown)
		stats = GameObject.Find("Stats").GetComponent <Stats>();//conects to Stats script

		GetBuildingsXML();//reads Buildings.xml
		UpdatePrices ();//updates price labels on building buttons
	}

	private void GetBuildingsXML()//reads buildings XML
	{
		XmlDocument xmlDoc = new XmlDocument(); 
		xmlDoc.LoadXml(BuildingsXML.text); 
		XmlNodeList buildingsList = xmlDoc.GetElementsByTagName("Building");

		foreach (XmlNode buildingInfo in buildingsList)
		{
			XmlNodeList buildingsContent = buildingInfo.ChildNodes;	
			dictionary = new Dictionary<string, string>();
			
			foreach (XmlNode buildingItems in buildingsContent) // levels itens nodes.
			{
				if(buildingItems.Name == "ID")
				{
					dictionary.Add("ID",buildingItems.InnerText); // put this in the dictionary.
				}				
				if(buildingItems.Name == "Name")
				{
					dictionary.Add("Name",buildingItems.InnerText); 
				}				
				if(buildingItems.Name == "Description")
				{
					dictionary.Add("Description",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Type")
				{
					dictionary.Add("Type",buildingItems.InnerText); 
				}	
				if(buildingItems.Name == "BuilderPop")
				{
					dictionary.Add("BuilderPop",buildingItems.InnerText); 
				}	
				if(buildingItems.Name == "PopBonus")
				{
					dictionary.Add("PopBonus",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "ProdPerSec")
				{
					dictionary.Add("ProdPerSec",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "StoreCap")
				{
					dictionary.Add("StoreCap",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "MaxCap")
				{
					dictionary.Add("MaxCap",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Time")
				{
					dictionary.Add("Time",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "XpAward")
				{
					dictionary.Add("XpAward",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "GoldBased")
				{
					dictionary.Add("GoldBased",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "ResCost")
				{
					dictionary.Add("ResCost",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "StoreCategory")
				{
					dictionary.Add("StoreCategory",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "ObjPrereq")
				{
					dictionary.Add("ObjPrereq",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Cap1Prereq")
				{
					dictionary.Add("Cap1Prereq",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Cap2Prereq")
				{
					dictionary.Add("Cap2Prereq",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Cap3Prereq")
				{
					dictionary.Add("Cap3Prereq",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Cap4Prereq")
				{
					dictionary.Add("Cap4Prereq",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Cap5Prereq")
				{
					dictionary.Add("Cap5Prereq",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Up2LevelReq")
				{
					dictionary.Add("Up2LevelReq",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Up2ObjReq")
				{
					dictionary.Add("Up2ObjReq",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Up2Cost")
				{
					dictionary.Add("Up2Cost",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Up2Time")
				{
					dictionary.Add("Up2Time",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Up2XpAward")
				{
					dictionary.Add("Up2XpAward",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Lev2StoreCap")
				{
					dictionary.Add("Lev2StoreCap",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Lev2Prod")
				{
					dictionary.Add("Lev2Prod",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Lev2PopBonus")
				{
					dictionary.Add("Lev2PopBonus",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Up3LevReq")
				{
					dictionary.Add("Up3LevReq",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Up3ObjReq")
				{
					dictionary.Add("Up3ObjReq",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Up3Cost")
				{
					dictionary.Add("Up3Cost",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Up3Time")
				{
					dictionary.Add("Up3Time",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Up3XpAward")
				{
					dictionary.Add("Up3XpAward",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Lev3StoreCap")
				{
					dictionary.Add("Lev3StoreCap",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Lev3Prod")
				{
					dictionary.Add("Lev3Prod",buildingItems.InnerText); 
				}
				if(buildingItems.Name == "Lev3PopBonus")
				{
					dictionary.Add("Lev3PopBonus",buildingItems.InnerText); 
				}
			}
			buildings.Add(dictionary);
		}
	}

	private void UpdatePrices()//updates price labels on building buttons
	{
		for (int i = 0; i < noOfBuildings; i++) 
		{
			BuildingPriceLbs[i].text = buildings [i] ["ResCost"];
			//updates the trans data loot values (half of price) and type goldbased or mana
			((TransData)transData).buildingValues[i] = int.Parse (buildings [i] ["ResCost"])/2;
			((TransData)transData).buildingGoldBased[i] = bool.Parse (buildings [i] ["GoldBased"]);
		}
	}

	// Update is called once per frame
	void Update () {
		if (MovingPad.activeSelf) 
		{
			touchMoveCounter += Time.deltaTime;

			if(touchMoveCounter > touchMoveTime)
			{
				touchMoveCounter=0;
				TouchMove();
			}

			if (Input.touchCount > 0 && Input.GetTouch(0).tapCount == 2 )//&& Input.GetTouch(0).phase == TouchPhase.Ended
			{
				OK();
			}
		}	
	}

	//receive a NGUI button message to build
	public void OnBuild0()	 { currentSelection=0; 	VerifyConditions("Academy"); }//when a building construction menu button is pressed
	public void OnBuild1()	 { currentSelection=1;  VerifyConditions("Barrel"); }
	public void OnBuild2()	 { currentSelection=2;  VerifyConditions("Chessboard"); }
	public void OnBuild3()	 { currentSelection=3;  VerifyConditions("Classroom"); }
	public void OnBuild4()	 { currentSelection=4; 	VerifyConditions("Forge"); }
	public void OnBuild5()	 { currentSelection=5;  VerifyConditions("Generator"); }
	public void OnBuild6()	 { currentSelection=6;  VerifyConditions("Globe"); }
	public void OnBuild7()	 { currentSelection=7;  VerifyConditions("Summon"); }
	public void OnBuild8()	 { currentSelection=8;  VerifyConditions("Toolhouse"); }
	public void OnBuild9()	 { currentSelection=9;  VerifyConditions("Vault"); }
	public void OnBuild10() { currentSelection=10; VerifyConditions("Workshop"); }
	
	//receive a Tk2d button message to select an existing building; the button is in the middle of each building prefab and is invisible 
	public void OnReselect0(){currentSelection = 0; StartCoroutine(ReselectObject("Academy"));}//when a building is reselected
	public void OnReselect1(){currentSelection = 1; StartCoroutine(ReselectObject("Barrel"));}
	public void OnReselect2(){currentSelection = 2; StartCoroutine(ReselectObject("Chessboard"));}
	public void OnReselect3(){currentSelection = 3; StartCoroutine(ReselectObject("Classroom"));}
	public void OnReselect4(){currentSelection = 4; StartCoroutine(ReselectObject("Forge"));}
	public void OnReselect5(){currentSelection = 5; StartCoroutine(ReselectObject("Generator"));}
	public void OnReselect6(){currentSelection = 6; StartCoroutine(ReselectObject("Globe"));}
	public void OnReselect7(){currentSelection = 7; StartCoroutine(ReselectObject("Summon"));}
	public void OnReselect8(){currentSelection = 8; StartCoroutine(ReselectObject("Toolhouse"));}
	public void OnReselect9(){currentSelection = 9; StartCoroutine(ReselectObject("Vault"));}
	public void OnReselect10(){currentSelection = 10; StartCoroutine(ReselectObject("Workshop"));}
	
	public void CancelObject()//cancel construction, or reselect building and destroy/cancel
	{	
		if (!isReselect) 
		{
			((Stats)stats).occupiedDobbitNo--;//frees the dobbit
			if (buildings [currentSelection] ["GoldBased"] == "true")//refunds the gold/mana 
			{
				((Stats)stats).gold += int.Parse (buildings [currentSelection] ["ResCost"]);				
			} 
			else 
			{
				((Stats)stats).mana += int.Parse (buildings [currentSelection] ["ResCost"]);				
			}
			((Stats)stats).UpdateUI();//update stats interface
		}

		else 
		{
			if (currentSelection == 1) //1=Barrel (mana)
			{
				DecreaseStorage(2);
			}
			else if (currentSelection == 4) //4=Forge (gold)
			{
				((Stats)stats).productionBuildings[0]--;
				DecreaseStorage(1);
			} 
			else if (currentSelection == 5) //5=Generator (mana)
			{
				((Stats)stats).productionBuildings[1]--;	
				DecreaseStorage(2);
			}

			else if (currentSelection == 9) //9=Vault (gold)
			{
				DecreaseStorage(1);
			}

			DeactivateStatsPad ();
		}

		((Stats)stats).experience -= int.Parse (buildings [currentSelection] ["XpAward"]);

		Destroy(selectedBuilding);
		existingBuildings [currentSelection]--;//decreases the array which counts how many buildings of each type you have 
		Destroy(selectedGrass);

		MovingPad.SetActive(false);					//deactivates the arrow building moving platform
		BuildingSelectedPanel.SetActive (false);	//deactivates the buttons move/upgrade/place/cancel, at the bottom of the screen
		((Relay)relay).pauseInput = false;			//while the building is selected, pressing other buttons has no effect
		((Relay)relay).pauseMovement = false;		//the confirmation screen is closed
		if(isReselect){ isReselect = false;}		//end the reselect state		
	}

	private void DecreaseStorage(int restype)//when a building is reselected and destroyed, the gold/mana storage capacity decrease; 
	{
		if(restype==1)//gold
		{
			((Stats)stats).maxStorageGold -= int.Parse (buildings [currentSelection] ["StoreCap"]);//the destroyed building storage cap
			if(((Stats)stats).gold > ((Stats)stats).maxStorageGold)//more gold than max storage?
			{
				((Stats)stats).gold = ((Stats)stats).maxStorageGold;//discards resources exceeding storage capacity

			}
		}
		else if (restype==2) //mana
		{
			((Stats)stats).maxStorageMana -= int.Parse (buildings [currentSelection] ["StoreCap"]);
			if(((Stats)stats).mana > ((Stats)stats).maxStorageMana)
			{
				((Stats)stats).mana = ((Stats)stats).maxStorageMana;

			}
		}	
		((Stats)stats).UpdateUI();//updates the interface numbers
	}

	//  verifies if the building can be constructed:
	//  exceeds max number of buildings / enough gold/mana/free dobbits to build?
	//  pays the price to Stats; updates the Stats interface numbers
	private void VerifyConditions(string type)
	{
		bool canBuild = true;//must pass as true through all verifications

		//max allowed buildings ok?
		if (int.Parse (buildings [currentSelection] ["MaxCap"]) > 0 && //there is a maximum number of buildings permitted for this one; 0 means irrelevant, you can have as many as you want
		existingBuildings[currentSelection] >= int.Parse(buildings [currentSelection] ["MaxCap"]))//max already reached
		{					
			canBuild = false;
			((Messenger)messenger).DisplayMessage("Maximum " + buildings [currentSelection] ["MaxCap"] + 
			                              " buildings of type " +
			                              buildings [currentSelection] ["Name"]+"."); //displays the hint - you can have only 3 buildings of this type
		}

		//enough gold?
		if (buildings [currentSelection] ["GoldBased"] == "true") //this needs gold
		{
			if (((Stats)stats).gold < int.Parse (buildings [currentSelection] ["ResCost"])) 
			{
				canBuild = false;
				((Messenger)messenger).DisplayMessage("Not enough gold.");//updates hint text
			}
		} 
		else  //this needs mana; enough mana?
		{
			if(((Stats)stats).mana < int.Parse (buildings [currentSelection] ["ResCost"]))
			{
				canBuild = false;
				((Messenger)messenger).DisplayMessage("Not enough mana.");//updates hint text
			}
		}

		if (((Stats)stats).occupiedDobbitNo >= ((Stats)stats).dobbitNo) //dobbit available?
		{
			canBuild = false;
			((Messenger)messenger).DisplayMessage("You need more dobbits.");
		}

		if (canBuild) 
		{
			((MainMenu)mainMenu).constructionGreenlit = true;//ready to close menus and place the building; 
			//constructionGreenlit bool necessary because the command is sent by pressing the button anyway
			existingBuildings [currentSelection]++;//an array that keeps track of how many buildings of each type exist

			((Stats)stats).maxPopulation += int.Parse (buildings [currentSelection] ["PopBonus"]); //increase maxPopulation ; not displayed on interface/taken into account yet - no units
			((Stats)stats).experience += int.Parse (buildings [currentSelection] ["XpAward"]); //increases Stats experience

			//pays the gold/mana price to Stats
			if(buildings [currentSelection] ["GoldBased"] == "true")
			{
				((Stats)stats).gold -= int.Parse (buildings [currentSelection] ["ResCost"]); 
			}
			else
			{
				((Stats)stats).mana -= int.Parse (buildings [currentSelection] ["ResCost"]);
			}

			((Stats)stats).UpdateUI();//tells stats to update the interface - otherwise new numbers are updated but not displayed
			LoadBuilding ();
		} 
		else 
		{
			((MainMenu)mainMenu).constructionGreenlit = false;//halts construction - the button message is sent anyway, but ignored
		}


	}

	private void LoadBuilding()//instantiates the building and grass prefabs
	{
		((Stats)stats).occupiedDobbitNo++;//get one dobbit

		((Relay)relay).pauseInput = true;//pause all other input - the user starts moving the building
		
		pivotCorrection = false;//used to flag necessary correction so all buildings are centered on the grid square
		
		switch (currentSelection) //instantiates the building + appropriate underlying grass patch
		{
		case 0:
			GameObject Academy = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);			
			GameObject G4xAcademy = (GameObject)Instantiate(Grass4x, new Vector3(0,0,grassZ), Quaternion.identity);	
			pivotCorrection = true;
			SelectObject("Academy");
			break;
		case 1:
			GameObject Barrel = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);
			GameObject G3xBarrel = (GameObject)Instantiate(Grass3x, new Vector3(0,0,grassZ), Quaternion.identity);	
			SelectObject("Barrel");			
			break;
		case 2:
			GameObject Chessboard = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);	 
			GameObject G3xChessboard = (GameObject)Instantiate(Grass3x, new Vector3(0,0,grassZ), Quaternion.identity);	
			SelectObject("Chessboard");
			break;
		case 3:
			GameObject Classroom = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);	 
			GameObject G3xClassroom = (GameObject)Instantiate(Grass3x, new Vector3(0,0,grassZ), Quaternion.identity);	
			SelectObject("Classroom");
			break;
		case 4:
			GameObject Forge = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);	
			GameObject G3xForge = (GameObject)Instantiate(Grass3x, new Vector3(0,0,grassZ), Quaternion.identity);

			((Stats)stats).productionRates[0] = float.Parse(buildings [currentSelection] ["ProdPerSec"]);//if the building produces resources, the productionRates is passed to Stats;

			SelectObject("Forge");
			break;
		case 5:
			GameObject Generator = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);	
			GameObject G3xGenerator = (GameObject)Instantiate(Grass3x, new Vector3(0,0,grassZ), Quaternion.identity);	

			((Stats)stats).productionRates[1] = float.Parse(buildings [currentSelection] ["ProdPerSec"]);

			SelectObject("Generator");						
			break;
		case 6:
			GameObject Globe = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);	
			GameObject G3xGlobe = (GameObject)Instantiate(Grass3x, new Vector3(0,0,grassZ), Quaternion.identity);	
			SelectObject("Globe");
			break;
		case 7:			
			GameObject Summon = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);	
			GameObject G3xSummon = (GameObject)Instantiate(Grass3x, new Vector3(0,0,grassZ), Quaternion.identity);	
			SelectObject("Summon");
			break;
		case 8:
			GameObject Toolhouse = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);	
			GameObject G2xToolhouse = (GameObject)Instantiate(Grass2x, new Vector3(0,0,grassZ), Quaternion.identity);	
			pivotCorrection = true;
			SelectObject("Toolhouse");
			break;	
		case 9:
			GameObject Vault = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);;	
			GameObject G3xVault = (GameObject)Instantiate(Grass3x, new Vector3(0,0,grassZ), Quaternion.identity);	
			SelectObject("Vault");
			break;
		case 10:
			GameObject Workshop = (GameObject)Instantiate(BuildingPrefabs[currentSelection], new Vector3(0,0,buildingZ), Quaternion.identity);	
			GameObject G3xWorkshop = (GameObject)Instantiate(Grass3x, new Vector3(0,0,grassZ), Quaternion.identity);	
			SelectObject("Workshop");
			break;				
		default:
		break;
		}	
	}
			
	private void SelectObject(string buildingTag) //after the grass/building prefabs are instantiated, they must be selected from the existing buildings on the map
	{
		BuildingSelectedPanel.SetActive (true);// the move/upgrade/place/cancel, at the bottom of the screen
		selectedBuildingType = GameObject.FindGameObjectsWithTag(buildingTag);//finds all existing buildings with the apropriate string tag(ex “Forge”)	
		selectedGrassType = GameObject.FindGameObjectsWithTag("Grass");//finds all grass	
			
		//both the grass and buildings are instantiated with the isSelected bool variable as true; 
		//this allows us to find them on the map, as being the new/latest ones.
		foreach (GameObject grass in selectedGrassType) 
		{
		if(((GrassSelector)grass.GetComponent("GrassSelector")).isSelected)				
			{
				selectedGrass = grass;
				break;//found it. exit foreach loop
			}
		}
				
		foreach (GameObject building in selectedBuildingType) 
		{
		if(((BuildingSelector)building.GetComponent("BuildingSelector")).isSelected)				
			{				
				selectedBuilding = building; //the selected building is registered for the entire class				
				int posX = (int) (DummyObj.transform.position.x-//calculates the middle of the screen - the DummyObj position,
				DummyObj.transform.position.x%gridx); //and adjusts it to match the grid
				int posY = (int)(DummyObj.transform.position.y-
					DummyObj.transform.position.y%gridy);
								
				MovingPad.SetActive(true);//activates the arrow move platform
								
				if(pivotCorrection)					
				{
					selectedBuilding.transform.position = new Vector3(posX+gridx/2, posY, buildingZ-6);//-2 moves the building to position				
					selectedGrass.transform.position = new Vector3(posX+gridx/2, posY, grassZ-2);	//grass
					MovingPad.transform.position = new Vector3(posX+gridx/2, posY, padZ);//move pad
				}
				
				else
				{
					selectedBuilding.transform.position = new Vector3(posX, posY, buildingZ-6);//the building must appear in front				
					selectedGrass.transform.position = new Vector3(posX, posY, grassZ-2);	
					MovingPad.transform.position = new Vector3(posX, posY, padZ);
				}
			
				selectedBuilding.transform.parent = MovingPad.transform;//parents the selected building to the arrow moving platform
				selectedGrass.transform.parent = MovingPad.transform;//parents the grass to the move platform
				((Relay)relay).pauseInput = true;//pause other input so the user can move the building	
				((CameraController)cameraController).movingBuilding = true;
				break;//exit foreach loop
			}
		}
	}

	public void ActivateMovingPad()//move pad activated and translated into position
	{
		if (!MovingPad.activeSelf) 
		{
			MovingPad.SetActive (true);
			DeactivateStatsPad ();

			selectedBuilding.transform.parent = MovingPad.transform;
			selectedGrass.transform.parent = MovingPad.transform;

			if (isReselect) 
			{
			selectedGrass.transform.position = new Vector3 (selectedGrass.transform.position.x,
		                                       selectedGrass.transform.position.y,
		                                       selectedGrass.transform.position.z - 1.0f);//move to front 0.5

			selectedBuilding.transform.position = new Vector3 (selectedBuilding.transform.position.x,
			                                   selectedBuilding.transform.position.y,
			                                   selectedBuilding.transform.position.z - 6);//move to front
			displacedonZ = true;
			}
			((CameraController)cameraController).movingBuilding = true;
		}
	}

	public void ActivateStatsPad()//displays the small stats window
	{
		StatsPad.SetActive (true);

		StatsName.text = buildings [currentSelection] ["Name"];
		StatsDescription.text = buildings [currentSelection] ["Description"];

		ProductionLabel.SetActive (false);
		StatsCoin.SetActive (false);
		StatsMana.SetActive (false);

		if (buildings [currentSelection] ["Name"] == "Gold Forge") 
		{
			ProductionLabel.SetActive (true);
			StatsCoin.SetActive (true);
			StatsGoldProduction.text = buildings [currentSelection] ["ProdPerSec"];
		} 
		else if (buildings [currentSelection] ["Name"] == "Mana Generator") 
		{
			ProductionLabel.SetActive (true);
			StatsMana.SetActive (true);
			StatsManaProduction.text = buildings [currentSelection] ["ProdPerSec"];
		}

		StatsPad.transform.position = selectedBuilding.transform.position;
	}

	public void DeactivateStatsPad()
	{
		StatsPad.SetActive (false);
	}

	public void DeactivateStatsPadSound()
	{
		((SoundFX)soundFX).Close();
		StatsPad.SetActive (false);
	}


	private IEnumerator ReselectObject(string type)//building reselect
	{	
		yield return new WaitForSeconds(0.25f);

		BuildingSelectedPanel.SetActive (true);

		selectedBuildingType = GameObject.FindGameObjectsWithTag(type);		
		selectedGrassType = GameObject.FindGameObjectsWithTag("Grass");	
				
		foreach (GameObject building in selectedBuildingType) 
		{
		if(((BuildingSelector)building.GetComponent("BuildingSelector")).isSelected)				
			{
				selectedBuilding = building;

				MovingPad.transform.position = 
					new Vector3(building.transform.position.x,
						building.transform.position.y, padZ);

				((Relay)relay).pauseInput = true;				
				break;
			}
		}
		
		foreach (GameObject grass in selectedGrassType) 
		{
		if(((GrassSelector)grass.GetComponent("GrassSelector")).grassIndex ==
				((BuildingSelector)selectedBuilding.GetComponent("BuildingSelector")).buildingIndex)				
			{
				selectedGrass = grass;
				selectedGrass.GetComponentInChildren<GrassCollider>().enabled = true;
				((GrassCollider)selectedGrass.GetComponentInChildren<GrassCollider>()).isMoving = true;
				break;
			}
		}

		ActivateStatsPad ();
	}

	public void MoveNW(){Move(0);}//-+
	public void MoveNE(){Move(1);}//++
	public void MoveSE(){Move(2);}//+-
	public void MoveSW(){Move(3);}//--
	public void Cancel()
	{
		CancelObject();
		((Relay)relay).DelayInput();
	}
	public void OK()
	{
		inCollision = selectedGrass.GetComponentInChildren<GrassCollider>().inCollision;
		if (!inCollision) 
		{
			if(isReselect){	DeactivateStatsPad();}
			PlaceObject ();
		}
		((Relay)relay).pauseMovement = false;	//the confirmation screen is closed
	}

	private void TouchMove()
	{
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)		        
		{
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;

			if(touchDeltaPosition.x < 0)
			{
				if(touchDeltaPosition.y < 0)
				{
					MoveSW();
				}
				else if(touchDeltaPosition.y > 0)
				{
						MoveNW();
				}
			}
			else if(touchDeltaPosition.x > 0)
			{
				if(touchDeltaPosition.y < 0)
				{
					MoveSE();

				}
				else if(touchDeltaPosition.y > 0)
				{
					MoveNE();
				}
			}
		}
	}

	private void Move(int i)
	{
		if (((Relay)relay).pauseMovement || ((Relay)relay).delay) return;

		((SoundFX)soundFX).Move(); //128x64

		switch (i) 
		{
			case 0:
				MovingPad.transform.position += new Vector3(-gridx/2,gridy/2,0);//NW	
			break;
			
			case 1:
				MovingPad.transform.position += new Vector3(gridx/2,gridy/2,0);//NE		
			break;
			
			case 2:
				MovingPad.transform.position += new Vector3(gridx/2,-gridy/2,0);//SE		
			break;
			
			case 3:
				MovingPad.transform.position += new Vector3(-gridx/2,-gridy/2,0);//SW		
			break;				
		}	
		CheckPosition ();
	}

	private void CheckPosition()	//since we are using increments for movement, we chack at each step and correct if necessary
	{
		float posX = MovingPad.transform.position.x;
		float posY = MovingPad.transform.position.y;

		float posXdif = MovingPad.transform.position.x % (gridx / 2);//-0.1
		float posYdif = MovingPad.transform.position.y % (gridy / 2);//0.2

		float adjX = posX - posXdif;
		float adjY = posY - posYdif;

		MovingPad.transform.position = new Vector3 (adjX, adjY, padZ);
	}

	public void PlaceObject()
	{
		if(!isReselect)
		{
			buildingIndex++;//unique number for pairing the buildings and the grass patches underneath
			((BuildingSelector)selectedBuilding.GetComponent("BuildingSelector")).buildingIndex = buildingIndex;
			((GrassSelector)selectedGrass.GetComponent("GrassSelector")).grassIndex = buildingIndex;


			//instantiates the construction prefab and pass the relevant info;
			GameObject Construction = (GameObject)Instantiate(ConstructionPrefab, 
			                                                  new Vector3(selectedBuilding.transform.position.x, 
			            									  selectedBuilding.transform.position.y, 
			            									  selectedBuilding.transform.position.z+6), 
			                                                  Quaternion.identity);
			selectedConstruction = Construction;
			((ConstructionSelector)selectedConstruction.GetComponent("ConstructionSelector")).constructionIndex = buildingIndex;
			((ConstructionSelector)selectedConstruction.GetComponent("ConstructionSelector")).buildingTime =
				int.Parse (buildings [currentSelection] ["Time"]);
			((ConstructionSelector)selectedConstruction.GetComponent("ConstructionSelector")).storageIncrease=
				int.Parse (buildings [currentSelection] ["StoreCap"]);
			((ConstructionSelector)selectedConstruction.GetComponent("ConstructionSelector")).buildingType=
				((BuildingSelector)selectedBuilding.GetComponent("BuildingSelector")).buildingType;
		}
		((Relay)relay).DelayInput();			//delay and pause input = two different things 
		((Relay)relay).pauseInput = false;		//main menu butons work again
		((BuildingSelector)selectedBuilding.GetComponent("BuildingSelector")).isSelected = false;
		((GrassSelector)selectedGrass.GetComponent("GrassSelector")).isSelected = false;
		
		((GrassCollider)selectedGrass.GetComponentInChildren<GrassCollider>()).isMoving = false;		
		selectedGrass.GetComponentInChildren<GrassCollider>().enabled = false;		
				
		//--> Reselect
		if(!isReselect)
		{
			selectedConstruction.transform.parent = GroupBuildings.transform;
			selectedGrass.transform.parent = selectedConstruction.transform;

			selectedBuilding.transform.position = new Vector3(selectedBuilding.transform.position.x, 
			                                                  selectedBuilding.transform.position.y, 
			                                                  selectedBuilding.transform.position.z+6); 

			selectedGrass.transform.position = new Vector3 (selectedGrass.transform.position.x,
			                                                selectedGrass.transform.position.y,
			                                                selectedGrass.transform.position.z + 1);//cancel the instantiation z correction

			selectedBuilding.transform.parent = selectedConstruction.transform;
			selectedBuilding.SetActive(false);
		}
		else if(displacedonZ)
		{
			//send the buildings 1 z unit to the background
			selectedGrass.transform.position = new Vector3 (selectedGrass.transform.position.x,
			                                                selectedGrass.transform.position.y,
			                                                selectedGrass.transform.position.z + 1.0f);//move to back 0.5
			selectedBuilding.transform.position = new Vector3(selectedBuilding.transform.position.x, 
			                                                  selectedBuilding.transform.position.y, 
			                                                  selectedBuilding.transform.position.z+6); 
			selectedBuilding.transform.parent = GroupBuildings.transform;
			selectedGrass.transform.parent = selectedBuilding.transform;	
			displacedonZ = false;
		}

		//<--		
		MovingPad.SetActive(false);
		((CameraController)cameraController).movingBuilding = false;
		BuildingSelectedPanel.SetActive (false);
		isReselect = false;

	}	
}
