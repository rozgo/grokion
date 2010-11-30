using UnityEngine;
using System.Collections;

public class KinematicMove : MonoBehaviour {
	
	public string lockKey;
	public float distance;
	public Vector3 velocity;
	public AudioClip stopClip;
	public bool moveOnStart = false;
	
	new Transform transform;
	Vector3 originalPosition;
	
	void Awake () {
		transform = gameObject.transform;
		originalPosition = transform.position;
		if (lockKey.Length > 0 && PlayerPrefs.HasKey(lockKey)) {
			gameObject.SetActiveRecursively(false);
		}
	}
	
	void Start () {
		if (moveOnStart) {
			Move();
		}
		else {
			enabled = false;
		}
	}
	
	void OnSwitchDown() {
		OnSwitch();
	}
	
	void OnSwitchUp () {
		OnSwitch();
	}
	
	void OnHotspotEnter () {
		if (enabled) {
			Stop();
		}
		else {
			Move();
		}
	}
	
	void OnSwitch () {
		if (enabled) {
			Stop();
		}
		else {
			Move();
		}
	}
	
	void Move () {
		enabled = true;
		audio.volume = Game.fx.soundVolume;
		audio.Play();
	}
	
	void Stop () {
		enabled = false;
		audio.volume = Game.fx.soundVolume;
		audio.Stop();
		Game.fx.PlaySound(stopClip);
	}
	
	void FixedUpdate () {
		if (Vector3.Distance(originalPosition, transform.position) < distance) {
			rigidbody.MovePosition(transform.position + velocity * Time.deltaTime);
		}
		else {
			Stop();
		}
	}
}
