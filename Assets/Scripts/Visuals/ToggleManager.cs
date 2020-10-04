﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToggleManager : MonoBehaviour
{
    public bool toggled = false;

    public Color colour = new Color();
    public Color toggledColour = new Color();

    List<LineRenderer> lrs = new List<LineRenderer>();

    public GameObject player;
    Player playerScript;

    public float toggledRadius = 1f;

    bool prevFrameInRadius = false;

    public List<Atom> affectedAtoms = new List<Atom>();

    // Start is called before the first frame update
    void Start()
    {
        lrs = GetComponentsInChildren<LineRenderer>().ToList();

        if (player == null)
        {
            Debug.LogError("Toggle manager needs player reference");
        }
        else
        {
            playerScript = player.GetComponent<Player>();
        }

        foreach (Atom a in affectedAtoms)
        {
            a.colour = colour;
            //a.poweredColour = toggledColour;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (toggled)
        {
            foreach (LineRenderer lr in lrs)
            {
                lr.startColor = toggledColour;
                lr.endColor = toggledColour;
            }
        }
        else
        {
            foreach (LineRenderer lr in lrs)
            {
                lr.startColor = colour;
                lr.endColor = colour;
            }
        }

        if ((player.transform.position - transform.position).magnitude <= toggledRadius)
        {
            if(!prevFrameInRadius)
            {
                toggled = !toggled;

                foreach (Atom a in affectedAtoms)
                {
                    a.powered = !a.powered;
                }
            }

            prevFrameInRadius = true;
        }
        else
        {
            prevFrameInRadius = false;
        }
    }
}
