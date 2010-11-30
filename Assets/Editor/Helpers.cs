using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
 
public class Helpers : ScriptableObject {
	
    [UnityEditor.MenuItem ("GameObject/Static")]
    static void HelpersStaticObject() {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Unfiltered);
        foreach (Transform transform in transforms) {
            transform.gameObject.isStatic = true;
            foreach (Transform child in transform) {
            	child.gameObject.isStatic = true;
            }   
        }        
    }

    [UnityEditor.MenuItem ("GameObject/Unstatic")]
    static void HelpersUnstaticObject() {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.Unfiltered);
        foreach (Transform transform in transforms) {
            transform.gameObject.isStatic = false;
            foreach (Transform child in transform) {
            	child.gameObject.isStatic = false;
            }  
        }        
    }

    [UnityEditor.MenuItem ("GameObject/Unlock &u")]
    static void HelpersUnlockObject() {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel);
        foreach (Transform transform in transforms) {
            transform.gameObject.hideFlags &= ~HideFlags.NotEditable;
        }        
    }

    [UnityEditor.MenuItem ("Helpers/Generate Tokens")]
    static void HelpersGenerateTokens() {
    	Debug.Log("Generating tokens");
        Token[] tokens = (Token[])FindObjectsOfType(typeof(Token));
        for (int i=0; i<(3-tokens.Length); ++i) {
            EditorUtility.InstantiatePrefab(Resources.Load("Token", typeof(GameObject)));
        }
        tokens = (Token[])FindObjectsOfType(typeof(Token));
        for (int i=0; i<3; ++i) {
            tokens[i].gameObject.name = "Token|" + 
                System.IO.Path.GetFileNameWithoutExtension(EditorApplication.currentScene) + "|" + (i+1).ToString();
        }
    }

    [UnityEditor.MenuItem ("Helpers/Debug Spawn Door")]
    static void HelpersDebugSpawnDoor() {
        if (Selection.activeGameObject != null) {
            Door activeDoor = (Door)Selection.activeGameObject.GetComponent(typeof(Door));
            if (activeDoor == null) {
                Debug.Log("No door selected");
            }
            else {
                Door[] doors = (Door[])FindObjectsOfType(typeof(Door));
                foreach (Door door in doors) {
                    door.debugSpawn = false;
                }
                activeDoor.debugSpawn = true;
                Debug.Log("Debug spawn: " + activeDoor.name);
            }
        }
        else {
            Debug.Log("No door selected");
        }
    }

    [UnityEditor.MenuItem ("Helpers/Generate MapTrace.txt")]
    static void HelpersTraceMap() {
    	Debug.Log("Tracing map");
        TextWriter mapTrace = new StreamWriter(Application.dataPath + "/Tests/MapTrace.txt");
        string mapPath = Application.dataPath + "/Map";
        string[] roomPaths = Directory.GetFiles(mapPath, "*.unity");
        foreach (string roomPath in roomPaths) {
            string roomName = System.IO.Path.GetFileNameWithoutExtension(roomPath);
            Debug.Log("Loading scene: " + roomName);
            mapTrace.WriteLine("[" + roomName + "]");
            string scenePath = "Assets/Map/" + roomName + ".unity";
            EditorApplication.OpenScene(scenePath);
            Debug.Log(Selection.objects.Length);
            foreach (Door door in FindObjectsOfType(typeof(Door))) {
                Debug.Log(door.name + ">" + door.nextDoor);
                mapTrace.WriteLine(door.name + ">" + door.nextDoor);
            }
            foreach (EnergyTank e in FindObjectsOfType(typeof(EnergyTank))) {
                Debug.Log(e.name);
                mapTrace.WriteLine(e.name);
            }
        }
        mapTrace.Close();
        Debug.Log("Map tracing done");
    }

    [UnityEditor.MenuItem ("Helpers/Debug MapTrace.txt")]
    static void HelpersDebugMap() {
        ArrayList doors = new ArrayList();
        TextReader mapTrace = new StreamReader(Application.dataPath + "/Tests/MapTrace.txt");
        string line;
        string room = "";
        TextWriter doorTest = new StreamWriter(Application.dataPath + "/Tests/MapTraceDebug.txt");
        while ((line = mapTrace.ReadLine()) != null) {
            if (line[0] == '[') {
                room = line.Substring(1, line.Length - 2);
            }
            else if (line.StartsWith("Door|")) {
                string[] doorsInfo = line.Split('>');
                doors.Add(line);
                string[] thisDoorInfo = doorsInfo[0].Split('|');
                if (thisDoorInfo.Length != 3){
                    doorTest.WriteLine("Room " + room + " has illegal door " + doorsInfo[0]);
                }
                else if (thisDoorInfo.Length == 3 && !thisDoorInfo[1].Equals(room)) {
                    doorTest.WriteLine("Room " + room + " has illegal door " + doorsInfo[0]);
                }
            }
        }
        foreach (string doorA in doors) {
            string[] doorAInfo = doorA.Split('>');
            bool found = false;
            foreach (string doorB in doors) {
                string[] doorBInfo = doorB.Split('>');
                if (doorAInfo[1] == doorBInfo[0]) {
                    found = true;
                    if (doorBInfo[1] != doorAInfo[0]) {
                        doorTest.WriteLine(doorBInfo[0] + " failed to link back to " + doorAInfo[0]);
                        Debug.Log(doorBInfo[0] + " failed to link back to " + doorAInfo[0]);
                    }
                    break;
                }
            }
            if (!found) {
                doorTest.WriteLine(doorAInfo[0] + " can't find " + doorAInfo[1]);
                Debug.Log(doorAInfo[0] + " can't find " + doorAInfo[1]);
            }
        }
        doorTest.Close();
        mapTrace.Close();
    }
    
