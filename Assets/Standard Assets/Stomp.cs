using UnityEngine;
using System.Collections;

public class Stomp : MonoBehaviour {

    public int damage = 200;
    public int stompCount = -1;
    public float stompRate = 4;
    public float stompDuration = 2;
    public float initialDelay = 0;
    public float stompSpeed = 10;
   	public float stompDistance = 3.8f;
    public AudioClip moveClip;
    public AudioClip doneClip;
	
	new Renderer renderer;
	new Transform transform;
	Vector3 startPos;
	Vector3 endPos;
	
	void Awake () {
		transform = gameObject.transform;
		renderer = GetComponentInChildren<Renderer>();
		startPos = transform.position;
		endPos = startPos + transform.up * stompDistance;
	}
	
	void Start () {
		StartCoroutine(DoStomp());
	}

	IEnumerator DoStomp () {
		float dist;
		yield return new WaitForSeconds(initialDelay);
		while (stompCount != 0) {
			if (stompCount > 0) {
				--stompCount;
			}
			audio.volume = Game.fx.soundVolume;
			audio.clip = moveClip;
			audio.Play();
			dist = stompDistance;
			while (dist > 0) {
				float speed = Time.deltaTime * stompSpeed;
				dist -= speed;
				rigidbody.MovePosition(transform.position + transform.up * speed);
				yield return new WaitForFixedUpdate();
			}
			transform.position = endPos;
			audio.volume = Game.fx.soundVolume;
			audio.clip = doneClip;
			audio.Play();
			yield return new WaitForSeconds(stompDuration);
			audio.volume = Game.fx.soundVolume;
			audio.clip = moveClip;
			audio.Play();
			dist = stompDistance;
			while (dist > 0) {
				float speed = Time.deltaTime * stompSpeed;
				dist -= speed;
				rigidbody.MovePosition(transform.position - transform.up * speed);
				yield return new WaitForFixedUpdate();
			}
			transform.position = startPos;
			audio.volume = Game.fx.soundVolume;
			audio.clip = doneClip;
			audio.Play();
			yield return new WaitForSeconds(stompRate);
		}
	}
	
    bool lit = false;
    IEnumerator Lit () {
        lit = true;
        Material sharedMaterial = renderer.sharedMaterial;
        renderer.material.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        renderer.material = sharedMaterial;
        lit = false;
    }
		
	void OnTriggerEnter (Collider collider) {
		collider.SendMessage("Hit", damage, SendMessageOptions.DontRequireReceiver);
		if (!lit && renderer != null) {
			StartCoroutine(Lit());
		}
	}
	
	void OnCollisionEnter (Collision collision) {
	}
}
