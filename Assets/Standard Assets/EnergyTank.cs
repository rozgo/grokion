using UnityEngine;
using System.Collections;

public class EnergyTank : MonoBehaviour {
	
	new Transform transform;
	Vector3 position;
	
	void Awake () {
		enabled = false;
		transform = gameObject.transform;
		position = transform.position;
	}
	
	void Update () {
		Vector3 newPos = position;
		newPos.y += Mathf.Sin(Time.time * 10) * 0.03f;
		transform.position = newPos;
	}
	
	void OnBecameVisible () {
		enabled = true;
	}
	
	void OnBecameInvisible () {
		enabled = false;
	}
	
	void OnHotspotDone () {
		Destroy(gameObject);
	}
}
