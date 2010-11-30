using UnityEngine;
using System.Collections;

public class Stunt : MonoBehaviour {
	
	public AudioClip goodClip;
	public AudioClip badClip;
	public AudioClip rewardClip;
	public GameObject rewardAsset;
	public GameObject receiver;
	public float threshold;
	public Stunt linkedStunt;
	public int cost = 5;
	public bool saveState = false;
    public bool cinematic = false;
    public GameObject showObject;
	
	int tone;
	float timer;
	
	void Awake () {
		timer = -10;
		tone = 0;
		if (saveState && PlayerPrefs.HasKey(name)) {
            SaveState();
		}
	}

	IEnumerator WaitForRealSeconds(float time) {
	    float startTime = Time.realtimeSinceStartup;
	    while (Time.realtimeSinceStartup - startTime < time)
	        yield return 1;
	}

    void SaveState () {
        if (linkedStunt != null) {
            linkedStunt.SaveState();
        }
        gameObject.SetActiveRecursively(false);
    }
	
	public void Init () {
		if (linkedStunt != null) {
			linkedStunt.Init();
		}
		tone = 0;
		timer = -10;
	}
	
	IEnumerator Track (Collider collider) {
		if (linkedStunt != null) {
			float dt = Time.time - linkedStunt.timer;
			if (tone > 0 || dt > threshold) {
				Init();
				Game.fx.PlaySound(badClip);
			}
			else {
				tone = linkedStunt.tone + 1;
				timer = Time.time;
				audio.pitch = (tone / (float)cost) * 0.4f + 0.6f;
				audio.PlayOneShot(goodClip);
			}
		}
		else {
			float dt = Time.time - timer;
			if (tone > 0 && dt > threshold) {
				tone = 0;
				Game.fx.PlaySound(badClip);
			}
			else {
				tone = 1;
				timer = Time.time;
				audio.pitch = (tone / (float)cost) * 0.4f + 0.6f;
				audio.PlayOneShot(goodClip);
			}
		}
		if (tone == cost) {
            Character character = (Character)collider.GetComponent(typeof(Character));
			Game.fx.PlaySound(goodClip);
			yield return new WaitForSeconds(0.2f);
            if (cinematic) {
                character.Pause();
                Time.timeScale = 0;
                Game.fx.SetLetterBox(true);
                if (showObject != null) {
                    float directorTime = 0;
                    Vector3 directorStart = Game.director.transform.position;
                    Vector3 directorEnd = 
                        showObject.transform.position + new Vector3(0, 0, Character.cameraDistance);
                    while (directorTime < 1) {
                        Game.director.transform.position = Vector3.Lerp(directorStart, directorEnd, directorTime);
                        directorTime += Game.realDeltaTime;
                        yield return 0;
                    }
                }
            }
            if (rewardClip != null) {
                Game.fx.PlaySound(rewardClip);
            }
			if (rewardAsset != null) {
                yield return new WaitForSeconds(1);
				GameObject rewardObject = (GameObject)Instantiate(rewardAsset);
				rewardObject.transform.position = transform.position;
			}
			if (receiver != null) {
                receiver.SendMessage("OnStunt", this, SendMessageOptions.DontRequireReceiver);
			}
			if (saveState) {
				PlayerPrefs.SetInt(name, 1);
			}
            if (cinematic) {
                yield return StartCoroutine(WaitForRealSeconds(0.5f));
                character.Continue();
                Time.timeScale = 1;
                Game.fx.SetLetterBox(false);
            }
			yield return 0;


			Stunt linked = linkedStunt;
			while (linked != null) {
				Destroy(linked.gameObject);
				linked = linked.linkedStunt;
			}
			Destroy(gameObject);
		}
		yield return 0;
	}
	
	void OnTriggerEnter (Collider collider) {
		if (collider.gameObject.layer == Game.characterLayer) {
			StartCoroutine(Track(collider));
		}
	}
}
