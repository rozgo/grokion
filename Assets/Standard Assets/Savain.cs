using UnityEngine;
using System.Collections;

public class Savain : StateMachine {

    public AudioClip projectileClip;
    public AudioClip missileClip;
    public AudioClip grenadeClip;
    public AudioClip burningClip;
    public AudioClip dieClip;
    public float floatingRadius = 0.6f;
    public float floatingSpeed = 0;
    public float aimingSpeed = 1.0f;
    public AudioClip hitClip;
    public GameObject flamesAsset;
    public new Renderer renderer;
    public GameObject hover;
    
    new Transform transform;
    new Rigidbody rigidbody;
    Health health;
    bool shooting = false;
    bool burning = false;
    ParticleEmitter flames;
    
    void Awake () {
        transform = gameObject.transform;
        rigidbody = gameObject.rigidbody;
        health = (Health)GetComponent(typeof(Health));
    }

    void Start () {
		renderer = (Renderer)GetComponentInChildren(typeof(Renderer));
        Assault();
    }

    class SavainState : State {

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

    class AssaultState : SavainState {

        Savain savain;
        
        public AssaultState (Savain savain) {
            this.savain = savain;
        }

        public override void OnEnter () {
        }

        public override void OnUpdate () {
            if (Game.character == null) {
                return;
            }
            savain.hover.transform.rotation *= Quaternion.Euler(0, 0, 500 * Time.deltaTime);
            Vector3 position = Game.character.transform.position;
            position.x += savain.floatingRadius * Mathf.Cos(Time.time * savain.floatingSpeed);
            position.y += savain.floatingRadius * Mathf.Sin(Time.time * savain.floatingSpeed) + savain.floatingRadius;
            position.z += savain.floatingRadius * Mathf.Cos(Time.time * savain.floatingSpeed);
            Vector3 velocity = position - savain.transform.position;
            savain.rigidbody.velocity = velocity;
            Vector3 lookAtPoint = Game.character.Head.position;
            Vector3 forward = lookAtPoint - position;
            forward.y = 0;
            forward.Normalize();
            float angle = Vector3.Angle(-savain.transform.right, forward);
            savain.transform.rotation *= Quaternion.AngleAxis(Mathf.Min(angle, savain.aimingSpeed) * Time.deltaTime, 
                                                              Vector3.Cross(-savain.transform.right, forward));
            if (!savain.shooting) {
                if (Vector3.Distance(lookAtPoint, Game.character.Head.position) < 5) { 
                    savain.StartCoroutine(savain.Shoot(savain.transform.position, -savain.transform.right * 14));
                }
            }

        }

        public override void OnParticleCollision (GameObject particle) {
            if (particle.gameObject.layer == Game.flamesLayer) {
                if (!savain.burning) {
                    savain.StartCoroutine(savain.Burn());
                }
            }
        }
        
        public override void OnCollisionEnter (Collision collision) {
            if (collision.collider.gameObject.layer == Game.projectilesLayer) {
                Projectile projectile = (Projectile)collision.collider.GetComponent(typeof(Projectile));
                savain.Hit(projectile.Damage);
            }
            else if (collision.collider.gameObject.layer == Game.characterLayer) {
                if (savain.burning) {
                    collision.collider.SendMessage("OnBurn");
                }
                //collision.collider.SendMessage("Hit",10);
                //savain.Attack();
            }
        }
        
        public override void OnExplosion (GameObject exploded) {
            if (Vector3.Distance(savain.transform.position, exploded.transform.position) < 2) {
                savain.Hit(100);
            }
        }

    }

    private class HitState : SavainState {
        
        Savain savain;
        float timer;
        Material sharedMaterial;
        
        public HitState (Savain savain) {
            this.savain = savain;
        }
        
        public override void OnEnter () {
            sharedMaterial = savain.renderer.sharedMaterial;
            savain.renderer.material.color = new Color(1, 0.3f, 0.3f);
            Game.fx.PlaySound(savain.hitClip);
            if (savain.health.HP < 0) {
                timer = 0;
                if (savain.rigidbody != null) {
                    savain.rigidbody.velocity = Vector3.zero;
                }
            }
            else {
                timer = 0.5f;
            }
        }
        
        public override void OnExit () {
            savain.renderer.material = sharedMaterial;
        }
        
        public override void OnUpdate () {
            savain.transform.rotation *= 
                Quaternion.Euler(0, Mathf.Sign(savain.transform.forward.z) * 500 * Time.deltaTime, 0);
			timer -= Time.deltaTime;
			if (timer < 0) {
				if (savain.health.Alive() && this is HitState) {
					savain.Assault();
				}
			}
        }
    }

