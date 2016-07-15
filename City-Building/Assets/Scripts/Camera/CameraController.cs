using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CameraController : MonoBehaviour {

	public bool 
		MoveNb, 					//bools for button controlled moving Move North, East, South, West
		MoveEb, 
		MoveSb, 
		MoveWb, 
		ZoomInb, 
		ZoomOutb,	
		movingBuilding = false,
		stopZoom = false;

	private float 
		touchPanSpeed = 5.0f,				//3.0f	
		zoomMax = 1,						//caps for zoom
		zoomMin = 0.25f,
		zoom, 								// the current camera zoom factor
		currentFingerDistance,
		previousFingerDistance,
		camStep = 10;

	public int 								//camera bounds
		northMax = 4200,		//4200
		southMin = -4300,  		//-4300
		eastMax = 5200,			//5200
		westMin = -5200;		//-5200
		
	private Component relay;

	void Start () 
	{
		relay = GameObject.Find ("Relay").GetComponent<Relay>();
	}

	//to prevent selecting the buildint underneath the move buttons

	private void Delay() { ((Relay)relay).DelayInput(); }

	//Move
	public void MoveN()	{ MoveSb = false; MoveNb = !MoveNb; Delay (); }	
	public void MoveE()	{ MoveWb = false; MoveEb = !MoveEb; Delay (); }	
	public void MoveS()	{ MoveNb = false; MoveSb = !MoveSb; Delay (); }	
	public void MoveW()	{ MoveEb = false; MoveWb = !MoveWb; Delay (); }

	//Zoom
	public void ZoomIn() { ZoomOutb = false; ZoomInb = !ZoomInb; Delay (); }	
	public void ZoomOut() { ZoomInb = false; ZoomOutb = !ZoomOutb; Delay (); }
	
	// conditions keep the camera from going off-map, leaving a margin for zoom-in/out

	private void MoveCam(float speedX,float speedY){transform.position += new Vector3 (speedX, speedY, 0);}
		
	void Update()
	{	
		TouchMoveZoom ();
		ButtonMoveZoom ();
	}

	private void ButtonMoveZoom()
	{
		//NSEW distance bounds

		if(MoveNb && transform.position.y < northMax){MoveCam(0,camStep);} 
		else if(MoveSb && transform.position.y > southMin){MoveCam(0,-camStep);}
		else{MoveNb = false; MoveSb = false;}
		
		if (MoveEb && transform.position.x < eastMax){MoveCam(camStep,0);}
		else if (MoveWb && transform.position.x > westMin){MoveCam(-camStep,0);}
		else{MoveEb = false; MoveWb = false;}

		zoom = ((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor;		

		//zoom in/out

		if(ZoomInb && zoom<zoomMax){((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor += 0.005f;}
		else if(ZoomOutb && zoom>zoomMin)
		{
			((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor -= 0.005f;		
		}
		else {StopZoom();} 
	}

	private void TouchMoveZoom()
	{
			
		zoom = ((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor;
		
		if (Input.touchCount > 1 && Input.GetTouch(0).phase == TouchPhase.Moved//chech for 2 fingers on screen
				&& Input.GetTouch(1).phase == TouchPhase.Moved) 
		{
			if(!((Relay)relay).delay) Delay ();
			Vector2 touchPosition0 = Input.GetTouch(0).position;//positions for both fingers for pinch zoom in/out
			Vector2 touchPosition1 = Input.GetTouch(1).position;
				
			currentFingerDistance = Vector2.Distance(touchPosition0,touchPosition1);//distance between fingers
				
			//AUTO ZOOM - stopped with tap and brief hold
			/*
			if (currentFingerDistance>previousFingerDistance && zoom<zoomMax)
			{
				if(!ZoomInb)
					{
						ZoomOutb = false;
						ZoomInb = true;
					}
			}
			else if(zoom>zoomMin)
			{
				if(!ZoomOutb)
				{
					ZoomInb = false;
					ZoomOutb = true;			 
				}
			}
			*/
			//MANUAL ZOOM

			if (currentFingerDistance>previousFingerDistance && !stopZoom)
			{
				if(zoom<zoomMax)
				((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor += 0.05f;//0.02f;zoomin
				else 
				stopZoom = true;
			}

			else if(currentFingerDistance<previousFingerDistance && zoom>zoomMin)
			{
				stopZoom = false;
				((tk2dCamera)this.GetComponent("tk2dCamera")).ZoomFactor -= 0.05f;//0.02f;/zoomout
			}
			 

			previousFingerDistance = currentFingerDistance;
		
			}
			
			//else, if one finger on screen - scroll
		else if (!movingBuilding && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) //drag
		{
			if(!((Relay)relay).delay) Delay ();//prevents selecting the buildings underneath 
            Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
            
			//MANUAL MOVE

			float scrollCorrection = 1/ Math.Abs(zoom);

			if(touchDeltaPosition.x < 0)
			{
				if( transform.position.x < 4500)
				{
					transform.Translate(-touchDeltaPosition.x*touchPanSpeed*scrollCorrection, 0, 0);	
				}
			}
			else if( transform.position.x > -4500)
				{
				transform.Translate(-touchDeltaPosition.x*touchPanSpeed*scrollCorrection, 0, 0);	
				}
			
			
			if(touchDeltaPosition.y < 0)
			{
				if( transform.position.y < 3500)
				{
				transform.Translate(0, -touchDeltaPosition.y*touchPanSpeed*scrollCorrection, 0);	
				}
			}
			
			else if( transform.position.y > -3500)
			{
				transform.Translate(0, -touchDeltaPosition.y*touchPanSpeed*scrollCorrection, 0);	
			}
		}
		else if (Input.touchCount ==1 && Input.GetTouch(0).phase == TouchPhase.Stationary)
		{
			StopAll ();
		}
	}

	private void StopAll()
	{
		MoveNb=false; MoveSb=false; MoveEb=false; MoveWb=false;
		ZoomInb = false; ZoomOutb = false;	
	}
	private void StopMove()
	{
		MoveNb=false; MoveSb=false; MoveEb=false; MoveWb=false;
	}
	private void StopZoom()
	{
		ZoomInb = false; ZoomOutb = false;	
	}
}

