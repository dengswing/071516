using UnityEngine;
using System.Collections;
using System;

public class Messenger : MonoBehaviour {		//displays the status messages on the left side of the screen

	public UILabel UserMessages;
	private string userMessagesTxt;
	
	private bool displayMessage;
	private float displayTimer = 0, displayTime = 0, idleTimer = 0, idleTime = 8;

	private Component soundFX;

	void Start () {
		soundFX = GameObject.Find ("SoundFX").GetComponent<SoundFX> ();
	}

	void Update () {
		if(displayMessage)
		{
			displayTimer += Time.deltaTime;
			idleTimer += Time.deltaTime;

			if(displayTimer>displayTime||idleTimer>idleTime)		//idleTimer - we have a large stack of identical messages
			{														//no point waiting 20 seconds
				displayTimer = 0;
				idleTimer = 0;
				ResetUserMessage();
			}
		}
	}

	public void DisplayMessage(string text)
	{
		((SoundFX)soundFX).Move ();

		if(displayTime>20)//in case messages keep coming, start scrolling down
		{
			string[] lines = userMessagesTxt.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);//to preserve the last input //{"\r\n", "\n"}

			userMessagesTxt= "";
			displayTime = 0;
			displayTimer = 0;

			for (int i = 1; i < lines.Length-1; i++) //start from 1 - discard oldest message; also discard last empty line
			{
				displayTime+=2;
				userMessagesTxt += lines[i] + "\n" ;
			}
		
			userMessagesTxt += text + "\n";
		}
		else
		{
			userMessagesTxt += text + "\n";
			displayTime += 2; 
		}

		idleTimer = 0;
		displayMessage = true;
		UserMessages.text = userMessagesTxt;
	}
	
	private void ResetUserMessage()
	{
		((SoundFX)soundFX).End ();			//erase curent message queue
		userMessagesTxt = "";
		displayMessage = false;
		displayTime = 0; 
		UserMessages.text = userMessagesTxt;
	}
}
