using UnityEngine;
using System.Collections;

public class BarLevelLoading : MonoBehaviour {

	public string levelToLoad;

    public string[] progressLines;

    public TextMesh statusLine01;
    public TextMesh statusLine02;
    public TextMesh statusLine03;

    public GameObject endPortal;
    EndTutorial tutorialEnd;
    
    float nextLoadSet;
    int currentLine = 2;

	void Start () {
        nextLoadSet = 0.3F;
        tutorialEnd = (EndTutorial)endPortal.transform.GetComponent(typeof(EndTutorial));
	}
	
	void Update () {

        transform.localScale = new Vector3(Application.GetStreamProgressForLevel(levelToLoad), 1, 1);

        if (Application.GetStreamProgressForLevel(levelToLoad) > nextLoadSet) {
            statusLine01.text = progressLines[currentLine - 2];
            statusLine02.text = progressLines[currentLine - 1];
            statusLine03.text = progressLines[currentLine];
            nextLoadSet = nextLoadSet + 0.1f;
            currentLine++;
        }

        if (Application.GetStreamProgressForLevel(levelToLoad) == 1.0f && tutorialEnd.on == false) {
            tutorialEnd.TurnOn(true);
        }
	}
}
