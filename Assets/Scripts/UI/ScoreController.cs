using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{

	public GameManager GM;
	public Text text;

	public float pulseAmp;
	public float wiggleAmp;

    public Vector3 basePos;
    float shakeDuration;
    float shakeMagnitude;
    float shakeDamp;
    Vector2 shakeVector;

    // Start is called before the first frame update
    public void Init(){
        if(GM == null) GM = GameManager.GetGM();
        if(text == null) text = GetComponent<Text>();
        basePos = transform.position;
    }

    public void Shake(float magnitude, float duration, float damp = 1){
        shakeMagnitude = magnitude;
        shakeDuration = duration;
        shakeDamp = damp;
    }

    // Update is called once per frame
    void Update(){
    	text.text = "SCORE - "+ GM.score;

        if(shakeDuration > 0){
            shakeVector = (Vector2)(Random.insideUnitSphere*shakeMagnitude);
            shakeMagnitude *= shakeDamp;
            shakeDuration -= Time.deltaTime;
        }else{
            shakeVector = Vector2.zero;
        }

        text.transform.position = basePos + (Vector3)shakeVector;
        text.transform.localScale = new Vector3(1, 1, 1) * (1.1f + pulseAmp*Mathf.Cos(Time.time * GM.BPS));
        text.transform.eulerAngles = new Vector3(0, 0, wiggleAmp * Mathf.Sin(Time.time * GM.BPS));


    }
}
