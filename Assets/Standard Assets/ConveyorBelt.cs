using UnityEngine;
using System.Collections;

public class ConveyorBelt : MonoBehaviour {
    
    public bool reverse = false;
    
    void Awake () {
        enabled = false;
    }

    void Update () {
        float speed = 0.5f;
        if (reverse) {
            speed *= -1;
        }
        renderer.material.mainTextureOffset += new Vector2(speed * Time.deltaTime, 0);
        
    }

    // void OnBecameVisible () {
    //     enabled = true;
    // }

    // void OnBecameInvisible () {
    //     enabled = false;
    // }

    void OnCollisionStay (Collision collision) {
        if (enabled) {
            float speed = -0.8f;
            if (reverse) {
                speed *= -1;
            }
            collision.collider.rigidbody.velocity = new Vector3(speed, 0, 0);
        }
    }
}
