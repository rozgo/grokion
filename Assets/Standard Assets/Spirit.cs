using UnityEngine;
using System.Collections;

public class Spirit : MonoBehaviour {
	
	public AudioClip spiritAlertClip;
	
	GameObject target;
	new Transform transform;
	
	void Awake () {
		if (FindObjectsOfType(GetType()).Length > 1) {
			Debug.LogError("Multiple objects of type: "+GetType());
		}
		transform = gameObject.transform;
		Game.spirit = this;
		gameObject.SetActiveRecursively(false);
		if (Game.character != null) {
			transform.position = Game.character.collider.bounds.center +
				new Vector3(0, 0, 1.2f);
		}
	}
	
	public void SetTarget (GameObject target) {
		if (!gameObject.active) {
			if (Game.character != null) {
				Game.fx.PlaySound(spiritAlertClip);
				transform.position = Game.character.collider.bounds.center +
					new Vector3(0, 0, 1.2f);
			}
		}
		gameObject.SetActiveRecursively(true);
		this.target = target;
	}
	
	void Update () {
		if (Game.character != null && target == Game.character.gameObject) {
 			Vector3 targetPosition = target.transform.position + 
 				target.transform.right * 2 + target.transform.up * 1.5f +
				new Vector3(0, Mathf.Sin(Game.realTime * 2) * 0.3f, 1.2f);
			Vector3 targetDirection = targetPosition - transform.position;
			float totalSpeed = Mathf.Min(targetDirection.magnitude, 8);
			transform.position += targetDirection.normalized * Game.realDeltaTime * totalSpeed;
		}
		else if (target != null && target.collider != null) {
 			Vector3 targetPosition = target.collider.bounds.center + 
				new Vector3(0, Mathf.Sin(Game.realTime * 2) * 0.3f, 1.2f);
			Vector3 targetDirection = targetPosition - transform.position;
			float totalSpeed = Mathf.Min(targetDirection.magnitude, 8);
			transform.position += targetDirection.normalized * Game.realDeltaTime * totalSpeed;
		}
		else if (target != null) {
 			Vector3 targetPosition = target.transform.position + 
				new Vector3(0, Mathf.Sin(Game.realTime * 2) * 0.3f, 1.2f);
			Vector3 targetDirection = targetPosition - transform.position;
			float totalSpeed = Mathf.Min(targetDirection.magnitude, 8);
			transform.position += targetDirection.normalized * Game.realDeltaTime * totalSpeed;
		}
		else {
 			Vector3 targetPosition = Game.character.collider.bounds.center + 
				new Vector3(0, 0, 1.2f);
			Vector3 targetDirection = targetPosition - transform.position;
			float distance = targetDirection.magnitude;
			float totalSpeed = 8;
			transform.position += targetDirection.normalized * Game.realDeltaTime * totalSpeed;
			if (distance < 0.5f) {
				gameObject.SetActiveRecursively(false);
			}
		}
		if (target != null && target.rigidbody != null) {
			//transform.position += target.rigidbody.velocity * Time.deltaTime;
		}
	}
}
