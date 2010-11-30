using UnityEngine;
using System;
using System.Collections;


public class GoogleServer : MonoBehaviour {

    public AnimationCurve curve;
	
	IEnumerator GrokionAdd () {
		
		ArrayList reqList = new ArrayList();
		reqList.Add("Add");
		reqList.Add(1);
		reqList.Add(5);
		
		String requestJson = JSON.JsonEncode(reqList);
		System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
		WWW request = new WWW("http://grokion.appspot.com/rpc", encoding.GetBytes(requestJson));
		//WWW request = new WWW("http://localhost:8080/rpc", encoding.GetBytes(requestJson));
		
		yield return request;
		
		String resultJson = System.Text.ASCIIEncoding.ASCII.GetString(request.bytes);
		Debug.Log(resultJson);
		object result = JSON.JsonDecode(resultJson);
		Debug.Log(result.GetType());
	}
	
	void Start () {
		StartCoroutine(GrokionAdd());
	}
	
}

