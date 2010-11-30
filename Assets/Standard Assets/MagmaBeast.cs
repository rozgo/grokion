using UnityEngine;
using System.Collections;

public class MagmaBeast : MonoBehaviour {

    public Health health;
    public AudioClip[] growls;
    public AudioClip hitClip;
    public AudioClip deathClip;
    public new Renderer renderer;
    public GameObject head;
    public MagmaBeastGun[] guns;
    public ObjectMove lava;
	public MagmaBeastGun lastGun;
	
    bool hit = false;
    int stage = 3000;
    bool destroyGun = false;
    
    public enum State {
    	
    	CenterStart,
    	CenterAttack,
    	CenterEnd,
    	
    	LeftStart,
    	LeftAttack,
    	LeftEnd,
    	
    	RightStart,
    	RightAttack,
    	RightEnd,
    	
    	DestroyGun,
    	
        Die,
    }

    public State state = State.CenterStart;
    
    string attackZone = "AttackZone|Center";
    
    void OnHotspotEnter (Hotspot hotspot) {
    	attackZone = hotspot.name;
    }
    
    void SelectHub () {
    	if (!health.Alive()) {
    		state = State.Die;
    	}
    	else if (destroyGun) {
    		state = State.DestroyGun;
    	}
    	else if (attackZone.StartsWith("AttackZone|Left")) {
    		state = State.LeftStart;
    	}
    	else if (attackZone.StartsWith("AttackZone|Right")) {
    		state = State.RightStart;
    	}
    	else {
    		state = State.CenterStart;
    	}
    	lava.PingPong();
    }
    
    IEnumerator CenterStartState () {
        animation.CrossFade("CENTER_START");
        animation["CENTER_START"].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation["CENTER_START"].length);
        state = State.CenterAttack;
        NextState();
    }
    
    IEnumerator CenterAttackState () {
    	Growl();
        animation.CrossFade("CENTER_ATTACK1");
        animation["CENTER_ATTACK1"].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation["CENTER_ATTACK1"].length);
        state = State.CenterEnd;
        NextState();
    }
    
    IEnumerator CenterEndState () {
        animation.CrossFade("CENTER_END");
        animation["CENTER_END"].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation["CENTER_END"].length);
        SelectHub();
        NextState();
    }

    IEnumerator LeftStartState () {
        animation.CrossFade("LEFT_START");
        animation["LEFT_START"].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation["LEFT_START"].length);
        state = State.LeftAttack;
        NextState();
    }
    
    IEnumerator LeftAttackState () {
    	Growl();
    	string attackAnim = "LEFT_ATTACKLOW";
    	if (attackZone == "AttackZone|LeftHigh") {
    		attackAnim = "LEFT_ATTACKHIGH";
    	}
    	else if (attackZone == "AttackZone|LeftMid") {
    		attackAnim = "LEFT_ATTACKMID";
    	}
        animation.CrossFade(attackAnim);
        animation[attackAnim].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation[attackAnim].length);
        state = State.LeftEnd;
        NextState();
    }
    
    IEnumerator LeftEndState () {
        animation.CrossFade("LEFT_END");
        animation["LEFT_END"].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation["LEFT_END"].length);
        SelectHub();
        NextState();
    }

    IEnumerator RightStartState () {
        animation.CrossFade("RIGHT_START");
        animation["RIGHT_START"].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation["RIGHT_START"].length);
        state = State.RightAttack;
        NextState();
    }
    
    IEnumerator RightAttackState () {
    	Growl();
    	string attackAnim = "RIGHT_ATTACKLOW";
    	if (attackZone == "AttackZone|RightHigh") {
    		attackAnim = "RIGHT_ATTACKHIGH";
    	}
    	else if (attackZone == "AttackZone|RightMid") {
    		attackAnim = "RIGHT_ATTACKMID";
    	}
        animation.CrossFade(attackAnim);
        animation[attackAnim].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation[attackAnim].length);
        state = State.RightEnd;
        NextState();
    }
    
    IEnumerator RightEndState () {
        animation.CrossFade("RIGHT_END");
        animation["RIGHT_END"].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation["RIGHT_END"].length);
        SelectHub();
        NextState();
    }

    IEnumerator DestroyGunState () {
    	destroyGun = false;
   		MagmaBeastGun mostUsedGun = lastGun;
   		if (mostUsedGun == null) {
	   		int count = -1;
	   		foreach (MagmaBeastGun gun in guns) {
	   			if (gun != null && gun.count > count) {
	   				count = gun.count;
	   				mostUsedGun = gun;
	   			}
	   		}
   		}
   		mostUsedGun.Explode();
   		mostUsedGun.animation.Play();
   		string destroyAnim = mostUsedGun.name.ToUpper();
        animation.CrossFade(destroyAnim);
        animation[destroyAnim].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation[destroyAnim].length);
        Destroy(mostUsedGun.gameObject);
        SelectHub();
        NextState();
    }

    IEnumerator HitReact (int damage) {
        hit = true;
        Game.fx.PlaySoundPitched(hitClip);
        health.Damage(damage);
        Material sharedMaterial = renderer.sharedMaterial;
        renderer.material.SetColor("_Color", Color.white);
        float timer = 0.1f;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return 0;
        }
        renderer.material = sharedMaterial;
        hit = false;
    }

    void Hit (int damage) {
        if (health.Alive() && !hit) {
            StartCoroutine(HitReact(damage));
            if (health.HP < stage) {
            	destroyGun = true;
            	stage -= 1000;
            }
        }
    }

    IEnumerator DieState () {
    	foreach (MagmaBeastGun gun in guns) {
    		if (gun != null) {
    			gun.gameObject.SetActiveRecursively(false);
    		}
    	}
        animation.CrossFade("CENTER_START");
        animation["CENTER_START"].wrapMode = WrapMode.Once;
        yield return new WaitForSeconds(animation["CENTER_START"].length);
        Game.fx.PlaySound(deathClip);
        animation["CENTER_DAMAGE1"].speed = 0.5f;
        animation.CrossFade("CENTER_DAMAGE1");
        yield return new WaitForSeconds(2);
        foreach (Collider collider in GetComponentsInChildren(typeof(Collider))) {
        	Game.fx.Explode(collider.gameObject);
        	yield return new WaitForSeconds(0.6f);
        }
    }
    
    void Growl () {
    	int i = Random.Range(0,growls.Length);
    	Game.fx.PlaySoundPitched(growls[i]);
    }	
    
    void OnDeath () {
    }

    void OnCollisionEnter (Collision collision) {
		if (collision.collider.gameObject.layer == Game.projectilesLayer) {
            Projectile projectile = (Projectile)collision.collider.GetComponent(typeof(Projectile));            
            Hit(projectile.Damage);
		}
    }

    void Awake () {
    }

    void Start () {
        NextState();
    }
    
    void LateUpdate () {
//    	if (Game.character) {
//    		Vector3 lookDir = Game.character.transform.position - head.transform.position;
//    		head.transform.rotation = Quaternion.LookRotation(lookDir, -Vector3.up) *
//    			Quaternion.Euler(0, 90, 90);
//    	}
    }

    void NextState () {
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info = 
            GetType().GetMethod(methodName, 
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }
    
}