using UnityEngine;
using System.Collections;

public class ElectricWall : MonoBehaviour {

    public int damage = 200;
	
	public Renderer[] renderers;
	
	void Awake () {
	}
	
    bool lit = false;
    IEnumerator Lit () {
        lit = true;
		for (int i=0; i<8; ++i) {
			foreach (Renderer renderer in renderers) {
		        renderer.material.SetColor("_TintColor", Color.red);
			}
			yield return new WaitForSeconds(0.05f);
			foreach (Renderer renderer in renderers) {
				renderer.material.SetColor("_TintColor", Color.white);
			}
			yield return new WaitForSeconds(0.05f);
		}
        lit = false;
    }
		
	void OnTriggerEnter (Collider collider) {
		collider.SendMessage("Hit", damage, SendMessageOptions.DontRequireReceiver);
		if (!lit) {
			StartCoroutine(Lit());
		}
	}
	
	void OnCollisionEnter (Collision collision) {
		collision.collider.SendMessage("Hit", damage, SendMessageOptions.DontRequireReceiver);
	}
}
