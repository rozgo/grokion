using UnityEngine;
using System.Collections;

public class Enemy : StateMachine {
	
	public string species = "";
	public bool saveState = false;
    public string unlockKey;
    public string lockKey;
	public float speed = 2;
	public int damage = 10;
	public AudioClip hitClip;
	public AudioClip dieClip;
	public GameObject energyAsset;
	public Transform[] targets;
	public int targetIndex = 0;
	public bool autoDisable = true;
	public float idleSpeed = 1.0f;
	public bool fly = false;
    public bool canSwim = false;
	
	new Rigidbody rigidbody;
	new Transform transform;
	public new Renderer renderer;
	public Color hitColor = Color.red;
	new Animation animation;
	bool burning = false;
    Health health;
	ParticleEmitter flames;
	
	class EnemyState : State {
		
		public virtual void OnParticleCollision (GameObject particle) {
		}
		
		public virtual void OnCollisionEnter (Collision collision) {
		}
		
		public virtual void OnExplosion (GameObject exploded) {
		}
		
		public virtual void OnChildBecameVisible () {
		}
		
		public virtual void OnChildBecameInvisible () {
		}
		
		public virtual void OnHotspotEnter (Hotspot hotspot) {
		}
		
		public virtual void OnHotspotExit (Hotspot hotspot) {
		}

	}
	
	class PatrolState : EnemyState {
		
		Enemy enemy;
		Vector3 targetPosition;
		float excited = 0;
		
		public PatrolState (Enemy enemy) {
			this.enemy = enemy;
		}
		
		public override void OnEnter () {
			if (enemy.animation["IDLE"] != null) {
				enemy.animation.CrossFade("IDLE");
			}
			if (enemy.targets.Length > 0) {
				targetPosition = enemy.targets[enemy.targetIndex].position;
			}
		}
		
		public override void OnUpdate () {
			if (enemy.speed != 0 && enemy.targets.Length > 0) {
				Vector3 velocity = targetPosition - enemy.transform.position;
				float magnitude = velocity.magnitude;
				float speed = enemy.speed;
				if (excited > 0) {
                    speed += 4;
                    excited -= Time.deltaTime;
                    if (Game.character != null) {
                        targetPosition = Game.character.collider.bounds.center;
                    }
                    if (excited <= 0) {
                        targetPosition = enemy.transform.position;
                    }
				}
				velocity = velocity.normalized * speed;
				if (magnitude < 0.5f) {
					++enemy.targetIndex;
					if (enemy.targetIndex >= enemy.targets.Length) {
						enemy.targetIndex = 0;
					}
					targetPosition = enemy.targets[enemy.targetIndex].position;
					excited = 0;
				}
				if (enemy.burning) {
					velocity *= 0.5f;
				}
				if (!enemy.fly) {
					velocity -= Vector3.up * Time.deltaTime * 10;
					velocity.y = Mathf.Max(enemy.rigidbody.velocity.y, -10);
				}
				if (velocity.x > 0) {
					enemy.transform.rotation = Quaternion.AngleAxis(180,Vector3.up);
				}
				else if (velocity.x < 0) {
					enemy.transform.rotation = Quaternion.AngleAxis(0,Vector3.up);
				}
				enemy.rigidbody.velocity = velocity;
			}
		}
		
		public override void OnParticleCollision (GameObject particle) {
			if (particle.gameObject.layer == Game.flamesLayer) {
				if (!enemy.burning) {
					enemy.StartCoroutine(enemy.Burn());
				}
			}
		}
		
		public override void OnCollisionEnter (Collision collision) {
			if (collision.collider.gameObject.layer == Game.projectilesLayer) {
                Projectile projectile = (Projectile)collision.collider.GetComponent(typeof(Projectile));
				enemy.Hit(projectile.Damage);
			}
			else if (collision.collider.gameObject.layer == Game.characterLayer) {
				if (enemy.burning) {
					collision.collider.SendMessage("OnBurn");
				}
				collision.collider.SendMessage("Hit",enemy.damage);
				enemy.Attack();
			}
		}
		
		public override void OnExplosion (GameObject exploded) {
			if (Vector3.Distance(enemy.transform.position, exploded.transform.position) < 3) {
				enemy.Hit(100);
			}
		}
		
