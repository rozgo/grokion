using UnityEngine;
using System.Collections;

public class Hotspot : MonoBehaviour {
	
	public enum HideSpirit {
		Never,
		OnDone,
		OnExit,
	}
	
    public enum PowerUp {
        None,
        Projectile,
        Missile,
        FlameThrower,
        Grenade,
        Swimsuit,
        Armor,
        GravityBoots,
        Grapple,
        Map,
        Charge,
        EnergyTank,
    }
	
	public string lockKey;
	public string unlockKey;
	public string setKey;
	public string clearKey;
    public string objective;
	public bool saveState = false;
	public bool autoDestroy = false;
	public GameObject[] enterReceivers;
	public GameObject[] exitReceivers;
	public GameObject[] doneReceivers;
	public GameObject showObject;
	public bool cinematic = false;
	public bool playFoundClip = false;
	public PowerUp powerUp;
	public bool showSpirit = false;
	public GameObject spiritTarget;
	public HideSpirit hideSpirit = HideSpirit.Never;
	
	float lastTriggerTime = -10;
	string[] message;
	bool activated = false;

	void Awake () {
		if (lockKey.Length > 0 && PlayerPrefs.HasKey(lockKey)) {
			gameObject.SetActiveRecursively(false);
		}
		else if (unlockKey.Length > 0 && !PlayerPrefs.HasKey(unlockKey)) {
			gameObject.SetActiveRecursively(false);
		}
		else if (saveState && PlayerPrefs.HasKey(name)) {
			gameObject.SetActiveRecursively(false);
		}
        else if (objective.Length > 0 && PlayerPrefs.HasKey(objective)) {
            gameObject.SetActiveRecursively(false);
        }
	}
	
	void OnTriggerEnter (Collider collider) {
		if (collider.gameObject.layer == Game.characterLayer) {	
			if (showSpirit && Game.spirit == null) {
				return;
			}
			if ((Time.time - lastTriggerTime) > 2 || !cinematic) {
				activated = true;
				StartCoroutine(Interact(collider));
			}
			if (enterReceivers.Length > 0) {
				foreach (GameObject enterReceiver in enterReceivers) {
					if (enterReceiver != null) {
						enterReceiver.SendMessage("OnHotspotEnter", this, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
	}
	
	void OnTriggerExit (Collider collider) {
		if (collider.gameObject.layer == Game.characterLayer) {
			lastTriggerTime = Time.time;
			if (activated) {
				activated = false;
				if (hideSpirit == HideSpirit.OnExit && Game.spirit != null && Game.spirit.gameObject.active) {
					Game.spirit.SetTarget(null);
				}
				if (autoDestroy) {
					Destroy(gameObject);
				}
			}
			if (exitReceivers.Length > 0) {
				foreach (GameObject exitReceiver in exitReceivers) {
					if (exitReceiver != null) {
						exitReceiver.SendMessage("OnHotspotExit", this, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
		}
	}
	
	IEnumerator WaitForRealSeconds(float time) {
	    float startTime = Time.realtimeSinceStartup;
	    while (Time.realtimeSinceStartup - startTime < time)
	        yield return 1;
	}
	
	IEnumerator Interact (Collider collider) {
		Character character = (Character)collider.GetComponent(typeof(Character));
		if (showSpirit) {
			if (spiritTarget == null && character != null) {
				spiritTarget = character.gameObject;
			}
			Game.spirit.SetTarget(spiritTarget);
		}
		if (cinematic) {
			if (powerUp == PowerUp.EnergyTank) {
				message = TextTable.GetLines("EnergyTank");
			}
			else {
				message = TextTable.GetLines(name);
			}
			character.Pause();
			Time.timeScale = 0;
			Game.fx.SetLetterBox(true);
			if (showObject != null) {
				float directorTime = 0;
				Vector3 directorStart = Game.director.transform.position;
				Vector3 directorEnd = 
					showObject.transform.position + new Vector3(0, 0, Character.cameraDistance);
				while (directorTime < 1) {
					Game.director.transform.position = Vector3.Lerp(directorStart, directorEnd, directorTime);
					directorTime += Game.realDeltaTime;
					yield return 0;
				}
			}
			else if (message.Length > 0) {
				yield return StartCoroutine(WaitForRealSeconds(0.5f));
			}
			for (int i=0; i<message.Length; ++i) {
	            if (i == (message.Length - 1) && playFoundClip) {
	                break;
	            }
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
			if (message.Length == 0) {
				yield return StartCoroutine(WaitForRealSeconds(2));
			}
	        if (playFoundClip) {
	            if (message.Length > 0) {
	                Game.hud.Message(message[message.Length - 1], 5);
	            }
	            Game.fx.PlayFoundMusic();
	            yield return StartCoroutine(WaitForRealSeconds(7.5f));
	            Game.fx.PlayCollectSound();
	            Game.fx.PlayMusic();
	        }
			character.Continue();
			Time.timeScale = 1;
			Game.fx.SetLetterBox(false);
		}
		if (powerUp != PowerUp.None) {
			ApplyPowerUp(character);
		}
		if (saveState) {
			PlayerPrefs.SetInt(name, 1);
		}
        if (setKey.Length > 0) {
            PlayerPrefs.SetInt(setKey, 1);
        }
        if (clearKey.Length > 0) {
        	PlayerPrefs.DeleteKey(clearKey);
        }
        if (objective.Length > 0) {
            PlayerPrefs.SetInt(objective, 1);
        }
		if (hideSpirit == HideSpirit.OnDone && Game.spirit != null && Game.spirit.gameObject.active) {
			Game.spirit.SetTarget(null);
		}
        if (doneReceivers.Length > 0) {
        	foreach (GameObject doneReceiver in doneReceivers) {
        		if (doneReceiver != null) {
        			doneReceiver.SendMessage("OnHotspotDone", this, SendMessageOptions.DontRequireReceiver);
        		}
        	}
        }
	}
	
	void ApplyPowerUp (Character character) {
		
		if (powerUp == PowerUp.EnergyTank) {
        	character.AddEnergyTank();
		}
		else {
			PlayerPrefs.SetInt(powerUp.ToString(), 1);
		}
        
        if (powerUp == PowerUp.Armor) {
            character.SetSuit(Character.Suit.Armor);
        }
        else if (powerUp == PowerUp.Swimsuit) {
            character.SetSuit(Character.Suit.Swimsuit);
        }
        else if (powerUp == PowerUp.Projectile) {
            character.SetWeapon(Character.Weapon.Projectile);
            Game.hud.UpdateWeapons();
        }
        else if (powerUp == PowerUp.Missile) {
            character.SetWeapon(Character.Weapon.Missile);
            Game.hud.UpdateWeapons();
        }
        else if (powerUp == PowerUp.Grenade) {
            character.SetWeapon(Character.Weapon.Grenade);
            Game.hud.UpdateWeapons();
        }
        else if (powerUp == PowerUp.Grapple) {
            character.SetWeapon(Character.Weapon.Grapple);
            Game.hud.UpdateWeapons();
        }
        else if (powerUp == PowerUp.FlameThrower) {
            character.SetWeapon(Character.Weapon.FlameThrower);
            Game.hud.UpdateWeapons();
        }
        else if (powerUp == PowerUp.Charge) {
            character.SetWeapon(Character.Weapon.Projectile);
            character.CanCharge = true;
            Game.hud.UpdateWeapons();
        }
	}
}
