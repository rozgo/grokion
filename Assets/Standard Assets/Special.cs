using UnityEngine;
using System.Collections;

public class Special : MonoBehaviour {
	
    public GameObject fxAsset;
	public bool saveState = false;

    bool saved = false;
	
	void Awake () {
		if (saveState && PlayerPrefs.HasKey(name)) {
            saved = true;
            if (gameObject.active) {
                gameObject.SetActiveRecursively(false);
            }
            else {
                gameObject.SetActiveRecursively(true);
            }
		}
	}

    void ApplySpecial () {
        if (fxAsset != null) {
			GameObject fxObject = (GameObject)Instantiate(fxAsset);
			fxObject.transform.position = transform.position;
        }
        if (gameObject.active) {
            gameObject.SetActiveRecursively(false);
        }
        else {
            gameObject.SetActiveRecursively(true);
        }
    }    

    void OnSwitch () {
        if (saved) {
            return;
        }
        if (saveState) {
            PlayerPrefs.SetInt(name, 1);
        }
        ApplySpecial();
    }

	void OnSwitchUp () {
		OnSwitch();
	}
	
	void OnSwitchDown () {
		OnSwitch();
	}
	
    void OnStunt (Stunt stunt) {
        if (saved) {
            return;
        }
        if (saveState) {
            PlayerPrefs.SetInt(name, 1);
        }
        ApplySpecial();
    }
		
}

