using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
	
	public float spawnRate = 2;
	public int spawnCount = -1;
    
	GameObject clone;
	ArrayList rigidbodies = new ArrayList();

	void Start () {
		if (spawnCount != 0) {
			clone = (GameObject)Instantiate(gameObject);
			clone.name = gameObject.name;
			clone.transform.position = transform.position;
			clone.transform.rotation = transform.rotation;
			clone.transform.parent = transform.parent;
			Spawner spawner = (Spawner)clone.GetComponent(typeof(Spawner));
			spawner.spawnCount = --spawnCount;
			clone.SetActiveRecursively(false);
		}
	}

    public void Spawn () {
        AttachPoint attachPoint = (AttachPoint)GetComponentInChildren(typeof(AttachPoint));
        if (attachPoint != null) {
            Destroy(attachPoint.gameObject);
        }
        if (spawnCount != 0) {
            StartCoroutine(SpawnClone());
        }
        else {
            Destroy(gameObject);
        }
    }

	IEnumerator SpawnClone () {
        Component[] joints = GetComponentsInChildren(typeof(Joint));
        for (int i=0; i<joints.Length; ++i) {
            Destroy(joints[i]);
        }
        Component[] components = GetComponentsInChildren(typeof(Component));
        for (int i=0; i<components.Length; ++i) {
            Component component = components[i];
            if (component is Transform) {
            }
            else if (component is Spawner) {
            }
            else if (component is Collider) {
                Collider collider = component as Collider;
                collider.isTrigger = true;
                collider.gameObject.layer = Game.ignoreRaycastLayer;
            }
            else {
                Destroy(component);
            }
        }
        transform.position = clone.transform.position;
        transform.rotation = clone.transform.rotation;
        yield return new WaitForSeconds(spawnRate);
        bool free;
        do {
            free = true;
            foreach (Rigidbody rb in rigidbodies) {
                if (rb != null && rb.gameObject.active) {
                    free = false;
                }
            }
            yield return new WaitForSeconds(1);
        } while (!free);
        clone.SetActiveRecursively(true);
		Destroy(gameObject);
	}

	void OnTriggerEnter (Collider collider) {
		rigidbodies.Add(collider.rigidbody);
	}
	
	void OnTriggerExit (Collider collider) {
		rigidbodies.Remove(collider.rigidbody);
	}
	
}

