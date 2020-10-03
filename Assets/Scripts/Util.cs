using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util 
{
    //Returns radian angle of a vector relative to Vector2.right
 	public static float VectorAngle(Vector2 v){
 		int q;
 		if(v.x > 0){
 			if(v.y >= 0){
 				q = 0;
 			}else{
 				q = 3;
 			}
		}else{
			if(v.y > 0){
				q = 1;
			}else{
				q = 2;
			}
		}
 		
 		if(v.x != 0){
 			return Mathf.Atan(Mathf.Abs(v.y/v.x)) + q*(Mathf.PI/2);
		}else{
			return (v.y > 0) ? (Mathf.PI/2) : 3*(Mathf.PI/2);
		}
 		
 	}
}
