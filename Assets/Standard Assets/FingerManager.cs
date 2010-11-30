using UnityEngine;
using System.Collections;

public class FingerManager : MonoBehaviour {

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_EDITOR

    public Vector3 GetTouchPosition (Collider collider) {
        return Vector3.zero;
    }
	
#elif UNITY_IPHONE

	Collider[] colliders = new Collider[5];

    int hudLayerMask;
    float distance = 100;
    new Camera camera;

    void Awake () {
        if (FindObjectsOfType(GetType()).Length > 1) {
            Debug.LogError("Multiple singletons of type: "+GetType());
        }
        camera = (Camera)GetComponent(typeof(Camera));
        hudLayerMask = 1 << LayerMask.NameToLayer("Hud");
        hudLayerMask |= 1 << LayerMask.NameToLayer("Menu");
        hudLayerMask |= 1 << LayerMask.NameToLayer("Control");
    }
    
    public Vector3 GetTouchPosition (Collider collider) {
        for (int t=0; t<Input.touchCount; ++t) {
            Touch evt = Input.GetTouch(t);
            if (collider == colliders[evt.fingerId]) {
                return evt.position;
            }
        }
        return Vector3.zero;
    }
    
    void Update () {
        for (int t=0; t<Input.touchCount; ++t) {	
            Touch evt = Input.GetTouch(t);
            if (evt.fingerId < 0 || evt.fingerId >= colliders.Length) {
            	continue;
            }
            if (evt.phase==TouchPhase.Began) {
                colliders[evt.fingerId] = null;
                Ray ray = camera.ScreenPointToRay(evt.position);
                RaycastHit hit;
                if (Physics.Raycast(ray.origin, ray.direction, out hit, distance, hudLayerMask)) {
                    colliders[evt.fingerId] = hit.collider;
                    hit.collider.SendMessage("OnFingerBegin",null,SendMessageOptions.DontRequireReceiver);
                }
            }
            else if (evt.phase==TouchPhase.Moved) {
                if (colliders[evt.fingerId] == null) {
                	Ray ray = camera.ScreenPointToRay(evt.position);
	                RaycastHit hit;
	                if (Physics.Raycast(ray.origin, ray.direction, out hit, distance, hudLayerMask)) {
	                    colliders[evt.fingerId] = hit.collider;
	                    hit.collider.SendMessage("OnFingerBegin",null,SendMessageOptions.DontRequireReceiver);
	                }
                }
                else {
                    colliders[evt.fingerId].SendMessage("OnFingerMove",null,SendMessageOptions.DontRequireReceiver);
                }
            }
            else if (evt.phase==TouchPhase.Ended || evt.phase==TouchPhase.Canceled) {
                if (colliders[evt.fingerId] != null) {
                    Ray ray = camera.ScreenPointToRay(evt.position);
                    RaycastHit hit;
                    if (colliders[evt.fingerId].Raycast(ray, out hit, distance)) {
                        colliders[evt.fingerId].SendMessage("OnFingerEnd",null,SendMessageOptions.DontRequireReceiver);
                    }
                    else {
                        colliders[evt.fingerId].SendMessage("OnFingerCancel",null,SendMessageOptions.DontRequireReceiver);
                    }
                }
            }
        }
    }

#endif

}
