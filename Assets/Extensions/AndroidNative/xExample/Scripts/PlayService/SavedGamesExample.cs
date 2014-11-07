using UnityEngine;
using UnionAssets.FLE;
using System.Collections;

public class SavedGamesExample : MonoBehaviour {
	


	public GameObject avatar;
	private Texture defaulttexture;
	
	public DefaultPreviewButton connectButton;
	public SA_Label playerLabel;
	
	public DefaultPreviewButton[] ConnectionDependedntButtons;

	

	


	void Start() {
		
		playerLabel.text = "Player Diconnected";
		defaulttexture = avatar.GetComponent<Renderer>().material.mainTexture;
		
		//listen for GooglePlayConnection events
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
		GooglePlayConnection.instance.addEventListener(GooglePlayConnection.CONNECTION_RESULT_RECEIVED, OnConnectionResult);
		
		GooglePlaySavedGmaesManager.ActionNewGameSaveRequest += ActionNewGameSaveRequest;
		GooglePlaySavedGmaesManager.ActionGameSaveLoaded += ActionGameSaveLoaded;
		GooglePlaySavedGmaesManager.ActionConflict += ActionConflict;

		
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			//checking if player already connected
			OnPlayerConnected ();
		} 
		
	}

	void OnDestroy() {
		if(GooglePlayConnection.HasInstance) {
			GooglePlayConnection.instance.removeEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
			GooglePlayConnection.instance.removeEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
			GooglePlayConnection.instance.addEventListener(GooglePlayConnection.CONNECTION_RESULT_RECEIVED, OnConnectionResult);
			
		}

		if(GooglePlaySavedGmaesManager.HasInstance) {
			GooglePlaySavedGmaesManager.ActionNewGameSaveRequest -= ActionNewGameSaveRequest;
			GooglePlaySavedGmaesManager.ActionGameSaveLoaded -= ActionGameSaveLoaded;
			GooglePlaySavedGmaesManager.ActionConflict -= ActionConflict;
		}
		
	}


	
	private void ConncetButtonPress() {
		Debug.Log("GooglePlayManager State  -> " + GooglePlayConnection.state.ToString());
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			SA_StatusBar.text = "Disconnecting from Play Service...";
			GooglePlayConnection.instance.disconnect ();
		} else {
			SA_StatusBar.text = "Connecting to Play Service...";
			GooglePlayConnection.instance.connect ();
		}
	}
	

	
	

	
	void FixedUpdate() {
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			if(GooglePlayManager.instance.player.icon != null) {
				avatar.GetComponent<Renderer>().material.mainTexture = GooglePlayManager.instance.player.icon;
			}
		}  else {
			avatar.GetComponent<Renderer>().material.mainTexture = defaulttexture;
		}
		
		
		string title = "Connect";
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			title = "Disconnect";
			
			foreach(DefaultPreviewButton btn in ConnectionDependedntButtons) {
				btn.EnabledButton();
			}
			
			
		} else {
			foreach(DefaultPreviewButton btn in ConnectionDependedntButtons) {
				btn.DisabledButton();
				
			}
			if(GooglePlayConnection.state == GPConnectionState.STATE_DISCONNECTED || GooglePlayConnection.state == GPConnectionState.STATE_UNCONFIGURED) {
				
				title = "Connect";
			} else {
				title = "Connecting..";
			}
		}
		
		connectButton.text = title;
	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------



	private void ShowSavedGamesUI() {
		int maxNumberOfSavedGamesToShow = 5;
		GooglePlaySavedGmaesManager.instance.ShowSavedGamesUI("See My Saves", maxNumberOfSavedGamesToShow);
	}



	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void ActionNewGameSaveRequest () {
		SA_StatusBar.text = "New  Game Save Requested, Creating new save..";
		Debug.Log("New  Game Save Requested, Creating new save..");
		StartCoroutine(MakeScreenshotAndSaveGameData());
	}

	private void ActionGameSaveLoaded (GP_SpanshotLoadResult result) {

		Debug.Log("ActionGameSaveLoaded: " + result.message);
		if(result.isSuccess) {

			Debug.Log("Snapshot.Title: " 					+ result.Snapshot.Title);
			Debug.Log("Snapshot.Description: " 				+ result.Snapshot.Description);
			Debug.Log("Snapshot.CoverImageUrl): " 			+ result.Snapshot.CoverImageUrl);
			Debug.Log("Snapshot.stringData: " 				+ result.Snapshot.stringData);
			Debug.Log("Snapshot.LastModifiedTimestamp: " 	+ result.Snapshot.LastModifiedTimestamp);
			Debug.Log("Snapshot.bytes.Length: " 			+ result.Snapshot.bytes.Length);
		} 

		SA_StatusBar.text = "Games Loaded: " + result.message;

	}

	private void ActionGameSaveResult (GooglePlayResult result) {
		GooglePlaySavedGmaesManager.ActionGameSaveResult -= ActionGameSaveResult;
		Debug.Log("ActionGameSaveResult: " + result.message);

		if(result.isSuccess) {
			SA_StatusBar.text = "Games Saved.";
		} else {
			SA_StatusBar.text = "Games Save Failed";
		}
	}	

	private void ActionConflict (GP_SnapshotConflict result) {

		Debug.Log("Conflict Detected: ");

		GP_Snapshot snapshot = result.Snapshot;
		GP_Snapshot conflictSnapshot = result.ConflictingSnapshot;
		
		// Resolve between conflicts by selecting the newest of the conflicting snapshots.
		GP_Snapshot mResolvedSnapshot = snapshot;
		
		if (snapshot.LastModifiedTimestamp < conflictSnapshot.LastModifiedTimestamp) {
			mResolvedSnapshot = conflictSnapshot;
		}

		result.Resolve(mResolvedSnapshot);
	}


	private void OnPlayerDisconnected() {
		SA_StatusBar.text = "Player Diconnected";
		playerLabel.text = "Player Diconnected";
	}
	
	private void OnPlayerConnected() {
		SA_StatusBar.text = "Player Connected";
		playerLabel.text = GooglePlayManager.instance.player.name;
	}
	
	private void OnConnectionResult(CEvent e) {
		
		GooglePlayConnectionResult result = e.data as GooglePlayConnectionResult;
		SA_StatusBar.text = "ConnectionResul:  " + result.code.ToString();
		Debug.Log(result.code.ToString());
	}



	//--------------------------------------
	// PRIVATE METHODS
	//--------------------------------------
	
	private IEnumerator MakeScreenshotAndSaveGameData() {
		
		
		yield return new WaitForEndOfFrame();
		// Create a texture the size of the screen, RGB24 format
		int width = Screen.width;
		int height = Screen.height;
		Texture2D Screenshot = new Texture2D( width, height, TextureFormat.RGB24, false );
		// Read screen contents into the texture
		Screenshot.ReadPixels( new Rect(0, 0, width, height), 0, 0 );
		Screenshot.Apply();
		
		
		string currentSaveName =  "snapshotTemp-" + Random.Range(1, 281).ToString();
		string description  = "Modified data at: " + System.DateTime.Now.ToString("MM/dd/yyyy H:mm:ss");


		GooglePlaySavedGmaesManager.ActionGameSaveResult += ActionGameSaveResult;
		GooglePlaySavedGmaesManager.instance.CreateNewSpanShot(currentSaveName, description, Screenshot, "some save data, for example you can use JSON or byte array");
		
		
		
		Destroy(Screenshot);
	}


}
