using UnityEngine;
using System.Collections;

public class Credits : MonoBehaviour {
    
    public FX fx;
    public Director director;
    public Texture2D worldTexture;
    public Material[] worldMaterials;
    public AudioClip music;
    public AudioClip buttonClip;
    public Button exitButton;
    public Button twitterButton;
    public Button grokionButton;
    
    void Awake () {
		foreach (Material worldMaterial in worldMaterials) {
			worldMaterial.mainTexture = worldTexture;
		}
    }

    void Start () {
        fx.SetLetterBox(true);
        fx.PlayMusic(music);
    }

	IEnumerator ExitTask () {
		yield return new WaitForSeconds(1);
		Application.LoadLevel("StoryLoader");
	}

	void Update () {
		if (Input.GetButtonUp("Fire1") || Input.GetButtonUp("Jump")) {
			fx.PlaySound(buttonClip);
			StartCoroutine(ExitTask());
		}
	}

}
