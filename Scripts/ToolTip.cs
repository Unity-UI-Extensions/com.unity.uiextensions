/// Credit drHogan 
/// Sourced from - http://forum.unity3d.com/threads/screenspace-camera-tooltip-controller-sweat-and-tears.293991/#post-1938929

//ToolTip is written by Emiliano Pastorelli, H&R Tallinn (Estonia), http://www.hammerandravens.com
//Copyright (c) 2015 Emiliano Pastorelli, H&R - Hammer&Ravens, Tallinn, Estonia.
//All rights reserved.

//Redistribution and use in source and binary forms are permitted
//provided that the above copyright notice and this paragraph are
//duplicated in all such forms and that any documentation,
//advertising materials, and other materials related to such
//distribution and use acknowledge that the software was developed
//by H&R, Hammer&Ravens. The name of the
//H&R, Hammer&Ravens may not be used to endorse or promote products derived
//from this software without specific prior written permission.
//THIS SOFTWARE IS PROVIDED ``AS IS'' AND WITHOUT ANY EXPRESS OR
//IMPLIED WARRANTIES, INCLUDING, WITHOUT LIMITATION, THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour {

	//text of the tooltip
	public Text text;

	//if the tooltip is inside a UI element
	bool inside;
	
	bool xShifted = false;
	bool yShifted = false;

	int textLength;

	public float width;
	public float height;

	int screenWidth;
	int screenHeight;

	float canvasWidth;
	float canvasHeight;

	public float yShift;
	public float xShift;

	int canvasMode;

	RenderMode GUIMode;

	Camera GUICamera;

	// Use this for initialization
	public void Awake () {

		GUICamera = GameObject.Find("GUICamera").GetComponent<Camera>();

		text = this.gameObject.GetComponentInChildren<Text>();

		inside=false;

		//size of the screen
		screenWidth = Screen.width;
		screenHeight = Screen.height;

		xShift = 0f;
		yShift = -30f;

		xShifted=yShifted=false;

		GUIMode = this.transform.parent.GetComponent<Canvas>().renderMode;

		this.gameObject.SetActive(false);

	}

  //Call this function externally to set the text of the template and activate the tooltip
	public void SetTooltip(string ttext){

		if(GUIMode==RenderMode.ScreenSpaceCamera){
			//set the text and fit the tooltip panel to the text size
			text.text=ttext;
			
			this.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(text.preferredWidth+40f,text.preferredHeight+25f);
			
			OnScreenSpaceCamera();

		}
	}

  //call this function on mouse exit to deactivate the template
	public void HideTooltip(){
		if(GUIMode==RenderMode.ScreenSpaceCamera){
			this.gameObject.SetActive(false);
			inside=false;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if(inside){
			if(GUIMode==RenderMode.ScreenSpaceCamera){
				OnScreenSpaceCamera();
			}
		}
	}

  //main tooltip edge of screen guard and movement
	public void OnScreenSpaceCamera(){
		Vector3 newPos = GUICamera.ScreenToViewportPoint(Input.mousePosition-new Vector3(xShift,yShift,0f));

		width = this.transform.GetComponent<RectTransform>().sizeDelta[0];
		height = this.transform.GetComponent<RectTransform>().sizeDelta[1];

		// check and solve problems for the tooltip that goes out of the screen on the horizontal axis
		float val;

		Vector3 lowerLeft  = GUICamera.ViewportToWorldPoint(new Vector3(0.0f,0.0f,0.0f));
		Vector3 upperRight = GUICamera.ViewportToWorldPoint(new Vector3(1.0f,1.0f,0.0f));

		//check for right edge of screen
		val = (GUICamera.ViewportToWorldPoint(newPos).x+width/2);
		if(val>upperRight.x){
			Vector3 shifter = new Vector3(val-upperRight.x,0f,0f);
			Vector3 newWorldPos = new Vector3(GUICamera.ViewportToWorldPoint(newPos).x-shifter.x,newPos.y,0f);
			newPos.x = GUICamera.WorldToViewportPoint(newWorldPos).x;
		}
		//check for left edge of screen
		val = (GUICamera.ViewportToWorldPoint(newPos).x-width/2);
		if(val<lowerLeft.x){
			Vector3 shifter = new Vector3(lowerLeft.x-val,0f,0f);
			Vector3 newWorldPos = new Vector3(GUICamera.ViewportToWorldPoint(newPos).x+shifter.x,newPos.y,0f);
			newPos.x = GUICamera.WorldToViewportPoint(newWorldPos).x;
		}

		// check and solve problems for the tooltip that goes out of the screen on the vertical axis

		//check for upper edge of the screen
		val = (GUICamera.ViewportToWorldPoint(newPos).y+height/2);
		if(val>upperRight.y){
			Vector3 shifter = new Vector3(0f,35f+height/2,0f);
			Vector3 newWorldPos = new Vector3(newPos.x,GUICamera.ViewportToWorldPoint(newPos).y-shifter.y,0f);
			newPos.y = GUICamera.WorldToViewportPoint(newWorldPos).y;
		}

		//check for lower edge of the screen (if the shifts of the tooltip are kept as in this code, no need for this as the tooltip always appears above the mouse bu default)
		val = (GUICamera.ViewportToWorldPoint(newPos).y-height/2);
		if(val<lowerLeft.y){
			Vector3 shifter = new Vector3(0f,35f+height/2,0f);
			Vector3 newWorldPos = new Vector3(newPos.x,GUICamera.ViewportToWorldPoint(newPos).y+shifter.y,0f);
			newPos.y = GUICamera.WorldToViewportPoint(newWorldPos).y;
		}

		this.transform.position= new Vector3(GUICamera.ViewportToWorldPoint(newPos).x,GUICamera.ViewportToWorldPoint(newPos).y,0f);
		this.gameObject.SetActive(true);
		inside=true;
	}
}