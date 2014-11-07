using UnityEngine;
using System;
using System.Collections;

public class GooglePlaySavedGmaesManager :  SA_Singleton<GooglePlaySavedGmaesManager> {


	//Events
	public const string NEW_GAME_SAVE_REQUES   	= "new_game_save_reques";
	public const string GAME_SAVE_RESULT   		= "game_save_result";

	public const string GAME_SAVE_LOADED  	 	= "new_game_save_reques";
	public const string CONFLICT  			 	= "conflict";


	
	//Actions
	public static Action ActionNewGameSaveRequest 	= delegate {};
	public static Action<GooglePlayResult> ActionGameSaveResult 	= delegate {};

	public static Action<GP_SpanshotLoadResult> ActionGameSaveLoaded 	= delegate {};
	public static Action<GP_SnapshotConflict> ActionConflict 	= delegate {};






	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	//--------------------------------------
	// PUBLIC API CALL METHODS
	//--------------------------------------

	public void ShowSavedGamesUI(string title, int maxNumberOfSavedGamesToShow)  {
		if (!GooglePlayConnection.CheckState ()) { return; }
		AndroidNative.ShowSavedGamesUI(title, maxNumberOfSavedGamesToShow);
	}


	public void CreateNewSpanShot(string name, string description, Texture2D coverImage, string spanshotData)  {
		CreateNewSpanShot(name, description, coverImage, GetBytes(spanshotData));
	}


	public void CreateNewSpanShot(string name, string description, Texture2D coverImage, byte[] spanshotData)  {
		string mdeia = string.Empty;

		if(coverImage != null) {
			byte[] val = coverImage.EncodeToPNG();
			mdeia = System.Convert.ToBase64String (val);
		}  else {
			Debug.LogWarning("GooglePlaySavedGmaesManager::CreateNewSpanShot:  coverImage is null");
		}

		string data = System.Convert.ToBase64String (spanshotData);

		AndroidNative.CreateNewSpanShot(name, description, mdeia, data);
	}


	//--------------------------------------
	// PRIVATE  METHODS
	//--------------------------------------

	private static byte[] GetBytes(string str) {
		byte[] bytes = new byte[str.Length * sizeof(char)];
		System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}


	private static string GetString(byte[] bytes) {
		char[] chars = new char[bytes.Length / sizeof(char)];
		System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
		return new string(chars);
	}

	
	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void OnSavedGamePicked(string data) {

		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);


		GP_SpanshotLoadResult result = new GP_SpanshotLoadResult (storeData [0]);
		if(result.isSuccess) {
			string Title = storeData [1];
			long LastModifiedTimestamp = System.Convert.ToInt64(storeData [2]) ;
			string Description = storeData [3];
			string CoverImageUrl = storeData [4];
			byte[] decodedFromBase64 = System.Convert.FromBase64String(storeData [5]);
			
			
			GP_Snapshot  Snapshot =  new GP_Snapshot();
			Snapshot.Title 					= Title;
			Snapshot.Description 			= Description;
			Snapshot.CoverImageUrl 			= CoverImageUrl;
			Snapshot.bytes 					= decodedFromBase64;
			Snapshot.LastModifiedTimestamp 	= LastModifiedTimestamp;
			Snapshot.stringData 			= GetString(decodedFromBase64);

			result.SetSnapShot(Snapshot);
		
		}


		dispatch(GAME_SAVE_LOADED, result);
		ActionGameSaveLoaded(result);

	}

	private void OnSaveResult(string code) {
		GooglePlayResult result = new GooglePlayResult (code);
		ActionGameSaveResult(result);
		dispatch(GAME_SAVE_RESULT, result);
	}

	private void OnConflict(string data)  {
		string[] storeData;
		storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		
		string Title = storeData [0];
		long LastModifiedTimestamp = System.Convert.ToInt64(storeData [1]) ;
		string Description = storeData [2];
		string CoverImageUrl = storeData [3];
		byte[] decodedFromBase64 = System.Convert.FromBase64String(storeData [4]);

		GP_Snapshot  Snapshot1 =  new GP_Snapshot();
		Snapshot1.Title 					= Title;
		Snapshot1.Description 			= Description;
		Snapshot1.CoverImageUrl 			= CoverImageUrl;
		Snapshot1.bytes 					= decodedFromBase64;
		Snapshot1.LastModifiedTimestamp 	= LastModifiedTimestamp;
		Snapshot1.stringData 			= GetString(decodedFromBase64);


		Title = storeData [5];
		LastModifiedTimestamp = System.Convert.ToInt64(storeData [6]) ;
		Description = storeData [7];
		CoverImageUrl = storeData [8];
		decodedFromBase64 = System.Convert.FromBase64String(storeData [9]);

		GP_Snapshot  Snapshot2 =  new GP_Snapshot();
		Snapshot2.Title 					= Title;
		Snapshot2.Description 			= Description;
		Snapshot2.CoverImageUrl 			= CoverImageUrl;
		Snapshot2.bytes 					= decodedFromBase64;
		Snapshot2.LastModifiedTimestamp 	= LastModifiedTimestamp;
		Snapshot2.stringData 			= GetString(decodedFromBase64);


		GP_SnapshotConflict result =  new GP_SnapshotConflict(Snapshot1, Snapshot2);

		dispatch(CONFLICT, result);
		ActionConflict(result);

	}

	private void OnLoadResult(string code) {
		GooglePlayResult result = new GooglePlayResult (code);
		ActionGameSaveResult(result);
		dispatch(GAME_SAVE_RESULT, result);
	}

	private void OnNewGameSaveRequest(string data) {
		dispatch(NEW_GAME_SAVE_REQUES);
		ActionNewGameSaveRequest();

	}































}
