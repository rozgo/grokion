using UnityEngine;
using System.Collections;

public class Game : StateMachine {
	
	public static bool debug = true;
	public static Game game;
	public static Grid grid;
	public static Hud hud;
	public static Director director;
	public static FX fx;
	public static Character character;
	public static Spirit spirit;
	public static Checkpoint[] checkpoints;
	public static Door[] doors;
	public static GameObject cache;
	public static string door;
	public static float realDeltaTime = 0.01f;
	public static float realTime = 0;
	public static bool casual = false;
	public static int characterLayer;
	public static int projectilesLayer;
	public static int missilesLayer;
	public static int grenadesLayer;
	public static int flamesLayer;
	public static int defaultLayer;
	public static int hiddenLayer;
	public static int hudLayer;
	public static int ignoreRaycastLayer;
	public static LayerMask characterMask;
	public static LayerMask projectilesMask;
	public static LayerMask missilesMask;
	public static LayerMask grenadesMask;
	public static LayerMask flamesMask;
	public static LayerMask defaultMask;
	public static LayerMask hiddenMask;
	public static LayerMask hudMask;
	public static LayerMask ignoreRaycastMask;
	
	public Texture2D worldTexture;
	public Texture2D sceneLightMap; // One of Jonny's silly additions
	public Material[] worldMaterials;
	public float setMipMapBias = -0.5f; // One of Jonny's silly additions
	public AudioClip musicClip;
	public string alternateMusicForKey;
	public AudioClip alternateMusicClip;
	public Material dramaBoxMaterial;
	public AudioClip ctrlClip;
	
	GameObject controlPanel;
	float lastForcedGC = 0;
	
	IEnumerator GameOverCoroutine (bool violent) {
		
		Game.fx.Flash(0.1f);
		Game.character.Limbo();
		if (violent) {
			Game.fx.StopMusic();
			Game.fx.Death();
			yield return new WaitForSeconds(0.2f);
			Disintegrate disintegrate = character.gameObject.GetComponent<Disintegrate>();
			if (disintegrate != null) {
				disintegrate.DesintegrateFX();
				yield return new WaitForSeconds(disintegrate.deathDelay);
			}
		}
		character.gameObject.SetActiveRecursively(false);
		Game.fx.Flash(0.1f);
		yield return new WaitForSeconds(0.2f);
		Game.fx.Flash(0.1f);
		yield return new WaitForSeconds(0.2f);
		yield return new WaitForSeconds(2);
		if (grid == null) {
			Game.fx.FadeOut(1);
		}
		if (casual) {
			if (character.MaxHP() < 200) {
				PlayerPrefs.SetInt("HP", 100);
			}
			else {
				PlayerPrefs.SetInt("HP", (int)(character.MaxHP() * 0.5f));
			}
		}
		else {
        	PlayerPrefs.SetInt("HP", (int)(character.MaxHP() * 0.3f));
		}
		if (grid == null) {
			yield return new WaitForSeconds(2);
			Application.LoadLevel("GameOver");
		}
		else {
			yield return new WaitForSeconds(2);
			Application.LoadLevelAdditive("GridEnd");
			Game.director.gameObject.SetActiveRecursively(false);
		}
	}
	
	public void GameOver (bool violent) {
		StartCoroutine(GameOverCoroutine(violent));
	}
	
	public void GarbageCollect () {
		if ((Time.time - lastForcedGC) > 5) {
			System.GC.Collect();
			lastForcedGC = Time.time;
		}
	}

	void Awake () {
		realTime = 0;
		grid = gameObject.GetComponent<Grid>();
		if (PlayerPrefs.HasKey("Casual")) {
			if (!PlayerPrefs.HasKey("CasualStigmata")) {
				PlayerPrefs.SetInt("CasualStigmata", 1);
			}
			casual = true;
		}
		defaultLayer = LayerMask.NameToLayer("Default");
		hiddenLayer = LayerMask.NameToLayer("Hidden");
		flamesLayer = LayerMask.NameToLayer("Flames");
		projectilesLayer = LayerMask.NameToLayer("Projectiles");
		missilesLayer = LayerMask.NameToLayer("Missiles");
		grenadesLayer = LayerMask.NameToLayer("Grenades");
		characterLayer = LayerMask.NameToLayer("Character");
		hudLayer = LayerMask.NameToLayer("Hud");
		ignoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
		defaultMask = 1 << defaultLayer;
		hiddenMask = 1 << hiddenLayer;
		flamesMask = 1 << flamesLayer;
		projectilesMask = 1 << projectilesLayer;
		missilesMask = 1 << missilesLayer;
		grenadesMask = 1 << grenadesLayer;
		characterMask = 1 << characterLayer;
		hudMask = 1 << hudLayer;
		ignoreRaycastMask = 1 << ignoreRaycastLayer;
		foreach (Material worldMaterial in worldMaterials) {
			worldMaterial.mainTexture = worldTexture;
			worldMaterial.mainTexture.mipMapBias = setMipMapBias;  // One of Jonny's silly additions
            if (worldMaterial.HasProperty("_LightMap")) {
                worldMaterial.SetTexture("_LightMap", sceneLightMap); // One of Jonny's silly additions
            }
		}
		game = this;
		if (FindObjectsOfType(GetType()).Length > 1) {
			Debug.LogError("Multiple singletons of type: "+GetType());
		}
		cache = new GameObject("Cache");
		
		if (grid == null) {
			GameObject hudObject = (GameObject)Instantiate(Resources.Load("Hud"));
			hud = (Hud)hudObject.GetComponent(typeof(Hud));
			hud.EnableControls(false);
		}
		else {
			GameObject hudObject = (GameObject)Instantiate(Resources.Load("GridHud"));
			hud = (Hud)hudObject.GetComponent(typeof(Hud));
			hud.sharedMaterial.mainTexture = (Texture2D)Resources.Load("GridHud", typeof(Texture2D));
			hud.sharedActiveMaterial.mainTexture = (Texture2D)Resources.Load("GridHud", typeof(Texture2D));
			hud.EnableControls(false);
		}

		Instantiate(Resources.Load("Director"));
		director = (Director)FindObjectOfType(typeof(Director));
		director.transform.rotation = Quaternion.identity;
		fx = (FX)FindObjectOfType(typeof(FX));
		checkpoints = (Checkpoint[])FindObjectsOfType(typeof(Checkpoint));
		doors = (Door[])FindObjectsOfType(typeof(Door));
	}
	
