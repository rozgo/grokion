using UnityEngine;
using System.Collections;

public class CustomUVAnim : MonoBehaviour {
	
	public float scrollSpeed = 0.5f;
	
	void Update () {
		float offset = Game.realTime * scrollSpeed;
    	renderer.material.SetTextureOffset("_Rotation", new Vector2(offset,0));
	}
	
}
