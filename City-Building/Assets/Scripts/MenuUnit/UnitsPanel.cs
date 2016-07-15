using UnityEngine;
using System.Collections;

public class UnitsPanel : MonoBehaviour {//the panel with all the units, also used for selecting the army before attack

	public const int numberOfUnits = 12;  //correlate with MenuUnitBase.cs

	public UILabel[] 
		existingUnitsNo = new UILabel[numberOfUnits],
		battleUnitsNo = new UILabel[numberOfUnits];

	public UISprite[] existingUnitsPics = new UISprite[numberOfUnits];
	public GameObject[] minusBt = new GameObject[numberOfUnits];

	public GameObject startBt, commitAllBt, loadingLb;

	private Component transData, saveLoadMap, stats, messenger, soundFX;

	// Use this for initialization
	void Start () {

		stats = GameObject.Find("Stats").GetComponent <Stats>();
		messenger = GameObject.Find ("Messenger").GetComponent<Messenger> ();
		transData = GameObject.Find("TransData").GetComponent<TransData>();
		saveLoadMap = GameObject.Find("SaveLoadMap").GetComponent<SaveLoadMap>();
		soundFX = GameObject.Find("SoundFX").GetComponent<SoundFX>();
		InitializeMinusButtons();
	}

	public void LoadMultiplayer0()
	{	
		bool unitsAssigned = false;

		for (int i = 0; i < ((Stats)stats).battleUnits.Length; i++) 
		{
			if(((Stats)stats).battleUnits[i]>0)
			{
				unitsAssigned = true;
				break;
			}
		}

		if(unitsAssigned && ((Stats)stats).gold >= 250)		
		{
			StartCoroutine(LoadMultiplayerMap(0)); 
		}
		else if(!unitsAssigned)
		{
			((Messenger)messenger).DisplayMessage("Assign units to the battle.");
		}
		else
		{
			((Messenger)messenger).DisplayMessage("You need more gold.");
		}
	
	}

	private IEnumerator LoadMultiplayerMap(int levelToLoad)				//building loot values = half the price
	{	
		((Stats)stats).gold -= 250;										//this is where the price for the battle is payed, before saving the game
		((SaveLoadMap)saveLoadMap).SaveGame ();							//local autosave at battle load

		startBt.SetActive (false);
		commitAllBt.SetActive (false);
		loadingLb.SetActive (true);

		((TransData)transData).battleUnits = ((Stats)stats).battleUnits;
		((TransData)transData).leftBehindUnits = ((Stats)stats).existingUnits;
		((TransData)transData).tutorialBattleSeen = ((Stats)stats).tutorialBattleSeen;

		((TransData)transData).soundOn = ((SoundFX)soundFX).soundOn;
		((TransData)transData).musicOn = ((SoundFX)soundFX).musicOn;

		yield return new WaitForSeconds (0.50f);
		switch (levelToLoad) 
		{
			case 0:
			Application.LoadLevel("Map01");
			break;		
		}
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
	public void Commit10()  {Commit (10);}
	public void Commit11()  {Commit (11);}


	public void Cancel0() {Cancel (0);}
	public void Cancel1() {Cancel (1);}
	public void Cancel2() {Cancel (2);}
	public void Cancel3() {Cancel (3);}
	public void Cancel4() {Cancel (4);}
	public void Cancel5() {Cancel (5);}
	public void Cancel6() {Cancel (6);}
	public void Cancel7() {Cancel (7);}
	public void Cancel8() {Cancel (8);}
	public void Cancel9() {Cancel (9);}
	public void Cancel10(){Cancel (10);}
	public void Cancel11(){Cancel (11);}

	public void CommitAll()
	{
		for (int i = 0; i < ((Stats)stats).existingUnits.Length; i++) 
		{
			if (((Stats)stats).existingUnits [i] > 0) 
			{
				if (((Stats)stats).battleUnits [i] == 0) 
				{
					minusBt[i].SetActive(true);
				}
				((Stats)stats).battleUnits [i] += ((Stats)stats).existingUnits [i];
				((Stats)stats).existingUnits [i] = 0;
			} 
		}		
		UpdateUnits ();
	}

	public void ActivateStartBt()
	{
		if(!startBt.activeSelf)
		{
			startBt.SetActive(true);
			commitAllBt.SetActive (true);
		}
	}

	public void DeactivateStartBt()
	{
		if(startBt.activeSelf)
		{
			startBt.SetActive(false);
			commitAllBt.SetActive (false);
		}
	}
	private void InitializeMinusButtons()
	{
		for (int i = 0; i < numberOfUnits; i++) 
		{
			if (((Stats)stats).battleUnits [i] > 0) 
			{
				minusBt[i].SetActive(true);
			}
		}
	}
	private void Commit(int i)
	{
		if (((Stats)stats).existingUnits [i] > 0) 
		{
			if (((Stats)stats).battleUnits [i] == 0) 
			{
				minusBt[i].SetActive(true);
			}
			((Stats)stats).existingUnits [i]--; 
			((Stats)stats).battleUnits [i]++;
		} 
		else 
		{
			return;
		}

		UpdateUnits ();
	}

	private void Cancel(int i)
	{
		if (((Stats)stats).battleUnits [i] > 0) 
		{
			if (((Stats)stats).battleUnits [i] == 1) 
			{
				minusBt[i].SetActive(false);
			}
			((Stats)stats).battleUnits [i]--;
			((Stats)stats).existingUnits [i]++;
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
		for (int i = 0; i < existingUnitsNo.Length; i++) 
		{
			if(((Stats)stats).existingUnits[i]>0 )
			{
				existingUnitsNo[i].text =  ((Stats)stats).existingUnits[i].ToString();
				((UISprite)existingUnitsPics[i]).color = new Color(255,255,255);
			}
			else
			{
				existingUnitsNo[i].text = " ";
			}

			if(((Stats)stats).battleUnits[i]>0)
			{
				battleUnitsNo[i].text = ((Stats)stats).battleUnits[i].ToString();
				((UISprite)existingUnitsPics[i]).color = new Color(255,255,255);				//normal tint for existing units
			}
			else
			{
				battleUnitsNo[i].text = " ";
			}

			if(((Stats)stats).existingUnits[i] == 0 && ((Stats)stats).battleUnits[i] == 0)
			{
				((UISprite)existingUnitsPics[i]).color = new Color(0,0,0);						//black tint for non-existing units
			}
		}
	}

}
