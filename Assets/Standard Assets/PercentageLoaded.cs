using UnityEngine;
using System.Collections;

public class PercentageLoaded : MonoBehaviour {

	public TextMesh textOutput;
    public Loader loader;

    float percentageLoaded;
    int percentage;

	void Update () {
        percentageLoaded = Application.GetStreamProgressForLevel(loader.levelToLoad) * 100;
        percentage = (int)percentageLoaded;
        textOutput.text = percentage.ToString();
        textOutput.text += "%";

        if (Application.GetStreamProgressForLevel(loader.levelToLoad) == 1) {
            textOutput.text = "";
        }
	}
}
