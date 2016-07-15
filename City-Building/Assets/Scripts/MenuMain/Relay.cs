﻿using UnityEngine;
using System.Collections;

public class Relay : MonoBehaviour {//system-wide coordination for input and elements that are alternately activated 
	
	public GameObject MenuUnit, UnitProc;//alternates between the unit menu and the unit proc
	
	//Relayed variables
	public int currentTab = 0;
	public bool 
		pauseInput = false,	
		delay = false,
		pauseMovement = false;			//pause building movement while the destroy confirmation is on
	
	public void ActivateUnitMenu()
	{
		if(currentTab == 2)
		{
			MenuUnit.SetActive(true);
			((MenuUnit)MenuUnit.GetComponent("MenuUnit")).LoadValuesfromProc();		
		}		
	}
	
	public void ActivateUnitProc()
	{
		UnitProc.SetActive(true);	
		((MenuUnit)MenuUnit.GetComponent("MenuUnit")).PassValuestoProc();	
	}

	public void DelayInput()
	{
		delay = true;
		StopCoroutine("ResetDelayInput");
		StartCoroutine("ResetDelayInput");
	}

	IEnumerator ResetDelayInput()
	{
		yield return new WaitForSeconds(0.2f);
		delay = false;
	}
}
