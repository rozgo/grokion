using UnityEngine;
using System.Collections;

public class HiddenRock : MonoBehaviour {
    
    public Material fxMaterial;
    public AudioClip startDesintegrateClip;
    public AudioClip desintegrateClip;
    public bool Desintegrated { get { return desintegrated; } }
    
    bool desintegrated = false;
    float desintegrateRate = 5;
    bool desintegrating = false;
    Material sharedMaterial;
    ArrayList rigidbodies = new ArrayList();
    int layer;
    
    void Awake () {
        renderer.enabled = false;
        collider.isTrigger = true;
        layer = gameObject.layer;
    }
    
    void Start () {
        sharedMaterial = renderer.sharedMaterial;
    }
    
    IEnumerator Desintegrate () {
        desintegrating = true;
        collider.isTrigger = false;
        Game.fx.PlaySound(startDesintegrateClip);
        renderer.enabled = true;
        yield return new WaitForSeconds(desintegrateRate * 0.8f);
        renderer.material = fxMaterial;
        yield return new WaitForSeconds(desintegrateRate * 0.2f);
        desintegrated = true;
        Game.fx.PlaySound(desintegrateClip);
        renderer.material = sharedMaterial;
        renderer.enabled = false;
        gameObject.layer = Game.hiddenLayer;
        rigidbodies = new ArrayList();
        collider.isTrigger = true;
        yield return new WaitForSeconds(1);
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
        desintegrating = false;
        gameObject.layer = layer;
        desintegrated = false;
    }
    
    public void Activate () {
        if (!desintegrating) {
            StartCoroutine(Desintegrate());
        }
    }
    
    void OnExplosion () {
        if (!desintegrating) {
            StartCoroutine(Desintegrate());
        }
    }
    
    void OnTriggerEnter (Collider collider) {
        if (!desintegrating) {
            StartCoroutine(Desintegrate());
        }
        rigidbodies.Add(collider.rigidbody);
    }
    
    void OnTriggerExit (Collider collider) {
        rigidbodies.Remove(collider.rigidbody);
    }
    
}
