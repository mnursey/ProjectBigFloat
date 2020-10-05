using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public List<LevelData> levels;
	public LevelData currentLevel;

    public int BPM;
    [HideInInspector]
    public float BPS;

    public Player player;
    public CameraController camera;
    public AudioSource music;
    public CountdownController countdown;
    public Material BG;

    public int score;
    public int maxScore = 100000; 

    float BGSaturationBonus;
    public float BGSaturationBonusDamp;


    bool waitingForLevelStart;
    float levelStartTime;
    
    public float musicPositionSec;
    //[HideInInspector]
    public int pulsesPerBeat;


    //temp
    void Start(){
    	levels.Clear();
    	foreach(Transform t in transform){
    		LevelData l = t.GetComponent<LevelData>();
    		levels.Add(l);
    		l.map.gameObject.SetActive(false);
    		l.ionMap.gameObject.SetActive(false);

    	}

    	if(player == null) player = GameObject.Find("Player").GetComponent<Player>();
    	if(camera == null) camera = GameObject.Find("Main Camera").GetComponent<CameraController>();
    	if(music == null) music = GameObject.Find("Music Source").GetComponent<AudioSource>();
    	if(countdown == null) countdown = GameObject.Find("Countdown").GetComponent<CountdownController>();
    	if(BG == null) BG = camera.transform.GetChild(0).GetComponent<MeshRenderer>().material;

    	player.camera = camera;
    	camera.player = player;
    	player.gameObject.SetActive(false);
    	camera.enabled = false;
    	music.Stop();

    	//PreStartLevel(0);	
    }

    public void PreStartLevel(int i){
    	currentLevel = levels[i];

    	foreach(LevelData ld in levels){
    		ld.map.gameObject.SetActive(false);
    	}

    	BPM = currentLevel.BPM;
    	BPS = BPM/60f;

    	score = 5000;

    	SetBGColour(currentLevel.BGColour);

    	currentLevel.map.gameObject.SetActive(true);
    	currentLevel.ionMap.gameObject.SetActive(true);

    	foreach(Transform t in currentLevel.ionMap){
    		Ion ion = t.GetComponent<Ion>();
    		ion.SetVisible(true);
    	}

    	player.gameObject.SetActive(true);
    	player.PrepareForLevelStart(currentLevel.map, currentLevel.startAtom, currentLevel.startAngle, currentLevel.frequency);
    	
    	camera.enabled = true;
    	
    	music.clip = currentLevel.music;
    	music.PlayScheduled(AudioSettings.dspTime + 2);

    	levelStartTime = Time.time + 2 + currentLevel.firstBeatDelay;
    	waitingForLevelStart = true;

    	countdown.Set(levelStartTime);
    }

    public void StartLevel(){
    	player.enabled = true;

    	foreach(Transform t in currentLevel.ionMap){
    		Ion ion = t.GetComponent<Ion>();
    		ion.Reset();
    	}
    }

    public void DamagePlayer(){
    	PreStartLevel(0);
    }

    public void PlayerJump(Atom prev, Atom next){
    	score += 100;
    	BGSaturationBonus = 0.4f;

    	foreach(Transform t in currentLevel.ionMap){
    		Ion ion = t.GetComponent<Ion>();
    		if(ion.resetTrigger != null && ion.resetTrigger.transform == next.transform){
    			ion.Reset();
    		}
    	}
    }

    public void SetBGColour(Color c){
    	//BG.SetColor("Color_1432EB74", currentLevel.BGColour1);
    	BG.SetColor("Color_A528FD8E", c);
    }

    public void SetBGSaturation(float s){
    	Color c = BG.GetColor("Color_A528FD8E");
    	float H, x, V;
    	Color.RGBToHSV(c, out H, out x, out V);
    	BG.SetColor("Color_A528FD8E", Color.HSVToRGB(H, s, V));
    }

    void Update(){
    	if(currentLevel != null){
    		musicPositionSec = music.time + 2 - currentLevel.firstBeatDelay;

	    	if(waitingForLevelStart && Time.time > levelStartTime){
	    		waitingForLevelStart = false;
	    		StartLevel();
	    	}

	    	float saturation = Mathf.Min(0.1f + Mathf.Pow(score/(float)maxScore, 0.7f), 0.8f);
	    	SetBGSaturation(Mathf.Min(saturation + BGSaturationBonus, 1));
	    	BGSaturationBonus *= BGSaturationBonusDamp;
	    }

    	if(Input.GetKeyDown("space")){
    		//temp
    		PreStartLevel(0);
    	}

    	if(Input.GetMouseButtonDown(1)){
    		//Debug.Log(musicPositionSec*BPS - (int)(musicPositionSec*BPS));
    	}

    	score -= (int)(100*Time.deltaTime);
    }

    public static GameManager GetGM(){
    	return GameObject.Find("Game Manager").GetComponent<GameManager>();
    }



}
