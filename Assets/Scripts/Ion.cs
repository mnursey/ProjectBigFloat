using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum IonBehaviour{
    	Static,
    	Orbit, 
    	Follow
    }

public class Ion : MonoBehaviour
{
    public GameManager GM;
    
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


    void Update(){
    	if(!enabled) return;

    	switch(behaviour){
    		case IonBehaviour.Orbit:
    			float velocity = GM.BPS*(2*Mathf.PI)*orbitFreq;
				angle = angle + velocity*Time.deltaTime/parent.OuterRadius;
				transform.position = (Vector3)((Vector2)parent.transform.position + parent.radii[orbitLevel] * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
    		break;

    		case IonBehaviour.Follow:
    			Vector3 moveVector = (GM.player.transform.position - transform.position).normalized * followSpeed;
    			transform.position += moveVector;
    		break;

    		default:
    		break;
    	}

    	if((GM.player.transform.position - transform.position).magnitude < radius){
    		if(!touchingPlayer){
    			GM.DamagePlayer();
    			touchingPlayer = true;
    		}
    	}else{
    		touchingPlayer = false;
    	}
    }
}
