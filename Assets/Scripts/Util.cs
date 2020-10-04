using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util 
{
    public static float VectorAngle(Vector2 v){
 		if(v.x > 0){
 			if(v.y > 0){
 				return Mathf.Atan(v.y/v.x);
			}else{
				return Mathf.Atan(v.y/v.x) + 2*Mathf.PI;
			}
 		}else if(v.x < 0){
 				return Mathf.Atan(v.y/v.x) + Mathf.PI;
		}else{
			return (v.y > 0) ? (Mathf.PI/2) : (3*Mathf.PI/2);
		}
 		
 	}

 	public static float RadianWrap(float angle){
 		if(angle < 0){
 			return angle + 2*Mathf.PI;
 		}else if(angle > 2*Mathf.PI){
 			return angle - 2*Mathf.PI;
 		} 

 		return angle;
 	}
}