		public override void OnChildBecameVisible () {
			//enemy.enabled = true;
			enemy.gameObject.active = true;
		}
		
		public override void OnChildBecameInvisible () {
			if (enemy.autoDisable) {
				//enemy.enabled = false;
				enemy.gameObject.active = false;
			}
		}
		
		public override void OnHotspotEnter (Hotspot hotspot) {
			if (Game.character != null) {
				excited = 1;
			}
		}
	}
	
	class AttackState : EnemyState {
		
		Enemy enemy;
		float timer;
		
		public AttackState (Enemy enemy) {
			this.enemy = enemy;
		}
		
		public override void OnEnter () {
			if (enemy.animation["ATTACK"] != null) {
				enemy.animation.Play("ATTACK");
			}
			timer = 0.2f;
		}
		
		public override void OnExit () {
			if (Game.character != null) {
				float bestDot = -10000;
				int targetIndex = -1;
				for (int i=0; i<enemy.targets.Length; ++i) {
					Vector3 d1 = Game.character.transform.position - enemy.transform.position;
					Vector3 d2 = enemy.targets[i].position - enemy.transform.position;
					float dot = Vector3.Dot(d1, d2);
					if (dot > bestDot) {
						bestDot = dot;
						targetIndex = i;
					}
				}
				enemy.targetIndex = targetIndex;
			}
		}
		
		public override void OnUpdate () {
			timer -= Time.deltaTime;
			if (timer < 0) {
				enemy.Patrol();
			}
		}
		
		public override void OnParticleCollision (GameObject particle) {
			if (particle.gameObject.layer == Game.flamesLayer) {
				if (!enemy.burning) {
					enemy.StartCoroutine(enemy.Burn());
				}
			}
		}
		
		public override void OnExplosion (GameObject exploded) {
			if (Vector3.Distance(enemy.transform.position, exploded.transform.position) < 3) {
				enemy.Hit(100);
			}
		}
	}
	
	IEnumerator Burn () {
        if (flames == null) {
            Material sharedMaterial = renderer.sharedMaterial;
            renderer.material.color = new Color(1, 0.5f, 0);
            flames = Game.fx.Burn(gameObject);
            for (int i=0; i<3; ++i) {
                yield return new WaitForSeconds(1);
                Hit(50);
                renderer.material.color = new Color(1, 0.5f, 0);
            }
            flames.emit = false;
            renderer.material = sharedMaterial;
        }
	}
	
	private class HitState : EnemyState {
		
		Enemy enemy;
		float timer;
        Material sharedMaterial;
		
		public HitState (Enemy enemy) {
			this.enemy = enemy;
		}
		
		public override void OnEnter () {
			enemy.enabled = true;
			sharedMaterial = enemy.renderer.sharedMaterial;
			enemy.renderer.material.color = enemy.hitColor;
			Game.fx.PlaySound(enemy.hitClip);
			if (enemy.animation["HIT"] != null) {
				enemy.animation.Play("HIT");
			}
			if (enemy.health.HP < 0) {
				timer = 0;
				if (enemy.rigidbody != null) {
					enemy.rigidbody.velocity = Vector3.zero;
				}
			}
			else {
				timer = 0.1f;
			}
		}
		
		public override void OnExit () {
			enemy.renderer.material = sharedMaterial;
			if (Game.character != null) {
				float bestDot = -10000;
				int targetIndex = -1;
				for (int i=0; i<enemy.targets.Length; ++i) {
					Vector3 d1 = Game.character.transform.position - enemy.transform.position;
					Vector3 d2 = enemy.targets[i].position - enemy.transform.position;
					float dot = Vector3.Dot(d1, d2);
					if (dot > bestDot) {
						bestDot = dot;
						targetIndex = i;
					}
				}
				enemy.targetIndex = targetIndex;
			}
		}
		
		public override void OnUpdate () {
			timer -= Time.deltaTime;
			if (timer < 0) {
				if (enemy.health.Alive() && this is HitState) {
					enemy.Patrol();
				}
			}
		}
	}
	
	private class DieState : EnemyState {
		
		Enemy enemy;
		float timer;
		
		public DieState (Enemy enemy) {
			this.enemy = enemy;
		}
		
