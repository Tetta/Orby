using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ctrlDailyBonusClass : MonoBehaviour {


	public GameObject dailyBonusMenu;

	public static bool dailyBonusGiven = false;
	public static int realTime = 0;

	private string url = "https://time.yandex.ru/sync.json?geo=10393";

	//будет дейли бонус или нет
	IEnumerator Start() {
		if (name == "daily bonus") { 
			WWW www = new WWW(url);
			yield return www;
			try {
				Debug.Log( www.text.Substring(8, 10));

				DateTime now = new DateTime(1970, 1, 1, 0, 0, 0, 0);
				now = now.AddSeconds(System.Convert.ToInt64(www.text.Substring(8, 10)));
				realTime = int.Parse(www.text.Substring(8, 10));
				if (initClass.progress.Count == 0) initClass.getProgress();
				DateTime dailyBonus = new DateTime(1970, 1, 1, 0, 0, 0, 0);
				dailyBonus = dailyBonus.AddSeconds(System.Convert.ToInt64(initClass.progress["dailyBonus"]));
				if (now.ToShortDateString() != dailyBonus.ToShortDateString()) {
					//показать окно daily bonus
					dailyBonusMenu.SetActive(true);
				}

			} catch (System.Exception ex) {
				Debug.Log( ex.Message);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void clickDailyBonus () {
		GameObject bonusesTemp = GameObject.Find("bonuses temp");
		Dictionary<string, int> portions = new Dictionary<string, int>();
		portions["hints_1"] = 30;		portions["hints_2"] = 15;		portions["hints_3"] = 10;
		portions["webs_1"] = 150;		portions["webs_2"] = 75;			portions["webs_3"] = 50;
		portions["teleports_1"] = 150;	portions["teleports_2"] = 75;	portions["teleports_3"] = 50;
		portions["bubbles_1"] = 150;		portions["bubbles_2"] = 75;		portions["bubbles_3"] = 50;
		portions["coins_50"] = 150;		portions["coins_100"] = 75;		portions["coins_250"] = 30;
		int counter = 0;
		int bonusRand = Mathf.CeilToInt( UnityEngine.Random.Range(0, 1135));
		if (dailyBonusGiven) {
			portions["hints_1"] = 2;		portions["hints_2"] = 2;		portions["hints_3"] = 1;
			portions["webs_1"] = 2;			portions["webs_2"] = 2;			portions["webs_3"] = 1;
			portions["teleports_1"] = 2;	portions["teleports_2"] = 2;	portions["teleports_3"] = 1;
			portions["bubbles_1"] = 2;		portions["bubbles_2"] = 2;		portions["bubbles_3"] = 1;
			portions["coins_50"] = 2;		portions["coins_100"] = 2;		portions["coins_250"] = 1;
			bonusRand = Mathf.CeilToInt( UnityEngine.Random.Range(0, 25));
		}
		foreach (var portion in portions ) {
			if (bonusRand >= counter && bonusRand < counter + portion.Value) {
				string bonusName = portion.Key.Split(new Char[] {'_'})[0];

				int bonusCount = int.Parse(portion.Key.Split(new Char[] {'_'})[1]);
				GameObject bonusIcon = GameObject.Instantiate (GameObject.Find("bonuses/" + bonusName), transform.position, Quaternion.identity)  as GameObject;
				bonusIcon.transform.parent = bonusesTemp.transform;
				bonusIcon.transform.localScale = new Vector3(1, 1, 1);
				GameObject bonusBackAmount = GameObject.Instantiate (GameObject.Find("bonuses/back amount"), transform.position, Quaternion.identity)  as GameObject;
				bonusBackAmount.transform.parent = bonusesTemp.transform;
				bonusBackAmount.transform.localScale = new Vector3(1, 1, 1);
				bonusBackAmount.transform.localPosition = new Vector3(transform.localPosition.x + 90, transform.localPosition.y - 90, 1);
				GameObject bonusLabelAmount = GameObject.Instantiate (GameObject.Find("bonuses/label amount"),  transform.position, Quaternion.identity)  as GameObject;
				bonusLabelAmount.transform.parent = bonusesTemp.transform;
				bonusLabelAmount.transform.localScale = new Vector3(1, 1, 1);
				bonusLabelAmount.transform.localPosition = new Vector3(transform.localPosition.x + 90, transform.localPosition.y - 90, 1);
				bonusLabelAmount.GetComponent<UILabel>().text = bonusCount.ToString();
				if (!dailyBonusGiven) {
					GameObject bonusShine = GameObject.Instantiate (GameObject.Find("bonuses/shine"), transform.position, Quaternion.identity)  as GameObject;
					bonusShine.transform.parent = bonusesTemp.transform;
					bonusShine.transform.localScale = new Vector3(1, 1, 1);
					initClass.progress[bonusName] += bonusCount;
					initClass.progress["dailyBonus"] = realTime;
					initClass.saveProgress();
					dailyBonusGiven = true;
					Transform clouds = GameObject.Find("clouds").transform;
					for (int i = 0; i < 5; i++) {
						if (clouds.GetChild(i).gameObject != gameObject)
							StartCoroutine(coroutineClickOther(clouds.GetChild(i).GetComponent<UIPlayAnimation>()));
					}
				}
				
				break;
			}
			counter += portion.Value;
		}

	}

	IEnumerator coroutineClickOther(UIPlayAnimation go) {
		yield return new WaitForSeconds(0.5F);
		go.Play(true);
		StartCoroutine(coroutineCloseBonusMenuAnim());


	}

	IEnumerator coroutineCloseBonusMenuAnim() {
		yield return new WaitForSeconds(3F);
		dailyBonusMenu.transform.GetChild(0).GetComponent<Animation>().Play("menu exit");
		StartCoroutine(coroutineCloseBonusMenu());

		
		
	}
	IEnumerator coroutineCloseBonusMenu() {
		yield return new WaitForSeconds(0.5F);
		dailyBonusMenu.SetActive(false);
		
		dailyBonusGiven = false;
		
	}


}
