using UnityEngine;
using System.Collections;

public class gIceClass : MonoBehaviour {
	private GameObject ice2;


	// Use this for initialization
	void Start () {
		ice2 = gameObject.transform.GetChild(0).gameObject;		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionEnter2D(Collision2D collisionObject) {
		if (ice2.activeSelf) StartCoroutine(breakIce(true));
		else StartCoroutine(breakIce(false));
	}
	IEnumerator breakIce(bool flag) {
		yield return new WaitForSeconds(0.3F);
		if (!flag) ice2.SetActive(true);
		else gameObject.SetActive(false);
	}
}
