using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class initClass : MonoBehaviour {

	//private GameObject testLabel;
	public GameObject googlePlus;
	public GameObject googlePlay;
	public GameObject closeMenu;
	public float percentageLoaded = 0;

	static public Dictionary<string, int> progress = new Dictionary<string, int>();
	//static public string mainMenuState = "start";

	private int i;

	// Use this for initialization
	void Start () { 
		if (progress.Count == 0) getProgress();
		if (progress["googlePlay"] == 1) GooglePlayConnection.instance.connect ();

		//listen for GooglePlayConnection events
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
	}
	
	// Update is called once per frame
	void Update () {
		//Application.RegisterLogCallback(handleLog);
		if (Input.GetKey(KeyCode.Escape)) {
			closeMenu.SetActive(true);
		}
	}

	private void OnPlayerConnected() {
		NGUIDebug.Log("OnPlayerConnected");
		googlePlay.SetActive(true);
		initClass.progress["googlePlay"] = 1;
		initClass.saveProgress();
		//googlePlus.SetActive(false);
	}

	private void OnPlayerDisconnected() {
		NGUIDebug.Log("OnPlayerDisconnected");
		GooglePlayConnection.instance.disconnect ();
		googlePlay.SetActive(false);
		googlePlus.SetActive(true);
		initClass.progress["googlePlay"] = 0;
		initClass.saveProgress();
	}

	static public void saveProgress() {
		string strProgress = "";
		foreach (var item in progress ) {
			strProgress += item.Key + "=" + item.Value + ";";
		}
		PlayerPrefs.SetString("progress", strProgress);
		PlayerPrefs.Save();
	}
	
	static public void getProgress() {
		string strProgressDefault = "googlePlay=0;lastLevel=0;currentLevel=1;gold=10;stars=0;" +
			"level1=0;level2=0;level3=0;level4=0;level5=0;level6=0;level7=0;level8=0;level9=0;level10=0;" +
				"level11=0;level12=0;level13=0;level14=0;level15=0;level16=0;level17=0;level18=0;level19=0;level20=0;" +
				"level21=0;level22=0;level23=0;level24=0;level25=0;";
		//сброс прогресса
		//PlayerPrefs.SetString("progress", strProgressDefault);
		string strProgress = PlayerPrefs.GetString("progress");
		//NGUIDebug.Log(strProgress);
		if (strProgress == "") strProgress = strProgressDefault;
		string strKey = "", strValue = "";
		bool flag = true;
		for (int i = 0; i < strProgress.Length; i++) {
			if (strProgress.Substring(i, 1) == "=") flag = false;
			
			else if (strProgress.Substring(i, 1) == ";") {
				flag = true;
				progress[strKey] = int.Parse(strValue);
				strKey = "";
				strValue = "";
			} else if (flag) strKey += strProgress.Substring(i, 1);
			else if (!flag) strValue += strProgress.Substring(i, 1);
			
		}
		for (int i = 0; i < strProgressDefault.Length; i++) {
			if (strProgressDefault.Substring(i, 1) == "=") flag = false;
			
			else if (strProgressDefault.Substring(i, 1) == ";") {
				flag = true;
				if (!progress.ContainsKey(strKey)) progress[strKey] = 0;
				strKey = "";
			} else if (flag) strKey += strProgressDefault.Substring(i, 1);
		}
		saveProgress();
	}

	/*
	static public void updateProgress() {
		goldLabel.text = progress["gold"].ToString();
		starsLabel.text = progress["stars"].ToString();

	}
	*/

	void handleLog(string logString, string stackTrace, LogType type)
	{
		NGUIDebug.Log(type+": " + logString + "\n" + stackTrace);
	}
}