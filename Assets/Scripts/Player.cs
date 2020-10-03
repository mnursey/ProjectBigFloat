using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public Transform atomMap;

	public static float overlapThresh = 1f;

	public Atom parent;
	public float velocity;
	public float angle; //In RADIANS (2pi rads = 360 degs)
	public int level;

	void Start(){
		if(atomMap == null) atomMap = GameObject.Find("Atom Map").transform;
		if(parent == null) parent = atomMap.GetChild(0).GetComponent<Atom>();
		Debug.Log(Util.VectorAngle(new Vector2(1,1)));
		Debug.Log(Util.VectorAngle(new Vector2(-1,1)));
		Debug.Log(Util.VectorAngle(new Vector2(-1,-1)));
		Debug.Log(Util.VectorAngle(new Vector2(1,-1)));
	}

	public void Update(){
		angle = angle + velocity*Time.deltaTime/parent.OuterRadius;
		Vector2 newPos = ((Vector2)parent.transform.position) + parent.radii[level] * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		transform.position = (Vector3)newPos;

		if(Input.GetMouseButtonDown(0)){
			if(!TryJump(1)) Switch(-1);
			
		}else if(Input.GetMouseButtonDown(1)){
			if(!TryJump(-1)) Switch(1);
		}
	}

	public bool TryJump(int dir){
		foreach(Transform t in atomMap){
			if(t == parent.transform) continue;

			Atom a = t.GetComponent<Atom>();
			float dist = (transform.position - a.transform.position).magnitude;
			for(int l = 0; l < a.numLevels; l++){
				if(Mathf.Abs(dist - a.OuterRadius) < overlapThresh){
					if(dir == Mathf.Sign((a.transform.position - parent.transform.position).magnitude - parent.OuterRadius)){
						Jump(a);
						//Debug.Log("Jumped to "+ a.gameObject.name);
						return true;
					}
				}
			}
		}

		return false;
	}

	public void Switch(int inc){
		int newLevel = level+inc;
		if(newLevel >= 0 && newLevel < parent.radii.Length){
			level = newLevel;
		}

	}

	void Jump(Atom newParent){
		parent = newParent;
		level = 0;
		angle = Util.VectorAngle((Vector2)(transform.position - newParent.transform.position));
		velocity *= -1;
	}
}
