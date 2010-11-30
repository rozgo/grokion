using UnityEngine;
using System.Collections;

public class Hud : MonoBehaviour {
	
	public AudioClip skipClip;
	public GameObject energyTankFullAsset;
	public GameObject energyTankEmptyAsset;
	public GameObject controls;
	public GameObject pivotBase;
    public GameObject meter;
	public Pivot pivot;
	public Button buttonA;
	public Button buttonB;
	public Button menuButton;
	public Button skipButton;
	public TextMesh hpText;
	public TextMesh message;
	public TextMesh fps;
    public TextMesh room;
	public TextMesh time;
    public GameObject offProjectile;
    public GameObject onProjectile;
	public TextMesh projectileCount;
    public GameObject offMissile;
    public GameObject onMissile;
	public TextMesh missileCount;
    public GameObject offFlamethrower;
    public GameObject onFlamethrower;
	public TextMesh flameThrowerCount;
    public GameObject offGrenade;
    public GameObject onGrenade;
	public TextMesh grenadeCount;
    public GameObject offGrapple;
    public GameObject onGrapple;
	public TextMesh grappleCount;
	public TextMesh killsCount;
    public Material sharedMaterial;
	public Material sharedActiveMaterial;
    public Mesh tokenOnMesh;
    public Mesh tokenOffMesh;
    public GameObject[] tokenSlots;
    
	bool skipMessage = false;
	bool controlsEnabled = false;
	int hudLayer;
	GameObject[] emptyEnergyTanks;
	GameObject[] fullEnergyTanks;
    bool showMeter = false;
    Token[] tokens;

	void Awake () {
		if (FindObjectsOfType(GetType()).Length > 1) {
			Debug.LogError("Multiple singletons of type: "+GetType());
		}
		hudLayer = LayerMask.NameToLayer("Hud");
		if (Game.grid != null) {
			emptyEnergyTanks = new GameObject[0];
			fullEnergyTanks = new GameObject[0];
		}
		else {
			emptyEnergyTanks = new GameObject[20];
			fullEnergyTanks = new GameObject[20];
			for (int y=0; y<2; ++y) {
				for (int x=0; x<10; ++x) {
					int count = x + 10 * y;
					Vector3 position = new Vector3(-5.28f + x * 0.22f, 3.80f - y * 0.22f, 3);
					emptyEnergyTanks[count] = (GameObject)Instantiate(energyTankEmptyAsset);
					emptyEnergyTanks[count].transform.parent = transform;
					emptyEnergyTanks[count].transform.localPosition = position;
					emptyEnergyTanks[count].transform.localRotation = Quaternion.Euler(270, 180, 0);
					emptyEnergyTanks[count].layer = hudLayer;
					emptyEnergyTanks[count].renderer.enabled = false;
					fullEnergyTanks[count] = (GameObject)Instantiate(energyTankFullAsset);
					fullEnergyTanks[count].transform.parent = transform;
					fullEnergyTanks[count].transform.localPosition = position;
					fullEnergyTanks[count].transform.localRotation = Quaternion.Euler(270, 180, 0);
					fullEnergyTanks[count].layer = hudLayer;
					fullEnergyTanks[count].renderer.enabled = false;
				}
			}
		}
        onProjectile.active = false;
        onMissile.active = false;
        onFlamethrower.active = false;
        onGrenade.active = false;
        onGrapple.active = false;
        tokens = (Token[])FindObjectsOfType(typeof(Token));
		
		if (iPhoneSettings.model == "iPad") {
			controls.transform.localScale *= 0.55f;
		}
    }

    void Start () {
        if (showMeter) {
            meter.layer = Game.hudLayer;
        }
        else {
            meter.layer = Game.hiddenLayer;
        }
	}

	IEnumerator WaitForRealSeconds(float time) {
	    float startTime = Time.realtimeSinceStartup;
	    while (Time.realtimeSinceStartup - startTime < time)
	        yield return 1;
	}
	
