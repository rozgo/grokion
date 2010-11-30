using UnityEngine;
using System.Collections;

public class waterTest : MonoBehaviour {
	
	public float scrollSpeed = 0.5f;
	
	void Update () {
		float offset = Game.realTime * scrollSpeed;
    	renderer.material.SetTextureOffset("_MainTex", new Vector2(offset,0));
	}
	
}