	void Start () {
        string trail = "Trail|" + Application.loadedLevelName;
        PlayerPrefs.SetInt(trail, 1);
        if (alternateMusicForKey.Length > 0 && 
        	alternateMusicClip != null && 
        	PlayerPrefs.HasKey(alternateMusicForKey)) {
        	Game.fx.PlayMusic(alternateMusicClip);
        }
        else {
			Game.fx.PlayMusic(musicClip);
        }
		if (dramaBoxMaterial == null) {
			Game.fx.dramaBox.SetActiveRecursively(false);
		}
		else {
			Game.fx.dramaBox.renderer.material = dramaBoxMaterial;
		}
		if (character == null) {
			if (Game.door != null) {
				bool foundDoor = false;
				foreach (Door door in doors) {
					if (door.name == Game.door) {
						foundDoor = true;
                        PlayerPrefs.SetString("Checkpoint", door.name);
						door.Spawn();
					}
				}
				if (!foundDoor) {
					Debug.LogError( "Door not found... "+Game.door);
				}
			}
			else {
				Door debugDoor = null;
				if (debug) {
					foreach (Door door in doors) {
						if (door.debugSpawn) {
							debugDoor = door;
							break;
						}
					}
					if (debugDoor != null) {
                        PlayerPrefs.SetString("Checkpoint", debugDoor.name);
						debugDoor.Spawn();
					}
				}
				if (debugDoor == null && checkpoints.Length > 0) {
					int checkpointIndex = Random.Range(0,checkpoints.Length);
					if (checkpoints[checkpointIndex].gameObject.active) {
						checkpoints[checkpointIndex].Spawn();
					}
				}
			}
		}
		else {
			StartCoroutine(CharacterInLevel());
		}
		Game.door = null;
		GarbageCollect();
	}
	
	IEnumerator CharacterInLevel () {
		yield return 0;
		character.SetMarionette(false);
		character.Rest();
	}
	
	void Update () {
		realDeltaTime = Time.realtimeSinceStartup - realTime;
		realTime = Time.realtimeSinceStartup;
		state.OnUpdate();
	}
	
	IEnumerator WaitForRealSeconds(float time) {
	    float startTime = Time.realtimeSinceStartup; 
	    while (Time.realtimeSinceStartup - startTime < time) 
	        yield return 0;
	}
	
	public void ShowControlPanel () {
		StartCoroutine(ShowControlPanelTask());
	}
	
	public void HideControlPanel () {
		StartCoroutine(HideControlPanelTask());
	}
	
	IEnumerator ShowControlPanelTask () {
		Time.timeScale = 0;
		Game.fx.ResetLetterBox();
		Game.hud.gameObject.SetActiveRecursively(false);
		Game.director.gameObject.SetActiveRecursively(false);
		yield return 0;
		controlPanel = (GameObject)Instantiate(Resources.Load("ControlPanel"));
	}
	
	IEnumerator HideControlPanelTask () {
		Destroy(controlPanel);
		Time.timeScale = 1;
		Game.hud.gameObject.SetActiveRecursively(true);
        Game.hud.UpdateWeapons();
		Game.director.gameObject.SetActiveRecursively(true);
		if (dramaBoxMaterial == null) {
			Game.fx.dramaBox.SetActiveRecursively(false);
		}
		else {
			Game.fx.dramaBox.renderer.material = dramaBoxMaterial;
		}
		Game.fx.avatarCard.SetActiveRecursively(false);
		yield return 0;
		Game.fx.PlaySound(ctrlClip);
		yield return 0;
		GarbageCollect();
	}
}
