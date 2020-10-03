using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using GL = UnityEngine.GUILayout;
using EGL = UnityEditor.EditorGUILayout;

public class AtomToolWindow : EditorWindow
{
    private Transform map;
    public Atom toBePlaced;

    public bool isEditing = false;

	public static float baseRadiusUnit = 1; // This will eventually be determined by BPM and such
	public static float anchorSnapThresh = 10;

    public int radius;
    public int subdiv;
    public int numLevels;
    
    int radiusIndex;
    int subdivIndex;
    int numLevelsIndex;

   	string[] radiusLabels = {"1", "2", "3", "4", "6", "8", "12", "16"};
   	string[] subdivLabels = {"1/4", "1/6", "1/8", "1/12", "1/16", "NA"};
   	string[] numLevelsLabels = {"1", "2", "3", "4", "5"};

   	int[] radiusOptions = {1, 2, 3, 4, 6, 8, 12, 16};
   	int[] subdivOptions = {4, 6, 8, 12, 16, 0};

    [MenuItem("Window/Atom Placement Tool")]
	private static void OpenWindow(){
		AtomToolWindow window = GetWindow<AtomToolWindow>();
		window.titleContent = new GUIContent("Atom Placement Tool");
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
		if(map == null){
			map = GameObject.Find("Atom Map").transform;
			if(map == null){
				GUI.Label(new Rect((position.width - 100)/2, (position.height - 20)/2, 100, 20), "NO ATOM MAP");
				return;
			}
		}

		GL.Label("Radius:");
		radiusIndex = GL.Toolbar(radiusIndex, radiusLabels);

		GL.Label("Subdivision:");
		subdivIndex = GL.Toolbar(subdivIndex, subdivLabels);

		GL.Label("Levels:");
		numLevelsIndex = GL.Toolbar(numLevelsIndex, numLevelsLabels);

		bool prevEditingState = isEditing;
		GL.BeginHorizontal();
		//GL.Label("");
		isEditing = GL.Toggle(isEditing, "Enable/Disable Editing");
		GL.EndHorizontal();

		if(isEditing != prevEditingState) ToggleEditing(isEditing);

		radius = radiusOptions[radiusIndex];
		subdiv = subdivOptions[subdivIndex];
		numLevels = numLevelsIndex + 1;
	}


	public void OnSceneGUI(SceneView sceneView){
		Event e = Event.current;
		Vector2 mousePos = EditorToWorldPoint(e.mousePosition);

		if(toBePlaced == null){
			if(e.type == EventType.MouseDown && e.button == 0){
				toBePlaced = Instantiate(AssetDatabase.LoadAssetAtPath<Atom>("Assets/Prefabs/Atom.prefab")).GetComponent<Atom>();
				toBePlaced.Init(radius*baseRadiusUnit, numLevels);
				//Debug.Log("PREVIEW");
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
			Vector2 placePoint; // <-- goal is to obtain this

			if(map.transform.childCount > 0){
				Atom closest = null;
				float closestDist = float.MaxValue;
				foreach(Transform t in map){
					Atom a = t.GetComponent<Atom>();
					float dist = Mathf.Abs(((Vector2)a.transform.position - mousePos).sqrMagnitude - a.OuterRadius*a.OuterRadius);
					if(dist < closestDist){
						closest = a;
						closestDist = dist;
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
				toBePlaced.transform.SetParent(map);
				toBePlaced = null;
				//Debug.Log("PLACED");
			}
		}

		if(e.type == EventType.MouseDown || e.type == EventType.MouseUp){
			e.Use();
		}


	}

	public static Vector2 GetSnapDir(Atom anchor, Vector2 mousePos, int subdiv){
		Vector2 anchorPos = (Vector2)anchor.transform.position;
		float mouseAngle = Util.VectorAngle(mousePos - anchorPos);
		float divAngle = 2*Mathf.PI/subdiv;
		int pNum = (int)((mouseAngle + divAngle/2)/divAngle);
		return new Vector2(Mathf.Cos(divAngle*pNum), Mathf.Sin(divAngle*pNum));
	}


	public static Vector2 EditorToWorldPoint(Vector2 rawPos){
		rawPos.y = SceneView.currentDrawingSceneView.camera.pixelHeight - rawPos.y;
		return SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(rawPos);
	}

}
