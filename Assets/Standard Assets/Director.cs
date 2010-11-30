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
	
//#if UNITY_IPHONE
	void FixedUpdate() {
		
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft) {
			
			if (iPhoneSettings.screenOrientation != iPhoneScreenOrientation.LandscapeLeft) { 
				iPhoneSettings.screenOrientation = iPhoneScreenOrientation.LandscapeLeft;
			}
			
			camera.rect = new Rect(0,0,1,1);
			if (iPhoneSettings.model == "iPad") {
				camera.fov = 80;
			}
			else {
				camera.fov = 60;
			}
		}
		if (Input.deviceOrientation == DeviceOrientation.LandscapeRight) {
			
			if (iPhoneSettings.screenOrientation != iPhoneScreenOrientation.LandscapeRight) { 
				iPhoneSettings.screenOrientation = iPhoneScreenOrientation.LandscapeRight;
			}
			
			camera.rect = new Rect(0,0,1,1);
			if (iPhoneSettings.model == "iPad") {
				camera.fov = 80;
			}
			else {
				camera.fov = 60;
			}
		}
		
		if (iPhoneSettings.model == "iPad") {
			
			if (Input.deviceOrientation == DeviceOrientation.Portrait) {
				
				if (iPhoneSettings.screenOrientation != iPhoneScreenOrientation.Portrait) { 
					iPhoneSettings.screenOrientation = iPhoneScreenOrientation.Portrait;
				}
				
				camera.rect = new Rect(0,0.5f,1,0.5f);
				camera.fov = 80;
			}
			if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) {
				
				if (iPhoneSettings.screenOrientation != iPhoneScreenOrientation.PortraitUpsideDown) { 
					iPhoneSettings.screenOrientation = iPhoneScreenOrientation.PortraitUpsideDown;
				}
				
				camera.rect = new Rect(0,0.5f,1,0.5f);
				camera.fov = 80;
			}
		}
	}
//#endif
}
