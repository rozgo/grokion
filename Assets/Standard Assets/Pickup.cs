using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {
	
	Color originalColor;
	float alphaTime = 0;
	float timer = 9;
	
	void Awake () {
		originalColor = renderer.material.GetColor("_TintColor");
	}
	
	void Start () {
	}
	
	void Update () {
		Color color = originalColor;
		if (timer<3) {
			alphaTime += Time.deltaTime*13;
			float alphaColor = Mathf.PingPong(alphaTime,1);
			color.a = alphaColor;
		}
		else {
			alphaTime += Time.deltaTime*2;
			float alphaColor = Mathf.PingPong(alphaTime,0.5f)+0.5f;
			color.r = alphaColor;
			color.g = alphaColor;
			color.b = alphaColor;
			color.a = 1;
		}
		renderer.material.SetColor("_TintColor",color);
		timer -= Time.deltaTime;
		if (timer<0) {
			Destroy(gameObject);
		}
	}
}
