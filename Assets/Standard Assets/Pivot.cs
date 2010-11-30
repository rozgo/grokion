using UnityEngine;
using System.Collections;

public class Pivot : MonoBehaviour {
	
	public new Camera camera;
	public Material sharedMaterialNormal;
	public Material sharedMaterialActive;
	
	bool center = false;
	new Transform transform;
	new Collider collider;
	float keySpeed = 10;
	bool enableKeys = true;
	FingerManager fingerManager;

	void Awake () {
		transform = gameObject.transform;
		collider = gameObject.collider;
	}
	
	void Start () {
		fingerManager = (FingerManager)FindObjectOfType(typeof(FingerManager));
	}

	void Update () {
		if (center) {
			Vector3 localPosition = transform.localPosition;
			Vector3 velocity = localPosition * Time.deltaTime * 20;
			localPosition -= Vector3.Min(localPosition, velocity);
			if (localPosition.sqrMagnitude < 0.1f) {
				localPosition = Vector3.zero;
				center = false;
			}
			transform.localPosition = localPosition;
		}
		else if (enableKeys && Application.isEditor) {
			
			Vector3 localPosition = transform.localPosition;
			
			bool reset = true;
			if (Input.GetKey("w")) {
				reset = false;
				localPosition.y += Time.deltaTime * keySpeed;
			}
			if (Input.GetKey("s")) {
				reset = false;
				localPosition.y -= Time.deltaTime * keySpeed;
			}
			
			if (reset) {
				localPosition.y = 0;
			}
			
			reset = true;
			if (Input.GetKey("a")) {
				reset = false;
				localPosition.x -= Time.deltaTime * keySpeed;
			}
			if (Input.GetKey("d")) {
				reset = false;
				localPosition.x += Time.deltaTime * keySpeed;
			}
			
			if (reset) {
				localPosition.x = 0;
			}
			
			if (localPosition.sqrMagnitude > 1) {
				localPosition = localPosition.normalized;
			}
			transform.localPosition = localPosition;
		}
	}
	
	void OnFingerBegin () {
		enableKeys = false;
		center = false;
		renderer.material = sharedMaterialActive;
		Vector3 touchPosition = fingerManager.GetTouchPosition(collider);
		Vector3 worldTouchPos = camera.ScreenToWorldPoint(touchPosition);
		transform.position = worldTouchPos;
		Vector3 localPosition = transform.localPosition;
		localPosition.z = 0;
		transform.localPosition = localPosition;
		if (transform.localPosition.sqrMagnitude > 1) {
			transform.localPosition = transform.localPosition.normalized;
		}
	}
	
	void OnFingerMove () {
		Vector3 touchPosition = fingerManager.GetTouchPosition(collider);
		Vector3 worldTouchPos = camera.ScreenToWorldPoint(touchPosition);
		transform.position = worldTouchPos;
		Vector3 localPosition = transform.localPosition;
		localPosition.z = 0;
		transform.localPosition = localPosition;
		if (transform.localPosition.sqrMagnitude > 1) {
			transform.localPosition = transform.localPosition.normalized;
		}
	}
	
	void OnFingerEnd () {
		renderer.material = sharedMaterialNormal;
		center = true;
		enableKeys = true;
	}

	void OnFingerCancel () {
		renderer.material = sharedMaterialNormal;
		center = true;
		enableKeys = true;
	}
}
