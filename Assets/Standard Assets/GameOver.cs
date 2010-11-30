using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

    void Start () {
    	audio.volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
    	audio.Play();
        StartCoroutine(End());
    }
    
    IEnumerator End () {
        yield return new WaitForSeconds(1);
        while (Input.touchCount == 0 && !Input.anyKey) {
            yield return 0;
        }
        string[] checkpointInfo = PlayerPrefs.GetString("Checkpoint").Split('|');
        if (checkpointInfo.Length == 3 && checkpointInfo[0] == "Door") {
            Game.door = PlayerPrefs.GetString("Checkpoint");
            Application.LoadLevel(checkpointInfo[1]);
        }
        else if (checkpointInfo.Length == 2) {
            Application.LoadLevel(checkpointInfo[1]);
        }
        else {
            Application.LoadLevel("AO");
        }
    }
    
#if UNITY_IPHONE
	void FixedUpdate(){ 
		if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft && 
			iPhoneSettings.screenOrientation != iPhoneScreenOrientation.LandscapeLeft){ 
			iPhoneSettings.screenOrientation = iPhoneScreenOrientation.LandscapeLeft; 
		} 
		if (Input.deviceOrientation == DeviceOrientation.LandscapeRight &&
			iPhoneSettings.screenOrientation != iPhoneScreenOrientation.LandscapeRight){ 
			iPhoneSettings.screenOrientation = iPhoneScreenOrientation.LandscapeRight; 
		} 
	}
#endif

}

