using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour {

	public string lockKey;
	public AudioClip gateClip;
	
	void Awake () {
		if (lockKey.Length > 0 && PlayerPrefs.HasKey(lockKey)) {
			gameObject.SetActiveRecursively(false);
		}
	}
	
	void OnSwitch () {
		Game.fx.PlaySound(gateClip);
		animation.Play();
	}

}
