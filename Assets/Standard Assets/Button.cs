using UnityEngine;
using System.Collections;

public class Button : MonoBehaviour {
    
    public string key;
	public string button;
	public string proxy;
    public GameObject receiver;
    public Material sharedMaterialNormal;
    public Material sharedMaterialActive;
    public bool down = false;

	float lastValue = 0;
    
    void Awake () {
        if (receiver == null) {
            receiver = gameObject;
        }
    }
    
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
    	if (button.Length > 0) {
			if (Input.GetButtonDown(button)) {
			    down = true;
			    if (sharedMaterialActive != null) {
			        renderer.material = sharedMaterialActive;
			    }
			    receiver.SendMessage("OnButtonDown", this, SendMessageOptions.DontRequireReceiver);
			}
			else if (Input.GetButtonUp(button)) {
			    down = false;
			    if (sharedMaterialNormal != null) {
			        renderer.material = sharedMaterialNormal;
			    }
			    receiver.SendMessage("OnButtonUp", this, SendMessageOptions.DontRequireReceiver);
			}
    	}
		if (proxy.Length > 0) {
			InputProxy input = InputProxy.Get();
			float value = input.GetValue(proxy);
			if (value > 0 && lastValue == 0) {
			    down = true;
			    if (sharedMaterialActive != null) {
			        renderer.material = sharedMaterialActive;
			    }
			    receiver.SendMessage("OnButtonDown", this, SendMessageOptions.DontRequireReceiver);
			}
			else if (value == 0 && lastValue > 0) {
			    down = false;
			    if (sharedMaterialNormal != null) {
			        renderer.material = sharedMaterialNormal;
			    }
			    receiver.SendMessage("OnButtonUp", this, SendMessageOptions.DontRequireReceiver);				
			}
			lastValue = value;
		}
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
