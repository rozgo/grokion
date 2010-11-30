using UnityEngine;
using System.Collections;

public class ForwardHit : MonoBehaviour {
	
	public GameObject receiver;

	void Hit (int damage) {
		receiver.SendMessage("Hit", damage, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnCollisionEnter (Collision collision) {
		if (collision.collider.gameObject.layer == Game.projectilesLayer) {
            Projectile projectile = (Projectile)collision.collider.GetComponent(typeof(Projectile));            
            Hit(projectile.Damage);
		}
    }
	
}
