using UnityEngine;
using System.Collections;

public class Director : MonoBehaviour {
	
	public static float zoom = 0;
	public static float minZoom = -3;
	public static float maxZoom = 6;
	
	float zoomAccel = 0;
	
	void Awake () {
		if (FindObjectsOfType(GetType()).Length > 1) {
			Debug.LogError("Multiple singletons of type: "+GetType());
		}
	}
	
	void Update () {
		if (Input.GetKey(KeyCode.UpArrow)) {
			zoomAccel -= Time.deltaTime * 30;
		}
		else if (Input.GetKey(KeyCode.DownArrow)) {
			zoomAccel += Time.deltaTime * 30;
		}
		zoom += zoomAccel * Time.deltaTime;
		zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
		zoomAccel *= (1 - Mathf.Clamp(Time.deltaTime*10, 0.0f, 1.0f));
	}
	
}
