using UnityEngine;
using System.Collections;

public class Welcome : MonoBehaviour {

	void Start () {
		var kongregate = (Kongregate)GameObject.FindObjectOfType(typeof(Kongregate));
		var label = gameObject.GetComponent<TextMesh>();
		label.text = "Welcome, " + kongregate.username + "!";
	}

}
