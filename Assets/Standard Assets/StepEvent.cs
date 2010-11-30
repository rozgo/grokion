using UnityEngine;
using System.Collections;

public class StepEvent : MonoBehaviour {

    public AudioClip stepSound;

    void OnStepEvent () {
        Game.fx.PlaySoundPitched(stepSound);
    }

}
