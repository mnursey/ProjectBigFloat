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
	public bool enabled;
	public float smoothTime;
	public float hybridRadiusWeight;
	public float hybridRadiusOffset;

	public Player player;
	public Camera camera;
	Vector3 velocity;
	Vector3 offsetVelocity;
	float zoomVelocity;
	Vector3 targetPos;
	Vector3 basePos;
	Vector3 offset;

	float shakeDuration;
	float shakeMagnitude;
	float shakeDamp;
	Vector2 shakeVector;

    // Start is called before the first frame update
    void Start(){
        if(player == null) player = GameObject.Find("Player").GetComponent<Player>();
        if(camera == null) camera = GetComponent<Camera>();
        basePos = transform.position;
        offset = Vector3.zero;
    }

    // Update is called once per frame
    void Update(){
    	if(!enabled) return;

    	if(shakeDuration > 0){
    		shakeVector = (Vector2)(Random.insideUnitSphere*shakeMagnitude);
    		shakeMagnitude *= shakeDamp;
    		shakeDuration -= Time.deltaTime;
    	}else{
    		shakeVector = Vector2.zero;
    	}

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

        		transform.position = basePos + offset + (Vector3)shakeVector;
        	break;
        }

        float targetCameraSize = Mathf.Max(8, 8 + (player.parent.OuterRadius-10)/2);
        camera.orthographicSize = Mathf.SmoothDamp(camera.orthographicSize, targetCameraSize, ref zoomVelocity, 0.5f);

        //transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }

    public void Shake(float magnitude, float duration, float damp = 1){
    	shakeMagnitude = magnitude;
    	shakeDuration = duration;
    	shakeDamp = damp;
    }
}
