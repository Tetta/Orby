using UnityEngine;
using System.Collections;

public class lsSpiderClass : MonoBehaviour {

	private string spiderState = "";
	private NavMeshAgent spiderAgent;
	private GameObject spider2;

	// Use this for initialization
	void Start () {
		spiderAgent = GetComponent<NavMeshAgent>();
		spider2 = transform.GetChild(0).gameObject;
		if (initClass.progress.Count == 0) initClass.getProgress();

		spiderAgent.enabled = false;
		transform.position = GameObject.Find("level " + initClass.progress["currentLevel"]).transform.position;
		spiderAgent.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape)) Application.LoadLevel("menu");

		if (spiderAgent.desiredVelocity != new Vector3 (0, 0, 0) && spiderState == "") {
			spiderState = "move";
			GetComponent<MeshRenderer>().enabled = false;
			spider2.SetActive(true);
			animation.Play();
			animation["1"].speed = 10;
		} 
		
		if (spiderAgent.desiredVelocity == new Vector3 (0, 0, 0) && spiderState == "move") {
			GetComponent<MeshRenderer>().enabled = true;
			spider2.SetActive(false);
			animation.Stop();
			transform.rotation = new Quaternion(0, 180, 0, 1);
			
			spiderState = "";

			Application.LoadLevel("level" + initClass.progress["currentLevel"]);
		} 

	}


}
