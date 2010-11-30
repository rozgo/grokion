using UnityEngine;
using System.Collections;

public class Token : MonoBehaviour {
	
	void Start () {
		if (PlayerPrefs.HasKey(name)) {
			gameObject.SetActiveRecursively(false);
		}
	}
	
	IEnumerator CollectToken (Character character) {
		PlayerPrefs.SetInt(name, 1);
		Game.fx.PlayTokenSound();
		Time.timeScale = 0;
		float timer = Time.realtimeSinceStartup;
		while ((Time.realtimeSinceStartup - timer) < 0.1f) {
			yield return 0;
		}
		Time.timeScale = 1;
		character.Continue();
		Game.hud.UpdateTokenSlots();
		gameObject.SetActiveRecursively(false);
	}
	
	void OnTriggerEnter (Collider collider) {
		if (collider.gameObject.layer == Game.characterLayer) {
			Character character = (Character)collider.GetComponent(typeof(Character));
			StartCoroutine(CollectToken(character));
		}
	}
}
