using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NewLevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : MonoBehaviour
{
    public Transform map;
    public Atom startAtom;
    public float startAngle;
    public List<Atom> checkpoints = new List<Atom>();
    public int BPM;
    public AudioClip music;
}
