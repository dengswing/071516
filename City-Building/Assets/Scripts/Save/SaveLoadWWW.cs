using UnityEngine;
using System.Xml;
using System.IO;
using System.Collections;
using System.Text;

public class SaveLoadWWW : MonoBehaviour
{
	private string 
		filePath,
		fileExt = ".txt",
		attackExt = "_results",				//_attack

		fileNameLocal = "LocalSave",		//local - the full game save
		fileNameServer = "ServerSave",		//local - the full game save
		fileNameAttack = "ServerAttack",
		fileNameMapID = "MyMapID",				//local - saves the name of the map from server

		myMapIDCode;						//example: gfdfghjke

	private Component saveLoadMap, messenger, stats; 
	private bool saving = false;

	void Start () 
	{
		//filePath = Application.dataPath + "/";//windows - same folder as the project
		filePath = Application.persistentDataPath +"/";//iphone

		saveLoadMap = GameObject.Find ("SaveLoadMap").GetComponent<SaveLoadMap> ();
		messenger = GameObject.Find ("Messenger").GetComponent<Messenger> ();
		stats = GameObject.Find("Stats").GetComponent<Stats>();

		//StartCoroutine ("ServerAutoLoad");	//automatic server load at startup - prevents user from building and then loading on top
	}

	private IEnumerator ServerAutoLoad()
	{
		yield return new WaitForSeconds (2);
		LoadFromServer ();
	}

	private bool CheckLocalSaveFile()
	{
		if(!saving)
		((Messenger)messenger).DisplayMessage("Checking for local save file...");

		bool localSaveExists = File.Exists(filePath + fileNameLocal + fileExt);

		if(!localSaveExists) 
			((Messenger)messenger).DisplayMessage("Local save file not found. Save locally first.");

		return(localSaveExists);
	}


	private bool CheckServerSaveFile()//local recording of a previous save ID on server
	{
		((Messenger)messenger).DisplayMessage("Checking for map ID...");//local - result of server save

		bool serverSaveExists = File.Exists(filePath + fileNameMapID + fileExt);

		if(!serverSaveExists) 
			((Messenger)messenger).DisplayMessage("You have no map ID."); //no loca ID - no server save file


		return(serverSaveExists);
		//checks if the mapcode was saved locally, not if it is still available on server
	}

	private void GenerateUserid()
	{
		//generate a long random file name , to avoid duplicates and overwriting	
		string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		char[] stringChars = new char[10];
				
		for (int i = 0; i < stringChars.Length; i++)
		{
			stringChars[i] = chars[Random.Range(0, chars.Length)];
		}
		
		myMapIDCode = new string(stringChars);	
		((Messenger)messenger).DisplayMessage("New map ID generated: "+ myMapIDCode);
	}
	
	public void SaveUserid()
	{
		//the user has to retrieve his own file, in case someone has attacked his city
		StreamWriter sWriter = new StreamWriter (filePath + fileNameMapID + fileExt);
		sWriter.WriteLine (myMapIDCode);
		sWriter.Flush ();
		sWriter.Close ();
		((Messenger)messenger).DisplayMessage("New map ID saved: "+ myMapIDCode);
	}
	
	public void ReadUserid()
	{
		StreamReader sReader = new StreamReader(filePath + fileNameMapID + fileExt);
		myMapIDCode = sReader.ReadLine();
		((Messenger)messenger).DisplayMessage("Retrieved map ID: " + myMapIDCode);
	}

	public void SaveToServer()
	{
		((Stats)stats).gameWasLoaded = true;//saving the game also prevents the user from loading on top

		//can upload to server only if the game was previously saved locally
		if(CheckLocalSaveFile() && !saving)
		{
			saving = true;
			StartCoroutine("UploadLevel");//force the local map save before this
			((Messenger)messenger).DisplayMessage("Uploading to server...");
		}
		else if (saving) 
		{
			((Messenger)messenger).DisplayMessage("Upload in progress. Please wait...");
		}
	}

