using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {
	
	public GameObject field;
	public AudioClip spawnClip;
	public AudioClip regenClip;
	public bool silent = false;
	public float fieldDuration = 1.0f;

	void Start () {
		field.SetActiveRecursively(false);
	}
	
	void OnTriggerEnter (Collider collider) {
		if (silent) {
			return;
		}
		if (collider.gameObject.layer == Game.characterLayer) {
			Character character = (Character)collider.GetComponent(typeof(Character));
			if (PlayerPrefs.GetString("Checkpoint") != transform.parent.name) {
				StartCoroutine(CheckpointCoroutine(character));
			}
		}
	}
	
	void OnTriggerExit () {
	}
	
	public void Spawn () {
		StartCoroutine(SpawnCoroutine());
	}
	
	IEnumerator CheckpointCoroutine (Character character) {
		PlayerPrefs.SetString("Checkpoint", transform.parent.name);
		Vector3 moveTo = transform.position + new Vector3(0, 0.2f, 0);
		Game.fx.PlaySound(regenClip);
		Time.timeScale = 0;
		character.AddHP(3000);
		field.SetActiveRecursively(true);
		float timer = Time.realtimeSinceStartup;
		while ((Time.realtimeSinceStartup - timer) < 0.5f) {
			character.transform.position = 
				Vector3.Lerp(character.transform.position, moveTo, Game.realDeltaTime * 10);
			yield return 0;
		}
		Time.timeScale = 1;
		field.SetActiveRecursively(false);
	}
	
	IEnumerator SpawnCoroutine () {
		Vector3 directorPos = transform.position;
		directorPos.y += 1;
		directorPos.z = Character.cameraDistance;
		Game.director.transform.position = directorPos;
		if (!silent) {
			yield return new WaitForSeconds(0.5f);
			field.SetActiveRecursively(true);
			Game.fx.PlaySound(spawnClip);
		}
		yield return new WaitForSeconds(0.1f);
		GameObject characterObject = (GameObject)Instantiate(Resources.Load("Avatar", typeof(GameObject)));
		Character character = (Character)characterObject.GetComponent(typeof(Character));
		characterObject.transform.position = transform.position;
		characterObject.transform.rotation = transform.parent.rotation;
	
		character.Rest();
		character.SetMarionette(false);
		yield return new WaitForSeconds(fieldDuration);
		float sinkDuration = 0.2f;
		while (sinkDuration > 0) {
			Vector3 fieldPosition = field.transform.localPosition;
			fieldPosition.z -= 5 * Time.deltaTime;
			field.transform.localPosition = fieldPosition;
			sinkDuration -= Time.deltaTime;
			yield return 0;
		}
		field.SetActiveRecursively(false);
	}

}
