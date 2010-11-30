using UnityEngine;
using System.Collections;

public class Turbo : MonoBehaviour {
	
	public bool jumper = false;
	new Renderer renderer;
	
	void Awake () {
		renderer = GetComponentInChildren<Renderer>();
		renderer.material.SetColor("_TintColor", Color.gray);
	}
	
    bool lit = false;
    IEnumerator Lit () {
        lit = true;
        renderer.material.SetColor("_TintColor", Color.white);
        yield return new WaitForSeconds(0.5f);
        renderer.material.SetColor("_TintColor", Color.gray);
        lit = false;
    }
		
	void OnTriggerEnter (Collider collider) {
		if (!lit && renderer != null) {
			StartCoroutine(Lit());
			if (collider.gameObject.layer == Game.characterLayer) {		
				if (jumper) {
					Game.character.TurboJump(transform.right);
				}
				else if (Vector3.Dot(Game.character.transform.right, transform.right) > 0.9f) {
					Game.character.Turbo();
				}
			}
		}
	}

}
