using UnityEngine;
using System.Collections;

public class berryClass : MonoBehaviour {


	public GameObject completeLevelWindow;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter2D(Collider2D collisionObject) {
		if (collisionObject.gameObject.name == "finish point") {
			rigidbody2D.isKinematic = true;
			transform.position = collisionObject.gameObject.transform.position;
			//rigidbody2D.drag = 100;
			completeLevelWindow.SetActive(true);
		}
		if (collisionObject.gameObject.name == "star") {
			Destroy(collisionObject.gameObject);
		}

	}



}
