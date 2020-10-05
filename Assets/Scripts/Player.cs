using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public GameManager GM;
	public CameraController camera;
	public Transform atomMap;

	public static float overlapThresh = 0.5f;

	public Atom parent;
	public float freq;
	int direction ;
	float velocity;
	public float angle; //In RADIANS (2pi rads = 360 degs)
	public int level;
	public bool enabled;

	public float jumpShakeMag;
	public float jumpShakeDur;
	public float jumpShakeDamp;

	public void PrepareForLevelStart(Transform map, Atom initParent, float initAngle, float frequency){
		if(GM == null) GM = GameManager.GetGM();
		if(camera == null) camera = GameObject.Find("Main Camera").GetComponent<CameraController>();

		atomMap = map;
		parent = initParent;
		angle = initAngle*Mathf.Deg2Rad;
		level = 0;
		direction = 1;
		freq = frequency;
		enabled = false;

		transform.position = ((Vector2)parent.transform.position) + parent.OuterRadius * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
	}

	public void Update(){

		if(!enabled) return;

        velocity = GM.BPS * (2 * Mathf.PI) * freq;

        angle = angle + direction * velocity * Time.deltaTime / parent.OuterRadius;
        Vector2 newPos = ((Vector2)parent.transform.position) + parent.radii[level] * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        transform.position = (Vector3)newPos;

        if (Input.GetMouseButtonDown(0))
        {
            if (!TryJump(1)) Switch(-1);

        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (!TryJump(-1)) Switch(1);
        }
	}

	public bool TryJump(int dir){
		foreach(Transform t in atomMap){
			if(t == parent.transform) continue;

			Atom a = t.GetComponent<Atom>();

            if(!a.powered)
            {
                continue;
            }

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
			camera.Shake(jumpShakeMag/2, jumpShakeDur, jumpShakeDamp);
		}

	}

	void Jump(Atom newParent, int dirMod){
		GM.PlayerJump(parent, newParent);

		level = 0;
		direction *= dirMod;
		Vector2 atomDisp = (Vector2)(parent.transform.position - newParent.transform.position);
		parent = newParent;

		angle = QuantizeAngle(Util.VectorAngle((Vector2)(transform.position - newParent.transform.position)),
				Util.VectorAngle(atomDisp));
	}


	float QuantizeAngle(float playerAngle, float parentAngle){
    	float quantumAngle = Mathf.Abs(velocity)*(1f/(GM.BPS*GM.pulsesPerBeat))/parent.OuterRadius;

    	float numPulses = GM.musicPositionSec*GM.BPS*GM.pulsesPerBeat;
    	float pulseOffset = numPulses - (int)numPulses;
    	if(pulseOffset > 0.5f) pulseOffset -= 1;

    	float globalAngleOffset = direction*pulseOffset*quantumAngle;

    	float offsetAngle = Util.RadianWrap(playerAngle - parentAngle - globalAngleOffset);

    	float numQuantums = offsetAngle/quantumAngle;
    	int wholeQuantums = (int)numQuantums;
    	float remainder = numQuantums - wholeQuantums;

    	//Debug.Log("Angle: "+ angle/Mathf.PI);
    	Debug.Log("Pulse offset: "+ pulseOffset);
    	//Debug.Log("Parent angle: "+ parentAngle/Mathf.PI);
    	//Debug.Log("Global Offset: "+ globalAngleOffset/Mathf.PI);
    	//Debug.Log("Offset Angle: "+ offsetAngle/Mathf.PI);
    	//Debug.Log("Num Qs: "+ numQuantums);
    	//Debug.Log("Quantum angle: "+ quantumAngle/Mathf.PI);
    	//Debug.Log("Whole Qs: "+ wholeQuantums);
    	//Debug.Log("Remainder: "+ remainder*quantumAngle/Mathf.PI);
    	
		float finalAngle;
    	if(Mathf.Abs(remainder) > 0.5f){
    		finalAngle = Util.RadianWrap(quantumAngle*(wholeQuantums+1) + parentAngle + globalAngleOffset);
    		Debug.Log("Final Angle (A): "+ finalAngle);
		}else{
			finalAngle = Util.RadianWrap(quantumAngle*(wholeQuantums) + parentAngle + globalAngleOffset);
			Debug.Log("Final Angle (B): "+ finalAngle);
		}

		return finalAngle;


    }
}
