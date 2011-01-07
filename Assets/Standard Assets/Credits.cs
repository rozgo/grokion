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

	IEnumerator ExitTask () {
		yield return new WaitForSeconds(1);
		Application.LoadLevel("StoryLoader");
	}

	IEnumerator TwitterTask () {
		yield return new WaitForSeconds(1);
		Application.OpenURL("http://twitter.com/dododomination");
	}

	IEnumerator GrokionTask () {
		yield return new WaitForSeconds(1);
		Application.OpenURL("http://dododomination.com");
	}
	
	void DisableButtons () {
		foreach (Button button in FindObjectsOfType(typeof(Button))) {
			Destroy(button.collider);
		}
	}

    void OnButtonUp (Button button) {
        fx.PlaySound(buttonClip);
        DisableButtons();
        if (button == exitButton) {
        	StartCoroutine(ExitTask());
        }
        else if (button == twitterButton) {
        	StartCoroutine(TwitterTask());
        }
        else if (button == grokionButton) {
        	StartCoroutine(GrokionTask());
        }
    }
    
    void OnMouseDown () {
    }

    void OnMouseExit () {
    }

    void OnMouseUp () {
    }

    void OnFingerBegin () {
    }

    void OnFingerMove () {
    }

    void OnFingerEnd () {
    }

    void OnFingerCancel () {
    }
    
    void OnBecameVisible () {
    }
    
    void OnBecameInvisible () {
    }

}