	public IEnumerator ShowMessage (string msg, float timer) {
		message.text = msg;
		skipMessage = false;
		if (timer < 0) {
			while (!skipMessage) {
				yield return 0;
			}
			Game.fx.PlaySound(skipClip);
		}
		else {
			while (timer >= 0) {
				timer -= Game.realDeltaTime;
				yield return 0;
			}
		}
		message.text = "";
	}
	
	public void Message (string msg, float timer) {
		StartCoroutine(ShowMessage(msg, timer));
	}
	
	public void SetHP (int hp, int tankCount) {
		int tankLevel = hp / 100;
		int fragHP = Mathf.Max(hp % 100, 0);
		hpText.text = string.Format("{0:00}",fragHP);
		SetEnergyTankLevel(tankLevel, tankCount);
	}

    public void HideMeter () {
        showMeter = false;
        meter.layer = Game.hiddenLayer;
    }

    public void SetMeter (float weight) {
        showMeter = true;
        meter.layer = hudLayer;
        float hpRatio = Mathf.Clamp01(weight);
        hpRatio = ((int)(hpRatio/0.0625f))*0.0625f;
        Vector2 texoff = new Vector2(0,hpRatio*0.5f+0.5f);
        meter.renderer.material.mainTextureOffset = texoff;
    }

	void SetEnergyTankLevel (int level, int total) {
		if (Game.grid == null) {
			for (int y=0; y<2; ++y) {
				for (int x=0; x<10; ++x) {
					int count = x + 10 * y;
					if (count < level) {
						fullEnergyTanks[count].renderer.enabled = true;
						emptyEnergyTanks[count].renderer.enabled = false;
					}
					else if (count < total) {
						fullEnergyTanks[count].renderer.enabled = false;
						emptyEnergyTanks[count].renderer.enabled = true;
					}
					else {
						fullEnergyTanks[count].renderer.enabled = false;
						emptyEnergyTanks[count].renderer.enabled = false;
					}
				}
			}
		}
	}
	
	public void EnableControls (bool enable) {
		//try {throw (new System.Exception());} catch (System.Exception e) {Debug.Log(e.StackTrace);}
		controlsEnabled = enable;
		if (enable) {
			menuButton.gameObject.layer = hudLayer;
			skipButton.gameObject.layer = Game.hiddenLayer;
			hpText.gameObject.active = true;
			//fps.gameObject.active = true;
			if (room != null) {
            	room.gameObject.active = true;
			}
			foreach (GameObject emptyEnergyTank in emptyEnergyTanks) {
				emptyEnergyTank.layer = hudLayer;
			}
			foreach (GameObject fullEnergyTank in fullEnergyTanks) {
				fullEnergyTank.layer = hudLayer;
			}
			pivot.gameObject.layer = hudLayer;
			pivotBase.gameObject.layer = hudLayer;
			buttonA.gameObject.layer = hudLayer;
			buttonB.gameObject.layer = hudLayer;
            if (showMeter) {
                meter.layer = hudLayer;
            }
            foreach (Transform child in transform) {
                if (child.name.StartsWith("hud_")) {
                    child.gameObject.active = true;
                }
            }
            UpdateTokenSlots();
            UpdateWeapons();
		}
		else {
            foreach (Transform child in transform) {
                if (child.name.StartsWith("hud_")) {
                    child.gameObject.active = false;
                }
            }
            foreach (GameObject tokenSlot in tokenSlots) {
            	tokenSlot.active = false;
            }
			menuButton.gameObject.layer = Game.hiddenLayer;
			skipButton.gameObject.layer = hudLayer;
			hpText.gameObject.active = false;
			//fps.gameObject.active = false;
			if (room != null) {
            	room.gameObject.active = false;
			}
			foreach (GameObject emptyEnergyTank in emptyEnergyTanks) {
				emptyEnergyTank.layer = Game.hiddenLayer;
			}
			foreach (GameObject fullEnergyTank in fullEnergyTanks) {
				fullEnergyTank.layer = Game.hiddenLayer;
			}
			pivot.SendMessage("OnFingerCancel", pivot, SendMessageOptions.DontRequireReceiver);
			buttonA.SendMessage("OnFingerCancel", buttonA, SendMessageOptions.DontRequireReceiver);
			buttonB.SendMessage("OnFingerCancel", buttonB, SendMessageOptions.DontRequireReceiver);
			pivot.gameObject.layer = Game.hiddenLayer;
			pivotBase.gameObject.layer = Game.hiddenLayer;
			buttonA.gameObject.layer = Game.hiddenLayer;
			buttonB.gameObject.layer = Game.hiddenLayer;
            meter.layer = Game.hiddenLayer;
		}
	}
	
