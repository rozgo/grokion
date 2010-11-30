using UnityEngine;
using System.Collections;

public class Vignette : MonoBehaviour {
	
	public void MoveTo (GameObject obj, GameObject target, float duration) {
		StartCoroutine(MoveToCoroutine(obj, target, duration));
	}
	
	public IEnumerator MoveToCoroutine (GameObject obj, GameObject target, float duration) {
		float time = 0;
		Vector3 start = obj.transform.position;
		Vector3 end = target.transform.position;
		while (time < 1) {
			obj.transform.position = Vector3.Lerp(start, end, time);
			time += Game.realDeltaTime / duration;
			yield return 0;
		}
	}
	
	public void LookAt (GameObject obj, GameObject target, float duration) {
		StartCoroutine(LookAtCoroutine(obj, target, duration));
	}
	
	public IEnumerator LookAtCoroutine (GameObject obj, GameObject target, float duration) {
		float time = 0;
		Quaternion start = obj.transform.rotation;
		Quaternion end = Quaternion.LookRotation(target.transform.position - obj.transform.position);
		while (time < 1) {
			obj.transform.rotation = Quaternion.Slerp(start, end, time);
			time += Game.realDeltaTime / duration;
			yield return 0;
		}
	}
	
}
