using UnityEngine;
using System.Collections;

public class ControlSettings : MonoBehaviour {

	public static bool showCursor = false;
	public static int mouseSensitivity = 10;
	public static bool visualEffects = true;
    
    public AudioClip clickClip;
	new public AudioSource audio;
	new public Camera camera;
	public TextMesh[] inputs;

	float closedX = 2.01f;
	float openedX = 0.07f;

	GameObject input_select;
	TextMesh input_mouse_sensitivity;

	TextMesh selectedInput;

    void Start () {
        audio.volume = Game.fx.soundVolume;
		input_mouse_sensitivity = transform.Find("input_mouse_sensitivity").GetComponent<TextMesh>();
		input_select = transform.Find("input_select").gameObject;
		UpdateUI();
    }

	void UpdateUI () {

		input_mouse_sensitivity.text = mouseSensitivity.ToString() + "%";

		input_select.active = false;

		if (!showCursor) {
			transform.Find("cursor_on").gameObject.active = false;
			transform.Find("cursor_off").gameObject.active = true;
		}
		else {
			transform.Find("cursor_on").gameObject.active = true;
			transform.Find("cursor_off").gameObject.active = false;
		}

		if (!visualEffects) {
			transform.Find("visual_on").gameObject.active = false;
			transform.Find("visual_off").gameObject.active = true;
		}
		else {
			transform.Find("visual_on").gameObject.active = true;
			transform.Find("visual_off").gameObject.active = false;
		}

		transform.Find("input_move_right").GetComponent<TextMesh>().text = PlayerPrefs.GetString("input_move_right", "D");
		transform.Find("input_move_left").GetComponent<TextMesh>().text = PlayerPrefs.GetString("input_move_left", "A");
		transform.Find("input_look_up").GetComponent<TextMesh>().text = PlayerPrefs.GetString("input_look_up", "W");
		transform.Find("input_look_down").GetComponent<TextMesh>().text = PlayerPrefs.GetString("input_look_down", "S");
		transform.Find("input_zoom_in").GetComponent<TextMesh>().text = PlayerPrefs.GetString("input_zoom_in", "UpArrow");
		transform.Find("input_zoom_out").GetComponent<TextMesh>().text = PlayerPrefs.GetString("input_zoom_out", "DownArrow");
		transform.Find("input_fire").GetComponent<TextMesh>().text = PlayerPrefs.GetString("input_fire", "Mouse0");
		transform.Find("input_jump").GetComponent<TextMesh>().text = PlayerPrefs.GetString("input_jump", "Mouse1");
	}
    
