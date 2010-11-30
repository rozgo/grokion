using UnityEngine;
using System.Collections;

public class SimFixedTime : MonoBehaviour {
	
	void Start () {
		
	}
	
	IEnumerator Simulate () {
		
		bool done = false;
		float accumulator = 0;
		int fpsFixedSimulated = 0;
		float frameTime = 0;
		
		while (!done) {
			
			accumulator += Time.deltaTime;
			if (accumulator > Time.fixedDeltaTime) {
				accumulator -= Time.fixedDeltaTime;
				++fpsFixedSimulated;
				// move physics here
			}
			
			// optional, for debug
			frameTime += Time.deltaTime;
			if (frameTime > 1) {
				Debug.Log("Simulated fixed fps: " + fpsFixedSimulated.ToString());
				frameTime -= 1;
				fpsFixedSimulated = 0;
			}
			// end optional
			
			yield return 0;
		}
	}
}
