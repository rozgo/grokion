using UnityEngine;
using System.Collections;

public class ComponentCacheTest : MonoBehaviour {
	
	new Transform transform;

	void Start () {
		transform = Game.game.transform;
	
	}
	
	void Update () {
		Debug.Log(transform.name);
	}
}
