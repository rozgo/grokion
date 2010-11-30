using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class SnapToGrid : ScriptableObject
{
    [UnityEditor.MenuItem ("GameObject/Snap to Grid &g")]
    static void MenuSnapToGrid()
    {
        Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable);
       
        float gridx = 1.0f;
        float gridy = 1.0f;
        float gridz = 1.0f;
        
        float rotx = 90.0f;
        float roty = 90.0f;
        float rotz = 90.0f;
       
        foreach (Transform transform in transforms)
        {
            Vector3 newPosition = transform.position;
            newPosition.x = Mathf.Round(newPosition.x / gridx) * gridx;
            newPosition.y = Mathf.Round(newPosition.y / gridy) * gridy;
            newPosition.z = Mathf.Round(newPosition.z / gridz) * gridz;
            transform.position = newPosition;
            
            Vector3 newRotation = transform.rotation.eulerAngles;
            newRotation.x = Mathf.Round(newRotation.x / rotx) * rotx;
            newRotation.y = Mathf.Round(newRotation.y / roty) * roty;
            newRotation.z = Mathf.Round(newRotation.z / rotz) * rotz;
            transform.rotation = Quaternion.Euler(newRotation);
        }
    }
}
