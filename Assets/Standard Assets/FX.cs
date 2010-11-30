using UnityEngine;
using System.Collections;

public class FX : MonoBehaviour {
	
	public GameObject hitAsset;
	public GameObject dieAsset;
	public GameObject deathAsset;
	public GameObject flash;
	public GameObject cache;
	public GameObject dramaBox;
	public GameObject letterBoxUpper;
	public GameObject letterBoxLower;
	public GameObject avatarCard;
	public float explosionRadius = 5;
	public float soundVolume = 1.0f;
	public float musicVolume = 1.0f;
	
	Material specialMaterial;
    AudioClip deathClip;
    AudioClip collectClip;
    AudioClip tokenClip;
    AudioClip foundClip;
    AudioClip victoryClip;
	AudioClip explosionClip;
    AudioClip flamesClip;
	new AudioSource audio;
	AudioSource pitchedAudio;
	AudioSource loopAudio;
	AudioSource musicAudio;
	AudioClip currentMusic;
	float shakeDuration;
	float shakeAmount;
	bool shaking = false;
	float flashDuration;
	bool flashing;
	Color specialColor;
	float specialTime = 0;
	GameObject[] explosions;
	int maxExplosions = 5;
	GameObject[] hits;
	int maxHits = 5;
	GameObject[] kills;
	int maxKills = 5;
	int letterBoxState = 0;
	
