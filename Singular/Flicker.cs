//******************************************************************************//
//																				//
//		Created by awestsecurity 												//
//		Version date 5/29/2016													//
//																				//
//		Flicker script developed for use with Unity Game Engine 5.3				//
//      Add to GameObject with a light component. 								//
//		Works with the combustible script automatically							//
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

public class Flicker : MonoBehaviour {

	[Range(0.01f,1.0f)]
	public float rate = 0.2f;
	[Range(0.0f,8.0f)]
	public float max = 3;
	[Range(0.0f,8.0f)]
	public float min = 1;
	
	public enum FlickerMethod {random, perlin};
	public FlickerMethod flickerMethod;
	
	private Light lightComponent;
	private float nlight;
	private int choice;
	private float count;
	
	private float perlinX = 0.0f;
	private float perlinY = 0.0f;

	private bool go;

	void Start () {
		lightComponent = gameObject.GetComponent<Light>();
		nlight = lightComponent.intensity ;
		count = 0;
	}
	
	void Update () {
		go = ( gameObject.GetComponent<Light>().enabled ) ? true : false ;
		if ( go && flickerMethod == FlickerMethod.random ) {
//			if ( lightComponent.intensity == 0 )  StartCoroutine(_Globals.instance.FadeLight(lightComponent, 2f, min));
			count += Time.deltaTime;
			if ( count >= 1.0f-rate ) {
				choice = Random.Range(1,8);
					switch(choice) {
						case 1: //
							nlight = nlight/1.1f;
							break;
						case 2: //
							nlight = nlight*1.1f;
							break;
						case 3: //
							break;
						case 4: //
							nlight = nlight + 0.1f;
							break;
						case 5: //
							nlight = nlight - 0.1f;
							break;
						case 6: //
							nlight = nlight + 0.01f;
							break;
						case 7: //
							nlight = nlight - 0.01f;
							break;
						default:
							break;
					}
				nlight = Mathf.Clamp(nlight, min, max);
				lightComponent.intensity = nlight /* + (_Gui.lightLevel/5f) */ ;
			count -= 1.0f-rate;
			}
		} else if ( go && flickerMethod == FlickerMethod.perlin ) {
			float n = Random.Range(-1,1);
			perlinX += (n/10)*rate;
			n = Random.Range(-1,1);
			perlinY += (n/10)*rate;
			float i = Mathf.Lerp(min,max,Mathf.PerlinNoise(perlinX,perlinY));
			lightComponent.intensity = i;
		}
	}
	
}
