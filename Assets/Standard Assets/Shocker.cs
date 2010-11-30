using UnityEngine;
using System.Collections;

public class Shocker : MonoBehaviour {
	
	public float damage = 20;
	public float initialDelay = 0;
	public float rate = 3;
	public float duration = 3;
	public Transform terminalA;
	public Transform terminalB;
	public LineRenderer line;
	
	void Awake () {
	}
	
	void Start () {
		StartCoroutine(DoShocker());
	}
	
	IEnumerator DoShocker () {
		yield return new WaitForSeconds(initialDelay);
		while (true) {
			audio.volume = Game.fx.soundVolume;
			audio.Play();
			line.enabled = true;
			float shocking = duration;
			while (shocking > 0) {
				float w = Mathf.Sin(shocking * 70)*0.8f + 0.8f;
				shocking -= Time.deltaTime;
	            line.SetPosition(0, terminalA.position);
	            line.SetPosition(1, terminalB.position);
	            line.SetWidth(w,w);
	            RaycastHit[] hits = Physics.RaycastAll(terminalA.position,
	            	(terminalB.position-terminalA.position).normalized,
	            	Vector3.Distance(terminalA.position, terminalB.position));
	            foreach (RaycastHit hit in hits) {
	            	hit.collider.SendMessage("Hit", damage, SendMessageOptions.DontRequireReceiver);
	            }
	            yield return 0;
			}
			audio.Stop();
			line.enabled = false;
			yield return new WaitForSeconds(rate);
		}
	}

}
