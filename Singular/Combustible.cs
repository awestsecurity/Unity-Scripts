//******************************************************************************//
//																				//
//		Created by awestsecurity 												//
//		Version date 5/29/2016													//
//																				//
//		Combustible script developed for use with Unity Game Engine 5.3			//
//      Add to object with colliders and set appropriate						//
//		lights and particle systems.											//
//		  																		//
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
using System.Collections.Generic;
using System;

[Serializable]
public class CFuelProperties {

	public bool lit = false;						// Is the object already on fire?
/*	public Combustable[] adjacentFires;   */ 		// Rather than auto lighting other fires it might be better to use multiple particles and colliders
	[Range(-1,3600)]								// -1 is infinit, 3600 is one hour
	public float burnTime = -1;						// How long will the fire burn for (max) in seconds
	public float fuel { get; private set; }			// Remaining burn time in seconds
	public bool waterProof = false;					// Will the flame ignore water objects?
	
	public bool infinit { get; private set; }		// Will the fire last forever?
	
	public void StartFire() {
		infinit = (burnTime < 1) ? true : false ; 
		fuel = burnTime;
		lit = true;
/*		if (adjacentfires.Length > 0) {				// This will light adjacent fires if that is needed
			for(int l = 0; l < adjacentfires.Length; l ++) {
				adjacentfires[l].StartFire();
			}
		}		*/
	}
	
	public void Burn(float timeBurnt) {
		fuel -= timeBurnt;
		if (fuel < 0) fuel = 0;
	}
	
	public void AddFuel(float f) {
		fuel = Mathf.Min(burnTime,fuel+f);
	}
	
	public void EndFire() {
		lit = false;
	}

}

[Serializable]
public class CLightProperties {

	public GameObject[] lightObjects;
	private Light[] lights;
	private Flicker[] flickers;
	private float[] lightMax;
	private float[] lightMin;
	
	public GameObject[] particleObjects;
	private ParticleSystem[] particleSystems;
	private float[] partSize;
	private float[] partRate;
	
	public void Init() {
	
		if ( !ValidateList (lightObjects, lightObjects.Length)) {
			Debug.LogError ("FATAL ERROR: There is a missing Light in your fire settings.");
		}
		if ( !ValidateList (particleObjects, particleObjects.Length)) {
			Debug.LogError ("FATAL ERROR: There is a missing Particle System in your fire settings.");
		}
		
		lights = new Light[lightObjects.Length];
		flickers = new Flicker[lightObjects.Length];
		lightMin = new float[lightObjects.Length];
		lightMax = new float[lightObjects.Length];
		for (int l = 0; l < lightObjects.Length; l ++) {
			lights[l] = lightObjects[l].GetComponent<Light>();
			if ( !lights[l] ) {
				lights[l] = lightObjects[l].AddComponent<Light>();
				Debug.LogError("ERROR: Light "+l+" is missing a light component. Fix This! A temporary light has been added.", lightObjects[l]);
			}
			flickers[l] = lightObjects[l].GetComponent<Flicker>();
			if ( flickers[l] ){
				lightMax[l] = flickers[l].max;
				lightMin[l] = flickers[l].min;
			} else {
				lightMax[l] = lights[l].intensity;
				lightMin[l] = lights[l].intensity;
			}
		}
		particleSystems = new ParticleSystem[particleObjects.Length];
		partSize = new float[particleObjects.Length];
		partRate = new float[particleObjects.Length];
		for(int l = 0; l < particleObjects.Length; l ++) { //Initialize Particle system behavior
			particleSystems[l] = particleObjects[l].GetComponent<ParticleSystem>();
			if (!particleSystems[l]) { 
				Debug.LogError ("FATAL ERROR: Missing Particle System.", particleObjects[l]);
				break;
			}
			partSize[l] = particleSystems[l].startSize;
			var em = particleSystems[l].emission;
			partRate[l] = em.rate.constantMax;
		}
	}
	
	public void StartFire(){
		for(int l = 0; l < lightObjects.Length; l ++) {
			lights[l].enabled = true;
			if (flickers[l]) {
				flickers[l].max = lightMax[l];
				flickers[l].min = lightMin[l];
			} else {
				lights[l].intensity = lightMax[l];
			}
		}
		
		if (particleSystems.Length > 0) {
			for(int l = 0; l < particleSystems.Length; l ++){
			particleSystems[l].startSize = partSize[l];
				var em = particleSystems[l].emission;
				var rate = em.rate;
				rate.mode = ParticleSystemCurveMode.Constant;
				rate.constantMax = partRate[l];
				rate.constantMin = partRate[l];
				em.rate = rate;
			particleSystems[l].Play();
			}
		}
	}
	
