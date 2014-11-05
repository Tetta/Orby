using UnityEngine;
using System.Collections;

public class yetiClass : MonoBehaviour {

	public GameObject back;
	public GameObject berry;
	private string yetiState = "";
	private GameObject[] chains;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnMouseUp() {
		if (yetiState == "") {
			yetiState = "active";
			berry.rigidbody2D.isKinematic = true;
			chains = GameObject.FindGameObjectsWithTag("chain");
			for (int i = 0; i < chains.Length; i++) {
				chains[i].rigidbody2D.isKinematic = true;
			}
			Time.timeScale = 0;
			back.SetActive(true);
			back.transform.position = new Vector2(0, 0);
			Debug.Log ( berry.rigidbody2D.inertia);
		} else {
			yetiState = "";
			berry.rigidbody2D.isKinematic = false;
			chains = GameObject.FindGameObjectsWithTag("chain");
			for (int i = 0; i < chains.Length; i++) {
				chains[i].rigidbody2D.isKinematic = false;
			}
			Time.timeScale = 1;
			back.SetActive(false);
		}
		
	}
}
