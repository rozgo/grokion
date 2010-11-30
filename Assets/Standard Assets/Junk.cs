using UnityEngine;
using System.Collections;

public class Junk : MonoBehaviour {

    Health health;

    void Awake () {
        health = (Health)GetComponent(typeof(Health));
    }

    void OnWaterEnter (Water water) {
        if (water.lava) {
            health.Kill();
        }
    }
}