using UnityEngine;
using System.Collections;

public class Gear : MonoBehaviour {

	public HingeJoint joint;
	
	void Start () {
	
	}
	
	void OnSwitchDown() {
		OnSwitch();
	}
	
	void OnSwitchUp () {
		OnSwitch();
	}
	
	void OnSwitch () {
		if (joint.useMotor) {
			joint.useMotor = false;
			rigidbody.angularVelocity = Vector3.zero;
		}
		else {
			joint.useMotor = true;
		}
	}

	void OnExplosion (GameObject explosion) {
		Vector3 affectPosition = collider.ClosestPointOnBounds(explosion.collider.bounds.center);
		Vector3 affectDirection = affectPosition - explosion.collider.bounds.center;
		float affectForce = (1 - Mathf.Clamp01(affectDirection.magnitude/Game.fx.explosionRadius)) * 20000;
		affectDirection.Normalize();
		if (affectDirection.sqrMagnitude == 0) {
			affectDirection = -Vector3.up;
		}
		rigidbody.AddForceAtPosition(affectDirection * affectForce, affectPosition);
	}

}
