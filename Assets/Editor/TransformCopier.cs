using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class TransformCopier : ScriptableObject
{
        private static Vector3 position;
        private static Quaternion rotation;
        private static Vector3 scale;
        //private static string myName; 
    
    [MenuItem ("Custom/Transform Copier/Copy Transform")]
    static void DoRecord()
    {
       position = Selection.activeTransform.localPosition;
       rotation = Selection.activeTransform.localRotation;
       scale = Selection.activeTransform.localScale;
       //myName = Selection.activeTransform.name;       
        
       //EditorUtility.DisplayDialog("Transform Copy", "Local position, rotation, & scale of "+myName +" copied relative to parent.", "OK", "");
    }
 
    [MenuItem ("Custom/Transform Copier/Paste Transform")]
    static void DoApply()
    {
        Selection.activeTransform.localPosition = position;
        Selection.activeTransform.localRotation = rotation;
        Selection.activeTransform.localScale = scale;      
        
        //EditorUtility.DisplayDialog("Transform Paste", "Local position, rotation, and scale of "+myName +"  pasted relative to parent of "+Selection.activeTransform.name+".", "OK", "");
    }
}
