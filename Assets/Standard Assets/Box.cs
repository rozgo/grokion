using UnityEngine;
using System.Collections;

public class Box : MonoBehaviour {
	
	static float lastBreak = 0;
	static float lastContact = 0;
	
	public GameObject chunksAsset;
	public AudioClip chunksClip;
	public AudioClip hitClip;
    public bool isIce = false;
	public bool sleepOnStart = true;

    Health health;
	ParticleEmitter flames;
	new Renderer renderer;
	
	void Awake () {
		renderer = gameObject.GetComponentInChildren<Renderer>();
        health = (Health)GetComponent(typeof(Health));
		if (sleepOnStart) {
			rigidbody.Sleep();
		}
	}
	
	IEnumerator Break () {
        if (flames != null) {
            flames.emit = false;
        }
		if (chunksAsset != null) {
			GameObject chunks = (GameObject)Instantiate(chunksAsset);
			chunks.transform.position = transform.position;
		}
		if ((Time.time - lastBreak) > 0.5f) {
			Game.fx.PlaySound(chunksClip);
            lastBreak = Time.time;
		}
		int orbs = Game.casual ? Random.Range(0,4) : Random.Range(0,2);
		Game.fx.ReleaseEnergy(orbs, collider.bounds.center);
        yield return 0;
        Spawner spawner = (Spawner)GetComponent(typeof(Spawner));
		if (spawner != null) {
            spawner.Spawn();
		}
        else {
            Destroy(gameObject);
        }
	}
	
	void OnExplosion (GameObject exploded) {
		if (!isIce && Vector3.Distance(transform.position, exploded.transform.position) < 3) {
            health.Kill();
		}
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
	
	void Hit (int damage) {
        health.Damage(damage);
        if (!lit) {
            StartCoroutine(Lit());
        }
	}

    bool lit = false;
    IEnumerator Lit () {
        lit = true;
        Material sharedMaterial = renderer.sharedMaterial;
        renderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        renderer.material = sharedMaterial;
        lit = false;
    }

	IEnumerator DieTask () {
		Disintegrate disintegrate = gameObject.GetComponent<Disintegrate>();
		if (disintegrate != null) {
			disintegrate.DesintegrateFX();
			yield return new WaitForSeconds(disintegrate.deathDelay);
		}
		StartCoroutine(Break());
	}

    void OnDeath () {
		StartCoroutine(DieTask());
    }
		
	void OnCollisionEnter (Collision collision) {
		if (!isIce && collision.collider.gameObject.layer == Game.projectilesLayer) {
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
