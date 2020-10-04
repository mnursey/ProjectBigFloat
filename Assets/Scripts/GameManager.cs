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

    	}

    	if(player == null) player = GameObject.Find("Player").GetComponent<Player>();
    	if(camera == null) camera = GameObject.Find("Main Camera").GetComponent<CameraController>();
    	if(music == null) music = GameObject.Find("Music Source").GetComponent<AudioSource>();

    	player.camera = camera;
    	camera.player = player;
    	player.gameObject.SetActive(false);
    	camera.enabled = false;
    	music.Stop();

    	//PreStartLevel(0);	
    }

    public void PreStartLevel(int i){
    	foreach(LevelData ld in levels){
    		ld.map.gameObject.SetActive(false);
    	}

    	currentLevel = levels[i];
    	currentLevel.map.gameObject.SetActive(true);

    	BPM = currentLevel.BPM;
    	BPS = BPM/60f;

    	music.clip = currentLevel.music;
    	music.PlayScheduled(AudioSettings.dspTime + 1);

    	levelStartTime = Time.time + 1.15f;
    	waitingForLevelStart = true;
    }

    public void StartLevel(){
    	player.gameObject.SetActive(true);
    	player.StartLevel(currentLevel.map, currentLevel.startAtom, currentLevel.startAngle);

    	camera.enabled = true;
    }

    void Update(){
    	musicPositionSec = music.time - 0.15f;

    	if(waitingForLevelStart && Time.time > levelStartTime){
    		waitingForLevelStart = false;
    		StartLevel();
    	}

    	if(Input.GetKeyDown("space")){
    		//temp
    		PreStartLevel(0);
    	}

    	if(Input.GetMouseButtonDown(1)){
    		//Debug.Log(musicPositionSec*BPS - (int)(musicPositionSec*BPS));
    	}
    	
    }



}
