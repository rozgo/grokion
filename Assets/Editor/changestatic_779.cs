using UnityEngine;
using UnityEditor;

/*
///////////////////////////////////////////////////////////////////////////////////
----------------~----------------
Script by Andrew Grant (reissgrant)
----------------~----------------
This editor script changes all selected objects to static to prepare them for lightmapping. 
1.) Select all objects you wish to change to static.
2.) Use the drop down menu marked "Change to static" to change the selected objects to static.

*modified code from texture import settings by Martin Schultz, Decane in August 2009
///////////////////////////////////////////////////////////////////////////////////
*/

public class ChangeObjectStatic : ScriptableObject {
   
    // ----------------------------------------------------------------------------
   
    [MenuItem ("Custom/Static/Make static ")]
    static void enableStatic() {
        SelectedChangeStatic(true);
    }
	
	[MenuItem ("Custom/Static/Make non-static ")]
    static void disableStatic() {
        SelectedChangeStatic(false);
    }
   
    static void SelectedChangeStatic(bool isEnabled) {
   
        Object[] selectedObjects = GetSelectedObjects();
        Selection.objects = new Object[0];
        foreach (UnityEngine.GameObject child in selectedObjects)  {
			child.isStatic = isEnabled;
        }
    }
	
	static Object[] GetSelectedObjects()
    {
        return Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
    }
	
	
}

 