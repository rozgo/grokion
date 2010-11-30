using UnityEngine;
using System.Collections;

public class Boulder : MonoBehaviour {
    
    static float lastBreak = 0;
    
    public GameObject rocksAsset;
    public AudioClip rocksClip;
    public bool saveState = false;
    
    new Rigidbody rigidbody;
    
    void Awake () {
        rigidbody = gameObject.rigidbody;
        if (rigidbody != null) {
            rigidbody.Sleep();
        }
        if (saveState && PlayerPrefs.HasKey(name)) {
            gameObject.SetActiveRecursively(false);
        }
    }
    
    void OnExplosion (GameObject exploded) {
        if (Vector3.Distance(transform.position, exploded.transform.position) < 3) {
            if (saveState) {
                PlayerPrefs.SetInt(name, 1);
            }
            GameObject rocks = (GameObject)Instantiate(rocksAsset);
            rocks.transform.position = transform.position;
            if ((Time.time - lastBreak) > 0.5f) {
                Game.fx.PlaySound(rocksClip);
            }
            lastBreak = Time.time;
            Destroy(gameObject);
        }
    }
    
    void Hit (int damage) {
        if (damage > 2000) {
            if (saveState) {
                PlayerPrefs.SetInt(name, 1);
            }
            GameObject rocks = (GameObject)Instantiate(rocksAsset);
            rocks.transform.position = transform.position;
            Game.fx.PlaySound(rocksClip);
            Destroy(gameObject);
        }
    }
}

