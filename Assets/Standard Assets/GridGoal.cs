using UnityEngine;
using System.Collections;

public class GridGoal : MonoBehaviour {
	
	public bool on = true;
	public Renderer[] renderers;
	
	void Awake () {
	}
	
	void Start () {
		TurnOn(on);
	}
	
	void OnTriggerEnter (Collider collider) {
		if (on && collider.gameObject.layer == Game.characterLayer) {
			Game.fx.PlayVictoryMusic();
			Game.grid.done = true;
			Game.game.GameOver(false);
		}
	}
	
	void OnBloodGate (string species) {
		TurnOn(true);
	}
	
	void TurnOn (bool on) {
		this.on = on;
		foreach (Renderer renderer in renderers) {
			renderer.enabled = on;
		}
	}
	
}
