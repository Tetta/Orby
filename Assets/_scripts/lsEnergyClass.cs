using UnityEngine;
using System.Collections;
using System;

public class lsEnergyClass : MonoBehaviour {

	public GameObject energyMenu;
	public UILabel minutes;
	public UILabel seconds;

	public static string energyMenuState = "";

	private UILabel label;
	private static int costEnergy = 60;
	private int maxEnergy = 10;

	// Use this for initialization
	void Start () {
		OnApplicationPause(false);
		if (energyMenuState == "energy") OnClick();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public IEnumerator Coroutine(){
		int mod = checkEnergy(true);
		label.text = (initClass.progress["energy"]).ToString();

		// остановка выполнения функции на costEnergy секунд
		yield return new WaitForSeconds(costEnergy - mod);
		
		// запускаем корутину снова
		StartCoroutine("Coroutine");
	}

	public static int checkEnergy(bool flag) {
		if (initClass.progress.Count == 0) initClass.getProgress();

		int maxEnergy = 10;
		//число секунд с 01.01.2015
		int now = (int)(DateTime.UtcNow - new DateTime(2015, 1, 1)).TotalSeconds;
		int deltaEnergy = Mathf.CeilToInt( (now - initClass.progress["energyTime"]) / costEnergy);
		initClass.progress["energy"] += deltaEnergy;
		int mod = (now - initClass.progress["energyTime"]) % costEnergy;
		initClass.progress["energyTime"] = now - mod;
		if (initClass.progress["energy"] > maxEnergy) initClass.progress["energy"] = maxEnergy;
		if (flag) {
			initClass.saveProgress();
			if (maxEnergy == initClass.progress["energy"]) GameObject.Find("energyLabel").SendMessage("stopCoroutineEnergyMenu");
			return mod;
		} else {
			if (initClass.progress["energy"] > 0) {
				initClass.progress["energy"] --;
				initClass.saveProgress();
				return 1;
			} else return 0;
		}

	}

	void OnApplicationPause(bool flag) {
		if (!flag) { 
			label = GetComponent<UILabel>();
			if (initClass.progress.Count == 0) initClass.getProgress();
			//StopCoroutine("Coroutine");
			StopAllCoroutines();
			StartCoroutine("Coroutine");
			if (energyMenu.activeSelf)StartCoroutine("CoroutineEnergyMenu");
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
