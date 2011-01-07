using UnityEngine;
using System.Collections;

public class PercentageLoaded : MonoBehaviour
{

	public TextMesh textOutput;
    public GameObject loader;
    Loader loaderComp;

    float percentageLoaded;
    int percentage;


	// Update is called once per frame
    void Start()
    {
        loaderComp = (Loader)loader.GetComponent("Loader"); 
    }
	void Update () {
        percentageLoaded = Application.GetStreamProgressForLevel(loaderComp.whichLevelLoad) * 100;
        percentage = (int)percentageLoaded;
        textOutput.text = percentage.ToString();
        textOutput.text += "%";

        if (Application.GetStreamProgressForLevel(loaderComp.whichLevelLoad) == 1)
        {
            textOutput.text = "";
        }
	}
}
