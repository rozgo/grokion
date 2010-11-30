using UnityEngine;
using System.Collections;

public class Avalanche : MonoBehaviour {
	
	static float lastContact = 0;
	static float lastBreak = 0;
	
	public Material litMaterial;
	public AudioClip hitClip;
	public float autoPrecipitate = 0;
	public GameObject chunksAsset;
	public AudioClip chunksClip;
    public int damage = 100;
    public GameObject energyAsset;

    bool attached = true;
	bool precipitating = false;
	ParticleEmitter flames;
    new Rigidbody rigidbody;
    new Transform transform;
    Health health;

    void Awake () {
        transform = gameObject.transform;
        rigidbody = gameObject.rigidbody;
        health = (Health)GetComponent(typeof(Health));
    }
	
	void Start () {
		if (autoPrecipitate > 0) {
			StartCoroutine(AutoPrecipitate());
		}
	}
	
	IEnumerator AutoPrecipitate () {
		yield return new WaitForSeconds(autoPrecipitate);
		if (attached) {
			StartCoroutine(Precipitate());
		}
	}
	
	IEnumerator Precipitate () {
        attached = false;
        float timer = 1;
        Vector3 initialPos = transform.position;
        Vector3 vibratePos = initialPos;
        while (timer > 0) {
            timer -= Time.deltaTime;
            vibratePos.x = initialPos.x + Mathf.Sin(Time.time * 30) * 0.05f;
            transform.position = vibratePos;
            yield return 0;
        }
		if (flames != null) {
			flames.transform.parent = null;
			flames.emit = false;
		}
		precipitating = true;
        if (rigidbody != null) {
            rigidbody.isKinematic = false;
            rigidbody.velocity = new Vector3(0, -1, 0);
        }
        while (rigidbody != null) {
            if (rigidbody.velocity.y < -1.0f) {
                precipitating = true;
            }
            else {
                precipitating = false;
            }
            yield return 0;
        }
	}

	IEnumerator Break () {
		GameObject chunks = (GameObject)Instantiate(chunksAsset);
		chunks.transform.position = transform.position;
		if ((Time.time - lastBreak) > 0.5f) {
			Game.fx.PlaySound(chunksClip);
            lastBreak = Time.time;
		}
        yield return 0;
       	if (energyAsset != null) {
			GameObject energyObject = (GameObject)Instantiate(energyAsset);
			Vector3 center = collider.bounds.center;
			center.z = 0;
			energyObject.transform.position = center;
       	}
        Spawner spawner = (Spawner)GetComponent(typeof(Spawner));
		if (spawner != null) {
            spawner.Spawn();
		}
        else {
            Destroy(gameObject);
        }
	}
	
	void OnExplosion (GameObject explosion) {
		if (attached) {
			StartCoroutine(Precipitate());
		}
        else {
            health.Kill();
        }
	}
	
	void OnHotspotEnter (Hotspot hotspot) {
		if (attached) {
			StartCoroutine(Precipitate());
		}
	}
	
	void OnHotspotExit (Hotspot hotspot) {
		if (attached) {
			StartCoroutine(Precipitate());
		}
	}
	
	void Hit (int damage) {
        if (!lit) {
            StartCoroutine(Lit());
        }
		if (attached) {
			StartCoroutine(Precipitate());
		}
		else {
            health.Damage(damage);
		}
	}

    bool lit = false;
    IEnumerator Lit () {
        lit = true;
        Material sharedMaterial = renderer.sharedMaterial;
        renderer.material = litMaterial;
        yield return new WaitForSeconds(0.1f);
        renderer.material = sharedMaterial;
        lit = false;
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
            if (attached) {
                StartCoroutine(Precipitate());
            }
		}
	}
		
	void OnCollisionEnter (Collision collision) {
		if (precipitating) {
            if (collision.contacts[0].normal.y > 0) {
                collision.collider.SendMessage("Hit", damage, SendMessageOptions.DontRequireReceiver);
            }
		}
        else if (attached) {
            StartCoroutine(Precipitate());
        }
        if (collision.gameObject.layer == Game.projectilesLayer) {
            Projectile projectile = (Projectile)collision.collider.GetComponent(typeof(Projectile));
            Hit(projectile.Damage);
            lastContact = 0;
        }
        if ((Time.time - lastContact) > 0.5f && collision.relativeVelocity.sqrMagnitude > 1) {
            Game.fx.PlaySoundPitched(hitClip);
            lastContact = Time.time;
        }
	}
		
}
