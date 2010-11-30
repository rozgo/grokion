using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {
	
	public Renderer water;
	public Material freezeMaterial;
	public AudioClip freezeClip;
	public AudioClip meltClip;
	public Collider freezeCollider;
	public GameObject ripplesAsset;
	public GameObject bubblesAsset;
	public GameObject splashAsset;
	public AudioClip splashClip;
	public bool lava = false;
	public bool frozen = false;
	public float speed = 0.2f;
	public bool animateTexture = false;
	
	int rbMax = 10;
	Rigidbody[] bodies;
	int rbCount = 0;
	Material originalMaterial;
	Vector3 prevPosition;
    bool restoreAnimateTexture = false;
	
	void Awake () {
        if (animation != null) {
            animation["IDLE"].speed = speed;
        }
		bodies = new Rigidbody[rbMax];
		prevPosition = transform.position;
	}
	
	void Start () {
		if (frozen) {
			StartCoroutine(Freeze());
		}
		else if (freezeCollider != null) {
			freezeCollider.gameObject.active = false;
		}
	}
	
	void FixedUpdate () {
        if (frozen) {
            return;
        }
		if (animateTexture) {
			float sign = transform.localScale.x < 0 ? -1 : 1;
			Vector2 offset = new Vector2(1, 1);
			offset.x = Mathf.Sin(sign * Time.time * 0.2f);
			if (water.materials.Length > 1) {
				water.materials[1].mainTextureOffset = offset;
			}
			offset.y = Mathf.Sin(Time.time * 2) * 0.05f;
			water.material.mainTextureOffset = offset;
		}
        if (collider == null) {
            return;
        }
		float top = collider.bounds.max.y;
		for (int i=0; i<rbCount; ) {
			Rigidbody rb = bodies[i];
			if (rb == null || !rb.gameObject.active) {
				bodies[i] = bodies[rbCount-1];
				--rbCount;
			}
			else {
				++i;
                // buoyancy hack, don't judge me
				float y = (top - rb.collider.bounds.min.y) * rb.mass * 500 * Time.deltaTime;
				rb.AddForce(Vector3.up * y, ForceMode.Force);
				Vector3 velocity = rb.velocity;
				velocity.x = Mathf.Clamp(velocity.x, -2, 2);
				velocity.y = Mathf.Clamp(velocity.y, -1, 1);
				velocity.z = 0;
				rb.velocity = velocity;
			}
		}
		if ((prevPosition - transform.position).sqrMagnitude > 1) {
			Rigidbody[] rbs = (Rigidbody[])FindObjectsOfType(typeof(Rigidbody));
			for (int i=0; i<rbs.Length; ++i) {
				rbs[i].WakeUp();
			}
			prevPosition = transform.position;
		}
	}
	
	IEnumerator Freeze () {
        restoreAnimateTexture = animateTexture;
		animateTexture = false;
        if (animation != null) {
            animation.Stop();
        }
		freezeCollider.gameObject.active = true;
		originalMaterial = water.material;
		water.material = freezeMaterial;
		foreach (ParticleEmitter emitter in GetComponentsInChildren(typeof(ParticleEmitter))) {
			emitter.emit = false;
		}
		for (int i=0; i<rbCount; ++i) {
			Rigidbody rb = bodies[i];
			if (rb != null && rb.gameObject.active) {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
			}
        }
		yield return 0;
	}
	
	IEnumerator Melt () {
        animateTexture = restoreAnimateTexture;
        if (animation != null) {
            animation.Play();
        }
		freezeCollider.gameObject.active = false;
		water.material = originalMaterial;
		foreach (ParticleEmitter emitter in GetComponentsInChildren(typeof(ParticleEmitter))) {
			emitter.emit = true;
		}
		for (int i=0; i<rbCount; ++i) {
			Rigidbody rb = bodies[i];
			if (rb != null && rb.gameObject.active) {
                rb.isKinematic = false;
			}
        }
		yield return 0;
	}
	
	void OnSwitch () {
		if (!frozen) {
			frozen = true;
			Game.fx.PlaySound(freezeClip);
			StartCoroutine(Freeze());
		}
		else {
			frozen = false;
			Game.fx.PlaySound(meltClip);
			StartCoroutine(Melt());
		}
	}
	
	void OnSwitchUp () {
		OnSwitch();
	}
	
	void OnSwitchDown () {
		OnSwitch();
	}
	
	void OnTriggerEnter (Collider collider) {
		int ignoreMask = Game.projectilesMask | Game.missilesMask | Game.grenadesMask;
		int colliderLayer = collider.gameObject.layer;
		int colliderMask = 1 << colliderLayer;
		Rigidbody rb = collider.rigidbody;
		if (rb != null && rb.isKinematic) {
			rb = null;
		}
        if ((colliderMask & ignoreMask) != 0) {
            rb.velocity *= 0.2f;
            rb.velocity += new Vector3(0, -1, 0);
        }
		if ((colliderLayer == Game.characterLayer ||
             (colliderMask & ignoreMask) == 0) && rb != null && rb.velocity.sqrMagnitude > 25) {
            Game.fx.PlaySound(splashClip);
            GameObject ripples = (GameObject)Instantiate(ripplesAsset);
            ripples.transform.parent = collider.transform;
            ripples.transform.position = collider.bounds.center;
            GameObject splash = (GameObject)Instantiate(splashAsset);
            splash.transform.position = collider.bounds.center;
			GameObject bubbles = (GameObject)Instantiate(bubblesAsset);
			bubbles.transform.parent = collider.transform;
            if (colliderLayer == Game.characterLayer) {
                bubbles.transform.position = collider.bounds.center - new Vector3(0, 1, 0);
            }
            else {
                bubbles.transform.position = collider.bounds.center;
            }
		}
		if (rb != null && ((colliderMask & ignoreMask) == 0) && colliderLayer != Game.characterLayer) {
			if (rbCount < rbMax) {
				Vector3 velocity = rb.velocity;
				velocity.y = 0;
				rb.angularVelocity = Vector3.zero;
				rb.velocity = velocity;
				bodies[rbCount++] = rb;
			}
		}
		collider.SendMessage("OnWaterEnter", this, SendMessageOptions.DontRequireReceiver);
	}

    public void CanSwim (Rigidbody rb) {
        for (int i=0; i<rbCount; ++i) {
            if (bodies[i] == rb) {
                bodies[i] = bodies[rbCount-1];
                --rbCount;
                break;
            }
        }        
    }
	
	void OnTriggerExit (Collider collider) {
        int colliderLayer = collider.gameObject.layer;
		collider.SendMessage("OnWaterExit", this, SendMessageOptions.DontRequireReceiver);
		Rigidbody rb = collider.rigidbody;
		if (rb != null && rb.isKinematic) {
			rb = null;
		}
		if (rb != null) {
			for (int i=0; i<rbCount; ++i) {
				if (bodies[i] == rb) {
					bodies[i] = bodies[rbCount-1];
					--rbCount;
					break;
				}
			}
		}
		Ripple ripples = (Ripple)collider.GetComponentInChildren(typeof(Ripple));
		if (ripples != null) {
			ripples.transform.parent = null;
		}
		Bubbles bubbles = (Bubbles)collider.GetComponentInChildren(typeof(Bubbles));
		if (bubbles != null) {
			Destroy(bubbles.gameObject);
		}
        if (colliderLayer == Game.characterLayer) {
            Game.fx.PlaySound(splashClip);
            GameObject ripplesObject = (GameObject)Instantiate(ripplesAsset);
            ripplesObject.transform.position = collider.bounds.center;
            GameObject splash = (GameObject)Instantiate(splashAsset);
            splash.transform.position = collider.bounds.center;
        }

	}
}
