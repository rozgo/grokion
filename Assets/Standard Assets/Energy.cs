using UnityEngine;
using System.Collections;

public class Energy : MonoBehaviour {
	
	public int energy = 20;
	public bool autoDestroy = true;

	void Start () {
		if (autoDestroy) {
			Destroy(gameObject, 5);
		}
	}
	
	void OnCollisionEnter (Collision collision) {
		if (collision.collider.gameObject.layer == Game.characterLayer) {
            Game.fx.PlayCollectSound();
            collision.collider.SendMessage("AddHP", energy);
            Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter (Collider collider) {
		if (collider.gameObject.layer == Game.characterLayer) {
            Game.fx.PlayCollectSound();
            collider.SendMessage("AddHP", energy);
            Destroy(gameObject);
		}
	}
}