	void OnButtonUp (Button button) {
		Debug.Log("Hud button pressed:" + button.ToString());
		if (controlsEnabled) {
			if (button == menuButton) {
				Game.game.ShowControlPanel();
			}
		}
		else {
			if (button == skipButton) {
				skipMessage = true;
			}
		}
        if (button.gameObject == onProjectile) {
            Game.character.SetWeapon(Character.Weapon.Projectile);
            Game.fx.PlaySound(skipClip);
            UpdateWeapons();
        }
        else if (button.gameObject == onMissile) {
            Game.character.SetWeapon(Character.Weapon.Missile);
            Game.fx.PlaySound(skipClip);
            UpdateWeapons();
        }
        else if (button.gameObject == onFlamethrower) {
            Game.character.SetWeapon(Character.Weapon.FlameThrower);
            Game.fx.PlaySound(skipClip);
            UpdateWeapons();
        }
        else if (button.gameObject == onGrenade) {
            Game.character.SetWeapon(Character.Weapon.Grenade);
            Game.fx.PlaySound(skipClip);
            UpdateWeapons();
        }
        else if (button.gameObject == onGrapple) {
            Game.character.SetWeapon(Character.Weapon.Grapple);
            Game.fx.PlaySound(skipClip);
            UpdateWeapons();
        }
	}
	
	GameObject FindToken (string tokenName) {
		foreach (Token token in tokens) {
			if (token.name == tokenName) {
				return token.gameObject;
			}
		}
		return null;
	}
	
	public void UpdateTokenSlots () {
        for (int i=0; i<3; ++i) {
        	string tokenName = "Token|" + Application.loadedLevelName + "|" + (i+1).ToString();
        	GameObject token = FindToken(tokenName);
        	MeshFilter meshFilter = (MeshFilter)tokenSlots[i].GetComponent(typeof(MeshFilter));
        	tokenSlots[i].active = true;
        	if (token == null) {
        		meshFilter.mesh = tokenOffMesh;
        	}
        	else {
        		if (PlayerPrefs.HasKey(tokenName)) {
        			meshFilter.mesh = tokenOnMesh;
        		}
        		else {
        			meshFilter.mesh = tokenOffMesh;
        		}
        	}
        }
	}
	
	public void UpdateAmmo () {
		if (Game.grid != null) {
			projectileCount.text = Game.grid.projectileCount.ToString("D3");
			missileCount.text = Game.grid.missileCount.ToString("D3");
			grenadeCount.text = Game.grid.grenadeCount.ToString("D3");
			flameThrowerCount.text = Game.grid.flameThrowerCount.ToString("D3");
			grappleCount.text = Game.grid.grappleCount.ToString("D3");
		}
	}

