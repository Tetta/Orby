using UnityEngine;
using System.Collections;

public class gCactusClass : MonoBehaviour {


	private GameObject restart;
	// Use this for initialization
	void Start () {
		restart = GameObject.Find("gui/restart");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collisionObject) {
		if (collisionObject.gameObject.name == "berry" || collisionObject.gameObject.name == "spider") {
			restart.SendMessage("OnClick");
		}
		
	}
}
