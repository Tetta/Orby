using UnityEngine;
using System.Collections;
using UnionAssets.FLE;

public class lsGiftClass : MonoBehaviour {
	//private const string MY_BANNERS_AD_UNIT_ID		 = "ca-app-pub-6101605888755494/1824764765"; 
	private const string MY_INTERSTISIALS_AD_UNIT_ID =  "ca-app-pub-7242500596489894/5124169969"; 

	// Use this for initialization
	void Start () {
		//AndroidAdMobController.instance.Init(MY_BANNERS_AD_UNIT_ID);
		AndroidAdMobController.instance.Init(MY_INTERSTISIALS_AD_UNIT_ID);

		//I whant to use Interstisial ad also, so I have to set additional id for it
		//AndroidAdMobController.instance.SetInterstisialsUnitID(MY_INTERSTISIALS_AD_UNIT_ID);

		//Optional, add data for better ad targeting
		/*
		AndroidAdMobController.instance.SetGender(GoogleGenger.Male);
		AndroidAdMobController.instance.AddKeyword("game");
		AndroidAdMobController.instance.SetBirthday(1989, AndroidMonth.MARCH, 18);
		AndroidAdMobController.instance.TagForChildDirectedTreatment(false);
		*/
		//Called when interstitial an ad opens an overlay that covers the screen.
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_OPENED, OnInterstisialsOpen);
		//Called when the user is about to return to the application after clicking on an ad.
		AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_INTERSTITIAL_AD_CLOSED, OnInterstisialsClosed);

		//listening for InApp Event
		//You will only receive in-app purchase (IAP) ads if you specifically configure an IAP ad campaign in the AdMob front end.
		//AndroidAdMobController.instance.addEventListener(GoogleMobileAdEvents.ON_AD_IN_APP_REQUEST, OnInAppRequest);

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick () {
		AndroidAdMobController.instance.StartInterstitialAd ();
	}

	private void OnInterstisialsOpen() {
		Debug.Log(333);

	}

	private void OnInterstisialsClosed() {
		initClass.progress["gold"] += 1;
		initClass.saveProgress();
		initLevelMenuClass.goldLabel.text = initClass.progress["gold"].ToString();
	}

	/*
	private void OnInAppRequest(CEvent e) {
		//getting product id
		string productId = (string) e.data;
		AN_PoupsProxy.showMessage ("In App Request", "In App Request for product Id: " + productId + " received");
		
		
		//Then you should perfrom purchase  for this product id, using this or another game billing plugin
		//Once the purchase is complete, you should call RecordInAppResolution with one of the constants defined in GADInAppResolution:
		
		AndroidAdMobController.instance.RecordInAppResolution(GADInAppResolution.RESOLUTION_SUCCESS);
	}
	*/
}