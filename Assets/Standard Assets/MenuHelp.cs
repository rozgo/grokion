using UnityEngine;
using System.Collections;

public class MenuHelp : MonoBehaviour {
	
	void Start () {
		renderer.enabled = false;
		//renderer.material.mainTexture = null;
		foreach (Transform child in transform) {
			child.gameObject.active = false;
		}
	}
	
	void Update () {
		float dist = Vector3.Distance(Camera.main.transform.position, transform.position);
		if (dist < 20) {
			if (!renderer.enabled) {
				renderer.enabled = true;
				//renderer.material.mainTexture = (Texture2D)Resources.Load(name, typeof(Texture2D));
				foreach (Transform child in transform) {
					child.gameObject.active = true;
				}
			}
		}
		else {
			if (renderer.enabled) {
				renderer.enabled = false;
				//renderer.material.mainTexture = null;
				foreach (Transform child in transform) {
					child.gameObject.active = false;
				}
			}
		}
	}

}
