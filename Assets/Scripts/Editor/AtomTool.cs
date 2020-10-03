using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AtomMap))]
public class AtomTool : Editor {
/*
	public AtomToolWindow window;
	public Atom toBePlaced;

	public static float baseRadiusUnit = 2; // This will eventually be determined by BPM and such
	public static float anchorSnapThresh = 10;

	public void OnSceneGUI(){
		if(window == null){
			window = EditorWindow.GetWindow<AtomToolWindow>();
			return;
		}

		AtomMap map = (AtomMap) target;
		Event e = Event.current;

		float radius = window.selectedRadius*baseRadiusUnit;
		int numLevels = window.selectedNumLevels;
		int subdiv = window.selectedSubdiv;

		Vector2 mousePos = EditorToWorldPoint(e.mousePosition);

		if(toBePlaced == null){
			if(e.type == EventType.MouseDown && e.button == 0){
				toBePlaced = Instantiate(AssetDatabase.LoadAssetAtPath<Atom>("Assets/Prefabs/Atom.prefab"), map.transform).GetComponent<Atom>();
				toBePlaced.Init(radius, numLevels);
				Debug.Log("PREVIEW");
			}

		//if toBePlaced != null
		}else{
			//Debug.Log("TO BE PLACED");
			if(e.type == EventType.MouseDown && e.button == 1){
				DestroyImmediate(toBePlaced.gameObject);
				toBePlaced = null;
				e.Use();
				Debug.Log("CANCEL");
				return;
			}

			Atom anchor = null;
			Vector2 placePoint; // <-- goal is to obtain this

			if(map.atoms.Count > 0){
				Atom closest = map.atoms[0];
				float closestDist = float.MaxValue;
				foreach(Atom a in map.atoms){
					if(Mathf.Abs(((Vector2)a.transform.position - mousePos).sqrMagnitude - a.OuterRadius) < closestDist){
						closest = a;
						closestDist = ((Vector2)a.transform.position - mousePos).sqrMagnitude;
					}
				}

				if(closestDist < anchorSnapThresh){
					anchor = closest;
				}
			}

			if(anchor != null){
				Vector2 snapDir = GetSnapDir(anchor, mousePos, subdiv);
				placePoint = (Vector2)anchor.transform.position + snapDir*(anchor.OuterRadius + radius);
			}else{
				placePoint = mousePos;
			}

			toBePlaced.transform.position = (Vector3)placePoint;

			if(e.type == EventType.MouseDown && e.button == 0){
				map.atoms.Add(toBePlaced);
				toBePlaced = null;
				Debug.Log("PLACED");
			}
		}

		e.Use();


	}

	public static Vector2 GetSnapDir(Atom anchor, Vector2 mousePos, int subdiv){
		Vector2 anchorPos = (Vector2)anchor.transform.position;
		float mouseAngle = Util.VectorAngle(mousePos - (Vector2)anchor.transform.position);
		float divAngle = 2*Mathf.PI/subdiv;

		int pNum = (int)(mouseAngle/divAngle);
		return new Vector2(Mathf.Cos(divAngle*pNum), Mathf.Sin(divAngle*pNum));
	}


	public static Vector2 EditorToWorldPoint(Vector2 rawPos){
		rawPos.y = SceneView.currentDrawingSceneView.camera.pixelHeight - rawPos.y;
		return SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(rawPos);
	}
	*/
}

