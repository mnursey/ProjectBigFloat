using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public GameManager GM;

	public static float overlapThresh = 3f;

	public Atom parent;
	public float velocity;
	public float angle; //In RADIANS (2pi rads = 360 degs)
	public int level;

	void Start(){
		if(GM == null) GM = GameObject.Find("GameManager").GetComponent<GameManager>();
		Debug.Log(Util.VectorAngle(new Vector2(1,1)));
		Debug.Log(Util.VectorAngle(new Vector2(-1,1)));
		Debug.Log(Util.VectorAngle(new Vector2(-1,-1)));
		Debug.Log(Util.VectorAngle(new Vector2(1,-1)));
	}

	public void Update(){
		angle = angle + velocity/parent.OuterRadius;
		Vector2 newPos = ((Vector2)parent.transform.position) + parent.radii[level] * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
		transform.position = (Vector3)newPos;

		if(Input.GetKeyDown("space")){
			TryJump();
		}
	}

	public void TryJump(){
		foreach(Atom a in GM.atoms){
			if(a == parent) continue;

			float dist = (transform.position - a.transform.position).magnitude;
			for(int l = 0; l < a.numLevels; l++){
				if(Mathf.Abs(dist - a.radii[l]) < overlapThresh){
					Jump(a, l);
					Debug.Log("Jumped to "+ a.gameObject.name);
					return;
				}
			}
		}
	}

	void Jump(Atom newParent, int newLevel){
		parent = newParent;
		level = newLevel;

		velocity *= -1;
	}
}
