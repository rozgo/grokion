using UnityEngine;
using System.Collections;

public class BallTurret : MonoBehaviour {

    static float lastContact = 0;
    
	public string species = "Ball Turret";
    public bool saveState = false;
    public string unlockKey;
    public string lockKey;
    public AudioClip turretClip;
    public float shootingSpeed = 0.2f;
    public float aimingSpeed = 1.0f;
    public AudioClip hitClip;
    public Transform pivot;
    public ParticleEmitter electricity;

    new Transform transform;
    bool shooting = false;
    ParticleEmitter flames;
    bool hit = false;
    Health health;

    void Awake () {
        health = (Health)GetComponent(typeof(Health));
        transform = gameObject.transform;
        electricity.emit = false;
        if (unlockKey.Length > 0 && !PlayerPrefs.HasKey(unlockKey)) {
			gameObject.SetActiveRecursively(false);
		}
		if (lockKey.Length > 0 && PlayerPrefs.HasKey(lockKey)) {
			gameObject.SetActiveRecursively(false);
		}
		if (saveState && PlayerPrefs.HasKey(name)) {
			gameObject.SetActiveRecursively(false);
		}
    }
    
    IEnumerator Shoot (Vector3 position, Vector3 velocity, float life) {
        shooting = true;
        if (!hit) {
            Projectile projectile = Projectile.Launch(position, velocity, life);
            if (projectile != null) {
                Game.fx.PlaySound(turretClip);
                Physics.IgnoreCollision(projectile.collider, collider);
            }
        }
        yield return new WaitForSeconds(shootingSpeed);
        shooting = false;
    }
    
    void FixedUpdate () {
        if (Game.character != null) {
            if (Vector3.Distance(Game.character.Head.position, transform.position) < 8) {
                Vector3 lookAtPoint = Game.character.Head.position;
                Vector3 forward = (lookAtPoint - transform.position).normalized;
                float angle = Vector3.Angle(-transform.forward, pivot.forward);
                Vector3 slerpedForward = Vector3.Slerp(pivot.forward, forward, Time.deltaTime * aimingSpeed);
                float slerpedAngle = Vector3.Angle(-transform.forward, slerpedForward);
                if (angle < 80 || slerpedAngle < angle) {
                    pivot.forward = slerpedForward;
                }
                if (!shooting) {
                    if (angle < 80) { 
                        StartCoroutine(Shoot(transform.position + pivot.forward * 1.0f, pivot.forward * 14, 1));
                    }
                }
            }
        }
    }
    
    void OnChildBecameVisible () {
        transform.rotation = Quaternion.identity;
        enabled = true;
    }
    
    void OnChildBecameInvisible () {
        enabled = false;
    }
    
    void Start () {
    }
    
    IEnumerator Explode () {
        enabled = false;
        electricity.emit = true;
        yield return new WaitForSeconds(1.5f);
        if (flames != null) {
            flames.transform.parent = null;
            flames.emit = false;
        }
        Game.fx.Explode(gameObject);
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
    
    void OnExplosion () {
        Hit(100);
    }
	
	IEnumerator DieTask () {
		Disintegrate disintegrate = gameObject.GetComponent<Disintegrate>();
		if (disintegrate != null) {
			disintegrate.DesintegrateFX();
			yield return new WaitForSeconds(disintegrate.deathDelay);
		}
		StartCoroutine(Explode());
	}

    void OnDeath () {
    	if (saveState) {
    		PlayerPrefs.SetInt(name, 1);
    	}
		if (Game.grid != null) {
			Game.grid.AddKill(species.Length > 0 ? species : name);
		}
		StartCoroutine(DieTask());
    }

    IEnumerator Burn () {
        Material sharedMaterial = renderer.sharedMaterial;
        renderer.material.color = Color.red;
        for (int i=0; i<3; ++i) {
            yield return new WaitForSeconds(1);
            Hit(50);
        }
        flames.emit = false;
        renderer.material = sharedMaterial;
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
        if (health.Alive() && !hit) {
            StartCoroutine(HitCoroutine(damage));
        }
    }
    
    IEnumerator HitCoroutine (int damage) {
        hit = true;
        Game.fx.PlaySoundPitched(hitClip);
        health.Damage(damage);
        Material sharedMaterial = renderer.sharedMaterial;
        renderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        renderer.material = sharedMaterial;
        hit = false;
    }

    void OnCollisionEnter (Collision collision) {
        if ((Time.time - lastContact) > 0.1f) {
            Game.fx.PlaySoundPitched(hitClip);
            lastContact = Time.time;
        }
        if (collision.collider.gameObject.layer == Game.projectilesLayer) {
            Projectile projectile = (Projectile)collision.collider.GetComponent(typeof(Projectile));
            Hit(projectile.Damage);
        }
    }
    
}
