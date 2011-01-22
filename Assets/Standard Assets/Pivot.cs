using UnityEngine;
using System.Collections;

public class Pivot : MonoBehaviour {
	
	public Vector3 mousePosition = Vector3.zero;
	
	new Transform transform;

	void Awake () {
		transform = gameObject.transform;
	}
	
	void Start () {
		mousePosition.x = Screen.width / 2;
		mousePosition.y = Screen.height / 2;
	}

	void Update () {

		if (!Game.hud.gameObject.active || !Game.hud.controlsEnabled || Game.character == null) {
			Game.hud.crosshair.renderer.enabled = false;
		}
		else if (ControlSettings.showCursor && (Screen.lockCursor || Game.hud.crosshair.renderer.enabled)) {
			Screen.lockCursor = false;
			Game.hud.crosshair.renderer.enabled = false;
		}
		else if (!ControlSettings.showCursor && (!Screen.lockCursor || !Game.hud.crosshair.renderer.enabled)) {
			Screen.lockCursor = true;
			Game.hud.crosshair.renderer.enabled = true;
		}

		if (Screen.lockCursor) {
			mousePosition.z = 0;
			mousePosition.x += Input.GetAxis("Mouse X") * ControlSettings.mouseSensitivity/100.0f;
			mousePosition.y += Input.GetAxis("Mouse Y") * ControlSettings.mouseSensitivity/100.0f;
			mousePosition.x = Mathf.Clamp(mousePosition.x, 0, Screen.width);
			mousePosition.y = Mathf.Clamp(mousePosition.y, 0, Screen.height);
		}
		else {
			mousePosition = Input.mousePosition;
		}

		Vector3 crosshairPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.z));
		crosshairPosition.z = 0;
		Game.hud.crosshair.transform.position = crosshairPosition;

		InputProxy input = InputProxy.Get();
		Vector3 localPosition = transform.localPosition;
		localPosition.x = input.GetValue("Horizontal");
		localPosition.y = input.GetValue("Vertical");			
		if (localPosition.sqrMagnitude > 1) {
			localPosition = localPosition.normalized;
		}
		transform.localPosition = localPosition;
	}

	void OnDrawGizmos () {
		if (Game.character != null) {
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(Game.hud.crosshair.transform.position, 0.1f);
		}
	}
}