    public void UpdateWeapons () {
        onProjectile.renderer.material = sharedMaterial;
        onMissile.renderer.material = sharedMaterial;
        onFlamethrower.renderer.material = sharedMaterial;
        onGrenade.renderer.material = sharedMaterial;
        onGrapple.renderer.material = sharedMaterial;
        if (PlayerPrefs.HasKey("Projectile") || (Game.grid != null && Game.grid.projectileCount > 0)) {
            onProjectile.active = true;
            offProjectile.active = false;
        }
        else {
            onProjectile.active = false;
            offProjectile.active = true;
        }
        if (PlayerPrefs.HasKey("Missile") || (Game.grid != null && Game.grid.missileCount > 0)) {
            onMissile.active = true;
            offMissile.active = false;
        }
        else {
            onMissile.active = false;
            offMissile.active = true;
        }
        if (PlayerPrefs.HasKey("FlameThrower") || (Game.grid != null && Game.grid.flameThrowerCount > 0)) {
            onFlamethrower.active = true;
            offFlamethrower.active = false;
        }
        else {
            onFlamethrower.active = false;
            offFlamethrower.active = true;
        }
        if (PlayerPrefs.HasKey("Grenade") || (Game.grid != null && Game.grid.grenadeCount > 0)) {
            onGrenade.active = true;
            offGrenade.active = false;
        }
        else {
            onGrenade.active = false;
            offGrenade.active = true;
        }
        if (PlayerPrefs.HasKey("Grapple") || (Game.grid != null && Game.grid.grappleCount > 0)) {
            onGrapple.active = true;
            offGrapple.active = false;
        }
        else {
            onGrapple.active = false;
            offGrapple.active = true;
        }
        Character.Weapon weapon = (Character.Weapon)PlayerPrefs.GetInt("Weapon", 0);
        Color color = new Color(180/255.0f, 120/255.0f, 50/255.0f);
        if (weapon == Character.Weapon.Projectile) {
            onProjectile.renderer.material.SetColor("_TintColor", color);
        }
        else if (weapon == Character.Weapon.Missile) {
            onMissile.renderer.material.SetColor("_TintColor", color);
        }
        else if (weapon == Character.Weapon.FlameThrower) {
            onFlamethrower.renderer.material.SetColor("_TintColor", color);
        }
        else if (weapon == Character.Weapon.Grenade) {
            onGrenade.renderer.material.SetColor("_TintColor", color);
        }
        else if (weapon == Character.Weapon.Grapple) {
            onGrapple.renderer.material.SetColor("_TintColor", color);
        }
    }
    
    public IEnumerator Dialog (string dialog) {
		string[] message = TextTable.GetLines(dialog);
		for (int i=0; i<message.Length; ++i) {
			string tableLine = message[i];
			if (tableLine.StartsWith("{")) {
				int d = tableLine.IndexOf("}");
				string[] parts = tableLine.Substring(1, d - 1).Trim().Split('|');
				string card = "";
				string vo = "";
				if (parts.Length > 1) {
					vo = parts[1];
				}
				if (parts.Length > 0) {
					card = parts[0];
				}
				tableLine = tableLine.Substring(d + 1, tableLine.Length - (d + 1)).Trim();
				if (card.Length > 0) {
					Game.fx.ShowAvatarCard(card);
				}
				if (vo.Length > 0) {
					AudioClip voClip = (AudioClip)Resources.Load("VO/" + vo, typeof(AudioClip));
					if (voClip != null) {
						Game.fx.PlaySound(voClip);
					}
				}
			}
			yield return Game.hud.StartCoroutine(Game.hud.ShowMessage(tableLine, -1));
		}
		Game.fx.HideAvatarCard();
	}
	
