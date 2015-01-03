using UnityEngine;
using System.Collections;

public class gCloudClass : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUp () {
		//tutorial
		gHandClass.delHand();

		gRecHintClass.recHint(transform);
		gHintClass.checkHint(gameObject);
		gameObject.SetActive(false);
	}
}
