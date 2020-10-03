using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public GameManager GM;
	public CameraController camera;
	public Transform atomMap;

	public static float overlapThresh = 1f;

	public Atom parent;
	public int freq;
	int direction = 1;
	public float velocity;
	public float angle; //In RADIANS (2pi rads = 360 degs)
	public int level;

	public float jumpShakeMag;
	public float jumpShakeDur;
	public float jumpShakeDamp;

	void Start(){
		if(GM == null) GM = GameObject.Find("Game Manager").GetComponent<GameManager>();
		if(camera == null) camera = GameObject.Find("Main Camera").GetComponent<CameraController>();
		if(atomMap == null) atomMap = GameObject.Find("Atom Map").transform;
		if(parent == null) parent = atomMap.GetChild(0).GetComponent<Atom>();
		angle = 0.1f;
	}

	public void Update(){
		velocity = ((GM.BPM/60f)*(2*Mathf.PI))/freq;

		angle = angle + direction*velocity*Time.deltaTime/parent.OuterRadius;
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
						Jump(a, -dir);
						camera.Shake(jumpShakeMag, jumpShakeDur, jumpShakeDamp);
						//Debug.Log("Jumped to "+ a.gameObject.name);
						return true;
					}
				}
			}
		}

		return false;
	}

	void Switch(int inc){
		int newLevel = level+inc;
		if(newLevel >= 0 && newLevel < parent.radii.Length){
			level = newLevel;
		}

	}

	void Jump(Atom newParent, int dirMod){
		parent = newParent;
		level = 0;
		angle = Util.VectorAngle((Vector2)(transform.position - newParent.transform.position));
		direction *= dirMod;
	}
}
