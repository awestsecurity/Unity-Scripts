//******************************************************************************//
//																				//
//		Created by awestsecurity 												//
//		Version date 5/29/2016													//
//																				//
//		Day Night script developed for use with Unity Game Engine 5.3			//
//      Add to Empty GameObject and call it a day. 								//
//																				//
//		Plans: Making this work with SkyboxBlended by Aras Pranckevicius  		//
//		  	   Add a moon control												//
//																				//
//		The MIT License (MIT)													//
//      Copyright (c) 2016 Allen West											//
//																				//
//		Permission is hereby granted, free of charge, to any person 			//
//		obtaining a copy of this software and associated documentation 			//
//		files (the "Software"), to deal in the Software without 				//
//		restriction, including without limitation the rights to use, 			//
//		copy, modify, merge, publish, distribute, sublicense, and/or 			//
//		sell copies of the Software, and to permit persons to whom the 			//
//		Software is furnished to do so, subject to the following conditions:	//
//																				//
//		The above copyright notice and this permission notice 					//
//		shall be included in all copies or substantial portions 				//
//		of the Software.														//
//																				//
//		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 		//
//		EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 		//
//		OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 				//
//		NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 			//
//		HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 			//
//		WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 			//
//		FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR 			//
//		OTHER DEALINGS IN THE SOFTWARE.											//
//																				//
//******************************************************************************//

using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class DNTimeProperties
{
	[Range(1,60)]
	public int dayLengthInMinutes = 20;
	[Range(1,24)]
	public int startTime = 12;

	public float accumulatedTime { get; private set; }
	public int currentTimeInSeconds { get { return (int)(accumulatedTime*(day/dayLengthInSeconds)); } 	}
	public int dayLengthInSeconds {	get { return dayLengthInMinutes * 60; }	}
	public float currentHour { get { return (float)currentTimeInSeconds / hour % 24f; } }
	public int currentDay { get { return 1+(currentTimeInSeconds / hour / 24); } }
	
	private const int second = 1;
	private const int minute = 60 * second;
	private const int hour = 60 * minute;
	private const int day = 24 * hour;
	private const float degreesRotation = 360/day;

	
	public override string ToString() {
		string output = "";
		int minutes = currentTimeInSeconds / minute % 60;
		output = "Day:"+currentDay+" - Time is "+(int)currentHour+":"+minutes+" - Current Time in Seconds: "+currentTimeInSeconds;
		return output;
	}
	
	public void AddTime(float additionalTime = 0) {
		if (additionalTime != 0) accumulatedTime += additionalTime;
		else accumulatedTime += Time.deltaTime;
	}
	
}

[Serializable]
public class DNRenderProperties
{
	public Transform sun;
	[Range(0.1f,0.9f)]
	public float sunOffset = 0.9f;
	[Range(5,20)]
	public float fadeSpeed = 15;
	public Color ambientDay = Color.blue;
	public Color ambientSunset = Color.yellow;
	public Color ambientNight = Color.black;
	public Color ambientSunrise = Color.yellow;
	
	public Color prevColor {get; private set;}
	public Color targetColor {get; private set;}
	private string targetColorName = "Not yet set";
	private float transitionPercent = 0.0f;
	
	public void SetTargetAmbientColor(int hourOfDay){
		if(hourOfDay>20)  {
			prevColor = ambientSunset;
			targetColor = ambientNight;
			targetColorName = "Night";
		}
		else if(hourOfDay>17) {
			prevColor = ambientDay;
			targetColor = ambientSunset;
			targetColorName = "Sunset";
		}
		else if(hourOfDay>10) { 
			prevColor = ambientSunrise;
			targetColor = ambientDay;
			targetColorName = "Day";
		}
		else if(hourOfDay>6) { 
			prevColor = ambientNight;
			targetColor = ambientSunrise;
			targetColorName = "Sunrise";
		}
   }
   
   public IEnumerator FadeAmbientLight(float increment) {
		transitionPercent = 0;
		Vector3 start = new Vector3(prevColor.r,prevColor.g,prevColor.b);
		Vector3 end = new Vector3(targetColor.r,targetColor.g,targetColor.b);
		
		do {
		transitionPercent += (Time.deltaTime/increment);
		Vector3 between = Vector3.Lerp(start,end,transitionPercent);
		RenderSettings.ambientLight = new Color(between.x,between.y,between.z);
		yield return new WaitForEndOfFrame();
		} while (RenderSettings.ambientLight != targetColor);
   }
   
   	public override string ToString() {
		string output = "Target color: "+targetColorName;
			   output += " - Percent complete: "+transitionPercent;
		return output;
   }
}

[Serializable]
public class DNTideProperties 		//Not a MonoBehaviour!
{
	public bool tideOn = false;
   public Transform waterPlane;
   public float maxHeight = 1.1f;
   public float minHeight = 0.9f;
   [Range(0,12)]
   public int highTideHour = 8;
   [Range(1,12)]
   public int cyclesPerDay = 2; 

