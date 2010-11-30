using UnityEngine;
using System.Collections;

public class Switch : MonoBehaviour {
	
	public GameObject step;
	public GameObject upReceiver;
	public GameObject downReceiver;
	public GameObject showUpObject;
	public GameObject showDownObject;
	public bool showSpirit = false;
	public GameObject spiritTarget;
	public bool oneShot = false;
	public bool startDown = false;
	public AudioClip moveClip;
	public AudioClip snapClip;
	public bool saveState = false;
	public float upSpeed = 1;
	
	float downSpeed = 1;
	float pressing = -1;
	bool pressed = false;
	bool locked = false;
	
	void Start () {
		if (saveState && PlayerPrefs.HasKey(transform.parent.name)) {
			Vector3 toPos = new Vector3(0, 0.16f, 0);
			step.transform.localPosition = toPos;
			gameObject.SetActiveRecursively(false);
		}
		else if (startDown) {
			Vector3 toPos = new Vector3(0, 0.16f, 0);
			step.transform.localPosition = toPos;
			pressed = true;
			StartCoroutine(Rewind());
		}
		audio.volume = Game.fx.soundVolume;
	}
	
	void OnTriggerStay (Collider collider) {
		if (collider.gameObject.layer == Game.characterLayer) {
			if (Game.character.InSwim()) {
				return;
			}
		}
		Rigidbody rb = collider.rigidbody;
		if (rb != null && !rb.isKinematic 
			&& (rb.mass > 40 || collider.gameObject.layer == Game.characterLayer)) {
			if (pressing < 0 && !locked) {
				pressing = 0.5f;
				StartCoroutine(Press());
			}
			pressing = 0.5f;
		}
	}
	
	IEnumerator Press () {
		audio.Stop();
		Game.fx.PlaySound(moveClip);
		while (pressing > 0 && !locked) {
			pressing -= Time.deltaTime;
			if (pressing < 0) {
				StartCoroutine(Rewind());
			}
			Vector3 toPos = step.transform.localPosition + new Vector3(0, -downSpeed, 0) * Time.deltaTime;
			if (toPos.y < 0.16f) {
				toPos.y = 0.16f;
				if (!pressed) {
					Game.fx.PlaySound(snapClip);
					pressed = true;
					if (showDownObject != null) {
						if (showSpirit) {
							if (spiritTarget == null && Game.character != null) {
								spiritTarget = Game.character.gameObject;
							}
							Game.spirit.SetTarget(spiritTarget);
						}
						string[] message = TextTable.GetLines(transform.parent.name);
						if (Game.character != null) {
							Game.character.Pause();
						}
						Time.timeScale = 0;
						Game.fx.SetLetterBox(true);
						float letterTime = Time.realtimeSinceStartup;
						if (downReceiver != null) {
							float directorTime = 0;
							Vector3 directorStart = Game.director.transform.position;
							Vector3 directorEnd = 
								showDownObject.transform.position + new Vector3(0, 0, Character.cameraDistance);
							while (directorTime < 1) {
								Game.director.transform.position = Vector3.Lerp(directorStart, directorEnd, directorTime);
								directorTime += Game.realDeltaTime;
								yield return 0;
							}
						}
						if ((Time.realtimeSinceStartup - letterTime) < 1) {
							yield return StartCoroutine(WaitForRealSeconds(1));
						}
						if (downReceiver != null) {
							downReceiver.SendMessage("OnSwitchDown", this, SendMessageOptions.DontRequireReceiver);
						}
						if (message.Length == 0) {
							yield return StartCoroutine(WaitForRealSeconds(2));
						}
						for (int i=0; i<message.Length; ++i) {
							yield return Game.hud.StartCoroutine(Game.hud.ShowMessage(message[i], -1));
						}
						if (Game.character != null) {
							Game.character.Continue();
						}
						Time.timeScale = 1;
						Game.fx.SetLetterBox(false);
						if (Game.spirit != null && Game.spirit.gameObject.active) {
							Game.spirit.SetTarget(null);
						}
						showDownObject = null;
					}
					else if (downReceiver != null) {
						downReceiver.SendMessage("OnSwitchDown", this, SendMessageOptions.DontRequireReceiver);
					}
					if (oneShot || saveState) {
						locked = true;
					}
					PlayerPrefs.SetInt(transform.parent.name, 1);
				}
			}
			step.transform.localPosition = toPos;
			yield return new WaitForFixedUpdate();
		}
	}
	
	IEnumerator Rewind () {
		if (upSpeed < 1) {
			audio.Play();
		}
		else {
			Game.fx.PlaySound(moveClip);
		}
		while (pressing < 0 && !locked) {
			Vector3 toPos = step.transform.localPosition + new Vector3(0, upSpeed, 0) * Time.deltaTime;
			if (toPos.y > 0.5f) {
				toPos.y = 0.5f;
				if (pressed) {
					audio.Stop();
					Game.fx.PlaySound(snapClip);
					pressed = false;
					if (showUpObject != null) {
						if (showSpirit) {
							if (spiritTarget == null && Game.character != null) {
								spiritTarget = Game.character.gameObject;
							}
							Game.spirit.SetTarget(spiritTarget);
						}
						string[] message = TextTable.GetLines(transform.parent.name);
						if (Game.character != null) {
							Game.character.Pause();
						}
						Time.timeScale = 0;
						Game.fx.SetLetterBox(true);
						float letterTime = Time.realtimeSinceStartup;
						if (downReceiver != null) {
							float directorTime = 0;
							Vector3 directorStart = Game.director.transform.position;
							Vector3 directorEnd = 
								showUpObject.transform.position + new Vector3(0, 0, Character.cameraDistance);
							while (directorTime < 1) {
								Game.director.transform.position = Vector3.Lerp(directorStart, directorEnd, directorTime);
								directorTime += Game.realDeltaTime;
								yield return 0;
							}
						}
						if ((Time.realtimeSinceStartup - letterTime) < 1) {
							yield return StartCoroutine(WaitForRealSeconds(1));
						}
						if (upReceiver != null) {
							upReceiver.SendMessage("OnSwitchUp", this, SendMessageOptions.DontRequireReceiver);
						}
						if (message.Length == 0) {
							yield return StartCoroutine(WaitForRealSeconds(2));
						}
						for (int i=0; i<message.Length; ++i) {
							yield return Game.hud.StartCoroutine(Game.hud.ShowMessage(message[i], -1));
						}
						if (Game.character != null) {
							Game.character.Continue();
						}
						Time.timeScale = 1;
						Game.fx.SetLetterBox(false);
						if (Game.spirit != null && Game.spirit.gameObject.active) {
							Game.spirit.SetTarget(null);
						}
						showUpObject = null;
					}
					else if (upReceiver != null) {
						upReceiver.SendMessage("OnSwitchUp", this, SendMessageOptions.DontRequireReceiver);
					}
					if (oneShot || saveState) {
						locked = true;
					}
					PlayerPrefs.SetInt(transform.parent.name, 1);
				}
			}
			step.transform.localPosition = toPos;
			yield return new WaitForFixedUpdate();
		}
	}
	
	IEnumerator WaitForRealSeconds(float time) {
	    float startTime = Time.realtimeSinceStartup;
	    while (Time.realtimeSinceStartup - startTime < time)
	        yield return 1;
	}

	
	
}
