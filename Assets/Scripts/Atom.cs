using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour{

	public static float levelSpacing = 1f;

	public float[] radii;
	public int numLevels;

	public float OuterRadius{
		get{
			return radii[0];
		}
		set{
			SetRadii(value);
		}
	}

	//temp
	public float radiusDebug;


	public void Awake(){
		Init(10, 1);
	}

	public void Init(float r, int l){
		numLevels = l;
		radii = new float[numLevels];
		SetRadii(r);
	}

	void Update(){
		OuterRadius = radiusDebug;
	}

    private void SetRadii(float outer){
    	for(int i = 0; i < numLevels; i++){
    		radii[i] = outer - levelSpacing*i;
    	}
    }
}