#if UNITY_3_0
    static Stack allObjects = null;
    static Stack allScenes = null;
    
    static void BreakPrefab () {
    	
    	if (allObjects == null) {
    		Debug.Log("Break prefabs: START");
    		EditorApplication.update += BreakPrefab;
    		allObjects = new Stack();
    		foreach (GameObject obj in FindObjectsOfType(typeof(GameObject))) {
    			allObjects.Push(obj);
    		}
    	}
    	else if (allObjects.Count > 0) {
    		GameObject obj = (GameObject)allObjects.Pop();
    		GameObject root = EditorUtility.FindPrefabRoot(obj);
    		if (EditorUtility.GetPrefabType(obj) == PrefabType.PrefabInstance && root == obj) {
    			root.AddComponent(typeof(BrokenPrefab));
    			Debug.Log(root.name);
    		}
    	}
    	else {
    		allObjects = null;
	    	foreach (GameObject obj in FindObjectsOfType(typeof(GameObject))) {
	    		DestroyImmediate(obj.GetComponent(typeof(BrokenPrefab)));
	       	}
	       	EditorApplication.update -= BreakPrefab;
	       	Debug.Log("Break prefabs: END");
	       	
	       	if (allScenes != null && allScenes.Count > 0) {
	       		EditorApplication.SaveScene(EditorApplication.currentScene);
	       		EditorApplication.OpenScene((string)allScenes.Pop());
	       		BreakPrefab();
	       	}
    	}
	    	
    }
    
    [UnityEditor.MenuItem ("Helpers/Break Prefabs")]
    static void HelpersBreakPrefabs() {
    	
    	BreakPrefab();
    	
    }
    
    [UnityEditor.MenuItem ("Helpers/Break Prefabs all Scenes")]
    static void HelpersBreakPrefabsScenes() {
    	
        string mapPath = Application.dataPath + "/Map";
        string[] roomPaths = Directory.GetFiles(mapPath, "*.unity");
        allScenes = new Stack();
        
        foreach (string roomPath in roomPaths) {
        	
            string roomName = System.IO.Path.GetFileNameWithoutExtension(roomPath);
            Debug.Log("Loading scene: " + roomName);
            string scenePath = "Assets/Map/" + roomName + ".unity";
            allScenes.Push(scenePath);
    	
        }
        
        BreakPrefab();
    	
    }
#endif

}
