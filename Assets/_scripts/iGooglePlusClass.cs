using UnityEngine;
using System.Collections;

public class iGooglePlusClass : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnClick() {
		//NGUIDebug.Log(initClass.progress["googlePlay"]);
		if (initClass.progress["googlePlay"] == 0) {
			GooglePlayConnection.instance.connect ();
		} else {
			GooglePlayConnection.instance.disconnect ();
		}
	}
}
