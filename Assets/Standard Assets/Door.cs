using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {
	
	public enum DoorType {
		Normal,
		Opened,
        Charged,
	}
	
	public string nextDoor;
	public GameObject field;
	public bool debugSpawn = false;
	public AudioClip doorClip;
	public DoorType doorType = DoorType.Normal;
	public string unlockKey;
	public Material lockedMaterial;
	public Material chargedMaterial;
	
	bool spawning = false;
	bool locked = false;
	
	void Awake () {
		if (nextDoor == "" && !debugSpawn) {
			Debug.LogError("Door: " + name + " goes nowhere");
		}
	}
	
	void Start () {
		if (doorType == DoorType.Opened) {
			field.renderer.enabled = false;
			collider.isTrigger = true;
			collider.gameObject.layer = Game.ignoreRaycastLayer;
		}
		else if (doorType == DoorType.Charged) {
			field.renderer.material = chargedMaterial;
		}
		if (unlockKey.Length > 0) {
			if (PlayerPrefs.HasKey(unlockKey)) {
				locked = false;
			}
			else {
				locked = true;
				field.renderer.material = lockedMaterial;
			}
		}
	}
	
	public void Open () {
		if (!collider.isTrigger && !locked) {
			Game.fx.PlaySound(doorClip);
			field.renderer.enabled = false;
			collider.isTrigger = true;
			collider.gameObject.layer = Game.ignoreRaycastLayer;
		}
	}
	
	public void Close () {
		if (doorType != DoorType.Opened) {
			if (collider.isTrigger) {
				field.renderer.enabled = true;
				collider.isTrigger = false;
				collider.gameObject.layer = Game.defaultLayer;
			}
		}
	}

	void OnSwitchDown() {
		OnSwitch();
	}
	
	void OnSwitchUp () {
		OnSwitch();
	}
	
	void OnSwitch () {
		if (collider.isTrigger) {
            Close();
		}
		else {
            Open();
		}
	}
	
	public void Spawn () {
		StartCoroutine(SpawnCoroutine());
	}
	
	void OnChildBecameInvisible (GameObject child) {
		Close();
	}
	
	IEnumerator SpawnCoroutine () {
		spawning = true;
		Open();
		yield return new WaitForSeconds(0.1f);
		GameObject characterObject =
            (GameObject)Instantiate(Resources.Load("Avatar",typeof(GameObject)));
		Character character = (Character)characterObject.GetComponent(typeof(Character));
		if (transform.right.y > 0.9f) {
			characterObject.transform.position =
				transform.position + transform.right * 0.6f;
			character.Jump();
		}
		else if (transform.right.y < -0.9f) {
			characterObject.transform.position =
				transform.position + transform.right * 0.6f;
			character.Fall();
		}
		else {
			characterObject.transform.position =
				transform.position + transform.right * 0.5f + new Vector3(0, -1.1f, 0);

			float prevLookX = Mathf.Sign(character.transform.right.x);
			if (transform.right.x < 0) {
				characterObject.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
			}
			else {
				characterObject.transform.rotation = Quaternion.AngleAxis(0, Vector3.up);
			}
			float newLookX = Mathf.Sign(character.transform.right.x);
			if (prevLookX != newLookX && Mathf.Sign(Game.hud.pivot.crosshair.x) != newLookX) {
				Game.hud.pivot.crosshair.x *= -1;
			}
			character.Run();
		}	
		yield return new WaitForSeconds(0.2f);
		character.SetMarionette(false);
		spawning = false;
	}
	
	IEnumerator Load (Collider collider) {
		Character character = (Character)collider.GetComponent(typeof(Character));
		character.Pause();
		float timer = Time.time + 0.2f;
		while (Time.time < timer) {
			character.transform.position = 
				Vector3.Lerp(character.transform.position, 
				transform.position - new Vector3(0, 0.5f, 0), Time.deltaTime * 5);
			yield return 0;
		}
		Game.fx.FadeOut(0.3f);
		yield return new WaitForSeconds(doorClip.length);
		string[] doorInfo = nextDoor.Split('|');
		if (doorInfo.Length == 3) {
			Game.door = nextDoor;
			Application.LoadLevel(doorInfo[1]);
		} else {
			Application.LoadLevel("GameOver");
		}
	}
	
	void OnParticleCollision (GameObject particle) {
		if (particle.gameObject.layer == Game.flamesLayer) {
			if (doorType != DoorType.Charged) {
				Open();
			}
		}
	}
	
	void OnExplosion (GameObject exploded) {
		if (Vector3.Distance(transform.position, exploded.transform.position) < 2) {
            if (doorType != DoorType.Charged) {
                Open();
            }
		}
	}
	
	void OnCollisionEnter (Collision collision) {
        if (doorType == DoorType.Charged) {
            if (collision.collider.gameObject.layer == Game.projectilesLayer) {
                Transform charge = collision.collider.gameObject.transform.Find("Charge(Clone)");
                if (charge != null) {
                    Open();
                }
            }
        }
        else {
            int collisionMask = Game.projectilesMask | Game.grenadesMask | Game.missilesMask;
            if (((1 << collision.collider.gameObject.layer) & collisionMask) != 0) {
                Open();
            }
        }
	}
	
	void OnTriggerEnter (Collider collider) {
		if (!spawning && collider.gameObject.layer == Game.characterLayer) {
			StartCoroutine(Load(collider));
		}
	}
	
	void OnTriggerExit (Collider collider) {
		if (collider.gameObject.layer == Game.characterLayer) {
			Close();
		}
	}
	
}
