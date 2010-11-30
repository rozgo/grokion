using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	
	public Vector3 axis = Vector3.up;
	public float speed = 10;
	public float angle = 0;
	public bool useRealTime = false;
	
	new Transform transform;
	Quaternion originalRotation;
	
	void Awake () {
		transform = gameObject.transform;
		originalRotation = transform.rotation;
	}

	void Start () {
	}
	
	void OnBecameVisible () {
		enabled = true;
	}
	
	void OnBecameInvisible () {
		enabled = false;
	}
	
	void Update () {
		if (useRealTime) {
			angle += Game.realDeltaTime * speed;
		}
		else {
			angle += Time.deltaTime * speed;
		}
		transform.rotation = originalRotation * Quaternion.AngleAxis(angle,axis);
	}
}

