using UnityEngine;
using System.Collections;

public class Pivot : MonoBehaviour {
	
	public Vector3 crosshair = Vector3.right;
	
	new Transform transform;

	void Awake () {
		transform = gameObject.transform;
	}
	
	void Start () {
	}

	void Update () {
		crosshair.z = 0;
		crosshair.x -= Input.GetAxis("Mouse X");
		crosshair.y += Input.GetAxis("Mouse Y");
		if (crosshair.magnitude > 1) {
			crosshair.Normalize();
		}
		else if (crosshair.magnitude < 0.1f) {
			crosshair = crosshair.normalized / 10;
		}

		Vector3 localPosition = transform.localPosition;
		localPosition.x = Input.GetAxis("Horizontal");
		localPosition.y = Input.GetAxis("Vertical");			
		if (localPosition.sqrMagnitude > 1) {
			localPosition = localPosition.normalized;
		}
		transform.localPosition = localPosition;
	}

	void OnDrawGizmos () {
		if (Game.character != null) {
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(Game.character.transform.position + new Vector3(0,1,0) + crosshair, 0.1f);			
		}
	}
}