    IEnumerator WaitForRealSeconds (float time) {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < time)
            yield return 1;
    }
    
    void OnButtonUp (Button button) {

		audio.PlayOneShot(clickClip);

		Debug.Log(button.name);
    	
        if (button.name == "title") {
        	StartCoroutine(LoadTitleScreen());
        }
		else if (button.name == "settings") {
			StartCoroutine(ToggleSettings());
		}
		else if (button.name == "default_settings") {
			ResetDefaults();
			Setup();
			UpdateUI();
		}
		else if (button.name == "cursor_on") {
			showCursor = false;
			PlayerPrefs.SetInt("input_show_cursor", 0);
			transform.Find("cursor_off").gameObject.active = true;
			transform.Find("cursor_on").gameObject.active = false;
		}
		else if (button.name == "cursor_off") {
			showCursor = true;
			PlayerPrefs.SetInt("input_show_cursor", 1);
			transform.Find("cursor_off").gameObject.active = false;
			transform.Find("cursor_on").gameObject.active = true;
		}
		else if (button.name == "visual_on") {
			visualEffects = false;
			PlayerPrefs.SetInt("enable_visual_effects", 0);
			transform.Find("visual_on").gameObject.active = false;
			transform.Find("visual_off").gameObject.active = true;
		}
		else if (button.name == "visual_off") {
			visualEffects = true;
			PlayerPrefs.SetInt("enable_visual_effects", 1);
			transform.Find("visual_on").gameObject.active = true;
			transform.Find("visual_off").gameObject.active = false;
		}
		else if (button.name == "cp_mouse_minus") {
			mouseSensitivity = Mathf.Max(10, mouseSensitivity - 10);
			input_mouse_sensitivity.text = mouseSensitivity.ToString() + "%";
			PlayerPrefs.SetInt("input_mouse_sensitivity", mouseSensitivity);
		}
		else if (button.name == "cp_mouse_plus") {
			mouseSensitivity = Mathf.Min(100, mouseSensitivity + 10);
			input_mouse_sensitivity.text = mouseSensitivity.ToString() + "%";
			PlayerPrefs.SetInt("input_mouse_sensitivity", mouseSensitivity);
		}
		else if (button.name == "input_button") {
			input_select.active = true;
			Vector3 zDist = camera.transform.position - button.transform.position;
			Vector3 clickPosition = camera.ScreenToWorldPoint(
			    new Vector3(Input.mousePosition.x, Input.mousePosition.y, zDist.z));
			TextMesh clickedInput = ClosestInput(clickPosition);
			if (selectedInput == null || selectedInput != clickedInput) {
				selectedInput = clickedInput;
				input_select.transform.position = selectedInput.transform.position;
			}
			else {
				selectedInput.text = "Mouse0";
				PlayerPrefs.SetString(selectedInput.name, selectedInput.text);
				ValidateInput(selectedInput);
				selectedInput = null;
				audio.PlayOneShot(clickClip);
				input_select.active = false;
				Setup();
			}
		}

    }

    void OnGUI() {
		if (selectedInput != null) {
			Event e = Event.current;
			if ((e.isKey && e.keyCode != KeyCode.Escape) || (e.isMouse && e.button != 0)) {
				if (e.isKey) {
					selectedInput.text = e.keyCode.ToString();
				}
				else {
					selectedInput.text = "Mouse" + e.button.ToString();
				}
				PlayerPrefs.SetString(selectedInput.name, selectedInput.text);
				ValidateInput(selectedInput);
				selectedInput = null;
				audio.PlayOneShot(clickClip);
				input_select.active = false;
				Setup();
			}
		}
    }

	void ValidateInput (TextMesh text) {
		foreach (TextMesh input in inputs) {
			if (text != input && text.text == input.text) {
				input.text = "None";
				PlayerPrefs.SetString(input.name, input.text);
			}
		}
	}

	TextMesh ClosestInput (Vector3 to) {
		float closestDist = 10000;
		TextMesh closest = null;
		foreach (TextMesh text in inputs) {
			float dist = Vector3.Distance(text.transform.position, to);
			if (dist < closestDist) {
				closestDist = dist;
				closest = text;
			}
		}
		return closest;
	}

	IEnumerator LoadTitleScreen () {
		yield return StartCoroutine(WaitForRealSeconds(1));
		Time.timeScale = 1;
		Application.LoadLevel("StoryLoader");
	}

	IEnumerator ToggleSettings () {
		float closedDist = Mathf.Abs(transform.localPosition.x - closedX);
		float openedDist = Mathf.Abs(transform.localPosition.x - openedX);
		float targetX = closedDist > openedDist ? closedX : openedX;
		float targetDist = Mathf.Abs(transform.localPosition.x - targetX);
		float targetDir = targetX < transform.localPosition.x ? -1 : 1;
		Vector3 snapPosition = new Vector3(targetX, transform.localPosition.y, transform.localPosition.z);
		while (targetDist > 0) {
			float movement = 10 * Game.realDeltaTime;
			transform.localPosition += new Vector3(movement * targetDir, 0, 0);
			targetDist -= movement;
			yield return 0;
		}
		transform.localPosition = snapPosition;
	}

	public static void ResetDefaults () {

		PlayerPrefs.SetInt("input_defaults", 0);
		PlayerPrefs.SetString("input_move_right", "D");
		PlayerPrefs.SetString("input_move_left", "A");
		PlayerPrefs.SetString("input_look_up", "W");
		PlayerPrefs.SetString("input_look_down", "S");
		PlayerPrefs.SetString("input_zoom_in", "UpArrow");
		PlayerPrefs.SetString("input_zoom_out", "DownArrow");
		PlayerPrefs.SetString("input_fire", "Mouse0");
		PlayerPrefs.SetString("input_jump", "Mouse1");
		PlayerPrefs.SetInt("input_mouse_sensitivity", 50);
		PlayerPrefs.SetInt("input_show_cursor", 0);
		PlayerPrefs.SetInt("enable_visual_effects", 1);
	}

	public static void Setup () {

		if (!PlayerPrefs.HasKey("input_defaults")) {
			ResetDefaults();
		}
		
		mouseSensitivity = PlayerPrefs.GetInt("input_mouse_sensitivity");
		showCursor = PlayerPrefs.GetInt("input_show_cursor") == 0 ? false : true;
		visualEffects = PlayerPrefs.GetInt("enable_visual_effects") == 0 ? false : true;

		InputProxy input = InputProxy.Get();
		input.Remap("Horizontal", "positive", PlayerPrefs.GetString("input_move_right"));
		input.Remap("Horizontal", "negative", PlayerPrefs.GetString("input_move_left"));
		input.Remap("Vertical", "positive", PlayerPrefs.GetString("input_look_up"));
		input.Remap("Vertical", "negative", PlayerPrefs.GetString("input_look_down"));
		input.Remap("Zoom", "positive", PlayerPrefs.GetString("input_zoom_in"));
		input.Remap("Zoom", "negative", PlayerPrefs.GetString("input_zoom_out"));
		input.Remap("Jump", "positive", PlayerPrefs.GetString("input_jump"));
		input.Remap("Fire", "positive", PlayerPrefs.GetString("input_fire"));
	}

}
