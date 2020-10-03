using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour{

	public enum CameraMode{
		FollowPlayer,
		FollowAtom,
		Hybrid
	}

	public CameraMode mode;
	public float smoothTime;
	public float hybridRadiusWeight;
	public float hybridRadiusOffset;

	public Player player;
	Vector3 velocity;
	Vector3 offsetVelocity;
	Vector3 targetPos;
	Vector3 basePos;
	Vector3 offset;

    // Start is called before the first frame update
    void Start(){
        if(player == null) player = GameObject.Find("Player").GetComponent<Player>();
        basePos = transform.position;
        offset = Vector3.zero;
    }

    // Update is called once per frame
    void Update(){
        switch(mode){
        	case CameraMode.FollowPlayer:
        		transform.position = Vector3.SmoothDamp(transform.position,
        												new Vector3(player.transform.position.x, player.transform.position.y, -10),
        												ref velocity,
        												smoothTime);
        	break;

        	case CameraMode.FollowAtom:
        		transform.position = Vector3.SmoothDamp(transform.position,
        												new Vector3(player.parent.transform.position.x, player.parent.transform.position.y, -10),
        												ref velocity,
        												smoothTime);
        	break;

        	case CameraMode.Hybrid:
        		Vector3 atomPos = new Vector3(player.parent.transform.position.x, player.parent.transform.position.y, -10);
        		Vector3 playerPos = new Vector3(player.transform.position.x, player.transform.position.y, -10);
        		
        		basePos = Vector3.SmoothDamp(basePos, atomPos, ref velocity, smoothTime);
        		offset = Vector3.SmoothDamp(offset, (playerPos - atomPos).normalized*Mathf.Max(player.parent.OuterRadius*hybridRadiusWeight + hybridRadiusOffset, 0),
        											ref offsetVelocity,
        											smoothTime * Mathf.Clamp((basePos - atomPos).magnitude/player.parent.OuterRadius, 0.05f, 1f));

        		transform.position = basePos + offset;


        	break;
        }

        //transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
