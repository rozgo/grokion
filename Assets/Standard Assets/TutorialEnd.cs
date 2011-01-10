using UnityEngine;
using System.Collections;

public class TutorialEnd : MonoBehaviour {

    public bool on = true;
    public Renderer[] renderers;

    void Awake () {
    }

    void Start () {
        TurnOn(on);
    }

    void OnTriggerEnter (Collider collider) {
        if (on && collider.gameObject.layer == Game.characterLayer) {
            PlayerPrefs.DeleteAll();
            Application.LoadLevel("AO");
        }
    }

    void OnBloodGate (string species) {
        TurnOn(true);
    }

    public void TurnOn (bool on) {
        this.on = on;
        foreach (Renderer renderer in renderers) {
            renderer.enabled = on;
        }
    }

}
