using UnityEngine;
using System.Collections;

public class GridEnd : MonoBehaviour {
	
	public TextMesh statsTitle;
	public TextMesh statsResult;
	public GameObject[] tokens;
	public AudioClip goodClip;
	public AudioClip badClip;
	
	void Awake () {
		transform.parent.position = Game.character.transform.position;
		transform.parent.rotation = Quaternion.Euler(0,180,0);
		statsTitle.gameObject.SetActiveRecursively(false);
		statsResult.gameObject.SetActiveRecursively(false);
        foreach (GameObject token in tokens) {
        	token.active = false;
        }
	}

    void Start () {
    	audio.volume = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
    	audio.Play();
        StartCoroutine(PostStats());
    }
	
	IEnumerator UpdateTokenSlots () {
        for (int i=0; i<3; ++i) {
        	string tokenName = "Token|" + Application.loadedLevelName + "|" + (i+1).ToString();
        	MeshFilter meshFilter = (MeshFilter)tokens[i].GetComponent(typeof(MeshFilter));
        	tokens[i].active = true;
    		if (PlayerPrefs.HasKey(tokenName)) {
				audio.PlayOneShot(goodClip);
    			meshFilter.mesh = Game.hud.tokenOnMesh;
    		}
    		else {
				audio.PlayOneShot(badClip);
    			meshFilter.mesh = Game.hud.tokenOffMesh;
    		}
			yield return new WaitForSeconds(0.5f);
        }
	}
	
	void AddEntry (float y, string title, string result) {
		
		GameObject titleObject;
		GameObject resultObject;
		TextMesh textMesh;
		
		titleObject = (GameObject)Instantiate(statsTitle.gameObject);
		titleObject.transform.parent = statsTitle.transform.parent;
		titleObject.transform.localPosition = statsTitle.transform.localPosition;
		titleObject.transform.localRotation = statsTitle.transform.localRotation;
		titleObject.transform.localPosition -= new Vector3(0, y, 0);
		textMesh = titleObject.GetComponent<TextMesh>();
		textMesh.text = title;
		
		resultObject = (GameObject)Instantiate(statsResult.gameObject);
		resultObject.transform.parent = statsResult.transform.parent;
		resultObject.transform.localPosition = statsResult.transform.localPosition;
		resultObject.transform.localRotation = statsResult.transform.localRotation;
		resultObject.transform.localPosition -= new Vector3(0, y, 0);
		textMesh = resultObject.GetComponent<TextMesh>();
		textMesh.text = result;
	}
	
	IEnumerator PostStats () {
		
		yield return new WaitForSeconds(1);
		
		yield return StartCoroutine(UpdateTokenSlots());
		
		float y = 0;
		
		int mins, secs;
		
		secs = (int)(Game.grid.playTime);
		mins = secs / 60;
		secs = secs - mins * 60;
		AddEntry(y, "Total Time:", mins.ToString() + ":" + secs.ToString("D2"));
		audio.PlayOneShot(goodClip);
		yield return new WaitForSeconds(0.5f);
		
		audio.PlayOneShot(goodClip);
		AddEntry(y += 1, "Total Kills      :", Game.grid.kills.ToString());
		foreach (string species in Game.grid.killSpecies.Keys) {
			AddEntry(y += 0.5f, species+":", Game.grid.killSpecies[species].ToString());
		}
		
		yield return new WaitForSeconds(1);
		audio.PlayOneShot(goodClip);
		AddEntry(y += 2, "Results:", "Pass");
		
		yield return 0;
		
	}
	
	void Update () {
		if (Game.character != null) {
			transform.parent.position = Game.character.transform.position;
		}
	}
    
    IEnumerator End () {
		
        yield return new WaitForSeconds(1);
		/*
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
        */
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

