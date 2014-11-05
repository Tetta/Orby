using UnityEngine;
using System.Collections;

public class googleClass : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnClick() {
		GooglePlayConnection.instance.connect ();
	}
}
