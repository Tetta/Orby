using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.Cloud.Analytics;

public class ctrAdTestClass : MonoBehaviour {
	private int i;
	public GoogleAnalyticsV3 GA;
	void Awake() {
		/*
		if (Advertisement.isSupported) {
			Advertisement.allowPrecache = true;
			Advertisement.Initialize ("131623325"); //current id = id Forest Spider
			//Advertisement.Initialize ("131625236"); //current id = id Forest Spider IOS
		}
		*/
	}
	void Start () {
		//AndroidAdMobController.instance.Init("ca-app-pub-7242500596489894/3891963164");
		GA.LogScreen("test");

	}

	void Update () {
		i ++;
		if (i % 1000 == 0) {
			//if (Advertisement.isReady("rewardedVideoZone")) {
			//	Advertisement.Show("rewardedVideoZone");
			//}
			//AndroidAdMobController.instance.StartInterstitialAd();
		}
	}

	
}