	void Awake () {
		if (PlayerPrefs.GetString("GameType", "Grokion") == "Grokion") {
			specialMaterial = (Material)Resources.Load("FXAtlas", typeof(Material));
		}
		else if (PlayerPrefs.GetString("GameType") == "Grid") {
			specialMaterial = (Material)Resources.Load("FXGridAtlas", typeof(Material));
		}
		if (FindObjectsOfType(GetType()).Length > 1) {
			Debug.LogError("Multiple singletons of type: " + GetType());
		}
		audio = gameObject.audio;
		pitchedAudio = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		pitchedAudio.volume = soundVolume;
		loopAudio = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		loopAudio.volume = soundVolume;
		loopAudio.loop = true;
		musicAudio = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
		musicAudio.volume = musicVolume;
		specialColor = specialMaterial.GetColor("_TintColor");
		SetSoundVolume(PlayerPrefs.GetFloat("SoundVolume", 1.0f));
		//SetSoundVolume(0);
		SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1.0f));
		//SetMusicVolume(0);
	}

	void Start () {
		letterBoxUpper.transform.localPosition = new Vector3(0, 1.5f, 0.5f);
		letterBoxLower.transform.localPosition = new Vector3(0, -1.5f, 0.5f);
		letterBoxUpper.active = false;
		letterBoxLower.active = false;
		avatarCard.active = false;
	}
	
	public void SetSoundVolume (float volume) {
		PlayerPrefs.SetFloat("SoundVolume", volume);
		soundVolume = Mathf.Clamp(volume, 0, 1);
		foreach (AudioSource audioSource in FindObjectsOfType(typeof(AudioSource))) {
			if (audioSource != musicAudio) {
				audioSource.volume = soundVolume;
			}
		}
	}
	
	public void SetMusicVolume (float volume) {
		PlayerPrefs.SetFloat("MusicVolume", volume);
		musicVolume = Mathf.Clamp(volume, 0, 1);
		musicAudio.volume = volume;
	}
	
	public void SetLetterBox (bool on) {
		if (on) {
			letterBoxState = 1;
			StartCoroutine(LetterBoxOn());
		}
		else {
			letterBoxState = 2;
			StartCoroutine(LetterBoxOff());
		}
	}

	IEnumerator LetterBoxOn () {
		letterBoxUpper.active = true;
		letterBoxLower.active = true;
		float speed = 0.5f;
		int d = 0;
		while (letterBoxState == 1 && d != 2) {
			d = 0;
			Vector3 position = letterBoxUpper.transform.localPosition;
			position.y -= speed * Game.realDeltaTime;
			if (position.y < 1.2f) {
				position.y = 1.2f;
				++d;
			}
			letterBoxUpper.transform.localPosition = position; 
			position = letterBoxLower.transform.localPosition;
			position.y += speed * Game.realDeltaTime;
			if (position.y > -1.2f) {
				position.y = -1.2f;
				++d;
			}
			letterBoxLower.transform.localPosition = position;
			yield return 0;
		}
		letterBoxState = 0;
	}
	
	IEnumerator LetterBoxOff () {
		float speed = 0.5f;
		int d = 0;
		while (letterBoxState == 2 && d != 2) {
			d = 0;
			Vector3 position = letterBoxUpper.transform.localPosition;
			position.y += speed * Game.realDeltaTime;
			if (position.y > 1.3f) {
				position.y = 1.3f;
				++d;
			}
			letterBoxUpper.transform.localPosition = position; 
			position = letterBoxLower.transform.localPosition;
			position.y -= speed * Game.realDeltaTime;
			if (position.y < -1.3f) {
				position.y = -1.3f;
				++d;
			}
			letterBoxLower.transform.localPosition = position;
			yield return 0;
		}
		letterBoxState = 0;
		letterBoxUpper.transform.localPosition = new Vector3(0, 1.5f, 0.5f);
		letterBoxLower.transform.localPosition = new Vector3(0, -1.5f, 0.5f);
		letterBoxUpper.active = false;
		letterBoxLower.active = false;
	}
	
	public void ResetLetterBox () {
		letterBoxState = 0;
		letterBoxUpper.transform.localPosition = new Vector3(0, 1.5f, 0.5f);
		letterBoxLower.transform.localPosition = new Vector3(0, -1.5f, 0.5f);
		letterBoxUpper.active = false;
		letterBoxLower.active = false;
	}
	
	void Update () {
		if (shaking) {
			shakeDuration -= Time.deltaTime;
			if (shakeDuration<0) {
				shaking = false;
			}
			else {
				transform.position += 
					Random.onUnitSphere * shakeAmount * Time.deltaTime;
			}
		}
		if (flashing) {
			flashDuration -= Time.deltaTime;
			if (flashDuration<0) {
				flashing = false;
				flash.renderer.enabled = false;
			}
		}
		Color color = specialColor;
		specialTime += Game.realDeltaTime * 2;
		float alphaColor = Mathf.PingPong(specialTime, 0.5f) + 0.5f;
		color.r = alphaColor;
		color.g = alphaColor;
		color.b = alphaColor;
		color.a = 1;
		specialMaterial.SetColor("_TintColor", color);
	}
	
	public void LoopSound (AudioClip sound) {
		loopAudio.volume = soundVolume;
		loopAudio.pitch = 1;
		loopAudio.clip = sound;
		loopAudio.Play();
	}
	
	public void StopSound (AudioClip sound) {
		if (loopAudio.clip == sound) {
			loopAudio.Stop();
			loopAudio.clip = null;
		}
	}
	
	public void PlaySound (AudioClip sound) {
		audio.volume = soundVolume;
		audio.pitch = 1;
		audio.PlayOneShot(sound);
	}
	
	public void PlaySoundPitched (AudioClip sound) {
		pitchedAudio.volume = soundVolume;
		pitchedAudio.pitch = Random.Range(0.8f, 1);
		pitchedAudio.PlayOneShot(sound);
	}
	
	public void PlayCollectSound () {
        if (collectClip == null) {
            collectClip = (AudioClip)Resources.Load("Collect", typeof(AudioClip));
        }
		PlaySound(collectClip);
	}

	public void PlayTokenSound () {
        if (tokenClip == null) {
            tokenClip = (AudioClip)Resources.Load("sfx_token", typeof(AudioClip));
        }
		PlaySound(tokenClip);
	}
	
	public void ShowAvatarCard (string card) {
		avatarCard.active = true;
		Texture2D texture = (Texture2D)Resources.Load("avatars/" + card, typeof(Texture2D));
		avatarCard.renderer.material.mainTexture = texture;
	}
	
	public void HideAvatarCard () {
		avatarCard.active = false;
	}

    public ParticleEmitter Burn (GameObject target) {
        PlayFlamesSound();
        GameObject flamesObject = (GameObject)Instantiate(Resources.Load("Flames", typeof(GameObject)));
        if (target.collider != null) {
            flamesObject.transform.position = target.collider.bounds.center;
        }
        else {
            flamesObject.transform.position = target.transform.position;
        }
        flamesObject.transform.position = flamesObject.transform.position + new Vector3(0, 0, 1);
        ParticleEmitter flames = (ParticleEmitter)flamesObject.GetComponent(typeof(ParticleEmitter));
        flamesObject.transform.parent = target.transform;
        return flames;
    }

    public void PlayFlamesSound () {
        if (flamesClip == null) {
            flamesClip = (AudioClip)Resources.Load("Burning", typeof(AudioClip));
        }
        PlaySound(flamesClip);
    }

	public void Death () {
        if (deathClip == null) {
            deathClip = (AudioClip)Resources.Load("Death", typeof(AudioClip));
        }
		PlaySound(deathClip);
		GameObject deathObject = (GameObject)Instantiate(deathAsset);
		deathObject.transform.position = transform.position + transform.forward * 2;
		deathObject.transform.parent = transform;
	}
	
	public void PlayMusic (AudioClip music) {
		currentMusic = music;
		musicAudio.volume = musicVolume;
		musicAudio.clip = music;
		musicAudio.loop = true;
		musicAudio.Play();
	}
	
	public void PlayMusicEvent (AudioClip music) {
		musicAudio.volume = musicVolume;
		musicAudio.clip = music;
		musicAudio.loop = true;
		musicAudio.Play();
	}
	
	public void PlayMusic () {
		if (currentMusic != null) {
			PlayMusic(currentMusic);
		}
	}

	public void StopMusic () {
		musicAudio.Stop();
	}
	
	public void PlayFoundMusic () {
		musicAudio.volume = musicVolume;
        if (foundClip == null) {
            foundClip = (AudioClip)Resources.Load("Grokion-Victory", typeof(AudioClip));
        }
		musicAudio.clip = foundClip;
		musicAudio.loop = false;
		musicAudio.Play();
	}
	
	public void PlayVictoryMusic () {
		musicAudio.volume = musicVolume;
        if (victoryClip == null) {
            victoryClip = (AudioClip)Resources.Load("Grokion-YouWin", typeof(AudioClip));
        }
		musicAudio.clip = victoryClip;
		musicAudio.loop = false;
		musicAudio.Play();
	}
	
	public void FadeOut (float time) {
		CameraFade cameraFade = (CameraFade)GetComponent(typeof(CameraFade));
		cameraFade.FadeOut(time);
	}
	
	public void FadeIn (float time) {
		CameraFade cameraFade = (CameraFade)GetComponent(typeof(CameraFade));
		cameraFade.FadeIn(time);
	}
	
	public void QuickShake (float duration, float amount) {
		if (!shaking) {
			shaking = true;
		}
		this.shakeDuration = duration;
		this.shakeAmount = amount;
	}
	
	public void Flash (float duration) {
		flashing = true;
		this.flashDuration = duration;
		flash.renderer.enabled = true;
	}
	
	IEnumerator ReleaseKill (GameObject kill) {
		yield return new WaitForSeconds(0.3f);
		kill.SetActiveRecursively(false);
	}
	
	public void Kill (Vector3 position) {
		if (kills == null) {
			kills = new GameObject[maxKills];
			for (int i=0; i<maxKills; ++i) {
				kills[i] = (GameObject)Instantiate(dieAsset);
				kills[i].transform.parent = Game.cache.transform;
				kills[i].SetActiveRecursively(false);
			}
		}
		for (int i=0; i<maxKills; ++i) {
			GameObject kill = kills[i];
			if (!kill.active) {
				kill.SetActiveRecursively(true);
				kill.transform.position = position;
				StartCoroutine(ReleaseKill(kill));
				return;
			}
		}
	}
	
	IEnumerator ReleaseHit (GameObject hit) {
		yield return new WaitForSeconds(0.2f);
		hit.SetActiveRecursively(false);
	}
	
	public void Hit (Vector3 position, float scale) {
		if (hits == null) {
			hits = new GameObject[maxHits];
			for (int i=0; i<maxHits; ++i) {
				hits[i] = (GameObject)Instantiate(hitAsset);
				hits[i].transform.parent = Game.cache.transform;
				hits[i].SetActiveRecursively(false);
			}
		}
		for (int i=0; i<maxHits; ++i) {
			GameObject hit = hits[i];
			if (!hit.active) {
				hit.SetActiveRecursively(true);
				hit.transform.position = position;
				hit.transform.localScale = Vector3.one * scale;
				StartCoroutine(ReleaseHit(hit));
				return;
			}
		}
	}
	
	IEnumerator ReleaseExplosion (GameObject explosion) {
		yield return new WaitForSeconds(1);
		explosion.SetActiveRecursively(false);
	}
	
	public void Explode (GameObject explosionObject) {
		if (explosions == null) {
			explosions = new GameObject[maxExplosions];
			for (int i=0; i<maxExplosions; ++i) {
				if (Game.grid == null) {
					explosions[i] = (GameObject)Instantiate(Resources.Load("Explosion", typeof(GameObject)));
				}
				else {
					explosions[i] = (GameObject)Instantiate(Resources.Load("ExplosionVR", typeof(GameObject)));
				}
				explosions[i].transform.parent = Game.cache.transform;
				explosions[i].SetActiveRecursively(false);
			}
		}
		for (int i=0; i<maxExplosions; ++i) {
			GameObject explosion = explosions[i];
			if (!explosion.active) {
				explosion.SetActiveRecursively(true);
				explosion.transform.position = 
					explosionObject.collider.bounds.center + 
					(transform.position - explosionObject.collider.bounds.center).normalized * 3.0f;
				QuickShake(0.4f,20);
                if (explosionClip == null) {
                	if (Game.grid == null) {
                    	explosionClip = (AudioClip)Resources.Load("Explosion", typeof(AudioClip));
                	}
                	else {
                		explosionClip = (AudioClip)Resources.Load("ExplosionVR", typeof(AudioClip));
                	}
                }
				PlaySound(explosionClip);
				Collider[] hits = Physics.OverlapSphere(explosionObject.collider.bounds.center, explosionRadius);
				for (int h=0; h<hits.Length; ++h) {
					hits[h].SendMessage("OnExplosion", explosionObject, SendMessageOptions.DontRequireReceiver);
				}
				StartCoroutine(ReleaseExplosion(explosion));
				return;
			}
		}
	}
	
	public void ReleaseEnergy (int count, Vector3 position) {
		StartCoroutine(ReleaseEnergyTask(count, position));
	}
	
	IEnumerator ReleaseEnergyTask (int count, Vector3 position) {
    	for (int i=0; i<count; ++i) {
			GameObject energyObject = (GameObject)Instantiate(Resources.Load("Energy", typeof(GameObject)));
			energyObject.collider.isTrigger = false;
			Vector3 center = position + Random.insideUnitSphere;
			center.z = 0;
			energyObject.transform.position = center;
			energyObject.rigidbody.velocity = Random.insideUnitSphere * 5;
			yield return new WaitForSeconds(0.1f);
    	}
	}
}
