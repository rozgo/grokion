using UnityEngine;
using System.Collections;

public class InputProxy : MonoBehaviour {

	static InputProxy instance;
	public static InputProxy Get () {
		if (instance == null) {
			GameObject obj = (GameObject)Instantiate(Resources.Load("InputProxy", typeof(GameObject)));
			instance = obj.GetComponent<InputProxy>();
		}
		return instance;
	}

	public enum AxisType {
		Key,
		Mouse,
	}

	[System.Serializable]
	public class Axis {
		public string name;
		public string positive;
		public string negative;
		public float value;
		public float lastValue;
		public float sensitivity = 1;
		public float dead = 0.001f;
		public AxisType type;
	}
	
	public Axis[] axes;

	float deltaTime = 0.01f;
	float time = 0;

	public Axis GetAxis(string name) {
		foreach (Axis axis in axes) {
			if (axis.name == name) {
				return axis;
			}
		}
		return null;
	}

	public float GetValue (string name) {
		return GetAxis(name).value;
	}

	public void Remap (string name, string field, string key) {
		Axis axis = GetAxis(name);
		System.Reflection.FieldInfo info = axis.GetType().GetField(field);
		info.SetValue(axis, System.Convert.ChangeType(key, info.FieldType));
	}

	void Awake () {
		DontDestroyOnLoad(gameObject);
		instance = this;
	}

	void Update () {

		deltaTime = Time.realtimeSinceStartup - time;
		time = Time.realtimeSinceStartup;

		foreach (Axis axis in axes) {     
			if (axis.positive.Length > 0 && Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), axis.positive))) {
				axis.value = Mathf.Min(axis.value += deltaTime * axis.sensitivity, 1);
			}
			else if (axis.negative.Length > 0 && Input.GetKey((KeyCode)System.Enum.Parse(typeof(KeyCode), axis.negative))) {
				axis.value = Mathf.Max(axis.value -= deltaTime * axis.sensitivity, -1);
			}
			else if (Mathf.Abs(axis.value) > axis.dead) {
				axis.value += -Mathf.Sign(axis.value) * Mathf.Min(deltaTime * axis.sensitivity, Mathf.Abs(axis.value));
			}
			else {
				axis.value = 0;
			}
		}
	}
	
}
