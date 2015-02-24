using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;

public class unAdClass : MonoBehaviour {
	public static unAdClass instance = null;
	void Awake() {
		if(instance!=null){
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);

		if (Advertisement.isSupported) {
			Advertisement.allowPrecache = true;
			Advertisement.Initialize ("131624473");
		} else {
			//Debug.Log("Platform not supported");
		}
	}
	
}
