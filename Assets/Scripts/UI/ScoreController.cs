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

    // Start is called before the first frame update
    public void Init(){
        if(GM == null) GM = GameManager.GetGM();
        if(text == null) text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update(){
    	text.text = "SCORE - "+ GM.score;

        text.transform.localScale = new Vector3(1, 1, 1) * (1.1f + pulseAmp*Mathf.Cos(Time.time * GM.BPS));
        text.transform.eulerAngles = new Vector3(0, 0, wiggleAmp * Mathf.Sin(Time.time * GM.BPS));
    }
}
