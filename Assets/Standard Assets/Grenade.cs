using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {
	
	public static void ReleaseAll () {
		grenades = null;
		grenadesData = null;
	}
	
	// struct for better data locality
	struct GrenadeData {
		public int nextFree;
	}
	
	const int maxNumGrenades = 5;
	static Grenade[] grenades = null;
	static GrenadeData[] grenadesData = null;
	static int free = 0;
	
	// call this only once, it will initialize all grenades
	static void Init () {
		free = 0;
		// pre-allocate to avoid runtime allocations
		grenades = new Grenade[maxNumGrenades];
		grenadesData = new GrenadeData[maxNumGrenades];
		for (int i=0; i<maxNumGrenades; ++i) {
			// somewhere in Resources folder, get the grenade asset
			// by resusing the asset, unity will do automatic baching
			GameObject grenadeObject = (GameObject)Instantiate((GameObject)Resources.Load("Grenade"));
			grenadeObject.transform.parent = Game.cache.transform;
			grenadeObject.active = false;
			// prepare storage for O(1) lookup time
			Grenade grenade = (Grenade)grenadeObject.GetComponent(typeof(Grenade));
			if (i<(maxNumGrenades-1)) {
				grenadesData[i].nextFree = i+1;
			}
			else {
				grenadesData[i].nextFree = -1;
			}
			grenade.index = i;
			grenades[i] = grenade;
		}
	}
	
	// call this for every grenade you want to launch
	public static Grenade Launch (Vector3 position, Vector3 velocity, float life) {
		if (grenades == null || grenades[0] == null) {
			Init();
		}
		// O(1) lookup time
		if (free<0) {return null;}
		Grenade grenade = grenades[free];
		float angle = Mathf.Atan2(velocity.x,velocity.y) * Mathf.Rad2Deg;
		grenade.transform.rotation = Quaternion.Euler(270, 0, 0) * Quaternion.AngleAxis(angle + 90, Vector3.up);
		grenade.gameObject.active = true;
		grenade.transform.position = position;
		grenade.rigidbody.velocity = velocity;
		grenade.life = life;
		grenade.StartCoroutine(grenade.Life());
		free = grenadesData[free].nextFree;
		return grenade;
	}
    
    public AudioClip bounceClip;
    new Transform transform;
    new Rigidbody rigidbody;
    int index;
    float life;
	
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
		gameObject.active = false;
		grenadesData[index].nextFree = free;
		free = index;
		Game.fx.Explode(gameObject);
	}
	
	void OnCollisionEnter (Collision collision) {
		if (collision.relativeVelocity.sqrMagnitude > 1) {
			Game.fx.PlaySoundPitched(bounceClip);
		}
	}
}
