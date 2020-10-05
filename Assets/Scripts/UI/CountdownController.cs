using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
    public GameManager GM;
	public Text text;

	float startTime;
	float endTime;
	//float decTime;
	int dispVal;

	float sizeMod;

	public float pulseAmp;
	public float pulseDamp;
    // Start is called before the first frame update
    public void Init(){
        if(GM == null) GM = GameManager.GetGM();
        if(text == null) text = GetComponent<Text>();
    }

    public void Set(float end){
    	startTime = Time.time;
    	endTime = end;
    	//decTime = (endTime - startTime)/3f;
   		text.enabled = true;
   		sizeMod = pulseAmp;
    }

    // Update is called once per frame
    void Update(){
    	if(endTime - Time.time < 0){
    		text.enabled = false;
    		return;
    	}

    	int prevDisp = dispVal;
    	dispVal = (int)((endTime - Time.time)/(endTime - startTime)*3) + 1;
    	text.text = ""+ dispVal;


    	if(dispVal != prevDisp){
    		sizeMod = pulseAmp;
    	}

        text.transform.localScale = new Vector3(1, 1, 1)*(1 + sizeMod);
        sizeMod *= pulseDamp;
    }
}
