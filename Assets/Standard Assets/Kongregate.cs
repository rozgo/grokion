using UnityEngine;
using System.Collections;

public class Kongregate : MonoBehaviour {

	public bool isKongregate = false;
	public int userId = 0;
	public string username = "Guest";
	public string gameAuthToken = "";
	
	void Awake () {
		DontDestroyOnLoad(gameObject);
	}

	void Start () {
		StartCoroutine(SetupKongregate());
	}
	
	void OnKongregateAPILoaded (string userInfoString) {
		isKongregate = true;
		string[] parts = userInfoString.Split('|');
		userId = System.Convert.ToInt32(parts[0]);
		username = parts[1];
		gameAuthToken = parts[2];

		Application.ExternalEval(
            "kongregate.services.addEventListener('login', function(){" +
            "   var services = kongregate.services;" +
            "   var params=[services.getUserId(), services.getUsername(), services.getGameAuthToken()].join('|');" +
            "   kongregateUnitySupport.getUnityObject().SendMessage('Kongregate', 'OnKongregateUserSignedIn', params);" + 
            "});");
	}

	void OnKongregateUserSignedIn (string userInfoString) {
		string[] parts = userInfoString.Split('|');
		userId = System.Convert.ToInt32(parts[0]);
		username = parts[1];
		gameAuthToken = parts[2];
	}

	IEnumerator SetupKongregate () {

		Application.ExternalEval("if(typeof(kongregateUnitySupport) != 'undefined') {"+
								 "kongregateUnitySupport.initAPI('Kongregate', 'OnKongregateAPILoaded');};");

		yield return new WaitForSeconds(1);
	}

	void RemoveWarnings () {
		if (isKongregate && userId != 0 && username.Length > 0 && gameAuthToken.Length > 0) {
			return;
		}
	}
}
