////////////////////////////////////////////////////////////////////////////////
//  
// @module Android Native Plugin for Unity3D 
// @author Osipov Stanislav (Stan's Assets) 
// @support stans.assets@gmail.com 
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class GoogleCloudMessageService : SA_Singleton<GoogleCloudMessageService> {
	
	public const string CLOUD_MESSAGE_SERVICE_REGISTRATION_FAILED = "cloud_message_service_registration_failed";
	public const string CLOUD_MESSAGE_SERVICE_REGISTRATION_RECIVED = "cloud_message_service_registration_recived";


	public const string CLOUD_MESSAGE_LOADED = "cloud_message_loaded";


	private string _lastMessage = string.Empty;
	private string _registrationId = string.Empty;


	
	
	
	//--------------------------------------
	// INITIALIZE
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
	}
	
	public void RgisterDevice() {
		AndroidNative.GCMRgisterDevice(AndroidNativeSettings.Instance.GCM_SenderId);
	}

	//--------------------------------------
	// PUBLIC METHODS
	//--------------------------------------

	public void LoadLastMessage() {
		AndroidNative.GCMLoadLastMessage();
	}
	
	//--------------------------------------
	// GET / SET
	//--------------------------------------
	
	public string registrationId {
		get {
			return _registrationId;
		}
	}

	public string lastMessage {
		get {
			return _lastMessage;
		}
	}
	
	
	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void OnLastMessageLoaded(string data) {
		_lastMessage = data;
		dispatch(CLOUD_MESSAGE_LOADED, lastMessage);

	}


	
	private void OnRegistrationReviced(string regId) {
		_registrationId = regId;
		dispatch(CLOUD_MESSAGE_SERVICE_REGISTRATION_RECIVED, regId);
	}
	
	private void OnRegistrationFailed() {
		dispatch(CLOUD_MESSAGE_SERVICE_REGISTRATION_FAILED);
	}
	
	
	
}