	private float x;
	private float z;
	private bool raise;
   
	public void init () {
		if (waterPlane != null) {
			x = waterPlane.position.x;
			z = waterPlane.position.z;
		} else { 
			Debug.LogError ("You must assign a water object to use tides.");
			tideOn = false;
		}
	}
   
	//first get current time in floating point hours and compensate for high tide setting.		0-24f % cycleLength = 0-12
	//We'll then need to know how many hours each full cycle is.								12		( on earth )
	//Next Shift the values so zero is in the middle											0-12 - 6 (cycle length)
	//Next convert the range from -1 to 1 and take the absolute value							-6 - 6 / 6  = -1 - 1
	public void SetTideHeight (float t) {
		float cycleLength = 24 / cyclesPerDay;
		float pos = (Mathf.Abs(t-highTideHour)%24) % cycleLength; 
		pos = (pos - (cycleLength/2)) / (cycleLength/2);
		raise = (pos < 0) ? false : true;
		pos = Mathf.Abs(pos);
		float y = Mathf.Lerp (minHeight, maxHeight, pos);
		waterPlane.position = new Vector3 (x,y,z);
	}
	
	public override string ToString() {
		string output = "";
		if (raise) 	output = "Tide is raising. ";
		else		output = "Tide is falling. ";
					output += "Current height is" + waterPlane.position.y;
		return output;
   }
}

public class _DayNight : MonoBehaviour {

	private static _DayNight _instance;
		
		//This is the public reference that other classes will use
		public static _DayNight instance {
			get {
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<_DayNight>();
				return _instance;
			}
		}

   public DNTimeProperties 	timeProperties;
   public DNRenderProperties 	renderProperties;
   public DNTideProperties 	tideProperties;
   
   public bool debug = false;

   private float degreeRotation;
   
   private int currentHour;
   private Color currentTargetColor;

   void Start() {
		renderProperties.SetTargetAmbientColor((int)timeProperties.currentHour);
		RenderSettings.ambientLight = renderProperties.targetColor;
		RenderSettings.reflectionIntensity = 0.3f;
   		degreeRotation = 360 / (float)timeProperties.dayLengthInSeconds;

		//initialize the sun if it doesn't exist or can't be found
		if (renderProperties.sun == null) {
			if (debug) Debug.Log("No sun assigned. Searching for directional light.");
			Light[] lights = GameObject.FindObjectsOfType<Light>();
			foreach (Light light in lights) {
				if (light.type == LightType.Directional) {
					renderProperties.sun = light.gameObject.transform;
					renderProperties.sun.parent = this.gameObject.transform;
					if (debug) Debug.Log("A sun has been found.");
					break;
				}
			}
			if (renderProperties.sun == null) {
				if (debug) Debug.Log("No sun found. Creating directional light.");
				renderProperties.sun = new GameObject("Sun").transform;
				renderProperties.sun.gameObject.AddComponent<Light>();
				Light l = renderProperties.sun.gameObject.GetComponent<Light>();
				l.type = LightType.Directional;
				l.color = Color.yellow;
				renderProperties.sun.parent = this.transform;
			}
		}
		
		//Setup the starting time
		float angleOfSun = timeProperties.startTime*(360/24);
		angleOfSun = (angleOfSun + 270) % 360;
		renderProperties.sun.transform.localEulerAngles = new Vector3 (angleOfSun*renderProperties.sunOffset,angleOfSun*(1-renderProperties.sunOffset),0);
		float t = (timeProperties.startTime*60*60) / (86400 / timeProperties.dayLengthInSeconds);
		timeProperties.AddTime(t);
		
		//initialize tides
		if (tideProperties.tideOn) tideProperties.init();
		
		//initialize debugging
		if (debug) StartCoroutine(LogDebug());
   }
   
   void Update(){
		
		// Keep time moving.
		timeProperties.AddTime();

		// Run changes in ambient color.
		int theHour = (int)timeProperties.currentHour; //local variable for shorthand
		renderProperties.sun.Rotate((new Vector3(degreeRotation*renderProperties.sunOffset, degreeRotation*(1-renderProperties.sunOffset), 0) * Time.deltaTime));
		if (currentHour != theHour) {
			currentHour = theHour;
			renderProperties.SetTargetAmbientColor(theHour);

		}
		if (currentTargetColor != renderProperties.targetColor) {
			currentTargetColor = renderProperties.targetColor;
			StartCoroutine(renderProperties.FadeAmbientLight(timeProperties.dayLengthInSeconds/renderProperties.fadeSpeed));
		}
		
		// If there is a water object, set the tide.
		if (tideProperties.tideOn) {
			tideProperties.SetTideHeight (timeProperties.currentHour);
		}
   }
   
   IEnumerator LogDebug () {
		while(debug) {
			yield return new WaitForSeconds(5);
			Debug.Log(timeProperties);
			if (tideProperties.tideOn) Debug.Log(tideProperties);
			Debug.Log(renderProperties);
		}
   }
   
}

