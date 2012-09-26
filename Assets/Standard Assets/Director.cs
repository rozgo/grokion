using UnityEngine;
using System.Collections;

public class Director : MonoBehaviour {
	
	public static float zoom = 0;
	public static float minZoom = -2;
	public static float maxZoom = 4;
	
	float zoomAccel = 0;

//	MonoBehaviour bloom;
//	MonoBehaviour color;

	bool bloomEnabled;
	bool colorEnabled;
	
	void Awake () {
		if (FindObjectsOfType(GetType()).Length > 1) {
			Debug.LogError("Multiple singletons of type: "+GetType());
		}

//		bloom = (MonoBehaviour)GetComponent("BloomAndFlares");
//		bloomEnabled = bloom.enabled;
//		color = (MonoBehaviour)GetComponent("ColorCorrectionEffect");
//		colorEnabled = color.enabled;
	}
	
	void Update () {
		InputProxy input = InputProxy.Get();
		float zoomValue = input.GetValue("Zoom");
		if (zoomValue > 0) {
			zoomAccel -= Time.deltaTime * 30;
		}
		else if (zoomValue < 0) {
			zoomAccel += Time.deltaTime * 30;
		}
		zoom += zoomAccel * Time.deltaTime;
		zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
		zoomAccel *= (1 - Mathf.Clamp(Time.deltaTime*10, 0.0f, 1.0f));
	}

	void OnEnable () {
		ControlSettings.Setup();
		if (ControlSettings.visualEffects) {
//			if (bloomEnabled) {
//				bloom.enabled = true;
//			}
//			if (colorEnabled) {
//				color.enabled = true;
//			}
		}
		else {
//			if (bloomEnabled) {
//				bloom.enabled = false;
//			}
//			if (colorEnabled) {
//				color.enabled = false;
//			}
		}
		//gameObject.SendMessage("EnableFX", ControlSettings.visualEffects, SendMessageOptions.DontRequireReceiver);
	}
	
}
