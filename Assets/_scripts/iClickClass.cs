using UnityEngine;
using System.Collections;

public class iClickClass : MonoBehaviour {

	public string functionName;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		SendMessage(functionName);
	}

	void showAchievements () {
		//NGUIDebug.Log("mGooglePlayClass OnClick");
		GooglePlayManager.instance.ShowAchievementsUI();
	}

	void showLeaderboards () {
		GooglePlayManager.instance.ShowLeaderBoardsUI();
	}

	void connectGooglePlay () {
		if (initClass.progress["googlePlay"] == 0) {
			GooglePlayConnection.instance.connect ();
		} else {
			GooglePlayConnection.instance.disconnect ();
		}
	}

	void resetProgress () {
		string strProgressDefault = "googlePlay=0;lastLevel=0;currentLevel=1;gold=10;stars=0;" +
			"level1=0;level2=0;level3=0;level4=0;level5=0;level6=0;level7=0;level8=0;level9=0;level10=0;" +
				"level11=0;level12=0;level13=0;level14=0;level15=0;level16=0;level17=0;level18=0;level19=0;level20=0;" +
				"level21=0;level22=0;level23=0;level24=0;level25=0;";
		//сброс прогресса
		PlayerPrefs.SetString("progress", strProgressDefault);
		initClass.getProgress();
		GooglePlayManager.instance.ResetAllAchievements();
		GooglePlayManager.instance.SubmitScore("leaderboard_test_leaderboard", 0);
	}

	void loadLevel () {
		if (name == "restart") Application.LoadLevel(Application.loadedLevel);
		else if (name == "next") Application.LoadLevel("level menu");
		else if (name == "play") Application.LoadLevel("level" + initClass.progress["currentLevel"]);
	}

	void closeLevelMenu () {
		GameObject levelMenu = GameObject.Find("level menu");
		levelMenu.SetActive(false);
	}

	void selectLanguage() {
		Localization.language = name;
	}
}
