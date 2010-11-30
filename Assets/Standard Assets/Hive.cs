using UnityEngine;
using System.Collections;

public class Hive : MonoBehaviour {
	
	static float lastContact = 0;
	static float lastBreak = 0;
	
	public AudioClip hitClip;
	public GameObject chunksAsset;
	public AudioClip chunksClip;

    bool hitReacting = false;
	ParticleEmitter flames;
    new Transform transform;
    Health health;

    void Awake () {
        transform = gameObject.transform;
        health = (Health)GetComponent(typeof(Health));
    }
	
	void Start () {
	}
	
	IEnumerator HitReact () {
        hitReacting = true;
        float timer = 0.4f;
        Vector3 initialPos = transform.position;
        Vector3 vibratePos = initialPos;
        while (timer > 0) {
            timer -= Time.deltaTime;
            vibratePos.x = initialPos.x + Mathf.Sin(Time.time * 60) * 0.05f;
            transform.position = vibratePos;
            yield return 0;
        }
        hitReacting = false;
	}

	IEnumerator Break () {
		GameObject chunks = (GameObject)Instantiate(chunksAsset);
		chunks.transform.position = transform.position;
		if ((Time.time - lastBreak) > 0.5f) {
			Game.fx.PlaySound(chunksClip);
            lastBreak = Time.time;
		}
        yield return 0;
        Spawner spawner = (Spawner)GetComponent(typeof(Spawner));
		if (spawner != null) {
            spawner.Spawn();
		}
        else {
            Destroy(gameObject);
        }
	}
	
	void OnExplosion (GameObject explosion) {
        health.Kill();
	}
	
 	void OnHotspotEnter (Hotspot hotspot) {
	}
	
	void OnHotspotExit (Hotspot hotspot) {
	}
	
	void Hit (int damage) {
        health.Damage(damage);
        if (!hitReacting) {
            StartCoroutine(HitReact());
        }
	}

    void OnDeath () {
        StartCoroutine(Break());
    }

	IEnumerator Burn () {
		yield return new WaitForSeconds(3);
        if (flames != null) {
            flames.emit = false;
        }
        health.Kill();
	}

	void OnParticleCollision (GameObject particle) {
		if (particle.gameObject.layer == Game.flamesLayer) {
			if (flames == null) {
				flames = Game.fx.Burn(gameObject);
				StartCoroutine(Burn());
			}
		}
	}
		
	void OnCollisionEnter (Collision collision) {
        if ((Time.time - lastContact) > 0.5f && collision.relativeVelocity.sqrMagnitude > 1) {
            Game.fx.PlaySoundPitched(hitClip);
            lastContact = Time.time;
        }
        if (collision.gameObject.layer == Game.projectilesLayer) {
            health.Damage(20);
            if (!hitReacting) {
                StartCoroutine(HitReact());
            }
        }
	}
		
}
