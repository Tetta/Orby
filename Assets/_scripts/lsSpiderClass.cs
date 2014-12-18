using UnityEngine;
using System.Collections;

public class lsSpiderClass : MonoBehaviour {

	public GameObject levelMenu;
	public UILabel titleNumberLevel;
	public GameObject stars;
	public GameObject time;
	public GameObject web;
	public GameObject sluggish;
	public GameObject destroyer;
	public GameObject yeti;
	public GameObject groot;

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
			selectLevelMenu();
		} 

	}

	public void selectLevelMenu () {
		levelMenu.SetActive(true);
		titleNumberLevel.text = initClass.progress["currentLevel"].ToString();
		stars.transform.localPosition = new Vector3(stars.transform.localPosition.x, 117, stars.transform.localPosition.z);
		int levelDemands = staticClass.levels[initClass.progress["currentLevel"], 1];

		time.SetActive(false);
		web.SetActive(false);
		sluggish.SetActive(false);
		destroyer.SetActive(false);
		yeti.SetActive(false);
		groot.SetActive(false);

		if (levelDemands == 0) {
			stars.transform.localPosition = new Vector3(stars.transform.localPosition.x, -30, stars.transform.localPosition.z);
		} else if (levelDemands >= 1 && levelDemands <=99){
			time.SetActive(true);

		}
	}


}
