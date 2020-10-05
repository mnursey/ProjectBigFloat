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

    public int score = 1000;


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

    	currentLevel.map.gameObject.SetActive(true);
    	currentLevel.ionMap.gameObject.SetActive(true);

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

    }

    public void PlayerJump(Atom prev, Atom next){
    	score += 100;

    	foreach(Transform t in currentLevel.ionMap){
    		Ion ion = t.GetComponent<Ion>();
    		if(ion.resetTrigger != null && ion.resetTrigger.transform == next.transform){
    			ion.Reset();
    		}
    	}
    }

    void Update(){
    	if(currentLevel != null){
    		musicPositionSec = music.time + 2 - currentLevel.firstBeatDelay;

	    	if(waitingForLevelStart && Time.time > levelStartTime){
	    		waitingForLevelStart = false;
	    		StartLevel();
	    	}
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