    private class DieState : SavainState {
        
        Savain savain;
        float timer;
        
        public DieState (Savain savain) {
            this.savain = savain;
        }
        
        IEnumerator Die () {
            Game.fx.PlaySound(savain.dieClip);
            if (savain.rigidbody != null) {
                savain.rigidbody.velocity = Vector3.zero;
            }
            yield return 0;
            Destroy(savain.gameObject);
        }
        
        public override void OnEnter () {
            if (savain.rigidbody != null) {
                savain.rigidbody.velocity = Vector3.zero;
            }
            savain.StartCoroutine(Die());
        }
    }

    public void Assault () {
        AssaultState assaultState = (AssaultState)states[typeof(AssaultState)];
        if (assaultState == null) {
            assaultState = new AssaultState(this);
            states[typeof(AssaultState)] = assaultState;
        }
        Change(assaultState);
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
    
    IEnumerator Shoot (Vector3 position, Vector3 velocity) {
        shooting = true;
        if (Mathf.Abs(position.z) > 1) {
            Projectile projectile = Projectile.Launch(position, velocity, 2);
            if (projectile != null) {
                Game.fx.PlaySound(projectileClip);
                Physics.IgnoreCollision(projectile.collider, collider);
            }
            yield return new WaitForSeconds(0.2f);
        }
        else if (position.y < (Game.character.transform.position.y + 1)) {
            velocity.x = Mathf.Sign(velocity.x) * velocity.magnitude;
            velocity.y = 0;
            velocity.z = 0;
            Missile missile = Missile.Launch(position, velocity, 2);
            if (missile != null) {
                Game.fx.PlaySound(missileClip);
                Physics.IgnoreCollision(missile.collider, collider);
            }
            yield return new WaitForSeconds(1);
        }
        else {
            Grenade grenade = Grenade.Launch(position, Vector3.up * -2, 3);
            if (grenade != null) {
                Game.fx.PlaySound(grenadeClip);
                Physics.IgnoreCollision(grenade.collider, collider);
            }
            yield return new WaitForSeconds(2);
        }
        shooting = false;
    }
    
    void FixedUpdate () {
        state.OnUpdate();
    }
    
    IEnumerator Burn () {
        burning = true;
        Material sharedMaterial = renderer.sharedMaterial;
        renderer.material.color = new Color(1, 0.5f, 0);
        GameObject flamesObject = (GameObject)Instantiate(flamesAsset);
        flamesObject.transform.position = collider.bounds.center;
        ParticleEmitter flames = (ParticleEmitter)flamesObject.GetComponent(typeof(ParticleEmitter));
        flamesObject.transform.parent = transform;
        Game.fx.PlaySound(burningClip);
        for (int i=0; i<3; ++i) {
            yield return new WaitForSeconds(1);
            Hit(50);
            renderer.material.color = new Color(1, 0.5f, 0);
        }
        flames.emit = false;
        renderer.material = sharedMaterial;
        burning = false;
    }

    void OnParticleCollision (GameObject particle) {
        SavainState savainState = state as SavainState;
        if (savainState != null) {
            savainState.OnParticleCollision(particle);
        }
    }
    
    void OnCollisionEnter (Collision collision) {
        SavainState savainState = state as SavainState;
        if (savainState != null) {
            savainState.OnCollisionEnter(collision);
        }
    }
    
    void OnExplosion (GameObject exploded) {
        SavainState savainState = state as SavainState;
        if (savainState != null) {
            savainState.OnExplosion(exploded);
        }
    }
    
    void OnChildBecameVisible () {
        SavainState savainState = state as SavainState;
        if (savainState != null) {
            savainState.OnChildBecameVisible();
        }
    }
    
    void OnChildBecameInvisible () {
        SavainState savainState = state as SavainState;
        if (savainState != null) {
            savainState.OnChildBecameInvisible();
        }
    }
    
    void OnHotspotEnter (Hotspot hotspot) {
        SavainState savainState = state as SavainState;
        if (savainState != null) {
            savainState.OnHotspotEnter(hotspot);
        }
    }
    
    void OnHotspotExit (Hotspot hotspot) {
        SavainState savainState = state as SavainState;
        if (savainState != null) {
            savainState.OnHotspotExit(hotspot);
        }
    }

    void OnWaterEnter (Water water) {
        //water.CanSwim(rigidbody);
    }

    void OnDeath () {
        if (!(state is DieState)) {
            Die();
        }
    }
    
}
