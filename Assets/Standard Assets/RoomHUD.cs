using UnityEngine;
using System.Collections;

public class RoomHUD : MonoBehaviour {

	public TextMesh textMesh;

    void Start () {
        textMesh.text = Application.loadedLevelName;
    }
	
}
