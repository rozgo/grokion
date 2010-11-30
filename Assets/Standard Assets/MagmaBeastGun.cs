using UnityEngine;
using System.Collections;

public class MagmaBeastGun : MonoBehaviour {

	public Transform nozzle;
	public Transform pivot;
    public AudioClip shootClip;
    public float shootingSpeed = 0.2f;
    public MagmaBeast magmaBeast;
    public int count = 0;
    public float explodeTime = 1;

    bool shooting = false;

    void Awake () {
    	count = 0;
    }
    
   	void Start () {
   	}
   	
   	void OnSwitchDown () {
   		if (!shooting) {
   			shooting = true;
   			StartCoroutine(Shoot(pivot.position + pivot.forward, pivot.forward * 20, 1));
   		}
   	}
   	
   	void OnSwitchUp () {
   		shooting = false;
   	}
   	
   	public void Explode () {
   		StartCoroutine(ExplodeTask());
   	}
   	
   	IEnumerator ExplodeTask () {
   		yield return new WaitForSeconds(explodeTime);
   		Game.fx.Explode(gameObject);
   	}
    
    IEnumerator Shoot (Vector3 position, Vector3 velocity, float life) {
        while (shooting) {
            Projectile projectile = Projectile.Launch(position, velocity, life);
            if (projectile != null) {
            	magmaBeast.lastGun = this;
            	++count;
            	projectile.transform.localScale *= 3;
                Game.fx.PlaySound(shootClip);
                //Physics.IgnoreCollision(missile.collider, collider);
            }
        	yield return new WaitForSeconds(shootingSpeed);
        }
    }
    
}
