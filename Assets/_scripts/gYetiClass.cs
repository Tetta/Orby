using UnityEngine;
using System.Collections;

public class gYetiClass : MonoBehaviour {

	private GameObject back;
	private GameObject berry;
	private GameObject[] tumbleweeds;

	private string yetiState = "";
	private GameObject[] chains;
	// Use this for initialization
	void Start () {
		berry = GameObject.Find("berry");
		tumbleweeds = GameObject.FindGameObjectsWithTag("tumbleweed");
		back = GameObject.Find("gui").transform.Find("yeti back").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUp() {
		staticClass.useYeti = true;
		gRecHintClass.recHint(transform);
		gHintClass.checkHint(gameObject);
		if (yetiState == "") {
			yetiState = "active";
			berry.rigidbody2D.angularVelocity = 0;
			berry.rigidbody2D.velocity = new Vector2(0, 0);

			chains = GameObject.FindGameObjectsWithTag("chain");
			for (int i = 0; i < chains.Length; i++) {
				chains[i].rigidbody2D.isKinematic = true;
			}
			foreach (GameObject item in tumbleweeds) {
				item.rigidbody2D.isKinematic = true;
			}
			Time.timeScale = 0;
			back.SetActive(true);
		} else {
			yetiState = "";
			chains = GameObject.FindGameObjectsWithTag("chain");
			for (int i = 0; i < chains.Length; i++) {
				chains[i].rigidbody2D.isKinematic = false;
			}
			foreach (GameObject item in tumbleweeds) {
				item.rigidbody2D.isKinematic = false;
			}
			Time.timeScale = 1;
			back.SetActive(false);
		}
		
	}
}
