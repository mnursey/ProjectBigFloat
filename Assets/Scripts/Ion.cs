using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IonType{
	Positive,
	Negative
}

public enum IonBehaviour{
    Static,
    Orbit, 
    Follow
   }

public class Ion : MonoBehaviour
{
	public static float ionRadiusScaleFactor = 4;

    public GameManager GM;

    public IonType type;
    public IonBehaviour behaviour;
    public float radius;
    public bool touchingPlayer;
    public bool enabled;

    public Vector3 initPos;

    //Orbit-only
    public Atom parent;
    public float initAngle;
    public float orbitFreq;
    public int orbitLevel;
    float angle;

    //Follow only
    public float followSpeed;

    //If player orbits this atom, the ion resets and plays its behaviour
    //useful for synchronizing obstacles with the player
    public Atom resetTrigger;

    public void Init(IonBehaviour b, float r){
    	if(GM == null) GM = GameManager.GetGM();
    	behaviour = b;
    	radius = r;

    	transform.localScale = new Vector3(r, r, 1);
    }


    public void Reset(){
    	enabled = true;
    	SetVisible(true);

    	switch(behaviour){
    		case IonBehaviour.Orbit:
    			angle = initAngle;
    		break;

    		case IonBehaviour.Follow:
    			transform.position = initPos;
    		break;

    		default:
    		break;
    	}

    }

    public void SetVisible(bool state){
    	transform.GetChild(0).gameObject.SetActive(state);
    }


    void Update(){
    	if(!enabled) return;

    	switch(behaviour){
    		case IonBehaviour.Orbit:
    			float velocity = GM.BPS*(2*Mathf.PI)*orbitFreq;
				angle = angle + velocity*Time.deltaTime/parent.OuterRadius;
				transform.position = (Vector3)((Vector2)parent.transform.position + parent.radii[orbitLevel] * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
    			transform.eulerAngles = new Vector3(0, 0, angle*Mathf.Rad2Deg);
    		break;

    		case IonBehaviour.Follow:
    			Vector3 moveVector = (GM.player.transform.position - transform.position).normalized * followSpeed;
    			transform.position += moveVector*Time.deltaTime;
    		break;

    		default:
    		break;
    	}

    	if((GM.player.transform.position - transform.position).magnitude < radius*ionRadiusScaleFactor*((type == IonType.Negative) ? 1 : 0.1f)){
    		if(!touchingPlayer){
    			if(type == IonType.Negative){
	    			GM.DamagePlayer();
	    		}else{
	    			GM.score += 500;
	    			SetVisible(false);
	    		}

	    		touchingPlayer = true;
	    	}
    	}else{
    		touchingPlayer = false;
    	}
    }
}
