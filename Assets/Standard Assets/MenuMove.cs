using UnityEngine;
using System.Collections;

public class MenuMove : MonoBehaviour {
	
	public float distance;
	public Vector3 velocity;
	public AudioClip stopClip;
	public bool moveOnStart = false;
	
	new Transform transform;
	Vector3 targetA;
	Vector3 targetB;
	bool inverted = false;
	Switch lastSwitch;
	
	void Awake () {
		transform = gameObject.transform;
	}
	
	void Start () {
		if (moveOnStart) {
			Move();
		}
		else {
			enabled = false;
		}
	}
	
	void OnSwitchDown(Switch lastSwitch) {
		if (this.lastSwitch != null && this.lastSwitch != lastSwitch) {
			inverted = !inverted;
		}
		this.lastSwitch = lastSwitch;
		Move();
	}
	
	void OnSwitchUp () {
		Stop();
	}
	
	public void Move () {
		enabled = true;
		targetA = transform.position;
		targetB = targetA + velocity.normalized * distance;
		//audio.volume = Game.fx.soundVolume;
		//audio.Play();
	}
	
	void Stop () {
		enabled = false;
		//audio.volume = Game.fx.soundVolume;
		//audio.Stop();
		//Game.fx.PlaySound(stopClip);
	}
	
	void FixedUpdate () {
		if (inverted && Vector3.Distance(targetB, transform.position) < distance) {
			transform.position = transform.position - velocity * Time.deltaTime;
		}
		else if (!inverted && Vector3.Distance(targetA, transform.position) < distance) {
			transform.position = transform.position + velocity * Time.deltaTime;
		}
		else {
			Stop();
		}
	}
}