	public void Dim(float percent) {

		for(int p = 0; p < particleSystems.Length; p ++) {
			float size = Mathf.Lerp(0,partSize[p],percent);
			float emission = Mathf.Lerp(0,partRate[p],percent);
			particleSystems[p].startSize = size;
			var em = particleSystems[p].emission;
			var rate = em.rate;
			rate.constantMax = emission+3;
			rate.constantMin = emission+3;
			em.rate = rate;
		}
		for(int l = 0; l < lightObjects.Length; l ++) {
			float max = Mathf.Lerp(0,lightMax[l],percent);
			float min = Mathf.Lerp(0,lightMin[l],percent);
			lights[l].enabled = true;
			if (flickers[l]) {
				flickers[l].max = max;
				flickers[l].min = min;
			} else {
				lights[l].intensity = max;
			}
		}
	}
	
	public void EndFire(){
		for(int l = 0; l < lights.Length; l ++) {
			lights[l].enabled = false;
		}
		foreach (ParticleSystem particle in particleSystems) {
			particle.Stop();
		}
	}
	
	private bool ValidateList<T> (IList<T> array, int length) {
		bool ok = true;
		for ( int l = 0; l < length; l ++ ) {
			if ( array[l] == null ) {
				ok = false;
			}
		}
		return ok;
	}
	
}

public class Combustible : MonoBehaviour {
	
	public CFuelProperties fProperties;
	public CLightProperties lProperties;
	
	public bool debug = false;

	/// <summary>Once at the scene start it will populate the private variables and start/extinguish the fire as set in the inspector.
    /// </summary>
	void Start () {
		lProperties.Init();

		if(!fProperties.lit) {
			GoOut();
		} else {
			StartFire();
		}
	}
	
	/// <summary>Every frame this will diminish lights and particle systems, subtract fuel, and check to see if the fuel has run out.
    /// </summary>
	void Update () {
		if ( !fProperties.infinit && (fProperties.burnTime > 0) && fProperties.lit ) {
			lProperties.Dim( fProperties.fuel / fProperties.burnTime );
		}
		if ( fProperties.lit && fProperties.burnTime > 0 ) {
			fProperties.Burn(Time.deltaTime);
		}
		if ( !fProperties.infinit && fProperties.lit && fProperties.fuel <= 0 ) { 
			if (debug) Debug.Log ("Stopping particle systems and lights. The fire has gone out.", this.gameObject);
			GoOut();
		}

	}
	
	/// <summary>This checks for collisions with another fire object or water. 
    /// <para>It will light itself and other combustibles or extinguish itself. This object must have a collider set to trigger.</para>
	/// <param name="other">Auto populated by Unity. It's the reference to the collider of what hit this object.</param>
    /// </summary>
	private void OnTriggerEnter(Collider other) {
		Combustible SourceFire = other.gameObject.GetComponent<Combustible>();
		string debugString = "Trigger enter.";
    	if(SourceFire){
			debugString += " Object is also combustible. ";
			if(SourceFire.fProperties.lit == true) {
    			StartFire();
				debugString += " It is on fire.";
    		}
		}
		if( !fProperties.waterProof && (other.name == "Water" || other.tag == "Water")) {
    		GoOut();
			debugString += " It has extinguished the fire.";
    	}
		if (debug) Debug.Log(debugString);
	}

	/// <summary>Turns on lights, particle systems, and sounds if attached 
    /// <para>Lights and particle systems are assigned in the inspector. The audio source must be attached to this game object and should be set to loop.</para>
    /// </summary>
	public void StartFire() {
	
		lProperties.StartFire();
		fProperties.StartFire();
		
		if (gameObject.GetComponent<AudioSource>()){
			gameObject.GetComponent<AudioSource>().Play();
		}
		
	}

	/// <summary>Turns off lights, particle systems, and sounds if attached 
    /// <para>Lights and particle systems are assigned in the inspector. The audio source must be attached to this game object and should be set to loop.</para>
    /// </summary>
	private void GoOut() {
		lProperties.EndFire();
		fProperties.EndFire();
		
		if (gameObject.GetComponent<AudioSource>()){
			gameObject.GetComponent<AudioSource>().Stop();
		}

	}
	   
}