		IEnumerator Die () {
			int orbs = Game.casual ? Random.Range(0,4) : Random.Range(0,2);
			Game.fx.ReleaseEnergy(orbs, enemy.collider.bounds.center);
			Game.fx.PlaySound(enemy.dieClip);
			if (enemy.rigidbody != null) {
				enemy.rigidbody.velocity = Vector3.zero;
			}
			Game.fx.Kill(enemy.collider.bounds.center);
            yield return 0;
            Spawner spawner = (Spawner)enemy.GetComponent(typeof(Spawner));
            if (spawner != null) {
                spawner.Spawn();
            }
            else {
                Destroy(enemy.gameObject);
            }
		}
		
		public override void OnEnter () {
			if (enemy.rigidbody != null) {
				enemy.rigidbody.velocity = Vector3.zero;
			}
			enemy.StartCoroutine(Die());
		}
	}
	
	public void Patrol () {
		PatrolState patrolState = (PatrolState)states[typeof(PatrolState)];
		if (patrolState == null) {
			patrolState = new PatrolState(this);
			states[typeof(PatrolState)] = patrolState;
		}
		Change(patrolState);
	}

	public void Attack () {
		AttackState attackState = (AttackState)states[typeof(AttackState)];
		if (attackState == null) {
			attackState = new AttackState(this);
			states[typeof(AttackState)] = attackState;
		}
		Change(attackState);
	}

	public void Hit (int damage) {
		if (health.Alive() && !(state is HitState)) {
            health.Damage(damage);
			HitState hitState = (HitState)states[typeof(HitState)];
			if (hitState == null) {
				hitState = new HitState(this);
				states[typeof(HitState)] = hitState;
			}
			Change(hitState);
		}
	}
	
	public void Die () {
		DieState dieState = (DieState)states[typeof(DieState)];
		if (dieState == null) {
			dieState = new DieState(this);
			states[typeof(DieState)] = dieState;
		}
		Change(dieState);
	}
	
	void OnParticleCollision (GameObject particle) {
		EnemyState enemyState = state as EnemyState;
		if (enemyState != null) {
			enemyState.OnParticleCollision(particle);
		}
	}
	
	void OnCollisionEnter (Collision collision) {
		EnemyState enemyState = state as EnemyState;
		if (enemyState != null) {
			enemyState.OnCollisionEnter(collision);
		}
	}
	
	void OnExplosion (GameObject exploded) {
		EnemyState enemyState = state as EnemyState;
		if (enemyState != null) {
			enemyState.OnExplosion(exploded);
		}
	}
	
	void OnChildBecameVisible () {
		EnemyState enemyState = state as EnemyState;
		if (enemyState != null) {
			enemyState.OnChildBecameVisible();
		}
	}
	
	void OnChildBecameInvisible () {
		EnemyState enemyState = state as EnemyState;
		if (enemyState != null) {
			enemyState.OnChildBecameInvisible();
		}
	}
	
	void OnHotspotEnter (Hotspot hotspot) {
		EnemyState enemyState = state as EnemyState;
		if (enemyState != null) {
			enemyState.OnHotspotEnter(hotspot);
		}
	}
	
	void OnHotspotExit (Hotspot hotspot) {
		EnemyState enemyState = state as EnemyState;
		if (enemyState != null) {
			enemyState.OnHotspotExit(hotspot);
		}
	}

    void OnWaterEnter (Water water) {
        if (water.lava) {
            Hit(3000);
        }
        else if (canSwim) {
            water.CanSwim(rigidbody);
        }
    }

    void OnDeath () {
        if (!(state is DieState)) {
        	if (saveState) {
        		PlayerPrefs.SetInt(name, 1);
        	}
			if (Game.grid != null) {
				Game.grid.AddKill(species.Length > 0 ? species : name);
			}
            Die();
        }
    }
	
	void Awake () {
		transform = gameObject.transform;
		rigidbody = gameObject.rigidbody;
        health = (Health)GetComponent(typeof(Health));
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
		animation = (Animation)GetComponentInChildren(typeof(Animation));
		if (renderer == null) {
			renderer = (Renderer)GetComponentInChildren(typeof(Renderer));
		}
		if (animation["IDLE"] != null) {
			animation["IDLE"].speed = idleSpeed;
		}
        Patrol();
	}

	void Update () {
		state.OnUpdate();
	}
}
