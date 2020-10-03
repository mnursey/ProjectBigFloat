using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<Atom> atoms;

    //temp
    void Start(){
    	Transform atomHolder = GameObject.Find("AtomHolder").transform;
    	foreach(Transform t in atomHolder) atoms.Add(t.GetComponent<Atom>());
    }

}
