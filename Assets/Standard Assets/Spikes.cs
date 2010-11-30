using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour {

    public int damage = 200;
	
	new Renderer renderer;
	
	void Awake () {
		renderer = GetComponentInChildren<Renderer>();
	}
	
    bool lit = false;
    IEnumerator Lit () {
        lit = true;
        Material sharedMaterial = renderer.sharedMaterial;
        renderer.material.color = Color.white;
        yield return new WaitForSeconds(0.5f);
        renderer.material = sharedMaterial;
        lit = false;
    }
		
	void OnTriggerEnter (Collider collider) {
		collider.SendMessage("Hit", damage, SendMessageOptions.DontRequireReceiver);
		if (!lit && renderer != null) {
			StartCoroutine(Lit());
		}
	}
	
	void OnCollisionEnter (Collision collision) {
		collision.collider.SendMessage("Hit", damage, SendMessageOptions.DontRequireReceiver);
	}
}
