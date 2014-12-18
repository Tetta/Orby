using UnityEngine;
using System.Collections;

using UnionAssets.FLE;
using System.Collections.Generic;

public class initLevelMenuClass : MonoBehaviour {

	public UILabel gold;
	public UILabel stars;
	public static UILabel goldLabel;
	public static UILabel starsLabel;

	private const string INCREMENTAL_ACHIEVEMENT_ID = "CgkInJa4l7YVEAIQAA";
	private  const string PIE_GIFT_ID = "Pie";
	public Texture2D pieIcon;

	// Use this for initialization
	void Start () {
		//temp
		staticClass.initLevels();
		//temp
		goldLabel = gold;
		starsLabel = stars;
		if (initClass.progress.Count == 0) initClass.getProgress();
		goldLabel.text = initClass.progress["gold"].ToString();
		starsLabel.text = initClass.progress["stars"].ToString();

		//NGUIDebug.Log("level menu startttt");
		//GooglePlayManager.instance.addEventListener (GooglePlayManager.SEND_GIFT_RESULT_RECEIVED, OnGiftResult);
		//GooglePlayManager.instance.addEventListener (GooglePlayManager.PENDING_GAME_REQUESTS_DETECTED, OnPendingGiftsDetected);
		//GooglePlayManager.instance.addEventListener (GooglePlayManager.GAME_REQUESTS_ACCEPTED, OnGameRequestAccepted);

		//GooglePlayManager.instance.incrementAchievementById (INCREMENTAL_ACHIEVEMENT_ID, 1);
		//GooglePlayManager.instance.SendGiftRequest(GPGameRequestType.TYPE_GIFT, 1, pieIcon, "Here is some pie", PIE_GIFT_ID);
		//GooglePlayManager.instance.ShowRequestsAccepDialog();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//--------------------------------------
	// EVENTS
	//--------------------------------------

	/*
	private void OnGiftResult(CEvent e) {
		GooglePlayGiftRequestResult result = e.data as GooglePlayGiftRequestResult;
		SA_StatusBar.text = "Gift Send Result:  " + result.code.ToString();
	}
	
	private void OnPendingGiftsDetected(CEvent e) {
		AndroidDialog dialog = AndroidDialog.Create("Pending Gifts Detected", "You got few gifts from your friends, do you whant to take a look?");
		dialog.addEventListener(BaseEvent.COMPLETE, OnPromtGiftDialogClose);
	}
	
	private void OnPromtGiftDialogClose(CEvent e) {
		//removing listner
		(e.dispatcher as AndroidDialog).removeEventListener(BaseEvent.COMPLETE, OnPromtGiftDialogClose);
		
		//parsing result
		switch((AndroidDialogResult)e.data) {
		case AndroidDialogResult.YES:
			GooglePlayManager.instance.ShowRequestsAccepDialog();
			break;
			
			
		}
	}
	
	
	
	private void OnGameRequestAccepted(CEvent e) {
		List<GPGameRequest> gifts = e.data as List<GPGameRequest>;
		foreach(GPGameRequest g in gifts) {
			AN_PoupsProxy.showMessage("Gfit Accepted", g.playload + " is excepted");
		}
	}
	*/
}
