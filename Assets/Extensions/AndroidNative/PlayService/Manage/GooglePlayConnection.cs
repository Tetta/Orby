////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;


public class GooglePlayConnection : SA_Singleton<GooglePlayConnection> {
	
	private bool _isInitialized = false;


	public const string CONNECTION_STATE_CHANGED        = "connection_state_changed"; 
	public const string CONNECTION_RESULT_RECEIVED      = "connection_result_received"; 
	public const string PLAYER_CONNECTED       			= "player_connected";
	public const string PLAYER_DISCONNECTED   			= "player_disconnected";


	private static GPConnectionState _state = GPConnectionState.STATE_UNCONFIGURED;


	//--------------------------------------
	// INITIALIZE
	//--------------------------------------


	void Awake() {
		DontDestroyOnLoad(gameObject);
	}


	//--------------------------------------
	// PUBLIC API CALL METHODS
	//--------------------------------------

	private void init() {
		string connectionString = "";
		if(AndroidNativeSettings.Instance.EnableGamesAPI) {
			connectionString += "GamesAPI";
		}

		if(AndroidNativeSettings.Instance.EnableAppStateAPI) {
			connectionString += "AppStateAPI";
		}

		AndroidNative.playServiceInit(connectionString);

		_isInitialized = true;
	}

	public void connect()  {
		connect(null);
	}

	public void connect(string accountName) {

		if(_state == GPConnectionState.STATE_CONNECTED || _state == GPConnectionState.STATE_CONNECTING) {
			return;
		}

		OnStateChange(GPConnectionState.STATE_CONNECTING);
		if(!_isInitialized) {
			GooglePlayManager.instance.Create();
			init();
		}

		if(accountName != null) {
			AndroidNative.playServiceConnect (accountName);
		} else {
			AndroidNative.playServiceConnect ();
		}

	}

	public void disconnect() {

		if(_state == GPConnectionState.STATE_DISCONNECTED || _state == GPConnectionState.STATE_CONNECTING) {
			return;
		}

		OnStateChange(GPConnectionState.STATE_DISCONNECTED);
		AndroidNative.playServiceDisconnect ();

	}


	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------


	public static bool CheckState() {
		switch(_state) {
			case GPConnectionState.STATE_CONNECTED:
			return true;
			case GPConnectionState.STATE_DISCONNECTED:
			instance.connect ();
			return false;
			default:
			return false;
		}
	}



	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public static GPConnectionState state {
		get {
			return _state;
		}
	}


	public  bool isInitialized {
		get {
			return _isInitialized;
		}
	}




	//--------------------------------------
	// EVENTS
	//--------------------------------------
	


	private void OnConnectionResult(string data) {
		string[] res;
		res = data.Split(AndroidNative.DATA_SPLITTER [0]);
		GooglePlayConnectionResult result = new GooglePlayConnectionResult();
		result.code = (GP_ConnectionResultCode) System.Convert.ToInt32(res[0]);



		if(System.Convert.ToInt32(res[1]) == 1) {
			result.HasResolution = true;
		} else {
			result.HasResolution = false;
		}


		if(result.IsSuccess) {
			OnStateChange(GPConnectionState.STATE_CONNECTED);
		} else {
			if(!result.HasResolution) {
				OnStateChange(GPConnectionState.STATE_DISCONNECTED);
			}
		}


		dispatch(CONNECTION_RESULT_RECEIVED, result);

	}


	private void OnStateChange(GPConnectionState connectionState) {

		_state = connectionState;
		switch(_state) {
			case GPConnectionState.STATE_CONNECTED:
				dispatch(PLAYER_CONNECTED);
				break;
			case GPConnectionState.STATE_DISCONNECTED:
				dispatch(PLAYER_DISCONNECTED);
				break; 
		}

		dispatch(CONNECTION_STATE_CHANGED, _state);

		Debug.Log("Play Serice Connection State -> " + _state.ToString());
	}

	


}
