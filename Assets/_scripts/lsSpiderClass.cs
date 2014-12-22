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
	public GameObject cameraUI;

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
		if (!(transform.position.x <= -20.3F)) cameraUI.transform.position = new Vector3(transform.position.x, cameraUI.transform.position.y, cameraUI.transform.position.z);
		else cameraUI.transform.position = new Vector3(-20.3F, cameraUI.transform.position.y, cameraUI.transform.position.z);
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

		// to default
		levelMenu.transform.GetChild(0).GetComponent<UIToggle>().value = true;
		time.SetActive(false);
		web.SetActive(false);
		sluggish.SetActive(false);
		destroyer.SetActive(false);
		yeti.SetActive(false);
		groot.SetActive(false);
		for (int i = 1; i <= 3; i++) {
			stars.transform.GetChild(i - 1).GetComponent<UISprite>().color = new Color(0, 0, 0, 1);
		}
		//

		titleNumberLevel.text = initClass.progress["currentLevel"].ToString();
		stars.transform.localPosition = new Vector3(stars.transform.localPosition.x, 117, stars.transform.localPosition.z);

		int levelDemandsStars = staticClass.levels[initClass.progress["currentLevel"], 0];
		for (int i = 1; i <= levelDemandsStars; i++) {
			stars.transform.GetChild(i - 1).GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
		}
		stars.transform.GetChild(3).GetComponent<UILabel>().text = levelDemandsStars.ToString();

		int levelDemands = staticClass.levels[initClass.progress["currentLevel"], 1];
		if (levelDemands == 0) {
			stars.transform.localPosition = new Vector3(stars.transform.localPosition.x, -30, stars.transform.localPosition.z);
		} else if (levelDemands >= 1 && levelDemands <=99){
			time.SetActive(true);
			if (levelDemands < 10) time.transform.GetChild(0).GetComponent<UILabel>().text = "0" + levelDemands.ToString();
			else time.transform.GetChild(0).GetComponent<UILabel>().text = levelDemands.ToString();
		}	else if (levelDemands >= 100 && levelDemands <=199){
			web.SetActive(true);
			web.transform.GetChild(0).GetComponent<UILabel>().text = (levelDemands - 100).ToString();

		}
	}


}