	IEnumerator UploadLevel()   
	{
		byte[] levelData = System.IO.File.ReadAllBytes(filePath + fileNameLocal + fileExt);//full local save file
		//byte[] levelData = Encoding.UTF8.GetBytes(filePath + myMapLocal + fileExt);

		bool serverSaveExists = false;
		if(CheckServerSaveFile())
		{
			serverSaveExists = true;
			ReadUserid();
		}
		else
		{
			GenerateUserid();
		}

		/*
		- you save the generated ID, then retrieve the uploaded file, without the needs of listings
		- the same method is used to retrieve and validate any uploaded file
		- this method is similar to the one used by the games like Bike Baron
		- saves you from the hassle of making complex server side back ends which enlists available levels
		- you could enlist outstanding levels just by posting the level code on a server 
		- easier to share, without the need of user accounts or install procedures
		*/

		WWWForm form = new WWWForm();
						
		form.AddField("savefile","file");
		form.AddBinaryData( "savefile", levelData, myMapIDCode,"text/xml");//file
			
		//change the url to the url of the php file
		WWW w = new WWW("http://www.citybuildingkit.com/get_match.php?mapid=" + myMapIDCode, form);//myUseridFile 

		yield return w;

		if (w.error != null)
		{
			((Messenger)messenger).DisplayMessage("File upload error.");
			print ( w.error );    
		}
		else
		{
			//this part validates the upload, by waiting 5 seconds then trying to retrieve it from the web
			if(w.uploadProgress == 1 && w.isDone)
			{
				yield return new WaitForSeconds(5);
				//change the url to the url of the folder you want it the levels to be stored, the one you specified in the php file
				WWW w2 = new WWW("http://www.citybuildingkit.com/get_match.php?get_user_map=1&mapid=" + myMapIDCode);//returns a specific map
								
				yield return w2;

				if(w2.error != null)
				{
					((Messenger)messenger).DisplayMessage("Server file check error.");
					print ( w2.error );  
				}
				else
				{
					//then if the retrieval was successful, validate its content to ensure the map file integrity is intact
					if(w2.text != null && w2.text != "")
					{
						if(w2.text.Contains("###StartofFile###") && w2.text.Contains("###EndofFile###"))
						{
							//and finally announce that everything went well
							print ( "Received file map ID " + myMapIDCode + " contents are: \n\n" + w2.text);

							if(!serverSaveExists) SaveUserid();

							((Messenger)messenger).DisplayMessage("Server upload complete.");
						}
						else
						{
							print ( "Map file " + myMapIDCode + " is invalid");
							print ( "Response file " + myMapIDCode + " contents are: \n\n" + w2.text);//although file is broken, prints the content of the retrieved file

							((Messenger)messenger).DisplayMessage("Server upload incomplete. Try again.");
						}
					}
					else
					{
						print ( "Map file " + myMapIDCode + " is empty");
						((Messenger)messenger).DisplayMessage("Server upload check failed.");
					}
				}

			
			}     
		}
		saving = false; //even if failed, user can try to save again
	}

	public void LoadFromServer()//this sequence will also apply the adjustments if the map was attacked
	{
		if(((Stats)stats).gameWasLoaded) //prevents loading twice, since there are no safeties and the procedure should be automated at startup, not button triggered
		{
			((Messenger)messenger).DisplayMessage("Only one load per session is allowed. Canceling...");
			return;
		}

		if(CheckServerSaveFile())		
		{
			StartCoroutine("DownloadMyMap");//force the local map save before this
			((Messenger)messenger).DisplayMessage("Downoading map from server...");
		}			
	}
	
