////////////////////////////////////////////////////////////////////////////////
//  
// @module Common Android Native Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class AndroidNative {

	//--------------------------------------
	// Constants
	//--------------------------------------

	public const string DATA_SPLITTER = "|";
	public const string DATA_EOF = "endofline";
	
	//--------------------------------------
	// Goole Cloud
	//--------------------------------------

	public static void listStates() {
		CallActivityFunction("listStates");
	}

	
	public static void updateState(int stateKey, string data) {
		CallActivityFunction("updateState", stateKey.ToString(), data);
	}

	public static void resolveState(int stateKey, string resolvedData, string resolvedVersion) {
		CallActivityFunction("resolveState", stateKey.ToString(), resolvedData, resolvedVersion);
	}

	public static void deleteState(int stateKey)  {
		CallActivityFunction("deleteState", stateKey.ToString());
	}

	public static void loadState(int stateKey)  {
		CallActivityFunction("loadState", stateKey.ToString());
	}

	
	// --------------------------------------
	// Google Cloud Message
	// --------------------------------------
	
	public static void GCMRgisterDevice(string senderId) {
		CallActivityFunction("GCMRgisterDevice", senderId);
	}

	public static void GCMLoadLastMessage() {
		CallActivityFunction("GCMLoadLastMessage");
	}

	//--------------------------------------
	// Play Service
	//--------------------------------------
	

	public static void playServiceInit (string scopes) {
		CallActivityFunction("playServiceInit", scopes);
	}

	public static void playServiceConnect() {
		CallActivityFunction("playServiceConnect");
	}


	public static void playServiceConnect(string accountName) {
		CallActivityFunction("playServiceConnect", accountName);
	}

	public static void loadToken(string accountName, string scope) {
		CallActivityFunction("getToken", accountName, scope);
	}

	public static void loadToken() {
		CallActivityFunction("getToken");
	}
	
	public static void invalidateToken(string token) {
		CallActivityFunction("invalidateToken", token);
	}


	public static void playServiceDisconnect() {
		CallActivityFunction("playServiceDisconnect");
	}

	public static void showAchivmentsUI() {
		CallActivityFunction("showAchivments");
	}

	public static void showLeaderBoardsUI() {
		CallActivityFunction("showLeaderBoards");
	}

	public static void loadConnectedPlayers() {
		CallActivityFunction("loadConnectedPlayers");
	}
	

	public static void showLeaderBoard(string leaderboardName) {
		CallActivityFunction("showLeaderBoard", leaderboardName);
	}

	public static void showLeaderBoardById(string leaderboardId) {
		CallActivityFunction("showLeaderBoardById", leaderboardId);
	}


	public static void submitScore(string leaderboardName, long score) {
		CallActivityFunction("submitScore", leaderboardName, score.ToString());
	}

	public static void submitScoreById(string leaderboardId, long score) {
		CallActivityFunction("submitScoreById", leaderboardId, score.ToString());
	}

	public static void loadLeaderBoards() {
		CallActivityFunction("loadLeaderBoards");
	}


	public static void UpdatePlayerScore(string leaderboardId, int span, int leaderboardCollection) {
		CallActivityFunction("updatePlayerScore", leaderboardId, span.ToString(), leaderboardCollection.ToString());
	}


	public static void loadPlayerCenteredScores(string leaderboardId, int span, int leaderboardCollection, int maxResults) {
		CallActivityFunction("loadPlayerCenteredScores", leaderboardId, span.ToString(), leaderboardCollection.ToString(), maxResults.ToString());
	}
	
	public static void loadTopScores(string leaderboardId, int span, int leaderboardCollection, int maxResults) {
		CallActivityFunction("loadTopScores", leaderboardId, span.ToString(), leaderboardCollection.ToString(), maxResults.ToString());
	}

	public static void reportAchievement(string achievementName) {
		CallActivityFunction("reportAchievement", achievementName);
	}

	public static void reportAchievementById(string achievementId) {
		CallActivityFunction("reportAchievementById", achievementId);
	}
	

	public static void revealAchievement(string achievementName) {
		CallActivityFunction("revealAchievement", achievementName);
	}

	public static void revealAchievementById(string achievementId) {
		CallActivityFunction("revealAchievementById", achievementId);
	}

	public static void incrementAchievement(string achievementName, string numsteps) {
		CallActivityFunction("incrementAchievement", achievementName, numsteps);
	}

	public static void incrementAchievementById(string achievementId, string numsteps) {
		CallActivityFunction("incrementAchievementById", achievementId, numsteps);
	}

	public static void loadAchievements() {
		CallActivityFunction("loadAchievements");
	}


	public static void resetAchievement(string achievementId) {
		CallActivityFunction("resetAchievement", achievementId);
	}

	public static void ResetAllAchievements() {
		CallActivityFunction("resetAllAchievements");
	}


	public static void resetLeaderBoard(string leaderboardId) {
		CallActivityFunction("resetLeaderBoard", leaderboardId);
	}

	//--------------------------------------
	// GIFTS
	//--------------------------------------

	public static void sendGiftRequest(int type, string playload, int requestLifetimeDays, string icon, string description) {
		CallActivityFunction("sendGiftRequest", type.ToString(), playload, requestLifetimeDays.ToString(), icon, description);
	}

	public static void showRequestAccepDialog() {
		CallActivityFunction("showRequestAccepDialog");
	}


	public static void acceptRequests(string ids) {
		CallActivityFunction("acceptRequests", ids);
	}
	
	public static void dismissRequest(string ids) {
		CallActivityFunction("dismissRequest", ids);
	}

	public static void leaveRoom() {
		CallActivityFunction("leaveRoom");
	}


	public static void acceptInviteToRoom(string invId) {
		CallActivityFunction("acceptInviteToRoom", invId);
	}
	
	public static void showInvitationBox()  {
		CallActivityFunction("showInvitationBox");
	}

	// --------------------------------------
	// QUESTS And Events
	// --------------------------------------

	public static void sumbitEvent(string eventId, int count) {
		CallActivityFunction("sumbitEvent", eventId, count.ToString());
	}
	
	public static void loadEvents() {
		CallActivityFunction("loadEvents");
	}


	public static void showSelectedQuests(string questSelectors) {
		CallActivityFunction("showSelectedQuests", questSelectors);
	}

	public static void acceptQuest(string questId) {
		CallActivityFunction("acceptQuest", questId);
	}

	public static void loadQuests(string questSelectors, int sortOrder) {
		CallActivityFunction("loadQuests", questSelectors, sortOrder.ToString());
	}


	// --------------------------------------
	// RTM
	// --------------------------------------
	
	public static void RTMFindMatch(int minPlayers, int maxPlayers, int bitMask) {
		CallActivityFunction("RTMFindMatch", minPlayers.ToString(), maxPlayers.ToString(), bitMask.ToString());
	}

	public static void sendDataToAll(string data, int sendType) {
		CallActivityFunction("sendDataToAll", data, sendType.ToString());
	}
	
	public static void sendDataToPlayers(string data, string players, int sendType) {
		CallActivityFunction("sendDataToAll", data, players, sendType.ToString());
	}

	public static void ShowWaitingRoomIntent() {
		CallActivityFunction("showWaitingRoomIntent");
	}

	public static void InvitePlayers(int minPlayers, int maxPlayers) {
		CallActivityFunction("invitePlayers", minPlayers.ToString(), maxPlayers.ToString());
	}


	//--------------------------------------
	// Billing
	//--------------------------------------

	
	
	public static void connectToBilling(string ids, string base64EncodedPublicKey) {
		CallActivityFunction("connectToBilling", ids, base64EncodedPublicKey);
	}
	

	public static void retrieveProducDetails() {
		CallActivityFunction("retrieveProducDetails");
	}


	public static void consume(string SKU) {
		CallActivityFunction("consume", SKU);
	}



	public static void purchase(string SKU) {
		CallActivityFunction("purchase", SKU, "");
	}

	public static void purchase(string SKU, string developerPayload) {
		CallActivityFunction("purchase", SKU, developerPayload);
	}

	public static void subscribe(string SKU) {
		CallActivityFunction("subscribe", SKU, "");
	}
	
	public static void subscribe(string SKU, string developerPayload) {
		CallActivityFunction("subscribe", SKU, developerPayload);
	}

	
	//--------------------------------------
	//  MESSAGING
	//--------------------------------------


	public static void showDialog(string title, string message) {
		showDialog (title, message, "Yes", "No");
	}

	public static void showDialog(string title, string message, string yes, string no) {
		CallActivityFunction("showDialog", title, message, yes, no);
	}


	public static void showMessage(string title, string message) {
		showMessage (title, message, "Ok");
	}


	public static void showMessage(string title, string message, string ok) {
		CallActivityFunction("ShowMessage", title, message, ok);
	}

	public static void OpenAppRatePage(string url) {
		CallActivityFunction("OpenAppRatePage", url);
	}
	

	public static void showRateDialog(string title, string message, string yes, string laiter, string no, string url) {
		CallActivityFunction("ShowRateDialog", title, message, yes, laiter, no, url);
	}

	public static void ShowPreloader(string title, string message) {
		CallActivityFunction("ShowPreloader",  title, message);
	}
	
	public static void HidePreloader() {
		CallActivityFunction("HidePreloader");
	}
	

	//--------------------------------------
	// Other
	//--------------------------------------

	public static void LoadContacts() {
		CallActivityFunction("loadAddressBook");
	}

	public static void enableImmersiveMode() {
		CallActivityFunction("enableImmersiveMode");
	}


	public static void LoadPackageInfo() {
		CallActivityFunction("LoadPackageInfo");
	}


	//--------------------------------------
	// Google Ad
	//--------------------------------------

	public static void InitMobileAd(string id) {
		CallActivityFunction("InitMobileAd", id);
	}

	public static void ChangeBannersUnitID(string id) {
		CallActivityFunction("ChangeBannersUnitID", id);
	}

	public static void ChangeInterstisialsUnitID(string id) {
		CallActivityFunction("ChangeInterstisialsUnitID", id);
	}

	public static void CreateBannerAd(int gravity, int size, int id) {
		CallActivityFunction("CreateBannerAd", gravity.ToString(), size.ToString(), id.ToString());
	}

	public static void CreateBannerAdPos(int x, int y, int size, int id) {
		CallActivityFunction("CreateBannerAdPos", x.ToString(), y.ToString(), size.ToString(), id.ToString());
	}


	// By nastrandsky
	public static void SetBannerPosition(int gravity, int bannerId) {
		CallActivityFunction ("SetBannerPosition", gravity.ToString(), bannerId.ToString());
	}
	
	// By nastrandsky
	public static void SetBannerPosition(int x, int y, int bannerId) {
		CallActivityFunction ("SetBannerPosition", x.ToString(), y.ToString(), bannerId.ToString());
	}



	public static void HideAd(int id) { 
		CallActivityFunction ("HideAd", id.ToString());
	}

	public static void ShowAd(int id) { 
		CallActivityFunction ("ShowAd", id.ToString());
	}

	public static void RefreshAd(int id) { 
		CallActivityFunction ("RefreshAd", id.ToString());
	}


	public static void DestroyBanner(int id) { 
		CallActivityFunction ("DestroyBanner", id.ToString());
	}


	
	public static void StartInterstitialAd() {
		CallActivityFunction ("StartInterstitialAd");
	}
	
	public static void LoadInterstitialAd() {
		CallActivityFunction ("LoadInterstitialAd");
	}
	
	public static void ShowInterstitialAd() {
		CallActivityFunction ("ShowInterstitialAd");
	}

	public static void RecordInAppResolution(int res) {
		CallActivityFunction ("RecordInAppResolution", res.ToString());
	}

	public static void AddKeyword(string keyword) {
		CallActivityFunction ("AddKeyword", keyword);
	}
	

	public static void SetBirthday(int year, int month, int day) {
		CallActivityFunction ("SetBirthday", year.ToString(), month.ToString(), day.ToString());
	}
	
	public static void TagForChildDirectedTreatment(bool tagForChildDirectedTreatment) {
		if(tagForChildDirectedTreatment) {
			CallActivityFunction ("TagForChildDirectedTreatment", "1");
		} else {
			CallActivityFunction ("TagForChildDirectedTreatment", "0");
		}
		
	}

	public static void AddTestDevice(string deviceId) {
		CallActivityFunction ("AddTestDevice", deviceId);
	}

	// By nastrandsky: Ad various test devices at once.
	public static void AddTestDevices(string cvsDeviceIds) {
		CallActivityFunction ("AddTestDevices", cvsDeviceIds);
	}

	public static void SetGender(int gender) {
		CallActivityFunction ("SetGender", gender.ToString());
	}


	// --------------------------------------
	// Analytics
	// --------------------------------------

	public static void startAnalyticsTracking() {
		CallActivityFunction ("startAnalyticsTracking");
	}

	public static void SetTrackerID(string trackingID) {
		CallActivityFunction("SetTrackerID", trackingID);
	}

	public static void SendView() {
		CallActivityFunction("SendView");
	}

	public static void SendView(string appScreen) {
		CallActivityFunction("SendView", appScreen);
	}

	public static void SendEvent(string category, string action, string label, string value)  {
		CallActivityFunction("SendEvent", category, action, label, value);
	}

	public static void SendEvent(string category, string action, string label, string value, string key, string val)  {
		CallActivityFunction("SendEvent", category, action, label, value, key, val);
	}

	public static void SendTiming(string category, string intervalInMilliseconds, string name, string label) {
		CallActivityFunction("SendTiming", category, intervalInMilliseconds, name, label);
	}

	public static void CreateTransaction(string transactionId, string affiliation, string revenue, string tax, string shipping, string currencyCode) {
		CallActivityFunction("CreateTransaction", transactionId, affiliation, revenue, tax, shipping, currencyCode);
	}
	
	public static void CreateItem(string transactionId, string name, string sku, string category, string price, string quantity, string currencyCode) {
		CallActivityFunction("CreateItem", transactionId, name, sku, category, price, quantity, currencyCode);
	}



	public static void SetKey(string key, string value) {
		CallActivityFunction("SetKey", key, value);
	}

	public static void ClearKey(string key) {
		CallActivityFunction("ClearKey", key);
	}

	public static void SetLogLevel(int lvl) {
		CallActivityFunction("SetLogLevel", lvl.ToString());
	}


	public static void SetDryRun(string mode) {
		CallActivityFunction("SetDryRun", mode);
	}


	// --------------------------------------
	// Social
	// --------------------------------------


	public static void StartShareIntent(string caption, string message,  string subject, string filters) {
		CallActivityFunction("StartShareIntent", caption, message, subject, filters);
	}

	public static void StartShareIntent(string caption, string message, string subject, string media, string filters) {
		CallActivityFunction("StartShareIntentMedia", caption, message, subject, media, filters);
	}

	public static void SendMailWithImage(string caption, string message,  string subject, string email, string media) {
		CallActivityFunction("SendMailWithImage", caption, message, subject, email, media);
	}
	

	public static void SendMail(string caption, string message,  string subject, string email) {
		CallActivityFunction("SendMail", caption, message, subject, email);
	}

	// --------------------------------------
	// Twitter
	// --------------------------------------

	public static void TwitterInit(string consumer_key, string consumer_secret) {
		CallActivityFunction("TwitterInit", consumer_key, consumer_secret);
	}
	
	public static void AuthificateUser() {
		CallActivityFunction("AuthificateUser");
	}

	public static void LoadUserData() {
		CallActivityFunction("LoadUserData");
	}

	public static void TwitterPost(string status) {
		CallActivityFunction("TwitterPost", status);
	}

	public static void TwitterPostWithImage(string status, string data) {
		CallActivityFunction("TwitterPostWithImage", status, data);
	}

	public static void LogoutFromTwitter() {
		CallActivityFunction("LogoutFromTwitter");
	}

	// --------------------------------------
	// Instagram
	// --------------------------------------



	public static void InstagramPostImage(string data, string cpation) {
		CallActivityFunction("InstagramPostImage", data, cpation);
	}


	// --------------------------------------
	// Utils
	// --------------------------------------
	
	public static void isPackageInstalled(string packagename) {
		CallActivityFunction("isPackageInstalled", packagename);
	}

	public static void runPackage(string packagename) {
		CallActivityFunction("runPackage", packagename);
	}

	public static void loadGoogleAccountNames() {
		CallActivityFunction("loadGoogleAccountNames");
	}
	

	public static void ShowToastNotification(string text, int duration) {
		CallActivityFunction("ShowToastNotification", text, duration.ToString());
	}

	public static void requestCurrentAppLaunchNotificationId() { 
		CallActivityFunction("requestCurrentAppLaunchNotificationId");
	}


	public static void ScheduleLocalNotification(string title, string message, int seconds, int id) {
		CallActivityFunction("ScheduleLocalNotification", title, message, seconds.ToString(), id.ToString());
	}

	public static void CanselLocalNotification(int id) {
		CallActivityFunction("canselLocalNotification", id.ToString());
	}


	public static void InitCameraAPI(string folderName, int maxSize, int mode) {
		CallActivityFunction("InitCameraAPI", folderName, maxSize.ToString(), mode.ToString());
	}

	public static void SaveToGalalry(string ImageData, string name) {
		CallActivityFunction("SaveToGalalry", ImageData, name);
	}


	public static void GetImageFromGallery() {
		Debug.Log("GetImageFromGallery");
		CallActivityFunction("GetImageFromGallery");
	}
	

	
	public static void GetImageFromCamera() {
		Debug.Log("GetImageFromCamera");
		CallActivityFunction("GetImageFromCamera");
	}


	private static void CallActivityFunction(string methodName, params object[] args) {
       #if UNITY_ANDROID

		if(Application.platform != RuntimePlatform.Android) {
			return;
		}

		try {

			AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
			AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"); 

			jo.Call("runOnUiThread", new AndroidJavaRunnable(() => { jo.Call(methodName, args); }));


		} catch(System.Exception ex) {
			Debug.LogWarning(ex.Message);
		}
		#endif
	}

}
