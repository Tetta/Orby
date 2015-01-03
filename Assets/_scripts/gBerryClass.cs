using UnityEngine;
using System.Collections;
using System;
public class gBerryClass : MonoBehaviour {

	public static string berryState; 
	public static int starsCounter;

	private GameObject completeMenu;
	private UILabel guiTimer;
	private GameObject pauseMenu;
	private GameObject[] guiStars = new GameObject[3];
	private GameObject spider;
	// Use this for initialization
	void Start () {
		staticClass.useWeb = 0;
		staticClass.timer = 0;
		staticClass.useSluggish = false;
		staticClass.useDestroyer = false;
		staticClass.useYeti = false;
		staticClass.useGroot = false;

		starsCounter = 0;
		berryState = "";
		GameObject gui = GameObject.Find("gui");
		completeMenu = gui.transform.Find("complete menu").gameObject;
		pauseMenu = gui.transform.Find("pause menu").gameObject;
		guiStars[0] = GameObject.Find("gui star 1");
		guiStars[1] = GameObject.Find("gui star 2");
		guiStars[2] = GameObject.Find("gui star 3");
		spider = GameObject.Find("spider");
		//timer
		if (initLevelMenuClass.levelDemands == 1) {
			int levels = staticClass.levels[Convert.ToInt32(Application.loadedLevelName.Substring(5)), 1];
			if (levels >= 1 && levels <=99) {
				GameObject.Find("gui timer").GetComponent<UILabel>().enabled = true;
				guiTimer = GameObject.Find("gui timer 2").GetComponent<UILabel>();
				guiTimer.enabled = true;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x < -4 || transform.position.x > 4 || transform.position.y < -6 || transform.position.y > 6) Application.LoadLevel(Application.loadedLevelName);
		if (spider.transform.position.x < -4 || spider.transform.position.x > 4 || spider.transform.position.y < -6 || spider.transform.position.y > 6) Application.LoadLevel(Application.loadedLevelName);
		//timer
		if (initLevelMenuClass.levelDemands == 1) {
			int levels = staticClass.levels[Convert.ToInt32(Application.loadedLevelName.Substring(5)), 1];
			if (levels >= 1 && levels <=99) 
			if (Mathf.Ceil(Time.timeSinceLevelLoad) > staticClass.timer) {
				staticClass.timer = Convert.ToInt32(Mathf.Ceil(Time.timeSinceLevelLoad));
				if(levels - staticClass.timer <= 0)	guiTimer.text = "00";
				else if (levels - staticClass.timer < 10) guiTimer.text = "0" + (levels - staticClass.timer).ToString();
				else guiTimer.text = (levels - staticClass.timer).ToString();
			}
		}

		if (Input.GetKey(KeyCode.Escape)) {
			pauseMenu.SetActive(true);
		}
	}

	//void OnTriggerEnter2D(Collider2D collisionObject) {
	void OnCollisionEnter2D (Collision2D collisionObject) {
		if (collisionObject.gameObject.name == "spider") {
			//tutorial
			gHandClass.delHand();

			if (initClass.progress.Count == 0) initClass.getProgress();
			rigidbody2D.isKinematic = true;
			collisionObject.gameObject.rigidbody2D.isKinematic = true;
			//collisionObject.collider.isTrigger = true;
			transform.position = collisionObject.gameObject.transform.position;
			completeMenu.SetActive(true);


			//initClass.progress["stars"] = 3;
			if (GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
				GooglePlayManager.instance.SubmitScore ("leaderboard_test_leaderboard", initClass.progress["stars"]);
				if (Application.loadedLevelName == "level1") GooglePlayManager.instance.UnlockAchievement("achievement_complete_first_level");
			}
			//medals
			if (initLevelMenuClass.levelDemands == 0) {
				for (int i = 0; i < 3 ; i++) {
					GameObject star = Instantiate(guiStars[i], new Vector3(i - 1, 0.6F, 0), Quaternion.identity) as GameObject;
					star.GetComponent<UISprite>().height = 190;
					star.GetComponent<UISprite>().width = 200;
					star.GetComponent<UISprite>().depth = 2;
					star.transform.localScale = new Vector3(0.004F, 0.004F, 0.004F);
					star.transform.parent = completeMenu.transform;
				}
				if (starsCounter == 3 && initClass.progress[Application.loadedLevelName] != 1 && initClass.progress[Application.loadedLevelName] != 3) {
					initClass.progress["medals"] ++;
					if (initClass.progress[Application.loadedLevelName] == 0) initClass.progress[Application.loadedLevelName] = 1;
					else initClass.progress[Application.loadedLevelName] = 3;

				}
			} 			
			int lvlNumber = Convert.ToInt32(Application.loadedLevelName.Substring(5));
			if (initLevelMenuClass.levelDemands == 1 && initClass.progress[Application.loadedLevelName] < 2 && staticClass.levels[lvlNumber, 0] == starsCounter) {
				int levels = staticClass.levels[lvlNumber, 1];
				bool flag = false;
				if (levels == 0) flag = true;
				else if (levels >= 1 && levels <=99 && staticClass.timer <= levels) flag = true;
				else if (levels >= 100 && levels <=199 && staticClass.useWeb == levels) flag = true;
				else if (levels == 201 && staticClass.useSluggish == false) flag = true;
				else if (levels == 202 && staticClass.useDestroyer == false) flag = true;
				else if (levels == 203 && staticClass.useYeti == false) flag = true;
				else if (levels == 204 && staticClass.useGroot == false) flag = true;
				if (flag) {
					initClass.progress["medals"] ++;
					if (initClass.progress[Application.loadedLevelName] == 0) initClass.progress[Application.loadedLevelName] = 2;
					else initClass.progress[Application.loadedLevelName] = 3;
				}
			}
			if (lvlNumber >= initClass.progress["lastLevel"]) initClass.progress["lastLevel"] = lvlNumber;

			initClass.saveProgress();
		}
	}

	void OnTriggerEnter2D(Collider2D collisionObject) {
		if (collisionObject.gameObject.name == "star") {
			Destroy(collisionObject.gameObject);
			guiStars[starsCounter].GetComponent<UISprite>().color =  new Color(1, 1, 1, 1);
			starsCounter ++;
		}

	}


}
