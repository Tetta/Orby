using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Advertisements;

public class gBerryClass : MonoBehaviour {

	public static string berryState; 
	public static int starsCounter;

	private GameObject completeMenu;
	private UILabel guiTimer;
	private GameObject pauseMenu;
	private GameObject[] guiStars = new GameObject[3];
	private UISprite[] guiStarsComplete = new UISprite[3];
	//private GameObject spider;
	private GameObject restart;
	//private GameObject spider;
	private GameObject back;
	private Vector3 dir = new Vector3(0, 0, 0);
	private float t = 0;

	// Use this for initialization
	void Start () {

		Time.timeScale = 1;
		//energy
		/* OFF FOR TESTS
		if (lsEnergyClass.checkEnergy(false) == 0) {
			lsEnergyClass.energyMenuState = "energy";
			Application.LoadLevel("level menu");
		}
		*/
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

		guiStarsComplete[0] = GameObject.Find("gui").transform.GetChild(0).GetChild(0).GetComponent<UISprite>();
		guiStarsComplete[1] = GameObject.Find("gui").transform.GetChild(0).GetChild(1).GetComponent<UISprite>();
		guiStarsComplete[2] = GameObject.Find("gui").transform.GetChild(0).GetChild(2).GetComponent<UISprite>();
		restart = GameObject.Find("restart");
	
		//spider = GameObject.Find("spider");
		//timer
		if (initLevelMenuClass.levelDemands == 1) {
			int levels = staticClass.levels[Convert.ToInt32(Application.loadedLevelName.Substring(5)), 1];
			if (levels >= 1 && levels <=99) {
				GameObject.Find("gui timer").GetComponent<UILabel>().enabled = true;
				guiTimer = GameObject.Find("gui timer 2").GetComponent<UILabel>();
				guiTimer.enabled = true;
			}
		}


		staticClass.showAd ++;

		if (staticClass.showAdColony < 2 && staticClass.showAd >= 5) {
			if (!Advertisement.isReady("defaultVideoAndPictureZone")) {
				staticClass.showAd = 4;
				Debug.Log("!isReady 1");
			} else { 
				Debug.Log("isReady 1");
				staticClass.showAdColony ++;
				staticClass.showAd = 0;
				Advertisement.Show("defaultVideoAndPictureZone");
			}
		} else if (staticClass.showAdColony >= 2 && staticClass.showAd >= 5) {
			if (Advertisement.isReady("rewardedVideoZone")) {
				Debug.Log("isReady 2");
				staticClass.showAdColony = 0;
				staticClass.showAd = 0;
				Advertisement.Show("rewardedVideoZone");
			} 
		}

		/*
		staticClass.showAd ++;
		NGUIDebug.Log(AdColony.StatusForZone("vz3da177fe9cf44ef9b9")); 
		if (staticClass.showAd >= 5 && !staticClass.loadAd) {
			staticClass.showAd = 4;
			NGUIDebug.Log(staticClass.testCounter + ": showAd >= 5 && !loadAd");
			AndroidAdMobController.instance.LoadInterstitialAd ();
		} else if (staticClass.showAd >= 5 && staticClass.loadAd) {
			NGUIDebug.Log(staticClass.testCounter + ": showAd >= 5 && loadAd");
			staticClass.showAdColony ++;
			staticClass.loadAd = false;
			staticClass.showAd = 0;
			if (staticClass.showAdColony >= 3) {
				NGUIDebug.Log(staticClass.testCounter + ": showAdColony");
				staticClass.showAdColony = 0;
				staticClass.loadAd = true;
				// Check to see if a video is available in the zone.
				AdColony.ShowVideoAd("vz3da177fe9cf44ef9b9"); 
				if(AdColony.IsVideoAvailable("vz3da177fe9cf44ef9b9")) {
					NGUIDebug.Log(staticClass.testCounter + ": Play AdColony Video");
					AdColony.ShowVideoAd("vz3da177fe9cf44ef9b9"); 
				} else {
					NGUIDebug.Log(staticClass.testCounter + ": Video Not Available");
				}
			} else AndroidAdMobController.instance.ShowInterstitialAd ();
		}
		*/

		back = GameObject.Find("back forest");

	}
	
