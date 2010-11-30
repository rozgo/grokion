using UnityEngine;
using System.Collections;

public class Ammo : MonoBehaviour {
	
	public Character.Weapon weapon;
	public int amount = 1;

	void Start () {
	}
	
	void OnCollisionEnter (Collision collision) {
		if (collision.collider.gameObject.layer == Game.characterLayer) {
			Collect();
		}
	}
	
	void OnTriggerEnter (Collider collider) {
		if (collider.gameObject.layer == Game.characterLayer) {
            Collect();
		}
	}
	
	void Collect () {
		if (Game.character.GetWeapon() == Character.Weapon.None) {
			Game.character.SetWeapon(weapon);
		}
		Game.fx.PlayCollectSound();
		Game.grid.Refill(weapon, amount);
		Destroy(gameObject);
	}
}
