using UnityEngine;
using System.Collections;
using System;

public class lsEnergyClass : MonoBehaviour {

	public GameObject energyMenu;
	public UILabel minutes;
	public UILabel seconds;
	public UILabel energyLabel;
	public UISprite energyLine;

	public static string energyMenuState = "";

	public static int costEnergy = 60;
	public static int maxEnergy = 1000;

	// Use this for initialization
	void Start () {
		OnApplicationPause(false);
		if (energyMenuState == "energy") OnClick();

		//init complect, work only with scene "menu"
		//GameObject complect = GameObject.Instantiate(marketClass.instance.specialMenu.transform.GetChild(0).GetChild(0).gameObject);
		//complect.transform.parent = energyMenu.transform.GetChild(0).GetChild(0);
		//complect.transform.localPosition = new Vector3(-98, -776, -0.01F);
		//complect.transform.localScale = new Vector3(1F, 1F, 0);
		//
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator Coroutine(){
		int mod = checkEnergy(true);
		energyLabel.text = (initClass.progress["energy"]).ToString();
		energyLine.fillAmount = (float) initClass.progress["energy"] / maxEnergy;

		// остановка выполнения функции на costEnergy секунд
		yield return new WaitForSeconds(costEnergy - mod);
		
		// запускаем корутину снова
		StartCoroutine("Coroutine");
	}

	public static int checkEnergy(bool flag) {
		if (initClass.progress.Count == 0) initClass.getProgress();

		//число секунд с 01.01.2015
		int now = (int)(DateTime.UtcNow - new DateTime(2015, 1, 1)).TotalSeconds;
		int deltaEnergy = Mathf.CeilToInt( (now - initClass.progress["energyTime"]) / costEnergy);
		initClass.progress["energy"] += deltaEnergy;
		int mod = (now - initClass.progress["energyTime"]) % costEnergy;
		initClass.progress["energyTime"] = now - mod;
		if (initClass.progress["energy"] > maxEnergy) initClass.progress["energy"] = maxEnergy;
		if (flag) {
			initClass.saveProgress();
			if (maxEnergy == initClass.progress["energy"]) GameObject.Find("energy").SendMessage("stopCoroutineEnergyMenu");
			return mod;
		} else {
			if (initClass.progress["energy"] > 0) {
				initClass.progress["energy"] --;
				initClass.saveProgress();
				return 1;
			} else return 0;
		}

	}

	public void OnApplicationPause(bool flag) {
		if (!flag) { 
			if (initClass.progress.Count == 0) initClass.getProgress();
			//StopCoroutine("Coroutine");
			StopAllCoroutines();
			StartCoroutine("Coroutine");
			if (energyMenu.activeSelf) StartCoroutine("CoroutineEnergyMenu");
		}
	}

	void OnClick() {
		if (initClass.progress["energy"] < maxEnergy){
			energyMenu.SetActive(true);
			StartCoroutine("CoroutineEnergyMenu");
		}

	}

	public IEnumerator CoroutineEnergyMenu(){
		int mod = checkEnergy(true);
		minutes.text = (Mathf.CeilToInt((costEnergy - mod) / 60)).ToString();
		int modSec = (costEnergy - mod) % 60;
		if (modSec < 10) seconds.text = "0" + modSec.ToString();
		else seconds.text = modSec.ToString();

		// остановка выполнения функции
		yield return new WaitForSeconds(1);
		
		// запускаем корутину снова
		StartCoroutine("CoroutineEnergyMenu");
	}

	void stopCoroutineEnergyMenu () {
		energyMenu.SetActive(false);
		StopCoroutine("CoroutineEnergyMenu");
	}

}
