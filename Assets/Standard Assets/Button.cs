using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
    
    public string key;
    public GameObject receiver;
    public Material sharedMaterialNormal;
    public Material sharedMaterialActive;
    public bool down = false;
    
    void Awake () {
        if (receiver == null) {
            receiver = gameObject;
        }
    }
    
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR
    void Update () {
    	if (key.Length > 0) {
			if (Input.GetKeyDown(key)) {
			    down = true;
			    if (sharedMaterialActive != null) {
			        renderer.material = sharedMaterialActive;
			    }
			    receiver.SendMessage("OnButtonDown", this, SendMessageOptions.DontRequireReceiver);
			}
			else if (Input.GetKeyUp(key)) {
			    down = false;
			    if (sharedMaterialNormal != null) {
			        renderer.material = sharedMaterialNormal;
			    }
			    receiver.SendMessage("OnButtonUp", this, SendMessageOptions.DontRequireReceiver);
			}
    	}
    }
#endif
    
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
        down = true;
        if (sharedMaterialActive != null) {
            renderer.material = sharedMaterialActive;
        }
        receiver.SendMessage("OnButtonDown", this, SendMessageOptions.DontRequireReceiver);
    }

    void OnFingerMove () {
    }

    void OnFingerEnd () {
        down = false;
        if (sharedMaterialNormal != null) {
            renderer.material = sharedMaterialNormal;
        }
        receiver.SendMessage("OnButtonUp", this, SendMessageOptions.DontRequireReceiver);
    }

    void OnFingerCancel () {
        down = false;
        if (sharedMaterialNormal != null) {
            renderer.material = sharedMaterialNormal;
        }
        receiver.SendMessage("OnButtonCancel", this, SendMessageOptions.DontRequireReceiver);
    }
    
    void OnBecameVisible () {
        enabled = true;
    }
    
    void OnBecameInvisible () {
        enabled = false;
    }

}