	IEnumerator DownloadMyMap()   
	{
		((Messenger)messenger).DisplayMessage("Loading map from server...");
		ReadUserid();

		WWWForm form = new WWWForm();		
		form.AddField("savefile","file");

		WWW w2 = new WWW("http://www.citybuildingkit.com/finish_match.php?get_user_map=1&mapid=" + myMapIDCode);
		
		yield return w2;
		
		if(w2.error != null)
		{
			((Messenger)messenger).DisplayMessage("Server load failed.");
			print("Server load error" + w2.error);
		}
		
		else
		{
			//then if the retrieval was successful, validate its content to ensure the map file integrity is intact
			if(w2.text != null && w2.text != "")
			{
				if(w2.text.Contains("###StartofFile###") && w2.text.Contains("###EndofFile###"))
				{
					print ( "Your map ID " + myMapIDCode + " contents are: \n\n" + w2.text);	

					WriteMyMapFromServer(w2.text);

					((SaveLoadMap)saveLoadMap).LoadFromServer();
					((Messenger)messenger).DisplayMessage("Map downloaded and saved locally.");
				}
				
				else
				{			
					print ( "Your map file is Invalid. Contents are: \n\n" + w2.text);
					//although file is broken, prints the content of the retrieved file
					((Messenger)messenger).DisplayMessage("Server dowload incomplete. Try again.");
				}
			}
			else
			{
				print ( "Your map file is empty");
				((Messenger)messenger).DisplayMessage("Server dowload failed. Empty map.");
			}
		}
	}

	private void WriteMyMapFromServer(string text)//saves a copy of the server map locally
	{
		StreamWriter sWriter = new StreamWriter (filePath + fileNameServer + fileExt);
		sWriter.Write(text);
		sWriter.Flush ();
		sWriter.Close ();
		StartCoroutine("DownloadMyMapAttack");
		((Messenger)messenger).DisplayMessage("Downloading attack results...");
	}

	IEnumerator DownloadMyMapAttack()   
	{		
		WWWForm form = new WWWForm();		
		form.AddField("savefile","file");
		
		WWW w2 = new WWW("http://www.citybuildingkit.com/finish_match.php?get_user_map=1&mapid=" + myMapIDCode + attackExt);
		
		yield return w2;
		
		if(w2.error != null)
		{
			((Messenger)messenger).DisplayMessage("Attack file download failed.");
			print("Server load error" + w2.error);
		}
		
		else
		{
			//then if the retrieval was successful, validate its content to ensure the map file integrity is intact
			if(w2.text != null && w2.text != "")
			{
				if(w2.text.Contains("###StartofFile###") && w2.text.Contains("###EndofFile###"))
				{
					print ( "Your map ID "+ myMapIDCode + attackExt +" contents are: \n\n" + w2.text);	
					
					WriteMyMapAttackFromServer(w2.text);
					
					((SaveLoadMap)saveLoadMap).LoadAttackFromServer();
					print ("loading attack");
					StartCoroutine("EraseAttackFromServer");
					((Messenger)messenger).DisplayMessage("Attack results downloaded.");
				}
				
				else
				{			
					print ( "Your map file is invalid. Contents are: \n\n" + w2.text);
					//although file is broken, prints the content of the retrieved file
					((Messenger)messenger).DisplayMessage("Attack results corrupted. Download failed.");
				}
			}
			else
			{
				print ( "Attack file is empty");
				((Messenger)messenger).DisplayMessage("Your town was not attacked.");

			}
		}
	}

