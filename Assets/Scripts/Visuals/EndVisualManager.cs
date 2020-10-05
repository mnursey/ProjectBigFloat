using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class EndVisualManager : MonoBehaviour
{
    public bool finished = false;
    public float lineAmp;
    public float circleAmp;
    public float lineAmpFinished;
    public float circleAmpFinished;

    public LineVisual lv;
    public AtomVisualController avc;

    public Color colour = new Color();
    public Color finishedColour = new Color();

    List<LineRenderer> lrs = new List<LineRenderer>();

    public GameObject player;
    Player playerScript;

    public float finishedRadius = 1f;

    GameManager gm;
    MenuController mc;

    // Start is called before the first frame update
    void Start()
    {
        lv = GetComponentInChildren<LineVisual>();
        avc = GetComponentInChildren<AtomVisualController>();
        lrs = GetComponentsInChildren<LineRenderer>().ToList();
        gm = GameManager.GetGM();
        mc = MenuController.GetMC();

        player = gm.player.gameObject;
        playerScript = gm.player;
    }

    // Update is called once per frame
    void Update()
    {
        if(finished)
        {
            lv.pulseAmp = lineAmpFinished;
            avc.pulseAmp = circleAmpFinished;

            foreach (LineRenderer lr in lrs)
            {
                lr.startColor = finishedColour;
                lr.endColor = finishedColour;
            }
        } else
        {
            lv.pulseAmp = lineAmp;
            avc.pulseAmp = circleAmp;

            foreach (LineRenderer lr in lrs)
            {
                lr.startColor = colour;
                lr.endColor = colour;
            }
        }

        if((player.transform.position - transform.position).magnitude <= finishedRadius)
        {
            finished = true;

            // Pause player
            playerScript.freq = 0;
            player.transform.position = transform.position;

            gm.EnterMenuMode();
            mc.UpdateFinalScore(gm.score);
            mc.GoToNextLevelMenu();
        }
    }
}
