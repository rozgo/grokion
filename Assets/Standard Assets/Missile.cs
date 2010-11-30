using UnityEngine;
using System.Collections;

public class Missile : MonoBehaviour {
	
	public static void ReleaseAll () {
		missiles = null;
		missilesData = null;
	}
	
	// struct for better data locality
	struct MissileData {
		public int nextFree;
	}
	
	const int maxNumMissiles = 5;
	static Missile[] missiles = null;
	static MissileData[] missilesData = null;
	static int free = 0;
	
	// call this only once, it will initialize all missiles
	static void Init () {
		free = 0;
		// pre-allocate to avoid runtime allocations
		missiles = new Missile[maxNumMissiles];
		missilesData = new MissileData[maxNumMissiles];
		for (int i=0; i<maxNumMissiles; ++i) {
			// somewhere in Resources folder, get the missile asset
			// by resusing the asset, unity will do automatic baching
			GameObject missileObject = (GameObject)Instantiate((GameObject)Resources.Load("Missile"));
			missileObject.transform.parent = Game.cache.transform;
			missileObject.SetActiveRecursively(false);
			// prepare storage for O(1) lookup time
			Missile missile = (Missile)missileObject.GetComponent(typeof(Missile));
			if (i<(maxNumMissiles-1)) {
				missilesData[i].nextFree = i+1;
			}
			else {
				missilesData[i].nextFree = -1;
			}
			missile.index = i;
			missiles[i] = missile;
		}
	}
	
	// call this for every missile you want to launch
	public static Missile Launch (Vector3 position, Vector3 velocity, float life) {
		if (missiles == null || missiles[0] == null) {
			Init();
		}
		// O(1) lookup time
		if (free<0) {return null;}
		Missile missile = missiles[free];
		float angle = Mathf.Atan2(velocity.x,velocity.y) * Mathf.Rad2Deg;
		missile.transform.rotation = Quaternion.Euler(270, 0, 0) * Quaternion.AngleAxis(angle + 90, Vector3.up);
		missile.gameObject.SetActiveRecursively(true);
		missile.transform.position = position;
		missile.rigidbody.velocity = velocity;
		missile.life = life;
        missile.canExplode = true;
		missile.StartCoroutine(missile.Life());
		free = missilesData[free].nextFree;
		return missile;
	}
    
    new Transform transform;
    new Rigidbody rigidbody;
    int index;
    float life;
    bool canExplode = true;
	
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
		gameObject.SetActiveRecursively(false);
		missilesData[index].nextFree = free;
		free = index;
	}

    void OnWaterEnter () {
        canExplode = false;
    }
	
	void OnCollisionEnter () {
		if (missiles[index].life <= 0) {
			return;
		}
		missiles[index].life = -1;
        if (canExplode) {
            Game.fx.Explode(gameObject);
        }
	}
}