	IEnumerator EraseAttackFromServer()   
	{
		((Messenger)messenger).DisplayMessage("Erasing attack on server...");
		//byte[] levelData = System.IO.File.ReadAllBytes(filePath + battleResultSaveFile + fileExt);//full local save file
		byte[] levelData = Encoding.ASCII.GetBytes("###StartofFile###\n" 
		                                           + "0" + "," 
		                                           + "0" + "," 
		                                           + "0"+ "," 
		                                           + "0" + 
		                                           "\n###EndofFile###");
		
		WWWForm form = new WWWForm();

		form.AddField("savefile","file");
		
		form.AddBinaryData( "savefile", levelData, myMapIDCode + attackExt,"text/xml");//file
				
		//change the url to the url of the php file
		WWW w = new WWW("http://www.citybuildingkit.com/get_match.php?mapid=" + myMapIDCode + attackExt, form);//myUseridFile 

		yield return w;
	
		if (w.error != null)
		{
			((Messenger)messenger).DisplayMessage("Attack erase failed.");
			print ( w.error );    
		}
		else
		{
			//this part validates the upload, by waiting 5 seconds then trying to retrieve it from the web
			if(w.uploadProgress == 1 && w.isDone)
			{				
				yield return new WaitForSeconds(5);
				//change the url to the url of the folder you want it the levels to be stored, the one you specified in the php file
				WWW w2 = new WWW("http://www.citybuildingkit.com/get_match.php?get_user_map=1&mapid=" + myMapIDCode + attackExt);//returns a specific map
				
				yield return w2;
				if(w2.error != null)
				{
					((Messenger)messenger).DisplayMessage("Attack erase check failed.");
					print ( w2.error );  
				}
				else
				{
					//then if the retrieval was successful, validate its content to ensure the map file integrity is intact
					if(w2.text != null && w2.text != "")
					{
						if(w2.text.Contains("###StartofFile###") && w2.text.Contains("###EndofFile###"))
						{
							//and finally announce that everything went well
							print ( "Received file " + myMapIDCode + attackExt + " contents are: \n\n" + w2.text);

							((Messenger)messenger).DisplayMessage("Attack file from server\nhas been loaded and reset.");
						}
						else
						{
							print ( "Map file " + myMapIDCode + attackExt + " is invalid");//file
							print ( "Response file " + myMapIDCode + attackExt + " contents are: \n\n" + w2.text);//file although incorrect, prints the content of the retrieved file
							((Messenger)messenger).DisplayMessage("Attack file reset on server\nhas failed.");
						}
					}
					else
					{
						print ( "Map File " + myMapIDCode + attackExt + " is Empty");//file
						((Messenger)messenger).DisplayMessage("Attack file may be empty.");
					}
				}
			}     
		}
	}

	private void WriteMyMapAttackFromServer(string text)//saves a copy of the server map locally
	{
		StreamWriter sWriter = new StreamWriter (filePath + fileNameAttack + fileExt);
		sWriter.Write(text);
		sWriter.Flush ();
		sWriter.Close ();
	}

	public void LoadRandomFromServer()
	{
		StartCoroutine("DownloadRandomMap");//force the local map save before this
		((Messenger)messenger).DisplayMessage("Downloading random map...");
	}

	IEnumerator DownloadRandomMap()   
	{
		ReadUserid();

		WWW w2 = new WWW("http://www.citybuildingkit.com/get_match.php?get_random_map=1&mapid=" + myMapIDCode); //ADAN FIXED 5/22 because you need to include the user's mapid with the get_random_map to prevent the user's map from being downloaded by accident
			
		yield return w2;

		if(w2.error != null)
		{
			((Messenger)messenger).DisplayMessage("Random map download failed.");
			print("Server load error" + w2.error);
		}

		else
		{
			//then if the retrieval was successful, validate its content to ensure the map file integrity is intact
			if(w2.text != null && w2.text != "")
			{
				if(w2.text.Contains("###StartofFile###") && w2.text.Contains("###EndofFile###"))
				{
					print ( "Random file contents are: \n\n" + w2.text);
					((Messenger)messenger).DisplayMessage("Random map downloaded successfully");
				}

				else
				{			
					print ( "Random map file is invalid. Contents are: \n\n" + w2.text);
					//although incorrect, prints the content of the retrieved file
					((Messenger)messenger).DisplayMessage("Random map download failed/incomplete.");
				}
			}
			else
			{
				print ( "Random map file is empty");
				((Messenger)messenger).DisplayMessage("Downloaded random map is empty.");
			}
		}
	}

	/*
	void OnGUI()//to test any function separately, skipping the normal command flow
	{
		if(GUILayout.Button("Click me!"))
		{
			SaveToServer();
		}
	}
	*/
}
