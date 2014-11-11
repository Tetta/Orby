using UnityEngine;
using System.Collections;

public class iGooglePlayClass : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		NGUIDebug.Log("mGooglePlayClass OnClick");
		GooglePlayManager.instance.showAchievementsUI ();
	}
}
