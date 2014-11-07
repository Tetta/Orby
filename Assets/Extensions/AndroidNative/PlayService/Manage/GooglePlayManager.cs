////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GooglePlayManager : SA_Singleton<GooglePlayManager> {


	//Events
	public const string SCORE_SUBMITED            = "score_submited";
	public const string SCORE_UPDATED             = "score_updated";
	public const string LEADERBOARDS_LOEADED      = "leaderboards_loeaded";
	public const string FRIENDS_LOADED            = "players_loaded";
	public const string ACHIEVEMENT_UPDATED       = "achievement_updated";
	public const string ACHIEVEMENTS_LOADED       = "achievements_loaded";
	public const string SCORE_REQUEST_RECEIVED    = "score_request_received";


	public const string SEND_GIFT_RESULT_RECEIVED  = "send_gift_result_received";
	public const string REQUESTS_INBOX_DIALOG_DISMISSED  = "requests_inbox_dialog_dismissed";

	public const string PENDING_GAME_REQUESTS_DETECTED  = "pending_game_requests_detected";
	public const string GAME_REQUESTS_ACCEPTED  		= "game_requests_accepted";


	public const string AVALIABLE_DEVICE_ACCOUNTS_LOADED  = "avaliable_device_accounts_loaded";
	public const string OAUTH_TOCKEN_LOADED  			 = "oauth_tocken_loaded";


	//Actions
	public static Action<GP_GamesResult> ActionSoreSubmited 							= delegate {};
	public static Action<GP_GamesResult> ActionSoreUpdatedd								= delegate {};
	public static Action<GooglePlayResult> ActionLeaderboardsLoeaded 					= delegate {};
	public static Action<GooglePlayResult> ActionFriendsLoeaded 						= delegate {};
	public static Action<GP_GamesResult> ActionAchievementUpdated 						= delegate {};
	public static Action<GooglePlayResult> ActionAchievementsLoeaded 					= delegate {};
	public static Action<GooglePlayResult> ActionScoreRequestReceived 					= delegate {};

	public static Action<GooglePlayGiftRequestResult> ActionSendGiftResultReceived 		= delegate {};
	public static Action ActionRequestsInboxDialogDismissed 							= delegate {};
	public static Action<List<GPGameRequest>> ActionPendingGameRequestsDetected 		= delegate {};
	public static Action<List<GPGameRequest>> ActionGameRequestsAccepted 				= delegate {};

	public static Action<List<string>> ActionAvaliableDeviceAccountsLoaded 				= delegate {};
	public static Action<string> ActionOAuthTockenLoaded 								= delegate {};



	private GooglePlayerTemplate _player = null ;
	

	private Dictionary<string, GPLeaderBoard> _leaderBoards =  new Dictionary<string, GPLeaderBoard>();
	private Dictionary<string, GPAchievement> _achievements = new Dictionary<string, GPAchievement>();
	private Dictionary<string, GooglePlayerTemplate> _players = new Dictionary<string, GooglePlayerTemplate>();



	private List<string> _friendsList 		  				=  new List<string>();
	private List<string> _deviceGoogleAccountList 		 	=  new List<string>();
	private List<GPGameRequest> _gameRequests 				=  new List<GPGameRequest>();


	private string _loadedAuthTocken = "";
	private string _currentAccount = "";

	private static bool _IsLeaderboardsDataLoaded = false;
	


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}

	public void Create() {
		Debug.Log ("GooglePlayManager was created");

		//Creating sub managers
		GooglePlayQuests.instance.Init();
	}


	//--------------------------------------
	// PUBLIC API CALL METHODS
	//--------------------------------------

	public void RetriveDeviceGoogleAccounts() {
		AndroidNative.loadGoogleAccountNames();
	}

	public void LoadTocken(string accountName,  string scopes) {
		AndroidNative.loadToken(accountName, scopes);

	}

	public void LoadTocken() {
		AndroidNative.loadToken();
		
	}

	public void InvalidateToken(string token) {
		AndroidNative.invalidateToken(token);
	}



 
	public void showAchievementsUI() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.showAchivmentsUI ();
	}

	public void showLeaderBoardsUI() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.showLeaderBoardsUI ();
	}

	public void showLeaderBoard(string leaderboardName) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.showLeaderBoard (leaderboardName);
	}

	public void showLeaderBoardById(string leaderboardId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.showLeaderBoardById (leaderboardId);
	}
	
	public void submitScore(string leaderboardName, long score) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.submitScore (leaderboardName, score);
	}

	public void submitScoreById(string leaderboardId, long score) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.submitScoreById (leaderboardId, score);
	}

	public void loadLeaderBoards() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.loadLeaderBoards ();
	}


	public void UpdatePlayerScore(string leaderboardId, GPBoardTimeSpan span, GPCollectionType collection) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.UpdatePlayerScore(leaderboardId, (int) span, (int) collection);
	}



	public void loadPlayerCenteredScores(string leaderboardId, GPBoardTimeSpan span, GPCollectionType collection, int maxResults) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.loadPlayerCenteredScores(leaderboardId, (int) span, (int) collection, maxResults);
	}
	
	public void loadTopScores(string leaderboardId, GPBoardTimeSpan span, GPCollectionType collection, int maxResults) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.loadTopScores(leaderboardId, (int) span, (int) collection, maxResults);
	}


	public void reportAchievement(string achievementName) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.reportAchievement (achievementName);
	}

	public void reportAchievementById(string achievementId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.reportAchievementById (achievementId);
	}


	public void revealAchievement(string achievementName) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.revealAchievement (achievementName);
	}

	public void revealAchievementById(string achievementId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.revealAchievementById (achievementId);
	}

	public void incrementAchievement(string achievementName, int numsteps) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.incrementAchievement (achievementName, numsteps.ToString());
	}

	public void incrementAchievementById(string achievementId, int numsteps) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.incrementAchievementById (achievementId, numsteps.ToString());
	}

	public void loadAchievements() {

		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.loadAchievements ();
	}

	public void resetAchievement(string achievementId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.resetAchievement(achievementId);

	}

	public void ResetAllAchievements() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.ResetAllAchievements();
		
	}




	public void resetLeaderBoard(string leaderboardId) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.resetLeaderBoard(leaderboardId);

		if(leaderBoards.ContainsKey(leaderboardId)) {
			leaderBoards.Remove(leaderboardId);
		}

	}


	public void loadConnectedPlayers() {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.loadConnectedPlayers ();
	}


	//--------------------------------------
	// GIFTS
	//--------------------------------------
	
	public void SendGiftRequest(GPGameRequestType type, int requestLifetimeDays, Texture2D icon, string description, string playload = "") {
		if (!GooglePlayConnection.CheckState ()) { return; }

		byte[] val = icon.EncodeToPNG();
		string bytesString = System.Convert.ToBase64String (val);

		AndroidNative.sendGiftRequest((int) type, playload, requestLifetimeDays, bytesString, description);

	}

	public string currentAccount {
		get {
			return _currentAccount;
		}
	}
	
	public void ShowRequestsAccepDialog() {
		if (!GooglePlayConnection.CheckState ()) { return; }

		AndroidNative.showRequestAccepDialog();
	}

	public void AcceptRequests(params string[] ids) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		
		if(ids.Length == 0) {
			return;
		}
		
		
		AndroidNative.acceptRequests(string.Join(AndroidNative.DATA_SPLITTER, ids));
	}


	public void DismissRequest(params string[] ids) {
		if (!GooglePlayConnection.CheckState ()) { return; }
		
		if(ids.Length == 0) {
			return;
		}
		
		
		AndroidNative.dismissRequest(string.Join(AndroidNative.DATA_SPLITTER, ids));
	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	public GPLeaderBoard GetLeaderBoard(string leaderboardId) {
		if(_leaderBoards.ContainsKey(leaderboardId)) {
			return _leaderBoards[leaderboardId];
		} else {
			return null;
		}
	}
	

	public GPAchievement GetAchievement(string achievementId) {
		if(_achievements.ContainsKey(achievementId)) {
			return _achievements[achievementId];
		} else {
			return null;
		}
	}


	public GooglePlayerTemplate GetPlayerById(string playerId) {
		if(players.ContainsKey(playerId)) {
			return players[playerId];
		} else {
			return null;
		}
	}

	public GPGameRequest GetGameRequestById(string id) {
		foreach(GPGameRequest r in _gameRequests) {
			if(r.id.Equals(id)) {
				return r;
			} 
		}

		return null;
	} 


	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public GooglePlayerTemplate player {
		get {
			return _player;
		}
	}

	public Dictionary<string, GooglePlayerTemplate> players {
		get {
			return _players;
		}
	}
	

	public Dictionary<string, GPLeaderBoard> leaderBoards {
		get {
			return _leaderBoards;
		}
	}

	public Dictionary<string, GPAchievement> achievements {
		get {
			return _achievements;
		}
	}

	public List<string> friendsList {
		get {
			return _friendsList;
		}
	}


	public List<GPGameRequest> gameRequests {
		get {
			return _gameRequests;
		}
	}

	public List<string> deviceGoogleAccountList {
		get {
			return _deviceGoogleAccountList;
		}
	}

	public string loadedAuthTocken {
		get {
			return _loadedAuthTocken;
		}
	}

	public static bool IsLeaderboardsDataLoaded {
		get {
			return _IsLeaderboardsDataLoaded;
		}
	}

	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void OnGiftSendResult(string data) {

		Debug.Log("OnGiftSendResult");

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GooglePlayGiftRequestResult result =  new GooglePlayGiftRequestResult(storeData [0]);

		ActionSendGiftResultReceived(result);
		dispatch(SEND_GIFT_RESULT_RECEIVED, result);
	}

	private void OnRequestsInboxDialogDismissed(string data) {
		ActionRequestsInboxDialogDismissed();
		dispatch(REQUESTS_INBOX_DIALOG_DISMISSED);
	}


	private void OnAchievementsLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.isSuccess) {

			_achievements.Clear ();

			for(int i = 1; i < storeData.Length; i+=7) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}

				GPAchievement ach = new GPAchievement (storeData[i], 
				                                       storeData[i + 1],
				                                       storeData[i + 2],
				                                       storeData[i + 3],
				                                       storeData[i + 4],
				                                       storeData[i + 5],
				                                       storeData[i + 6]
				                                       );

				Debug.Log (ach.name);
				Debug.Log (ach.type);


				_achievements.Add (ach.id, ach);

			}

			Debug.Log ("Loaded: " + _achievements.Count + " Achievements");
		}

		ActionAchievementsLoeaded(result);
		dispatch (ACHIEVEMENTS_LOADED, result);

	}

	private void OnAchievementUpdated(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GP_GamesResult result = new GP_GamesResult (storeData [0]);
		result.achievementId = storeData [1];

		ActionAchievementUpdated(result);
		dispatch (ACHIEVEMENT_UPDATED, result);

	}

	private void OnScoreDataRecevied(string data) {
		Debug.Log("OnScoreDataRecevide");
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.isSuccess) {

			GPBoardTimeSpan 	timeSpan 		= (GPBoardTimeSpan)  System.Convert.ToInt32(storeData[1]);
			GPCollectionType 	collection  	= (GPCollectionType) System.Convert.ToInt32(storeData[2]);
			string leaderboardId 				= storeData[3];
			string leaderboardName 				= storeData[4];


			GPLeaderBoard lb;
			if(_leaderBoards.ContainsKey(leaderboardId)) {
				lb = _leaderBoards[leaderboardId];
			} else {
				lb = new GPLeaderBoard (leaderboardId, leaderboardName);
				Debug.Log("Added: "  + leaderboardId);
				_leaderBoards.Add(leaderboardId, lb);
			}

			lb.UpdateName(leaderboardName);

			for(int i = 5; i < storeData.Length; i+=8) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}


				
			 	long score = System.Convert.ToInt64(storeData[i]);
				int rank = System.Convert.ToInt32(storeData[i + 1]);


				string playerId = storeData[i + 2];
				if(!players.ContainsKey(playerId)) {
					GooglePlayerTemplate p = new GooglePlayerTemplate (playerId, storeData[i + 3], storeData[i + 4], storeData[i + 5], storeData[i + 6], storeData[i + 7]);
					AddPlayer(p);
				}

				GPScore s =  new GPScore(score, rank, timeSpan, collection, lb.id, playerId);
				lb.UpdateScore(s);

				if(playerId.Equals(player.playerId)) {
					lb.UpdateCurrentPlayerRank(rank, timeSpan, collection);
				}
			}
		}


		ActionScoreRequestReceived(result);
		dispatch (SCORE_REQUEST_RECEIVED, result);

	}

	private void OnLeaderboardDataLoaded(string data) {
		Debug.Log("OnLeaderboardDataLoaded");
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);


		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.isSuccess) {

			for(int i = 1; i < storeData.Length; i+=26) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}

				string leaderboardId = storeData[i];
				string leaderboardName = storeData [i + 1];

			
				GPLeaderBoard lb;
				if(_leaderBoards.ContainsKey(leaderboardId)) {
					lb = _leaderBoards[leaderboardId];
				} else {
					lb = new GPLeaderBoard (leaderboardId, leaderboardName);
					_leaderBoards.Add(leaderboardId, lb);
				}

				lb.UpdateName(leaderboardName);


				int start = i + 2;
				for(int j = 0; j < 6; j++) {

					long score = System.Convert.ToInt64(storeData[start]);
					int rank = System.Convert.ToInt32(storeData[start + 1]);

					GPBoardTimeSpan 	timeSpan 		= (GPBoardTimeSpan)  System.Convert.ToInt32(storeData[start + 2]);
					GPCollectionType 	collection  	= (GPCollectionType) System.Convert.ToInt32(storeData[start + 3]);

					//Debug.Log("timeSpan: " + timeSpan +   " collection: " + collection + " score:" + score + " rank:" + rank);

					GPScore variant =  new GPScore(score, rank, timeSpan, collection, lb.id, player.playerId);
					start = start + 4;
					lb.UpdateScore(variant);
					lb.UpdateCurrentPlayerRank(rank, timeSpan, collection);

				}


			}

			Debug.Log ("Loaded: " + _leaderBoards.Count + " Leaderboards");
		}

		_IsLeaderboardsDataLoaded = true;

		ActionLeaderboardsLoeaded(result);
		dispatch (LEADERBOARDS_LOEADED, result);

	}


	private void OnPlayerScoreUpdated(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("GooglePlayManager OnPlayerScoreUpdated, no data avaiable");
			return;
		}


		Debug.Log("OnPlayerScoreUpdated");


		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		GP_GamesResult result = new GP_GamesResult (storeData [0]);

		if(result.isSuccess) {

			GPBoardTimeSpan 	timeSpan 		= (GPBoardTimeSpan)  System.Convert.ToInt32(storeData[1]);
			GPCollectionType 	collection  	= (GPCollectionType) System.Convert.ToInt32(storeData[2]);

			string leaderboardId = storeData[3];

			long score = System.Convert.ToInt64(storeData[4]);
			int rank = System.Convert.ToInt32(storeData[5]);

			GPLeaderBoard lb;
			if(_leaderBoards.ContainsKey(leaderboardId)) {
				lb = _leaderBoards[leaderboardId];
			} else {
				lb = new GPLeaderBoard (leaderboardId, "");
				_leaderBoards.Add(leaderboardId, lb);
			}

			GPScore variant =  new GPScore(score, rank, timeSpan, collection, lb.id, player.playerId);
			lb.UpdateScore(variant);
			lb.UpdateCurrentPlayerRank(rank, timeSpan, collection);

		}

		ActionSoreUpdatedd(result);
		dispatch (SCORE_UPDATED, result);
	}

	private void OnScoreSubmitted(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("GooglePlayManager OnScoreSubmitted, no data avaiable");
			return;
		}

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		GP_GamesResult result = new GP_GamesResult (storeData [0]);
		result.leaderboardId = storeData [1];

		ActionSoreSubmited(result);
		dispatch (SCORE_SUBMITED, result);

	}

	private void OnPlayerDataLoaded(string data) {

		Debug.Log("OnPlayerDataLoaded");
		if(data.Equals(string.Empty)) {
			Debug.Log("GooglePlayManager OnPlayerLoaded, no data avaiable");
			return;
		}

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		_player = new GooglePlayerTemplate (storeData [0], storeData [1], storeData [2], storeData [3], storeData [4], storeData [5]);
		AddPlayer(_player);

		_currentAccount = storeData [6];
	}

	private void OnPlayersLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		GooglePlayResult result = new GooglePlayResult (storeData [0]);
		if(result.isSuccess) {

			for(int i = 1; i < storeData.Length; i+=6) {
				if(storeData[i] == AndroidNative.DATA_EOF) {
					break;
				}
				

				GooglePlayerTemplate p = new GooglePlayerTemplate (storeData[i], storeData[i + 1], storeData[i + 2], storeData[i + 3], storeData[i + 4], storeData[i + 5]);
				AddPlayer(p);
				if(!_friendsList.Contains(p.playerId)) {
					_friendsList.Add(p.playerId);
				}

			}
		}
		
		
		
		Debug.Log ("OnPlayersLoaded, total:" + players.Count.ToString());
		ActionFriendsLoeaded(result);
		dispatch (FRIENDS_LOADED, result);
	}

	private void OnGameRequestsLoaded(string data) {
		_gameRequests = new List<GPGameRequest>();
		if(data.Length == 0) {
			return;
		}


		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		for(int i = 0; i < storeData.Length; i+=6) {
			if(storeData[i] == AndroidNative.DATA_EOF) {
				break;
			}

			GPGameRequest r = new GPGameRequest();
			r.id = storeData[i];
			r.playload = storeData[i +1];

			r.expirationTimestamp 	 = System.Convert.ToInt64(storeData[i + 2]);
			r.creationTimestamp		 = System.Convert.ToInt64(storeData[i + 3]);

			r.sender = storeData[i +4];
			r.type = (GPGameRequestType) System.Convert.ToInt32(storeData[i + 5]);
			_gameRequests.Add(r);


		}

		ActionPendingGameRequestsDetected(_gameRequests);
		dispatch(PENDING_GAME_REQUESTS_DETECTED, _gameRequests);

	}

	private void OnGameRequestsAccepted(string data) {
		List<GPGameRequest> acceptedList =  new List<GPGameRequest>();

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		for(int i = 0; i < storeData.Length; i+=6) {
			if(storeData[i] == AndroidNative.DATA_EOF) {
				break;
			}
			
			GPGameRequest r = new GPGameRequest();
			r.id = storeData[i];
			r.playload = storeData[i +1];
			
			r.expirationTimestamp 	 = System.Convert.ToInt64(storeData[i + 2]);
			r.creationTimestamp		 = System.Convert.ToInt64(storeData[i + 3]);
			
			r.sender = storeData[i + 4];
			r.type = (GPGameRequestType) System.Convert.ToInt32(storeData[i + 5]);

			acceptedList.Add(r);
			
		}

		ActionGameRequestsAccepted(acceptedList);
		dispatch(GAME_REQUESTS_ACCEPTED, acceptedList);
	}

	private void OnAccountsLoaded(string data) {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		_deviceGoogleAccountList.Clear();

		foreach(string acc in storeData) {
			if(acc != AndroidNative.DATA_EOF) {
				_deviceGoogleAccountList.Add(acc);
			}
		}

		ActionAvaliableDeviceAccountsLoaded(_deviceGoogleAccountList);
		dispatch(AVALIABLE_DEVICE_ACCOUNTS_LOADED, _deviceGoogleAccountList);
	}

	private void OnTockenLoaded(string tocken) {
		_loadedAuthTocken = tocken;

		ActionOAuthTockenLoaded(_loadedAuthTocken);
		dispatch(OAUTH_TOCKEN_LOADED, _loadedAuthTocken);
	}


	//--------------------------------------
	// PRIVATE METHODS
	//--------------------------------------

	private void AddPlayer(GooglePlayerTemplate p) {
		if(!_players.ContainsKey(p.playerId)) {
			_players.Add(p.playerId, p);
		}
	}

}