	bool adjusted = false;
	void FixedUpdate () {
		
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft) {
			
			if (iPhoneSettings.screenOrientation != iPhoneScreenOrientation.LandscapeLeft) {
				iPhoneSettings.screenOrientation = iPhoneScreenOrientation.LandscapeLeft;
				if (!adjusted) {
					adjusted = true;
					foreach (Transform child in transform) {
						child.transform.position += new Vector3(0, 2, 0);
					}
				}
			}
			controls.transform.localPosition = new Vector3(-6, -3, 10);
			buttonA.transform.localPosition = new Vector3(16.0f+7, 1.5f, 0);
			buttonA.transform.localScale = new Vector3(1, 1, 1);
			buttonB.transform.localPosition = new Vector3(14.0f+7, -0.5f, 0);
			buttonB.transform.localScale = new Vector3(1, 1, 1);
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.rect = new Rect(0, 0, 1, 1);
			if (iPhoneSettings.model == "iPad") {
				camera.orthographicSize = 6;
			}
			else {
				camera.orthographicSize = 4;
			}
		}
		if (Input.deviceOrientation == DeviceOrientation.LandscapeRight) {
			
			if (iPhoneSettings.screenOrientation != iPhoneScreenOrientation.LandscapeRight) { 
				iPhoneSettings.screenOrientation = iPhoneScreenOrientation.LandscapeRight;
				if (!adjusted) {
					adjusted = true;
					foreach (Transform child in transform) {
						child.transform.position += new Vector3(0, 2, 0);
					}
				}
			}
			controls.transform.localPosition = new Vector3(-6, -3, 10);
			buttonA.transform.localPosition = new Vector3(16.0f+7, 1.5f, 0);
			buttonA.transform.localScale = new Vector3(1, 1, 1);
			buttonB.transform.localPosition = new Vector3(14.0f+7, -0.5f, 0);
			buttonB.transform.localScale = new Vector3(1, 1, 1);
			camera.clearFlags = CameraClearFlags.Nothing;
			camera.rect = new Rect(0, 0, 1, 1);
			if (iPhoneSettings.model == "iPad") {
				camera.orthographicSize = 6;
			}
			else {
				camera.orthographicSize = 4;
			}
		}
		
		if (iPhoneSettings.model == "iPad") {
			
			if (Input.deviceOrientation == DeviceOrientation.Portrait) {
				
				if (iPhoneSettings.screenOrientation != iPhoneScreenOrientation.Portrait) { 
					iPhoneSettings.screenOrientation = iPhoneScreenOrientation.Portrait;
					if (adjusted) {
						adjusted = false;
						foreach (Transform child in transform) {
							child.transform.position -= new Vector3(0, 2, 0);
						}
					}
				}
				controls.transform.localPosition = new Vector3(-4, 0.5f, 10);
				buttonA.transform.localPosition = new Vector3(16.0f, 1.5f, 0);
				buttonA.transform.localScale = new Vector3(1, 1, 1);
				buttonB.transform.localPosition = new Vector3(14.0f, -0.5f, 0);
				buttonB.transform.localScale = new Vector3(1, 1, 1);
				camera.clearFlags = CameraClearFlags.SolidColor;
				camera.rect = new Rect(0, 0, 1, 0.5f);
				camera.orthographicSize = 4;
			}
			if (Input.deviceOrientation == DeviceOrientation.PortraitUpsideDown) {
				
				if (iPhoneSettings.screenOrientation != iPhoneScreenOrientation.PortraitUpsideDown) { 
					iPhoneSettings.screenOrientation = iPhoneScreenOrientation.PortraitUpsideDown;
					if (adjusted) {
						adjusted = false;
						foreach (Transform child in transform) {
							child.transform.position -= new Vector3(0, 2, 0);
						}
					}
				}
				controls.transform.localPosition = new Vector3(-4, 0.5f, 10);
				buttonA.transform.localPosition = new Vector3(16.0f, 1.5f, 0);
				buttonA.transform.localScale = new Vector3(1, 1, 1);
				buttonB.transform.localPosition = new Vector3(14.0f, -0.5f, 0);
				buttonB.transform.localScale = new Vector3(1, 1, 1);
				camera.clearFlags = CameraClearFlags.SolidColor;
				camera.rect = new Rect(0, 0, 1, 0.5f);
				camera.orthographicSize = 4;
			}
		}
	}

}
