using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Atom : MonoBehaviour{

	public static float levelSpacing = 1.5f;

	public float[] radii;
	public int numLevels;
	public AtomVisualController[] visualizers;

    public GameObject centreVisual;

    public bool powered = true;

    public Color colour = new Color();
    public Color poweredColour = new Color();

    List<LineRenderer> lrs = new List<LineRenderer>();

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

    void Start()
    {
        lrs = GetComponentsInChildren<LineRenderer>().ToList();
    }

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

        if (powered)
        {
            foreach (LineRenderer lr in lrs)
            {
                lr.startColor = poweredColour;
                lr.endColor = poweredColour;
            }
        }
        else
        {
            foreach (LineRenderer lr in lrs)
            {
                lr.startColor = colour;
                lr.endColor = colour;
            }
        }
    }

    private void SetRadii(float outer){
    	for(int i = 0; i < numLevels; i++){
    		radii[i] = outer - levelSpacing*i;
    		visualizers[i].transform.localScale = new Vector3(radii[i], radii[i], 1);
    	}
    }
}
