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

    bool gameMode;

    public Player player;
    public CameraController camera;
    public AudioSource music;
    public CountdownController countdown;
    public ScoreController scoreController;
    public MenuController menu;
    public Material BG;

    public int score;
    public int maxScore = 100000; 

    float BGSaturationBonus;
    Color BGTintColour;
    public float BGTintAmp;
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
            l.specialsMap.gameObject.SetActive(false);
    	}

    	if(player == null) player = GameObject.Find("Player").GetComponent<Player>();
    	if(camera == null) camera = GameObject.Find("Main Camera").GetComponent<CameraController>();
    	if(music == null) music = GameObject.Find("Music Source").GetComponent<AudioSource>();
    	if(countdown == null) countdown = GameObject.Find("Countdown").GetComponent<CountdownController>();
    	if(scoreController == null) scoreController = GameObject.Find("Score").GetComponent<ScoreController>();
    	if(menu == null) menu = GameObject.Find("MenuController").GetComponent<MenuController>();
    	if(BG == null) BG = camera.transform.GetChild(0).GetComponent<MeshRenderer>().material;

    	player.camera = camera;
    	camera.player = player;

    	EnterMenuMode();
    	menu.GoToMenu(menu.mainMenu, false);
    }

    public void EnterMenuMode(){
    	foreach(LevelData l in levels){
    		l.map.gameObject.SetActive(false);
    		l.ionMap.gameObject.SetActive(false);
    		l.specialsMap.gameObject.SetActive(false);
    	}

    	player.gameObject.SetActive(false);
    	camera.enabled = false;

    	music.Stop();

    	countdown.Init();
    	countdown.text.enabled = false;

    	scoreController.Init();
    	scoreController.text.enabled = false;

    	gameMode = false;
    }

    public void EnterGameMode(){
    	menu.CloseAllMenus();
    	gameMode = true;
    }

    public void EnterLevel(int i){
    	currentLevel = levels[i];
    	EnterGameMode();
    	PreStartLevel(currentLevel);
    }

    public void PreStartLevel(LevelData level){
    	foreach(LevelData ld in levels){
    		ld.map.gameObject.SetActive(false);
    	}

    	BPM = level.BPM;
    	BPS = BPM/60f;

    	score = 5000;

    	SetBGColour(level.BGColour);

    	currentLevel.map.gameObject.SetActive(true);
    	currentLevel.ionMap.gameObject.SetActive(true);
        currentLevel.specialsMap.gameObject.SetActive(true);

    	foreach(Transform t in level.ionMap){
    		Ion ion = t.GetComponent<Ion>();
    		ion.Reset();
    		ion.SetVisible(true);
    	}

    	player.gameObject.SetActive(true);
    	player.PrepareForLevelStart(level.map, level.startAtom, level.startAngle, level.frequency);

        ResetSpecialsMap();

    	camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
    	camera.enabled = true;
    	
    	music.clip = currentLevel.music;
    	music.PlayScheduled(AudioSettings.dspTime + 2);

    	levelStartTime = Time.time + 2 + currentLevel.firstBeatDelay;
    	waitingForLevelStart = true;

    	countdown.Set(levelStartTime);
    }

    public void StartLevel(){
    	player.enabled = true;
    	scoreController.text.enabled = true;

    	foreach(Transform t in currentLevel.ionMap){
    		Ion ion = t.GetComponent<Ion>();
    		ion.enabled = true;
    	}

        ResetSpecialsMap();
    }

    public void DamagePlayer(){
    	PreStartLevel(currentLevel);
    }

    public void PlayerJump(Atom prev, Atom next){
    	score += 100;
    	BGSaturationBonus = 0.4f;
    	scoreController.Shake(20, 0.5f, 0.9f);


    	foreach(Transform t in currentLevel.ionMap){
    		Ion ion = t.GetComponent<Ion>();
    		if(ion.resetTrigger != null && ion.resetTrigger.transform == next.transform){
    			ion.Reset();
    		}
    	}
    }

    public void GetPickup(){
    	score += 500;
    	BGTintAmp = 1f;
    	scoreController.Shake(40, 0.5f, 0.9f);
    }

    public Color GetBGColour(){
    	return BG.GetColor("Color_A528FD8E");
    }

    public void SetBGColour(Color c){
    	BG.SetColor("Color_A528FD8E", c);
    }

    public void SetBGColour2(Color c){
    	BG.SetColor("Color_1432EB74", c);
    }

    public void SetBGSaturation(float s){
    	Color c = BG.GetColor("Color_A528FD8E");
    	float H, x, V;
    	Color.RGBToHSV(c, out H, out x, out V);
    	BG.SetColor("Color_A528FD8E", Color.HSVToRGB(H, s, V));
    }

    void Update(){
    	if(gameMode){
    		if(currentLevel != null){
	    		musicPositionSec = music.time + 2 - currentLevel.firstBeatDelay;

		    	if(waitingForLevelStart && Time.time > levelStartTime){
		    		waitingForLevelStart = false;
		    		StartLevel();
		    	}

		    	float saturation = Mathf.Min(0.1f + Mathf.Pow(score/(float)maxScore, 0.7f), 0.8f);
		    	SetBGSaturation(Mathf.Min(saturation + BGSaturationBonus, 1));
		    	BGSaturationBonus *= BGSaturationBonusDamp;

		    	SetBGColour2(Color.white*BGTintAmp);
		    	BGTintAmp *= BGSaturationBonusDamp;
		    }

	    	if(Input.GetKeyDown("space")){
	    		PreStartLevel(currentLevel);
	    	}

	    	if(Input.GetKeyDown("escape")){
	    		EnterMenuMode();
	    		menu.GoToMenu(menu.levelSelectMenu);
	    	}

	    	score -= (int)(100*Time.deltaTime);
    	}
    	
    }

    public static GameManager GetGM(){
    	return GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void ResetSpecialsMap()
    {
        foreach(Transform t in currentLevel.specialsMap)
        {
            // Reset ends
            EndVisualManager end = t.GetComponent<EndVisualManager>();

            if(end != null)
            {
                end.finished = false;
            }

            // Reset toggles
            ToggleManager toggle = t.GetComponent<ToggleManager>();

            if(toggle != null)
            {
                toggle.toggled = false;
                toggle.Reset();
            }
        }
    }
}
