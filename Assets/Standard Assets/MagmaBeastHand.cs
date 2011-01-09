using UnityEngine;
using System.Collections;

public class MagmaBeastHand : MonoBehaviour {

	void OnCollisionEnter (Collision collision) {
		
		Character character = (Character)collision.collider.gameObject.GetComponent(typeof(Character));
		if (character != null) {
			character.Hit(20);
		}
		else {
			collision.collider.SendMessage("Kill", this, SendMessageOptions.DontRequireReceiver);
		}
		
		//Game.fx.Explode(collision.collider.gameObject);
	}
    
}
