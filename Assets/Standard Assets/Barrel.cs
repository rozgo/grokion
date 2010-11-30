using UnityEngine;
using System.Collections;

public class Barrel : MonoBehaviour {
    
    static float lastContact = 0;
    
    public Material fxMaterial;
    public AudioClip hitClip;
    public float autoExplode = 0;
    public bool sleepOnStart = true;
    public float fuseTimer = 2;
	
	public new Renderer renderer;
    
    Health health;  
    ParticleEmitter flames;

    void Awake () {
        health = (Health)GetComponent(typeof(Health));
		renderer = GetComponentInChildren<Renderer>();
        if (sleepOnStart) {
            rigidbody.Sleep();
        }
    }
    
    void Start () {
        if (autoExplode > 0) {
            StartCoroutine(AutoExplode());
        }
    }
    
    IEnumerator AutoExplode () {
        yield return new WaitForSeconds(autoExplode);
        health.Kill();
    }

    void OnHotspotEnter () {
        fuseTimer = 0;
        health.Kill();
    }

    void OnHotspotExit () {
        fuseTimer = 0;
        health.Kill();
    }
    
    IEnumerator Explode () {
        if (renderer != null) {
            renderer.material = fxMaterial;
        }
        yield return new WaitForSeconds(fuseTimer);
        if (flames != null) {
            flames.emit = false;
        }
        Game.fx.Explode(gameObject);
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
        fuseTimer = Vector3.Distance(collider.bounds.center, explosion.collider.bounds.center) - 1;
        fuseTimer = Mathf.Clamp(fuseTimer, 0.3f, 2);            
        if (rigidbody != null) {
            Vector3 affectPosition = collider.ClosestPointOnBounds(explosion.collider.bounds.center);
            Vector3 affectDirection = affectPosition - explosion.collider.bounds.center;
            float affectForce = (1 - Mathf.Clamp01(affectDirection.magnitude/Game.fx.explosionRadius)) * 20000;
            affectDirection.Normalize();
            if (affectDirection.sqrMagnitude == 0) {
                affectDirection = -Vector3.up;
            }
            rigidbody.AddForceAtPosition(affectDirection * affectForce, affectPosition);
        }
        health.Kill();
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
			fuseTimer = 0;
		}
		StartCoroutine(Explode());
	}

    void OnDeath () {
		if (Game.grid != null) {
			Game.grid.AddBlast();
		}
		StartCoroutine(DieTask());
    }

    IEnumerator Burn () {
        yield return new WaitForSeconds(1);
        if (flames != null) {
            flames.emit = false;
        }
    }
        
    void OnParticleCollision (GameObject particle) {
        if (particle.gameObject.layer == Game.flamesLayer) {
            if (flames == null) {
                health.Kill();
                flames = Game.fx.Burn(gameObject);
                StartCoroutine(Burn());
            }
        }
    }
    
    void OnCollisionEnter (Collision collision) {
        if (collision.collider.gameObject.layer == Game.projectilesLayer) {
            Projectile projectile = (Projectile)collision.collider.GetComponent(typeof(Projectile));
            Hit(projectile.Damage);
            lastContact = 0;
        }
        if ((Time.time - lastContact) > 0.5f && collision.relativeVelocity.sqrMagnitude > 1) {
            Game.fx.PlaySoundPitched(hitClip);
            lastContact = Time.time;
        }
    }

    void OnWaterEnter (Water water) {
        if (water.lava) {
            fuseTimer = 0;
            health.Kill();
        }
    }
    
}
