using UnityEngine;
using System.Collections;

public class TouchRock : MonoBehaviour {
    
    public Material fxMaterial;
    public AudioClip startDesintegrateClip;
    public AudioClip desintegrateClip;
    public float desintegrateRate = 5;
    public bool Desintegrated { get { return desintegrated; } }
    
    bool desintegrated = false;
    bool desintegrating = false;
    Material sharedMaterial;
    ArrayList rigidbodies = new ArrayList();
    int layer;
    
    void Awake () {
        sharedMaterial = renderer.sharedMaterial;
        layer = gameObject.layer;
    }
    
    IEnumerator Desintegrate () {
        desintegrating = true;
        Game.fx.PlaySound(startDesintegrateClip);
        renderer.material = fxMaterial;
        yield return new WaitForSeconds(1);
        collider.isTrigger = true;
        Game.fx.PlaySound(desintegrateClip);
        renderer.enabled = false;
        desintegrated = true;
        gameObject.layer = Game.hiddenLayer;
        yield return new WaitForSeconds(desintegrateRate);
        bool free;
        do {
            free = true;
            foreach (Rigidbody rb in rigidbodies) {
                if (rb != null && rb.gameObject.active) {
                    free = false;
                }
            }
            yield return new WaitForSeconds(1);
        } while (!free);
        rigidbodies = new ArrayList();
        renderer.material = sharedMaterial;
        renderer.enabled = true;
        desintegrated = false;
        gameObject.layer = layer;
        collider.isTrigger = false;
        desintegrating = false;
    }
    
    void OnExplosion () {
        if (!desintegrating) {
            StartCoroutine(Desintegrate());
        }
    }
    
    public void Activate () {
        if (!desintegrating) {
            StartCoroutine(Desintegrate());
        }
    }
    
    void OnParticleCollision (GameObject particle) {
        if (particle.gameObject.layer == Game.flamesLayer) {
            if (!desintegrating) {
                StartCoroutine(Desintegrate());
            }
        }
    }
    
    void OnCollisionEnter (Collision collision) {   
        if (!desintegrating) {
            StartCoroutine(Desintegrate());
        }
    }
    
    void OnTriggerEnter (Collider collider) {
        rigidbodies.Add(collider.rigidbody);
    }
    
    void OnTriggerExit (Collider collider) {
        rigidbodies.Remove(collider.rigidbody);
    }
    
}
