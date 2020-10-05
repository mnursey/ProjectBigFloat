using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using GL = UnityEngine.GUILayout;
using EGL = UnityEditor.EditorGUILayout;

public class IonToolWindow : EditorWindow
{
	private Transform atomMapHolder;
	private Transform atomMap;
	private Transform ionMapHolder;
    private Transform ionMap;
    private int targetMapIndex = 0;
    public Ion toBePlaced;

    public bool isEditing = false;

	public static float baseRadiusUnit = 0.1f; // This will eventually be determined by BPM and such
	public static float anchorSnapThresh = 10;

	public IonBehaviour behaviour;
    public float radius;
    public int subdiv;
    public int subdivOffset;

    public float orbitFreq;
    public Atom resetTrigger;
    
    int radiusIndex;
    int subdivIndex;

   	//string[] radiusLabels = {"0.1", "0.2", "", "4", "6", "8", "12", "16"};
   	string[] subdivLabels = {"1/4", "1/6", "1/8", "1/12", "1/16", "1/24", "1/32"};

   	//int[] radiusOptions = {1, 2, 3, 4, 6, 8, 12, 16};
   	int[] subdivOptions = {4, 6, 8, 12, 16, 24, 32};

    [MenuItem("Window/Ion Placement Tool")]
	private static void OpenWindow(){
		IonToolWindow window = GetWindow<IonToolWindow>();
		window.titleContent = new GUIContent("Ion Placement Tool");
		window.maxSize = new Vector2(500, 150);
		window.minSize = new Vector2(300, 150);
		window.Show();

	}

	void ToggleEditing(bool state) {
		if(state == true){
			//https://answers.unity.com/questions/58018/drawing-to-the-scene-from-an-editorwindow.html
		    // Remove delegate listener if it has previously
		    // been assigned.
		    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
		    // Add (or re-add) the delegate.
		    SceneView.onSceneGUIDelegate += this.OnSceneGUI;
	    }else{
	    	
    		SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
	    }
		
	}

	void OnDestroy() {
    	// When the window is destroyed, remove the delegate
     	// so that it will no longer do any drawing.
     	ToggleEditing(false);
 	}

 	void OnEnable(){
 		isEditing = false;
 		ToggleEditing(false);
 	}

	public void OnGUI(){
		if(atomMapHolder == null){
			atomMapHolder = GameObject.Find("Atom Map List").transform;
			if(atomMapHolder == null){
				GUI.Label(new Rect((position.width - 100)/2, (position.height - 20)/2, 100, 20), "NO ATOM MAP LIST");
				return;
			}
		}

		if(ionMapHolder == null){
			ionMapHolder = GameObject.Find("Ion Map List").transform;
			if(ionMapHolder == null){
				GUI.Label(new Rect((position.width - 100)/2, (position.height - 20)/2, 100, 20), "NO ION MAP LIST");
				return;
			}
		}

		if(atomMap == null || ionMap == null) SetTargetMap(0);

		behaviour = (IonBehaviour)EGL.EnumPopup("Behaviour:", behaviour);

		radiusIndex = EGL.IntSlider("Radius:", radiusIndex, 0, 500);

		GL.Label("Subdivision:");
		subdivIndex = GL.Toolbar(subdivIndex, subdivLabels);

		subdivOffset = EGL.IntField("Subdiv Offset:", subdivOffset);

		if(behaviour == IonBehaviour.Orbit){
			orbitFreq = EGL.FloatField("Orbit Speed:", orbitFreq);
			resetTrigger = (Atom)EGL.ObjectField("Reset Trig:", resetTrigger, typeof(Atom));
		}

		bool prevEditingState = isEditing;
		GL.BeginHorizontal();
		//GL.Label("");
		isEditing = GL.Toggle(isEditing, "Enable/Disable Editing");
		if(isEditing != prevEditingState) ToggleEditing(isEditing);

		int prevTarget = targetMapIndex;
		targetMapIndex = (int)Mathf.Clamp(EGL.IntField("Target Map", targetMapIndex), 0, atomMapHolder.childCount-1);
		if(prevTarget != targetMapIndex) SetTargetMap(targetMapIndex);
		GL.EndHorizontal();

		radius = radiusIndex*baseRadiusUnit;
		subdiv = subdivOptions[subdivIndex];
	}


	public void OnSceneGUI(SceneView sceneView){
		Event e = Event.current;
		Vector2 mousePos = AtomToolWindow.EditorToWorldPoint(e.mousePosition);

		if(toBePlaced == null){
			if(e.type == EventType.MouseDown && e.button == 0){
				toBePlaced = Instantiate(AssetDatabase.LoadAssetAtPath<Ion>("Assets/Prefabs/Ion.prefab")).GetComponent<Ion>();
				toBePlaced.Init(behaviour, radius*baseRadiusUnit);
				//Debug.Log("PREVIEW");
			}else if(e.type == EventType.MouseDrag && e.button == 1){
				foreach(Transform t in ionMap){
					if(((Vector2)t.position - mousePos).sqrMagnitude < 1){
						DestroyImmediate(t.gameObject);
					}
				}
			}

		//if toBePlaced != null
		}else{
			//Debug.Log("TO BE PLACED");
			if(e.type == EventType.MouseDown && e.button == 1){
				DestroyImmediate(toBePlaced.gameObject);
				toBePlaced = null;
				e.Use();
				//Debug.Log("CANCEL");
				return;
			}



			Atom anchor = null;
			Vector2 placePoint = mousePos; // <-- goal is to obtain this

			switch(behaviour){
				case IonBehaviour.Orbit:
					if(atomMap.transform.childCount > 0){
						Atom closest = null;
						int closestLevel = 0;
						float closestDist = float.MaxValue;

						foreach(Transform t in atomMap){
							Atom a = t.GetComponent<Atom>();

							for(int l = 0; l < a.radii.Length; l++){
								float dist = Mathf.Abs(((Vector2)a.transform.position - mousePos).sqrMagnitude - a.radii[l]*a.radii[l]);
								if(dist < closestDist){
									closest = a;
									closestLevel = l;
									closestDist = dist;
								}
							}
							
						}

						if(closestDist < anchorSnapThresh){
							anchor = closest;

							Vector2 snapDir = AtomToolWindow.GetSnapDir(anchor, mousePos, subdiv, subdivOffset*Mathf.Deg2Rad);
							placePoint = (Vector2)anchor.transform.position + snapDir*(anchor.radii[closestLevel]);

							toBePlaced.parent = anchor;
							toBePlaced.initAngle = Util.VectorAngle(snapDir);
							toBePlaced.orbitLevel = closestLevel;
							toBePlaced.orbitFreq = orbitFreq;
							if(resetTrigger != null) toBePlaced.resetTrigger = resetTrigger;
						}
					}
				break;

				default:

				break;
			}

			

			toBePlaced.transform.position = (Vector3)placePoint;

			if(e.type == EventType.MouseDown && e.button == 0){
				toBePlaced.transform.SetParent(ionMap);
				toBePlaced = null;
				//Debug.Log("PLACED");
			}
		}

		if((e.type == EventType.MouseDown || e.type == EventType.MouseUp) && e.button != 2){
			e.Use();
		}


	}


	public void SetTargetMap(int i){
		atomMap = atomMapHolder.GetChild(i);
		ionMap = ionMapHolder.GetChild(i);
	}

}
