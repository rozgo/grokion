using UnityEngine;
using System.Collections;

public class Clock : MonoBehaviour {
	
	public int seconds = 5;
	
	//new Renderer renderer;
	TextMesh textMesh;
	
	void Awake () {
		//renderer = gameObject.GetComponentInChildren<Renderer>();
		textMesh = gameObject.GetComponentInChildren<TextMesh>();
	}

	void Start () {
		if (seconds < 0) {
			//renderer.material.SetColor("_TintColor", Color.red);
			textMesh.font.material.color = Color.red;
		}
		else {
			textMesh.font.material.color = Color.green;
		}
		textMesh.text = ":" + Mathf.Abs(seconds).ToString("D2");
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
		Game.grid.countDown += seconds;
		Game.fx.PlayCollectSound();
		Destroy(gameObject);
	}
}
