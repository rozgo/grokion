using UnityEngine;
using System.Collections;

public class ForwardVisibility : MonoBehaviour {
	
	public GameObject receiver;

	void OnBecameVisible () {
		receiver.SendMessage("OnChildBecameVisible", gameObject, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnBecameInvisible () {
		receiver.SendMessage("OnChildBecameInvisible", gameObject, SendMessageOptions.DontRequireReceiver);
	}
}
