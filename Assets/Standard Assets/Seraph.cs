using UnityEngine;
using System.Collections;

public class Seraph : StateMachine {
	
	public AudioClip chargeClip;
	public AudioClip flyingClip;
	
	new Transform transform;
	new AudioSource audio;
	Vector3 velocity = Vector3.zero;
	
	class Fly : State {

		Seraph seraph;
		Vector3 target = Vector3.zero;
		Vector3 lookAt = Vector3.zero;
		float timer;
		
		public Fly (Seraph seraph) {
			this.seraph = seraph;
		}
		
		public override void OnEnter () {
			timer = 0;
			target = seraph.transform.position;
			seraph.animation["IDLE"].wrapMode = WrapMode.Loop;
			seraph.animation.CrossFade("IDLE");
		}
		
		public override void OnUpdate () {
			timer += Time.deltaTime;
			if (Game.character != null) {
				target = Game.character.collider.bounds.center;
				lookAt = target;
				target += new Vector3(0, 2, 0);
				target.z = -10;
			}
			Vector3 lookDir = lookAt - seraph.transform.position;
			float dist = Vector3.Distance(seraph.transform.position, target);
			lookDir.Normalize();
			if (lookDir.sqrMagnitude == 0) {
				lookDir = Vector3.forward;
			}
			seraph.transform.rotation = 
				Quaternion.Slerp(seraph.transform.rotation, 
				Quaternion.LookRotation(lookDir) * Quaternion.Euler(0, 90, 0),
				Time.deltaTime * 2);
			if (dist > 20) {
				seraph.velocity = (target - seraph.transform.position).normalized * 10;
			}
			else if (dist > 4) {
				seraph.velocity += (target - seraph.transform.position).normalized * 10 * Time.deltaTime;
			}
			else {
				seraph.velocity -= seraph.velocity * Time.deltaTime * 2;
			}
			//seraph.transform.position = Vector3.Lerp(seraph.transform.position, target, Time.deltaTime * 0.5f);
			seraph.transform.position += seraph.velocity * Time.deltaTime;
			if (dist < 4 && Game.character != null && timer > 2) {
				seraph.Change(new Charge(seraph));
			}
		}
	}
	
	class Charge : State {
		
		Seraph seraph;
		Vector3 target = Vector3.zero;
		
		public Charge (Seraph seraph) {
			this.seraph = seraph;
		}
		
		public override void OnEnter () {
			seraph.audio.PlayOneShot(seraph.chargeClip);
			if (Game.character != null) {
				target = Game.character.transform.position - seraph.transform.position;
				target = target.normalized * 100;
			}
			else {
				target = seraph.transform.position;
				target.z = 10;
			}
			seraph.animation["CHARGE"].wrapMode = WrapMode.Once;
			seraph.animation.CrossFade("CHARGE");
		}
		
		public override void OnUpdate () {
			Vector3 lookAt = target;
			Vector3 lookDir = lookAt - seraph.transform.position;
			lookDir.Normalize();
			if (lookDir.sqrMagnitude == 0) {
				lookDir = Vector3.forward;
			}
			seraph.transform.rotation = 
				Quaternion.Slerp(seraph.transform.rotation, 
				Quaternion.LookRotation(lookDir) * Quaternion.Euler(0, 90, 0),
				Time.deltaTime * 2);
			seraph.velocity = 
				Vector3.Lerp(seraph.velocity, lookDir * 20, Time.deltaTime * 3);
			seraph.transform.position += seraph.velocity * Time.deltaTime;
			if (seraph.transform.position.z > 10) {
				if (Game.character != null) {
					Vector3 position = seraph.transform.position;
					if (position.x > Game.character.transform.position.x) {
						position.x = Game.character.transform.position.x + 40;
					}
					else {
						position.x = Game.character.transform.position.x - 40;
					}
					position.z = -10;
					seraph.transform.position = position;
				}
				seraph.velocity = Vector3.zero;
				seraph.Change(new Fly(seraph));
			}
			Collider[] hits = Physics.OverlapSphere(seraph.transform.position, 2);
			for (int i=0; i<hits.Length; ++i) {
				hits[i].SendMessage("OnExplosion", seraph.gameObject, SendMessageOptions.DontRequireReceiver);
				hits[i].SendMessage("Hit", 10, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void Awake () {
		transform = gameObject.transform;
		audio = gameObject.audio;
	}

	void Start () {
		audio.volume = Game.fx.soundVolume;
		Change(new Fly(this));
	}
	
	void Update () {
		state.OnUpdate();

	}
}
