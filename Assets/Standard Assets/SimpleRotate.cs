using UnityEngine;
using System.Collections;

public class SimpleRotate : MonoBehaviour {
	
	public Vector3 axis = Vector3.up;
	public float speed = 10;
	public bool useRealTime = false;
	
	new Transform transform;
	
	void Awake () {
		transform = gameObject.transform;
	}
	
	void LateUpdate () {
		float angle = 0;
		if (useRealTime) {
			angle = Game.realDeltaTime * speed;
		}
		else {
			angle = Time.deltaTime * speed;
		}
		transform.rotation *= Quaternion.AngleAxis(angle,axis);
	}
}