	// Update is called once per frame
	void Update () {
		//acceleration start
		if (Time.time - t > 0.02F) {
			t = Time.time;
			dir.y = Input.acceleration.y;
			dir.x = Input.acceleration.x;
			back.rigidbody2D.AddForce((-dir - back.transform.localPosition / 100) * 5);

			back.rigidbody2D.drag = (1 - (-new Vector2(dir.x, dir.y) - 
			                             new Vector2(back.transform.localPosition.x, back.transform.localPosition.y)  / 100).magnitude) * 10;
		}
		//acceleration end

		if (transform.position.x < -4 || transform.position.x > 4 || transform.position.y < -6 || transform.position.y > 6) StartCoroutine(gSpiderClass.coroutineCry());
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

		if (Input.GetKey(KeyCode.Escape) && !completeMenu.activeSelf) {
			pauseMenu.SetActive(true);
			Time.timeScale = 0;

		}
	}

	//void OnTriggerEnter2D(Collider2D collisionObject) {
	void OnCollisionEnter2D (Collision2D collisionObject) {
		if (collisionObject.gameObject.name == "spider") {
			//tutorial
			gHandClass.delHand();
			berryState = "start finish";

			//collisionObject.transform.GetChild(0).GetComponent<Animator>().StopPlayback();
			collisionObject.transform.GetChild(0).GetComponent<Animator>().Play("spider open month");
			animation.Play();
			transform.position = collisionObject.gameObject.transform.position;
			if (initClass.progress.Count == 0) initClass.getProgress();
			rigidbody2D.isKinematic = true;
			collider2D.enabled = false;
			//collisionObject.gameObject.rigidbody2D.isKinematic = true;
			StartCoroutine(coroutineEat(collisionObject));
		}

	}

	void OnTriggerEnter2D(Collider2D collisionObject) {
		if (collisionObject.gameObject.name == "star") {
			Destroy(collisionObject.gameObject);
			guiStars[starsCounter].GetComponent<UISprite>().color =  new Color(1, 1, 1, 1);
			starsCounter ++;
		}

	}
	public IEnumerator coroutineEat(Collision2D collisionObject){
		yield return new WaitForSeconds(0.2F);
		gameObject.GetComponent<UISprite>().enabled = false;
		collisionObject.transform.GetChild(0).GetComponent<Animator>().Play("spider eat");
		StartCoroutine(Coroutine(collisionObject));

	}

	public IEnumerator Coroutine(Collision2D collisionObject){
		// остановка выполнения функции на costEnergy секунд
		yield return new WaitForSeconds(2F);
		//collisionObject.transform.GetChild(0).GetComponent<Animator>().Play("idle");
		completeMenu.SetActive(true);
		berryState = "finish";

		int lvlNumber = Convert.ToInt32(Application.loadedLevelName.Substring(5));

		//initClass.progress["stars"] = 3;
		if (GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			GooglePlayManager.instance.SubmitScore ("leaderboard_forest", initClass.progress["stars"]);
			if (Application.loadedLevelName == "level1") GooglePlayManager.instance.UnlockAchievement("achievement_complete_first_level");

			if (lvlNumber > initClass.progress["lastLevel"]) {
				GooglePlayManager.instance.IncrementAchievement("achievement_complete_5_levels", 1);
				GooglePlayManager.instance.IncrementAchievement("achievement_complete_the_game", 1);

			}


		}

		//medals
		if (initLevelMenuClass.levelDemands == 0) {
			for (int i = 0; i < starsCounter ; i++) {
				guiStarsComplete[i].color = new Color32(255, 255, 255, 255);
			}
			if (starsCounter == 3 && initClass.progress[Application.loadedLevelName] != 1 && initClass.progress[Application.loadedLevelName] != 3) {
				initClass.progress["medals"] ++;
				if (initClass.progress[Application.loadedLevelName] == 0) initClass.progress[Application.loadedLevelName] = 1;
				else initClass.progress[Application.loadedLevelName] = 3;
			}
	}
		//initClass.progress["stars"] = initClass.progress["stars"] + starsCounter - initClass.progress["level" + lvlNumber];

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

		if (initClass.progress["level" + lvlNumber] < starsCounter) {
			if (GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) GooglePlayManager.instance.IncrementAchievement("achievement_collect_all_stars", starsCounter - initClass.progress["level" + lvlNumber]);
			initClass.progress["level" + lvlNumber] = starsCounter;
		}
		
		if (lvlNumber >= initClass.progress["lastLevel"]) initClass.progress["lastLevel"] = lvlNumber;
		
		initClass.saveProgress();

	}


}
