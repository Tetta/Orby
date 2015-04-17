using UnityEngine;
using System.Collections;

public class initTest : MonoBehaviour {

	private int i;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		i ++;
		if (i % 100 == 0) {

		}
	}

	void OnMouseDown () {
		Debug.Log ("OnMouseDown");
		Debug.Log (Input.mousePosition.x);
	}
}
