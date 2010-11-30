using UnityEngine;
using System.Collections;

public class CtrlCover : MonoBehaviour {
    
    public bool off = false;
    
    Color offColor;
    Color selectedColor;
    Color unselectedColor;
    
    string colorName = "_Color";
    
    void Awake () {
        offColor = Color.white;
        selectedColor = Color.clear;
        unselectedColor = Color.black;
        unselectedColor.a = 0.7f;
        renderer.material.SetColor(colorName, offColor);
    }

    void OnMouseDown () {
        OnFingerBegin();
    }

    void OnMouseExit () {
        //OnFingerCancel();
    }

    void OnMouseUp () {
        OnFingerEnd();
    }
    
    void OnFingerBegin () {
        if (!off) {
            renderer.enabled = false;
        }
    }
    
    void OnFingerEnd () {
        if (!off) {
            SendMessageUpwards("OnCoverClicked", this, SendMessageOptions.DontRequireReceiver);
            renderer.enabled = true;
        }
    }

    void OnFingerCancel () {
        if (!off) {
            renderer.enabled = true;
        }
    }
    
    public void Off () {
        off = true;
        renderer.material.SetColor(colorName, offColor);
    }
    
    public void Unselect () {
        renderer.material.SetColor(colorName, unselectedColor);
    }
    
    public void Select () {
        renderer.material.SetColor(colorName, selectedColor);
    }

}
