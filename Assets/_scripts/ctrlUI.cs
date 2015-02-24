using UnityEngine;
using System.Collections;

public class ctrlUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
		 if (name == "back transition") {
			animation.Play("back transition open");
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
