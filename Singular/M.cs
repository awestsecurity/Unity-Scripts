//******************************************************************************//
//																				//
//		Created by awestsecurity 												//
//		Version date 6/5/2016													//
//																				//
//		Handy mini-library of useful formulas									//
//      Can be used like MathF													//
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

public static class M {

	// Returns the float equidistant from two floats.
	public static float Average (float a, float b) {
		return a + ( ( b - a ) / 2.0f );
	}
	
	// Returns the slope of a line for which you have two points.
	// Step one in y=mx+b.
	public static float Slope (float x1, float y1, float x2, float y2) {
		return ( ( y2 - y1 ) / ( x2 - x1 ) );
	}
	
	// Returns the y-intercept of a line for which you have two points and the slope.
	// Step two in y=mx+b.
	public static float YIntercept (float x, float y, float slope) {
		return y - (slope * x);
	}
	
	// Returns the point at which two lines intersect.
	public static float[] IntersectPoint (float slope1, float yInter1, float slope2, float yInter2) {
		float[] point = new float[] {0.0f,0.0f};
		point[0] = ( yInter2 - yInter1 ) / ( slope1 - slope2 );
		point[1] = ( slope1 * point[0] ) + yInter1;
		return point;
	}
	
	// Returns the conversion of a number from one number range into another number range.
	//
	public static int Map ( int originMax, int outMax, int input ) {			// assumes both minimums are zero.
		return ( outMax / originMax ) * input;
	}
	
	public static int Map ( int originMin, int originMax, int outMin, int outMax, int input ) {
		return outMin + ((outMax - outMin) / (originMax - originMin)) * (input - originMin);
	}
	
	public static float Map ( float originMax, float outMax, float input ) {	// assumes both minimums are zero.
		return ( outMax / originMax ) * input;
	}
	
	public static float Map ( float originMin, float originMax, float outMin, float outMax, float input ) {
		return outMin + ((outMax - outMin) / (originMax - originMin)) * (input - originMin);
	}
}
