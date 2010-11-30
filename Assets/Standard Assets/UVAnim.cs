using UnityEngine;
using System.Collections;

public class UVAnim : MonoBehaviour {
	
	public int uvAnimationTileX = 4;
	public int uvAnimationTileY = 4;
	public float framesPerSecond = 10.0f;
	public bool loop = false;
	
	int index = 0;
	float time = 0;
	
	void Update () {

	    // Calculate index
	    index = (int)(time*framesPerSecond);
	    
	    if (loop) {
		    // repeat when exhausting all frames
		    index = index % (uvAnimationTileX*uvAnimationTileY);
	    }
	    
	    if (index < uvAnimationTileX*uvAnimationTileY) {
		    // Size of every tile
		    Vector2 size = new Vector2(1.0f/uvAnimationTileX,1.0f/uvAnimationTileY);
		   
		    // split into horizontal and vertical index
		    int uIndex = index % uvAnimationTileX;
		    int vIndex = index / uvAnimationTileX;
		
		    // build offset
		    // v coordinate is the bottom of the image in opengl so we need to invert.
		    Vector2 offset = new Vector2(uIndex*size.x,1.0f-size.y-vIndex*size.y);
		   
		    renderer.material.mainTextureOffset = offset;
		    renderer.material.mainTextureScale = size;
		    
		    time += Time.deltaTime;
	    }
	}
	
	void OnBecameVisible () {
		time = 0;
        if (loop) {
            enabled = true;
        }
	}

    void OnBecameInvisible () {
        if (loop) {
            enabled = true;
        }
    }
	
	void OnEnable () {
		time = 0;
	}
}
