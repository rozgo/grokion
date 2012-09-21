using UnityEngine;
using System.Collections;

public class Turret : MonoBehaviour {

    static float lastContact = 0;
    
    public enum Weapon {
    	Projectile,
    	Missile,
    	Grenade,
    }
    
	public string species = "Turret";
    public Weapon weapon = Weapon.Projectile;
    public bool saveState = false;
    public string unlockKey;
    public string lockKey;
    public AudioClip turretClip;
    public float floatingRadius = 0.6f;
    public float floatingSpeed = 0;
    public float shootingSpeed = 0.2f;
    public float aimingSpeed = 1.0f;
    public Material fxMaterial;
    public AudioClip hitClip;
    public bool damaged = false;
    public bool cull = true;
	public float shootLife = 1;
	public float awarenessRadius = 8;
    
    Vector3 originalPosition;
    new Transform transform;
    new Rigidbody rigidbody;
    bool shooting = false;
    ParticleEmitter flames;
    bool hit = false;
    Health health;
    new Renderer renderer;

    void Awake () {
    	renderer = gameObject.GetComponentInChildren<Renderer>();
        health = (Health)GetComponent(typeof(Health));
        if (cull) {
            enabled = false;
        }
        transform = gameObject.transform;
        rigidbody = gameObject.rigidbody;
        originalPosition = transform.position;
        rigidbody.Sleep();
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
    
    void Start () {
        if (damaged) {
            transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        }
    }
    
    IEnumerator Shoot (Vector3 position, Vector3 velocity, float life) {
        shooting = true;
        if (weapon == Weapon.Projectile) {
	        Projectile projectile = Projectile.Launch(position, velocity, life);
	        if (projectile != null) {
	            Game.fx.PlaySound(turretClip);
	            Physics.IgnoreCollision(projectile.collider, collider);
	        }
        }
        else if (weapon == Weapon.Missile) {
	        Missile missile = Missile.Launch(position, velocity, life);
	        if (missile != null) {
	            Game.fx.PlaySound(turretClip);
	            Physics.IgnoreCollision(missile.collider, collider);
	        }
        }
        else if (weapon == Weapon.Grenade) {
	        Grenade grenade = Grenade.Launch(position, velocity, life);
	        if (grenade != null) {
	            Game.fx.PlaySound(turretClip);
	            Physics.IgnoreCollision(grenade.collider, collider);
	        }
        }
        yield return new WaitForSeconds(shootingSpeed);
        shooting = false;
    }
    
    void FixedUpdate () {
        if (damaged) {
            return;
        }
        Vector3 position = originalPosition;
        position.x += floatingRadius * Mathf.Cos(Time.time * floatingSpeed);
        position.y += floatingRadius * Mathf.Sin(Time.time * floatingSpeed);
        Vector3 velocity = position - transform.position;
        if (!hit) {
            rigidbody.AddForce(velocity, ForceMode.Acceleration);
        }
        if (Game.character != null) {
            if (Vector3.Distance(Game.character.head.position, transform.position) < awarenessRadius) {
                Vector3 lookAtPoint = Game.character.head.position;
                Vector3 forward = (lookAtPoint - transform.position).normalized;
                forward.y = Mathf.Max(forward.y, -0.5f);
                Quaternion lookRotation = Quaternion.LookRotation(forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation,
                                                      Time.deltaTime * aimingSpeed);
                if (!shooting) {
                    if (Vector3.Distance(lookAtPoint, Game.character.head.position) < 1) { 
                        StartCoroutine(Shoot(transform.position, transform.forward * 14, shootLife));
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
        if (cull) {
            enabled = false;
        }
    }
    
    IEnumerator Explode () {
        enabled = false;
        rigidbody.useGravity = true;
        rigidbody.freezeRotation = false;
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
            renderer.material.color = new Color(1, 0.5f, 0);
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
