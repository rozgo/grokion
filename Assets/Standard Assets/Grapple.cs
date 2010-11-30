using UnityEngine;
using System.Collections;

public class Grapple : MonoBehaviour {
	
	public Character character;
	public bool Hooked { get { return hooked && (attachPoint != null); } }

    //public bool On {get {return on;}}
	bool on = false;
	float length = 8;
	int layerMask = 0;
	Collider hookedCollider;
	Rigidbody hookedRigidbody;
	bool hooked;
	bool retracting;
	Rigidbody rb;
	TouchRock touchRock;
	HiddenRock hiddenRock;

    public LineRenderer line;

    public Vector3 hookPoint {get{return attachPoint.transform.position;}}
    public GameObject attachPoint;

	void Start () {
		layerMask |= Game.defaultLayer;
        line.SetWidth(0.2f, 0.2f);
	}
	
	public void Begin () {
		rb = null;
		hooked = false;
		retracting = false;
		touchRock = null;
		hiddenRock = null;
		on = true;
		StartCoroutine(Grappling());
	}
	
	public void End () {
		on = false;
		hooked = false;
	}
	
	IEnumerator Grappling () {
        if (attachPoint == null) {
            attachPoint = new GameObject("AttachPoint");
            attachPoint.AddComponent(typeof(AttachPoint));
        }
        Vector3 lineOrigin = character.collider.bounds.center;
        lineOrigin.z = 0;
        Vector3 lineDirection = -character.rightArm.right;
        lineDirection.z = 0;
        lineDirection.Normalize();
        attachPoint.transform.parent = null;
        attachPoint.transform.position = lineOrigin + lineDirection * length;
		float scale = 0.5f;
		while (on) {
            if (attachPoint == null) {
                attachPoint = new GameObject("AttachPoint");
                attachPoint.AddComponent(typeof(AttachPoint));
                character.GrappleOff();
                continue;
            }
            line.SetPosition(0, transform.position);
            line.SetPosition(1,
                             transform.position + 
                             (attachPoint.transform.position - transform.position).normalized * scale );
			if (hooked) {
                scale = Vector3.Distance(attachPoint.transform.position, transform.position) + 1.5f;
                line.SetPosition(1, attachPoint.transform.position);
				if (touchRock != null && touchRock.Desintegrated) {
					character.GrappleOff();
				}
				if (hiddenRock != null && hiddenRock.Desintegrated) {
					character.GrappleOff();
				}
				if (scale > length + 2) {
					character.GrappleOff();
				}
				if (rb != null && scale > 3 && !character.InJump()) {
                    Vector3 direction = transform.right;
                    direction.z = 0;
                    direction.Normalize();
					Vector3 pointOfAffect = attachPoint.transform.position;
					rb.AddForceAtPosition(100000 * direction * Time.deltaTime, pointOfAffect, ForceMode.Force);
				}
			}
			else if (retracting) {
				if (rb != null) {
                    Vector3 direction = character.rightArm.right;
                    direction.z = 0;
                    direction.Normalize();
					Vector3 pointOfAffect = transform.position - direction * scale;
                    pointOfAffect.z = 0;
					rb.AddForceAtPosition(-200000 * direction * Time.deltaTime, pointOfAffect, ForceMode.Force);
				}
				scale -= 50.0f * Time.deltaTime;
				if (scale < 0.1f) {
					character.GrappleOff();
				}
			}
			else {
				scale += 50.0f * Time.deltaTime;
				RaycastHit hit;
				if (scale > 2 && !hooked && 
                    Physics.Raycast(lineOrigin, lineDirection, out hit, scale, Game.defaultMask)) {
                    attachPoint.transform.position = hit.point;
                    attachPoint.transform.parent = hit.collider.transform;
					scale = hit.distance + 2;
                    hit.collider.SendMessage("Hit", 0, SendMessageOptions.DontRequireReceiver);
					Door door = (Door)hit.collider.gameObject.GetComponent(typeof(Door));
					touchRock = (TouchRock)hit.collider.gameObject.GetComponent(typeof(TouchRock));
					hiddenRock = (HiddenRock)hit.collider.gameObject.GetComponent(typeof(HiddenRock));
					if (hit.collider.isTrigger && hiddenRock == null) {
					}
					else if (door) {
						retracting = true;
						door.Open();
                        hooked = true;
					}
					else if (hit.rigidbody != null && !hit.rigidbody.isKinematic) {
						rb = hit.rigidbody;
                        hooked = true;
					}
					else if (touchRock != null) {
						if (!touchRock.Desintegrated) {
							hooked = true;
							touchRock.Activate();
						}
					}
					else if (hiddenRock != null) {
						if (!hiddenRock.Desintegrated) {
							hooked = true;
							hiddenRock.Activate();
						}
					}
					else {
						hooked = true;
					}
					if (hooked) {
						character.jumpCount = 0;
					}
				}
				else if (scale > length) {
					retracting = true;
				}
				if (scale > length) {
					scale = length;
				}
			}
			yield return new WaitForFixedUpdate();
		}
	}
}
