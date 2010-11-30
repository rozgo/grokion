using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {
	
	public float speed = 2;
	public Transform[] targets;
	public int targetIndex = 0;
	
	new Rigidbody rigidbody;
	new Transform transform;

	void Awake () {
		transform = gameObject.transform;
		rigidbody = gameObject.rigidbody;
	}

    void Start () {
		float minDistance = 10000;
		targetIndex = 0;
		for (int i=0; i<targets.Length; ++i) {
			float distance = Vector3.Distance(targets[i].position, transform.position);
			if (distance < minDistance) {
				targetIndex = i;
				minDistance = distance;
			}
		}
    }
    
	void OnSwitchDown() {
		OnSwitch();
	}
	
	void OnSwitchUp () {
		OnSwitch();
	}
	
	void OnSwitch () {
		if (enabled) {
			rigidbody.isKinematic = true;
			enabled = false;
		}
		else {
			rigidbody.isKinematic = false;
			enabled = true;
		}
	}
	
	void OnHotspotEnter () {
		if (enabled) {
			rigidbody.isKinematic = true;
			enabled = false;
		}
		else {
			rigidbody.isKinematic = false;
			enabled = true;
		}
	}
	
	void FixedUpdate () {
		if (speed != 0 && targets.Length > 0) {
			Vector3 to = targets[targetIndex].position - transform.position;
			if (to.magnitude < 0.5f) {
				++targetIndex;
				if (targetIndex >= targets.Length) {
					targetIndex = 0;
				}
			}
			rigidbody.velocity = to.normalized * speed;
		}
	}
}
