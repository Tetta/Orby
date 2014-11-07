using UnityEngine;
using System.Collections;
using System;
public class berryClass : MonoBehaviour {


	private GameObject completeMenu;
	private GameObject pauseMenu;
	private GameObject[] guiStars = new GameObject[3];
	private int starsCounter = 0;
	// Use this for initialization
	void Start () {
		GameObject gui = GameObject.Find("gui");
		completeMenu = gui.transform.Find("complete menu").gameObject;
		pauseMenu = gui.transform.Find("pause menu").gameObject;
		guiStars[0] = GameObject.Find("gui star 1");
		guiStars[1] = GameObject.Find("gui star 2");
		guiStars[2] = GameObject.Find("gui star 3");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape)) {
			pauseMenu.SetActive(true);
		}
	}

	void OnTriggerEnter2D(Collider2D collisionObject) {
		if (collisionObject.gameObject.name == "spider") {
			rigidbody2D.isKinematic = true;
			transform.position = collisionObject.gameObject.transform.position;
			//rigidbody2D.drag = 100;
			completeMenu.SetActive(true);

			if (initClass.progress.Count == 0) initClass.getProgress();

			//initClass.progress["stars"] = 3;
			if (starsCounter > initClass.progress[Application.loadedLevelName]) {
				initClass.progress["stars"] += starsCounter - Convert.ToInt32(initClass.progress[Application.loadedLevelName]);
				initClass.progress[Application.loadedLevelName] = starsCounter;
			}
			if (Convert.ToInt32(Application.loadedLevelName.Substring(5)) >= initClass.progress["currentLevel"]) initClass.progress["currentLevel"] = Convert.ToInt32(Application.loadedLevelName.Substring(5)) + 1;

			initClass.saveProgress();
		}
		if (collisionObject.gameObject.name == "star") {
			Destroy(collisionObject.gameObject);
			guiStars[starsCounter].GetComponent<UISprite>().color =  new Color(1, 1, 1, 1);
			starsCounter ++;
		}

	}



}
