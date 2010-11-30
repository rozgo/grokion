using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public static void ReleaseAll () {
		projectiles = null;
		projectilesData = null;
	}
	
	// struct for better data locality
	struct ProjectileData {
		public int nextFree;
	}
	
	const int maxNumProjectiles = 20;
	static Projectile[] projectiles = null;
	static ProjectileData[] projectilesData = null;
	static int free = 0;
	
	// call this only once, it will initialize all projectiles
	static void Init () {
		free = 0;
		// pre-allocate to avoid runtime allocations
		projectiles = new Projectile[maxNumProjectiles];
		projectilesData = new ProjectileData[maxNumProjectiles];
		for (int i=0; i<maxNumProjectiles; ++i) {
			// somewhere in Resources folder, get the projectile asset
			// by resusing the asset, unity will do automatic baching
			GameObject projectileObject = (GameObject)Instantiate((GameObject)Resources.Load("Projectile"));
			projectileObject.transform.parent = Game.cache.transform;
			projectileObject.active = false;
			// prepare storage for O(1) lookup time
			Projectile projectile = (Projectile)projectileObject.GetComponent(typeof(Projectile));
			if (i<(maxNumProjectiles-1)) {
				projectilesData[i].nextFree = i+1;
			}
			else {
				projectilesData[i].nextFree = -1;
			}
			projectile.index = i;
			projectiles[i] = projectile;
		}
	}
	
	// call this for every projectile you want to launch
	public static Projectile Launch (Vector3 position, Vector3 velocity, float life) {
		if (projectiles == null || projectiles[0] == null) {
			Init();
		}
		// O(1) lookup time
		if (free<0) {return null;}
		Projectile projectile = projectiles[free];
		projectile.gameObject.active = true;
		projectile.transform.position = position;
		projectile.transform.localScale = Vector3.one;
		projectile.rigidbody.velocity = velocity;
        projectile.rigidbody.mass = 5;
		projectile.life = life;
        projectile.damage = 20;
		projectile.StartCoroutine(projectile.Life());
		free = projectilesData[free].nextFree;
		return projectile;
	}
    
    new Transform transform;
    new Rigidbody rigidbody;
    int index;
    float life;

    int damage = 20;
    public int Damage { 
        get {return damage;}
        set {damage = value;}
	}

	void Awake () {
		// cache component lookup
		transform = gameObject.transform;
		rigidbody = gameObject.rigidbody;
	}
	
	IEnumerator Life () {
		while (life > 0) {
			life -= Time.deltaTime;
			yield return 0;
		}
		Kill();
	}
	
	void Kill () {
		projectiles[index].gameObject.SetActiveRecursively(false);
		projectilesData[index].nextFree = free;
		free = index;
	}
	
	void OnCollisionEnter () {
		// kill projectile
		if (projectiles[index].life <= 0) {
			return;
		}
		Game.fx.Hit(transform.position, 0.3f);
		projectiles[index].life = -1;
	}
}
