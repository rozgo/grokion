using UnityEngine;
using System.Collections;

public class InstantKill : MonoBehaviour {
    
    void OnTriggerEnter (Collider collider) {
    	collider.SendMessage("Hit", 3000, SendMessageOptions.DontRequireReceiver);
    }
    
    void OnTriggerExit (Collider collider) {
    }

}