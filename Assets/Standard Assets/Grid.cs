using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour {
	
	public float devTime = 0;
	public float countDown = 0;
	
	public int projectileCount = 0;
	public int missileCount = 0;
	public int flameThrowerCount = 0;
	public int grenadeCount = 0;
	public int grappleCount = 0;
	
	public string bloodGateSpecies = "All";
	public int bloodGateKills = 0;
	public GameObject[] bloodGateReceivers;
	int bloodGateComplete = 0;
	public AudioClip bloodGateCompleteClip;
	
	[HideInInspector]
	public float playTime = 0;
	[HideInInspector]
	public int kills = 0;
	[HideInInspector]
	public int blasts = 0;
	[HideInInspector]
	public Dictionary<string, int> killSpecies = new Dictionary<string, int>();
	[HideInInspector]
	public bool countDownMode = false;
	
	[HideInInspector]
	public bool done = false;
	
	void Start () {
		if (countDown > 0) {
			countDownMode = true;
		}
		Game.hud.UpdateWeapons();
		Game.hud.UpdateAmmo();
		StartCoroutine(Counter());
	}
	
	public void Refill (Character.Weapon weapon, int amount) {
		if (weapon == Character.Weapon.Projectile) {
			projectileCount = Mathf.Min(999, projectileCount += amount);
			if (amount >= 0 && Game.character.GetWeapon() == Character.Weapon.None) {
				
			}
		}
		else if (weapon == Character.Weapon.Missile) {
			missileCount = Mathf.Min(999, missileCount += amount);
		}
		else if (weapon == Character.Weapon.Grapple) {
			grappleCount = Mathf.Min(999, grappleCount += amount);
		}
		else if (weapon == Character.Weapon.Grenade) {
			grenadeCount = Mathf.Min(999, grenadeCount += amount);
		}
		else if (weapon == Character.Weapon.FlameThrower) {
			flameThrowerCount = Mathf.Min(999, flameThrowerCount += amount);
		}
		Game.hud.UpdateWeapons();
		Game.hud.UpdateAmmo();
	}
	
	public void Consume (Character.Weapon weapon) {
		if (weapon == Character.Weapon.Projectile) {
			if (--projectileCount == 0) {
				Game.character.SetWeapon(Character.Weapon.None);
				Game.hud.UpdateWeapons();
			}
		}
		else if (weapon == Character.Weapon.Missile) {
			if (--missileCount == 0) {
				Game.character.SetWeapon(Character.Weapon.None);
				Game.hud.UpdateWeapons();
			}
		}
		else if (weapon == Character.Weapon.Grapple) {
			if (--grappleCount == 0) {
				Game.character.SetWeapon(Character.Weapon.None);
				Game.hud.UpdateWeapons();
			}
		}
		else if (weapon == Character.Weapon.Grenade) {
			if (--grenadeCount == 0) {
				Game.character.SetWeapon(Character.Weapon.None);
				Game.hud.UpdateWeapons();
			}
		}
		else if (weapon == Character.Weapon.FlameThrower) {
			if (--flameThrowerCount == 0) {
				Game.character.SetWeapon(Character.Weapon.None);
				Game.hud.UpdateWeapons();
			}
		}
		Game.hud.UpdateAmmo();
	}
	
	public void AddKill (string species) {
		++kills;
		if (killSpecies.ContainsKey(species)) {
			++killSpecies[species];
		}
		else {
			killSpecies[species] = 1;
		}
		Game.hud.killsCount.text = kills.ToString("D2");
		
		if (bloodGateKills > 0) {
			if (bloodGateSpecies == "All") {
				if (kills >= bloodGateKills) {
					bloodGateComplete = 1;
				}
			}
			else {
				if (killSpecies.ContainsKey(bloodGateSpecies) && 
					killSpecies[bloodGateSpecies] >= bloodGateKills) {
					bloodGateComplete = 1;
				}
			}
		}
		if (bloodGateComplete == 1) {
			bloodGateComplete = 2;
			Game.fx.PlaySound(bloodGateCompleteClip);
			foreach (GameObject receiver in bloodGateReceivers) {
				receiver.SendMessage("OnBloodGate", bloodGateSpecies, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	public void AddBlast () {
		++blasts;
	}
	
	IEnumerator Counter () {
		bool done = false;
		float time = Time.time;
		while (!done) {
			playTime += Time.time - time;
			countDown -= Time.time - time;
			time = Time.time;
		
			if (countDownMode) {
				int secs = (int)(countDown);
				int mins = secs / 60;
				secs = secs - mins * 60;
				Game.hud.time.text = mins.ToString() + ":" + secs.ToString("D2");
				if (mins < 1 && secs <= 30) {
					Game.hud.time.renderer.material.color = Color.red;
				}
				if (countDown < 0 && !done) {
					done = true;
					Game.game.GameOver(false);
				}
			}
			else {
				int secs = (int)(playTime);
				int mins = secs / 60;
				secs = secs - mins * 60;
				Game.hud.time.text = mins.ToString() + ":" + secs.ToString("D2");
			}
			yield return new WaitForSeconds(1);
		}
	}
	
}

