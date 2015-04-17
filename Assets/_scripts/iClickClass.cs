using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class iClickClass : MonoBehaviour {
	public static GameObject backTransition = null;

	public string functionStart = "";
	public string functionClick = "";
	public string functionPress = "";
	public string functionDragStart = "";
	public string functionDrag = "";
	public static GameObject currentButton = null;

	private float timeSinceTouch = 0;
	private bool enableDrag = false;
	//public string functionDestroy = "";

	// Use this for initialization
	void Start () {
		if (functionStart != "") SendMessage(functionStart);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick() {
		if (functionClick != "") SendMessage(functionClick);
	}

	void OnPress(bool isPressed) {
		if (functionPress != "") SendMessage(functionPress, isPressed);
	}

	void OnDragStart () {
		if (functionDragStart != "") SendMessage(functionDragStart);
	}
	void OnDrag () {
		if (functionDrag != "") SendMessage(functionDrag);
	}


	void pressMarketItem(bool isPressed) {
		transform.parent.GetComponent<UIScrollView>().Press(isPressed);
		if (!isPressed) {
			GetComponent<UIPlayAnimation>().enabled = true;
			GetComponent<UIButtonScale>().enabled = true;
			enableDrag = false;
		}
		timeSinceTouch = Time.time;
	}
	
	void dragStartMarketItem() {
		if (Time.time - timeSinceTouch < 0.5F) {
			GetComponent<UIButtonScale>().enabled = false;
			GetComponent<UIPlayAnimation>().enabled = false;
			enableDrag = true;
		}
	}

	void dragMarketItem() {
		if (enableDrag) transform.parent.GetComponent<UIScrollView>().Drag();
	}

	void OnDestroy() {
	}



	public void backTransitionOpen ( ) {
		//animation.Play("back transition open");
	}

	public void backTransitionExit ( ) {
		GetComponent<Animation>().Play("back transition exit");
		if (ActiveAnimation.current != null) {
			currentButton = ActiveAnimation.current.gameObject;
			//if (name == "market") marketMenu.SetActive(true);
		}
	}

	public void backTransitionExitFinished () {
		Debug.Log("currentButton: " + currentButton);
		if (currentButton != null) {
			if (currentButton.name == "button market") {
				marketClass.instance.gameObject.SetActive(true);
				marketClass.instance.marketMainMenu.SetActive(true);
				GetComponent<Animation>().Play("back transition open");
			} else if  (currentButton.name == "button settings") {
				GameObject.Find("settings folder").transform.GetChild(0).gameObject.SetActive(true);
				GetComponent<Animation>().Play("back transition open");
			} else if (currentButton.name == "button settings back") {
				GameObject.Find("settings").SetActive(false);
				GetComponent<Animation>().Play("back transition open");
			} else if (currentButton.name == "button market back") {
				if (marketClass.instance.coinsMenu.activeSelf) {
					marketClass.instance.coinsMenu.SetActive(false);
					marketClass.instance.marketMainMenu.SetActive(true);

				} else if (marketClass.instance.hintsMenu.activeSelf) {
					marketClass.instance.hintsMenu.SetActive(false);
					marketClass.instance.marketMainMenu.SetActive(true);
					
				} else if (marketClass.instance.customizationMenu.activeSelf) {
					marketClass.instance.customizationMenu.SetActive(false);
					marketClass.instance.marketMainMenu.SetActive(true);
					
				} else marketClass.instance.gameObject.SetActive(false);
				GetComponent<Animation>().Play("back transition open");
			} else if (currentButton.name == "special" || currentButton.name == "coins" || currentButton.name == "hints" || currentButton.name == "customization") {
				marketClass.instance.marketMainMenu.SetActive(false);
				marketClass.instance.specialMenu.SetActive(false);
				marketClass.instance.hintsMenu.SetActive(false);
				marketClass.instance.notCoinsMenu.SetActive(false);
				marketClass.instance.coinsMenu.SetActive(false);
				marketClass.instance.customizationMenu.SetActive(false);
				GetComponent<Animation>().Play("back transition open");
				if (currentButton.name == "special") {
					marketClass.instance.specialMenu.SetActive(true);
					//отмечаем, если комплект куплен
					if (initClass.progress["complect"] == 1) {
						//убираем label price и label currency [0] и [2]
						marketClass.instance.specialMenu.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
						marketClass.instance.specialMenu.transform.GetChild(0).GetChild(0).GetChild(1).gameObject.SetActive(false);
						//добавляем accept [3]
						marketClass.instance.specialMenu.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(true);
					}
					ActiveAnimation.Play(marketClass.instance.specialMenu.transform.GetChild(0).GetChild(0).GetComponent<Animation>(), AnimationOrTween.Direction.Forward);
					ActiveAnimation.Play(marketClass.instance.specialMenu.transform.GetChild(0).GetChild(1).GetComponent<Animation>(), AnimationOrTween.Direction.Forward);
				} else if (currentButton.name == "coins") {
					marketClass.instance.gameObject.SetActive(true);
					Debug.Log(marketClass.instance.enabled);
					Destroy( GameObject.Find("root/Camera/UI Root/not coins menu"));
					marketClass.instance.coinsMenu.SetActive(true);
					ActiveAnimation.Play(marketClass.instance.coinsMenu.transform.GetChild(0).GetChild(0).GetComponent<Animation>(), AnimationOrTween.Direction.Forward);
					ActiveAnimation.Play(marketClass.instance.coinsMenu.transform.GetChild(0).GetChild(1).GetComponent<Animation>(), AnimationOrTween.Direction.Forward);
				} else if (currentButton.name == "hints") {
					Debug.Log(555);
					marketClass.instance.gameObject.SetActive(true);
					marketClass.instance.hintsMenu.SetActive(true);
					ActiveAnimation.Play(marketClass.instance.hintsMenu.transform.GetChild(0).GetChild(0).GetComponent<Animation>(), AnimationOrTween.Direction.Forward);
					ActiveAnimation.Play(marketClass.instance.hintsMenu.transform.GetChild(0).GetChild(1).GetComponent<Animation>(), AnimationOrTween.Direction.Forward);
				} else if (currentButton.name == "customization") {
					marketClass.instance.customizationMenu.SetActive(true);
					for (int j = 1; j <= 2; j++) {
						//отмечаем купленные скины
						if (initClass.progress["skin" + j] > 0) {
							//убираем label price и icon price [0] и [2]
							marketClass.instance.customizationMenu.transform.GetChild(0).GetChild(j - 1).GetChild(0).gameObject.SetActive(false);
							marketClass.instance.customizationMenu.transform.GetChild(0).GetChild(j - 1).GetChild(2).gameObject.SetActive(false);
							//добавляем accept [3]
							marketClass.instance.customizationMenu.transform.GetChild(0).GetChild(j - 1).GetChild(3).gameObject.SetActive(true);
							//красим accept в белый
							if (initClass.progress["skin" + j] == 2) marketClass.instance.customizationMenu.transform.GetChild(0).GetChild(j - 1).GetChild(3).GetComponent<UISprite>().color = new Color32(255, 255, 255, 255);


						}
					}
					ActiveAnimation.Play(marketClass.instance.customizationMenu.transform.GetChild(0).GetChild(0).GetComponent<Animation>(), AnimationOrTween.Direction.Forward);
					ActiveAnimation.Play(marketClass.instance.customizationMenu.transform.GetChild(0).GetChild(1).GetComponent<Animation>(), AnimationOrTween.Direction.Forward);
				}
			}
		}
	}

	void initSettings () {
		if (name == "music button") {
			if (initClass.progress["music"] == 1) transform.GetChild(0).gameObject.SetActive(false);
			else transform.GetChild(0).gameObject.SetActive(true);
		}
		if (name == "soung button") {
			if (initClass.progress["sound"] == 1) transform.GetChild(0).gameObject.SetActive(false);
			else transform.GetChild(0).gameObject.SetActive(true);
		}
	}

	void clickSound () {
		if (initClass.progress["sound"] == 0) {
			initClass.setSound(true);
			initClass.progress["sound"] = 1;
			initClass.saveProgress();
			transform.GetChild(0).gameObject.SetActive(false);
		} else {
			initClass.setSound(false);
			initClass.progress["sound"] = 0;
			initClass.saveProgress();
			transform.GetChild(0).gameObject.SetActive(true);
		}
	}

	void clickMusic () {
		if (initClass.progress["music"] == 0) {
			GameObject.Find("music").GetComponent<AudioSource>().enabled = true;
			initClass.progress["music"] = 1;
			initClass.saveProgress();
			transform.GetChild(0).gameObject.SetActive(false);
		} else {
			GameObject.Find("music").GetComponent<AudioSource>().enabled = false;
			initClass.progress["music"] = 0;
			initClass.saveProgress();
			transform.GetChild(0).gameObject.SetActive(true);
		}
	}



	void showAchievements () {
		//NGUIDebug.Log("mGooglePlayClass OnClick");
		GooglePlayManager.instance.ShowAchievementsUI();
	}

	void showLeaderboards () {
		GooglePlayManager.instance.ShowLeaderBoardsUI();
	}

	void connectGooglePlay () {
		try {
			if (initClass.progress["googlePlay"] == 0) {
				GooglePlayConnection.instance.connect ();
			} else {
				GooglePlayConnection.instance.disconnect ();
			}

		} catch (System.Exception ex) {
			Debug.Log(ex.StackTrace);
		}
	}

	void resetProgress () {
		string strProgressDefault = "googlePlay=0;lastLevel=0;currentLevel=1;coins=1000;gems=0;energyTime=0;energy=20;" +
				"hints=3;webs=3;grabs=3;teleports=3;complect=0;music=1;sound=1;dailyBonus=0;" +
				"skin1=2;skin2=0;skin3=0;skin4=0;skin5=0;" +
				"level1=0;level2=0;level3=0;level4=0;level5=0;level6=0;level7=0;level8=0;level9=0;level10=0;" +
				"level11=0;level12=0;level13=0;level14=0;level15=0;level16=0;level17=0;level18=0;level19=0;level20=0;" +
				"level21=0;level22=0;level23=0;level24=0;level25=0;level26=0;level50=0;level51=0;level75=0;level76=0;";
		//сброс прогресса
		PlayerPrefs.SetString("progress", strProgressDefault);
		initClass.getProgress();
		GooglePlayManager.instance.ResetAllAchievements();
		GooglePlayManager.instance.SubmitScore("leaderboard_test_leaderboard", 0);
	}

	void loadLevel () {
		StartCoroutine(CoroutineLoadLevel());
	}


	void openMenu () {
		GameObject menu = null;
		if (name == "button market") {
			//marketMenu.SetActive(true);
		}else if (name == "next level menu") {
			menu = GameObject.Find("level menu");
			menu.SetActive(false);
			menu = transform.parent.parent.GetChild(3).gameObject;
			menu.SetActive(true);
		} else if (name == "prev level menu") {
			menu = GameObject.Find("level menu 2");
			menu.SetActive(false);
			menu = transform.parent.parent.GetChild(2).gameObject;
			menu.SetActive(true);
		} else if (name == "pause") {
			menu = transform.parent.GetChild(1).gameObject;
			menu.SetActive(true);
			Time.timeScale = 0;
		} else if (name == "play") {
			menu = GameObject.Find("pause menu");
			menu.SetActive(false);
			Time.timeScale = 1;
			
		} else if (name == "exit energy menu") GameObject.Find("energyLabel").SendMessage("stopCoroutineEnergyMenu");
		
		
	}
	public void closeMenu () {
		StartCoroutine(coroutineCloseMenu ());
	}

	public IEnumerator coroutineCloseMenu () {
		yield return new WaitForSeconds(0F);
		Debug.Log(name);
		GameObject menu = null;
		if (name == "exit level menu") {
			menu = GameObject.Find("level menu");
			menu.transform.GetChild(0).GetComponent<Animation>().Play("menu exit");
			yield return new WaitForSeconds(0.2F);
			menu.SetActive(false);
		}else if (name == "next level menu") {
			menu = GameObject.Find("level menu");
			menu.SetActive(false);
			menu = transform.parent.parent.GetChild(3).gameObject;
			menu.SetActive(true);
		} else if (name == "prev level menu") {
			menu = GameObject.Find("level menu 2");
			menu.SetActive(false);
			menu = transform.parent.parent.GetChild(2).gameObject;
			menu.SetActive(true);
		} else if (name == "pause") {
			menu = transform.parent.GetChild(1).gameObject;
			menu.SetActive(true);
			Time.timeScale = 0;
		} else if (name == "play") {
			menu = GameObject.Find("pause menu");
			menu.SetActive(false);
			Time.timeScale = 1;

		} else if (name == "exit energy menu") {
			GameObject.Find("energy menu").transform.GetChild(0).GetComponent<Animation>().Play("menu exit");
			yield return new WaitForSeconds(0.2F);
			GameObject.Find("energy").SendMessage("stopCoroutineEnergyMenu");
		
		} else if (name == "exit not coins menu") {
			menu = GameObject.Find("root/Camera/UI Root/not coins menu");
			if (menu != null) {
				menu.transform.GetChild(0).GetComponent<Animation>().Play("menu exit");
				yield return new WaitForSeconds(0.2F);
				Destroy(menu);
			} else {
				menu = marketClass.instance.notCoinsMenu;
				menu.transform.GetChild(0).GetComponent<Animation>().Play("menu exit");
				yield return new WaitForSeconds(0.2F);
				menu.SetActive(false);
			}
		} else if (name == "exit thanks menu") {
			menu = GameObject.Find("root/Camera/UI Root/thanks menu");
			if (menu != null) {
				menu.transform.GetChild(0).GetComponent<Animation>().Play("menu exit");
				yield return new WaitForSeconds(0.2F);
				Destroy(menu);
			} else {
				menu = marketClass.instance.thanksMenu;
				menu.transform.GetChild(0).GetComponent<Animation>().Play("menu exit");
				yield return new WaitForSeconds(0.2F);
				menu.SetActive(false);
			}

		}	

	}

	//public IEnumerator CoroutineCloseMenu(){
	
	//}

	void selectLanguage() {
		Localization.language = name;
	}

	public IEnumerator CoroutineLoadLevel(){
		if (name == "play 0") {
			yield return new WaitForSeconds(0.2F);
		} else if (name == "play 1") {
			yield return new WaitForSeconds(0.2F);
		}

		GameObject.Find("back transition").GetComponent<Animation>().Play("back transition exit");
		yield return new WaitForSeconds(0.1F);
		if (name == "start level menu") Application.LoadLevel("level menu");
		else if (name == "button back") Application.LoadLevel("menu");
		if (name == "restart") Application.LoadLevel(Application.loadedLevel);
		else if (name == "next") {
			Application.LoadLevel("level menu");
		} else if (name == "play 0") {
			initLevelMenuClass.levelDemands = 0;
			Application.LoadLevel("level" + initClass.progress["currentLevel"]);
		} else if (name == "play 1") {
			initLevelMenuClass.levelDemands = 1;
			Application.LoadLevel("level" + initClass.progress["currentLevel"]);
		} else if (name.Substring(0, 5) == "level") {
			if (initClass.progress["lastLevel"] >= Convert.ToInt32(name.Substring(5)) - 1) Application.LoadLevel("level" + Convert.ToInt32(name.Substring(5)));
		}

	}

	void buyEnergy () {
		marketClass.instance.item = gameObject;
		marketClass.instance.purchaseForCoins();
	}

	void clickBonusesArrow () {
		if (name == "arrow right") {
			gameObject.SetActive(false);
			GameObject.Find("bonuses/tween").transform.GetChild(1).gameObject.SetActive(true);
		} else {
			gameObject.SetActive(false);
			GameObject.Find("bonuses/tween").transform.GetChild(0).gameObject.SetActive(true);
		}

	}


}
