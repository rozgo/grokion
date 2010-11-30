using UnityEngine;
using System.Collections;

public class Loader : MonoBehaviour {
	
	public string scene;
    public Button startGame;
    public Button resetGame;
    public Button options;
    public Button optionsExit;
    public Button credits;
    public Button resetYes;
    public Button resetNo;
    public Button resetExit;
    public Button casualButton;
    public Button hardcoreButton;
    public Button helpButton;
    public GameObject casualCheck;
    public GameObject hardcoreCheck;
    public MenuMove sceneRotator;
    public AudioSource music;
    public AudioSource sound;
    public GameObject optionsScreen;
    public GameObject resetScreen;
    public SimpleRotate backdrop;
    public GameObject animatedMenu;
	
	void Awake () {
		//Game.debug = false;
	}

    IEnumerator LoadGame (bool reset) {
        if (reset) {
            PlayerPrefs.DeleteAll();
        }
        yield return new WaitForSeconds(1);
        string[] checkpointInfo = PlayerPrefs.GetString("Checkpoint").Split('|');
        if (checkpointInfo.Length == 3 && checkpointInfo[0] == "Door") {
            Game.door = PlayerPrefs.GetString("Checkpoint");
            Application.LoadLevel(checkpointInfo[1]);
        }
        else if (checkpointInfo.Length == 2) {
            Application.LoadLevel(checkpointInfo[1]);
        }
        else {
            Application.LoadLevel(scene);
        }
    }

    IEnumerator LoadCredits () {
        yield return new WaitForSeconds(1);
        Application.LoadLevel("Credits");
    }

    void OnButtonUp (Button button) {
        sound.Play();
        if (button == startGame) {
        	options.gameObject.SetActiveRecursively(false);
        	optionsExit.gameObject.SetActiveRecursively(false);
        	credits.gameObject.SetActiveRecursively(false);
        	startGame.gameObject.SetActiveRecursively(false);
            StartCoroutine(LoadGame(false));
        }
        else if (button == credits) {
        	options.gameObject.SetActiveRecursively(false);
        	optionsExit.gameObject.SetActiveRecursively(false);
        	credits.gameObject.SetActiveRecursively(false);
        	startGame.gameObject.SetActiveRecursively(false);
            StartCoroutine(LoadCredits());
        }
        else if (button == resetGame) {
        	optionsScreen.SetActiveRecursively(false);
        	resetScreen.SetActiveRecursively(true);
        }
        else if (button == resetNo || button == resetExit) {
        	optionsScreen.SetActiveRecursively(true);
        	resetScreen.SetActiveRecursively(false);
        	UpdateDifficulty();
        }
        else if (button == resetYes) {
        	optionsScreen.SetActiveRecursively(false);
        	resetScreen.SetActiveRecursively(false);
            StartCoroutine(LoadGame(true));
        }
        else if (button == options) {
        	optionsScreen.SetActiveRecursively(true);
        	resetScreen.SetActiveRecursively(false);
        	options.gameObject.SetActiveRecursively(false);
        	optionsExit.gameObject.SetActiveRecursively(true);
        	credits.gameObject.SetActiveRecursively(false);
        	startGame.gameObject.SetActiveRecursively(false);
        	UpdateDifficulty();
        	sceneRotator.velocity = -sceneRotator.transform.right * 20;
        	sceneRotator.distance = 40;
        	sceneRotator.Move();
        }
        else if (button == optionsExit) {
        	options.gameObject.SetActiveRecursively(true);
        	optionsExit.gameObject.SetActiveRecursively(false);
        	credits.gameObject.SetActiveRecursively(true);
        	UpdateDifficulty();
        	startGame.gameObject.SetActiveRecursively(true);
        	sceneRotator.velocity = sceneRotator.transform.right * 20;
        	sceneRotator.distance = 40;
        	sceneRotator.Move();
        }
        else if (button == hardcoreButton) {
        	PlayerPrefs.DeleteKey("Casual");
        	UpdateDifficulty();
        }
        else if (button == casualButton) {
        	PlayerPrefs.SetInt("Casual", 1);
        	UpdateDifficulty();
        }
        else if (button == helpButton) {
        	sceneRotator.velocity = sceneRotator.transform.up * 20;
        	sceneRotator.distance = 15;
        	sceneRotator.Move();
        }
        else if (button.name == "ArrowRight") {
        	sceneRotator.velocity = sceneRotator.transform.up * 20;
        	sceneRotator.distance = 15;
        	sceneRotator.Move();
        }
        else if (button.name == "ArrowLeft") {
        	sceneRotator.velocity = -sceneRotator.transform.up * 20;
        	sceneRotator.distance = 15;
        	sceneRotator.Move();
        }
    }       
    
    void UpdateDifficulty () {
    	if (PlayerPrefs.HasKey("Casual")) {
    		casualCheck.active = true;
    		hardcoreCheck.active = false;
    	}
    	else {
    		casualCheck.active = false;
    		hardcoreCheck.active = true;
    	}
    }     
	
	void Start () {
		sound.volume = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
		music.volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
		music.Play();
	}
	
	void Update () {
   		if (Input.touchCount > 0 || Input.anyKey) {
   			animatedMenu.animation["start2"].speed = 5;
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
