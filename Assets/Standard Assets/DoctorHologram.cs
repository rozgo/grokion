using UnityEngine;
using System.Collections;

public class DoctorHologram : MonoBehaviour {
	
	public GameObject doctor;
	
	void Awake () {
		doctor.SetActiveRecursively(false);
	}
	
	void Start () {
	}
	
	void OnHotspotEnter (Hotspot hotspot) {
		doctor.SetActiveRecursively(true);
		if (Game.character != null) {
			Vector3 lookAt = Game.character.transform.position - transform.position;
			lookAt.y = 0;
			transform.forward = lookAt;
		}
	}
	
	void OnHotspotDone (Hotspot hotspot) {
		doctor.SetActiveRecursively(false);
	}
	
	void OnHotspotExit (Hotspot hotspot) {
		doctor.SetActiveRecursively(false);
	}
	
}
