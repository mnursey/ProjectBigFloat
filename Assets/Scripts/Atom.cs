using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Atom : MonoBehaviour{

	public static float levelSpacing = 1.5f;

	public float[] radii;
	public int numLevels;
	public AtomVisualController[] visualizers;

    public GameObject centreVisual;

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

	//public void Awake(){
	//	Init(10, 1);
	//}

	public void Init(float r, int l){
		numLevels = l;
		radii = new float[numLevels];
		visualizers = new AtomVisualController[numLevels];

		//temp
		for(int i = 0; i < numLevels; i++){
			visualizers[i] = Instantiate(AssetDatabase.LoadAssetAtPath<AtomVisualController>("Assets/Prefabs/AtomVisual.prefab"), transform);
		}

		radiusDebug = r;
		SetRadii(r);

        if(centreVisual != null)
        {
            centreVisual.transform.localScale *= OuterRadius;
        }
	}

	void Update(){
		OuterRadius = radiusDebug;
	}

    private void SetRadii(float outer){
    	for(int i = 0; i < numLevels; i++){
    		radii[i] = outer - levelSpacing*i;
    		visualizers[i].transform.localScale = new Vector3(radii[i], radii[i], 1);
    	}
    }
}
