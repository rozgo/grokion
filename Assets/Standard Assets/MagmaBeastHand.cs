using UnityEngine;
using System.Collections;

public class MagmaBeastHand : MonoBehaviour {

	void OnCollisionEnter (Collision collision) {
		//Debug.Log(collision);
		
		Avalanche avalanche = (Avalanche)collision.collider.gameObject.GetComponent(typeof(Avalanche));
		if (avalanche != null) {
			collision.collider.SendMessage("Kill", SendMessageOptions.DontRequireReceiver);
		}
		
		Character character = (Character)collision.collider.gameObject.GetComponent(typeof(Character));
		if (character != null) {
			character.Hit(20);
		}
		
		//Game.fx.Explode(collision.collider.gameObject);
	}
    
}