using UnityEngine;
using System.Collections;

public class Fallen : MonoBehaviour {
    
    public bool explode = false;
    
    void Start () {
        audio.volume = Game.fx.soundVolume;
    }
    
    void OnChildBecameVisible () {
        animation.Play();
    }
    
    void OnChildBecameInvisible () {
        animation.Stop();
    }
    
    void OnHotspotDone () {
    	if (explode) {
    		Game.fx.Explode(gameObject);
    	}
    	else {
    		Destroy(gameObject);
    	}
    }
    
}
