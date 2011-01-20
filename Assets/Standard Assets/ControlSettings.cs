using UnityEngine;
using System.Collections;

public class ControlSettings : MonoBehaviour {
    
    public AudioClip clickClip;
	new public AudioSource audio;

    void Start () {
        audio.volume = Game.fx.soundVolume;
    }
    
    IEnumerator WaitForRealSeconds (float time) {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < time)
            yield return 1;
    }
    
    void OnButtonUp (Button button) {

		audio.PlayOneShot(clickClip);

		Debug.Log(button.name);
    	
        if (button.name == "title") {
        	StartCoroutine(LoadTitleScreen());
        }
        
		//soundVolume.text = Mathf.Round(Game.fx.soundVolume * 100).ToString();
		//musicVolume.text = Mathf.Round(Game.fx.musicVolume * 100).ToString();
    }

	IEnumerator LoadTitleScreen () {
		yield return StartCoroutine(WaitForRealSeconds(1));
		Time.timeScale = 1;
		Application.LoadLevel("StoryLoader");
	}

	public static void ResetDefaults () {

		PlayerPrefs.SetString("Controls|MoveRight", "a");
		PlayerPrefs.SetString("Controls|MoveLeft", "d");
		PlayerPrefs.SetString("Controls|LookUp", "w");
		PlayerPrefs.SetString("Controls|LookDown", "s");

		PlayerPrefs.SetString("Controls|Fire", "mouse 0");
		PlayerPrefs.SetString("Controls|Jump", "mouse 1");

		PlayerPrefs.SetString("Controls|ZoomIn", "up");
		PlayerPrefs.SetString("Controls|ZoomOut", "down");
	}

	public static void Setup () {
		
	}

}
