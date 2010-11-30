using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour {

    Health health;

    void Awake () {
    }

    void Update () {
        transform.rotation = Quaternion.Euler(0, 180, 0);
    }
}