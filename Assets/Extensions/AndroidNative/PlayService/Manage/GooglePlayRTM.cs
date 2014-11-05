using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GooglePlayRTM : SA_Singleton<GooglePlayRTM>  {



	public const string DATA_RECIEVED		      	= "data_recieved";
	public const string ROOM_UPDATED            	= "room_updated";


	public const string ON_CONNECTED_TO_ROOM        = "OnConnectedToRoom";
	public const string ON_DISCONNECTED_FROM_ROOM 	= "OnDisconnectedFromRoom";
	public const string ON_P2P_CONNECTED			= "OnP2PConnected";
	public const string ON_P2P_DISCONNECTED 		= "OnP2PDisconnected";
	public const string ON_PEER_DECLINED 			= "OnPeerDeclined";
	public const string ON_PEER_INVITED_TO_ROOM 	= "OnPeerInvitedToRoom";
	public const string ON_PEER_JOINED 				= "OnPeerJoined";
	public const string ON_PEER_LEFT 				= "OnPeerLeft";
	public const string ON_PEERS_CONNECTED 			= "OnPeersConnected";
	public const string ON_PEERS_DISCONNECTED 		= "OnPeersDisconnected";
	public const string ON_ROOM_AUTOMATCHING 		= "OnRoomAutoMatching";
	public const string ON_ROOM_CONNECTING 			= "OnRoomConnecting";
	public const string ON_JOINED_ROOM 				= "OnJoinedRoom";
	public const string ON_LEFT_ROOM 				= "OnLeftRoom";
	public const string ON_ROOM_CONNECTED 			= "OnRoomConnected";
	public const string ON_ROOM_CREATED 			= "OnRoomCreated";

	public const string ON_INVITATION_BOX_UI_CLOSED = "onInvitationBoxUiClosed";
	public const string ON_WATING_ROOM_INTENT_CLOSED = "OnWatingRoomIntentClosed";


	public const string ON_INVITATION_RECEIVED = "on_invitation_received";
	public const string ON_INVITATION_REMOVED = "on_invitation_removed";



	private const int BYTE_LIMIT = 256;
	private GP_RTM_Room _currentRoom = new GP_RTM_Room();
	private List<GP_RTM_Invite> _invitations =  new List<GP_RTM_Invite>();

	
	//--------------------------------------
	// INITIALIZATION
	//--------------------------------------

	void Awake() {
		DontDestroyOnLoad(gameObject);
		_currentRoom = new GP_RTM_Room();

		Debug.Log("GooglePlayRTM Created");

	}

	//--------------------------------------
	// API METHODS
	//--------------------------------------

	public void FindMatch(int minPlayers, int maxPlayers, int bitMask = 0) {
		AndroidNative.RTMFindMatch(minPlayers, maxPlayers, bitMask);
	}

	public void SendDataToAll(byte[] data, GP_RTM_PackageType sendType) {
		string dataString = ConvertByteDataToString(data);
		AndroidNative.sendDataToAll(dataString, (int) sendType);
	}
	
	public void sendDataToPlayers(byte[] data, GP_RTM_PackageType sendType, params string[] players) {
		string dataString = ConvertByteDataToString(data);
		string playersString = string.Join(AndroidNative.DATA_SPLITTER, players);
		AndroidNative.sendDataToPlayers(dataString, playersString, (int) sendType);
	}

	public void ShowWaitingRoomIntent() {
		AndroidNative.ShowWaitingRoomIntent();
	}

	public void OpenInvitationBoxUI(int minPlayers, int maxPlayers) {
		AndroidNative.InvitePlayers(minPlayers, maxPlayers);
	}

	public void LeaveRoom() {
		AndroidNative.leaveRoom();
	}


	public void AcceptInviteToRoom(string intitationId) {
		AndroidNative.acceptInviteToRoom(intitationId);
	}
	
	public void OpenInvitationInBoxUI()  {
		AndroidNative.showInvitationBox();
	}



	//--------------------------------------
	// GET / SET
	//--------------------------------------

	public GP_RTM_Room currentRoom {
		get {
			return _currentRoom;
		}
	}

	public List<GP_RTM_Invite> invitations {
		get {
			return _invitations;
		}
	}

	//--------------------------------------
	// EVENTS
	//--------------------------------------

	private void OnWatingRoomIntentClosed(string data) {

		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		AndroidActivityResult result =  new AndroidActivityResult(storeData[0], storeData[1]);


		dispatch(ON_WATING_ROOM_INTENT_CLOSED,  result);
	}

	private void OnRoomUpdate(string data) {


		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		_currentRoom =  new GP_RTM_Room();
		_currentRoom.id = storeData[0];
		_currentRoom.creatorId = storeData[1];

		string[] ids = storeData[2].Split(","[0]);

		for(int i = 0; i < ids.Length; i += 3) {
			if(ids[i] == AndroidNative.DATA_EOF) {
				break;
			}

			GP_Partisipant p =  new GP_Partisipant(ids[i], ids[i + 1], ids[i + 2]);
			_currentRoom.AddPartisipant(p);
		}




		_currentRoom.status =  (GP_RTM_RoomStatus) System.Convert.ToInt32(storeData[3]);
		_currentRoom.creationTimestamp = System.Convert.ToInt64(storeData[4]);

		Debug.Log("GooglePlayRTM OnRoomUpdate Room State: " + _currentRoom.status.ToString());

		dispatch(ROOM_UPDATED, _currentRoom);

	}


	private void OnMatchDataRecieved(string data) {
		if(data.Equals(string.Empty)) {
			Debug.Log("OnMatchDataRecieved, no data avaiable");
			return;
		}
		
		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		GP_RTM_Network_Package package = new GP_RTM_Network_Package (storeData[0], storeData [1]);
		
		dispatch (DATA_RECIEVED, package);
		Debug.Log ("GooglePlayManager -> DATA_RECEIVED");
	}


	private void OnWatingRoomIntentClosed() {

	}
	
	private void OnConnectedToRoom(string data) {
		dispatch (ON_CONNECTED_TO_ROOM);
	}
	
	private void OnDisconnectedFromRoom(string data) {
		dispatch (ON_DISCONNECTED_FROM_ROOM);
	}
	
	private void OnP2PConnected(string participantId) {
		dispatch (ON_P2P_CONNECTED, participantId);
	}
	
	private void OnP2PDisconnected(string participantId) {
		dispatch (ON_P2P_DISCONNECTED, participantId);
	}

	private void OnPeerDeclined(string data) {
		dispatch (ON_PEER_DECLINED, data.Split(","[0]));
	}
	
	private void OnPeerInvitedToRoom(string data) {
		dispatch (ON_PEER_INVITED_TO_ROOM, data.Split(","[0]));
	}
	
	private void OnPeerJoined(string data) {
		dispatch (ON_PEER_JOINED, data.Split(","[0]));
	}
	
	private void OnPeerLeft(string data) {
		dispatch (ON_PEER_LEFT, data.Split(","[0]));
	}
	
	private void OnPeersConnected(string data) {
		dispatch (ON_PEERS_CONNECTED, data.Split(","[0]));
	}
	
	private void OnPeersDisconnected(string data) {
		dispatch (ON_PEERS_DISCONNECTED, data.Split(","[0]));
	}
	
	private void OnRoomAutoMatching(string data) {
		dispatch (ON_ROOM_AUTOMATCHING);
	}
	
	private void OnRoomConnecting(string data) {
		dispatch (ON_ROOM_CONNECTING);
	}
		
	private void OnJoinedRoom(string data) {
		dispatch (ON_JOINED_ROOM, (GP_GamesStatusCodes)Convert.ToInt32(data));
	}
	
	private void OnLeftRoom(string data) {
		Debug.Log("OnLeftRoom Created OnRoomUpdate");
		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);
		GP_RTM_Result package = new GP_RTM_Result (storeData[0], storeData [1]);


		_currentRoom =  new GP_RTM_Room();
		dispatch(ROOM_UPDATED, _currentRoom);

		dispatch (ON_LEFT_ROOM, package);
	}
	
	private void OnRoomConnected(string data) {
		dispatch (ON_ROOM_CONNECTED, (GP_GamesStatusCodes)Convert.ToInt32(data));
	}
	
	private void OnRoomCreated(string data) {
		dispatch (ON_ROOM_CREATED, (GP_GamesStatusCodes)Convert.ToInt32(data));
	}

	private void OnInvitationBoxUiClosed(string data) {

		string[] storeData = data.Split(AndroidNative.DATA_SPLITTER [0]);

		AndroidActivityResult result =  new AndroidActivityResult(storeData[0], storeData[1]);
	
		
		dispatch(ON_INVITATION_BOX_UI_CLOSED,  result);
	}

	private void OnInvitationReceived(string invitationId) {
	

		GP_RTM_Invite inv =  new GP_RTM_Invite();
		inv.id = invitationId;
		_invitations.Add(inv);
		dispatch(ON_INVITATION_RECEIVED, invitationId);

	}

	private void OnInvitationRemoved(string invitationId) {
		foreach(GP_RTM_Invite inv in _invitations) {
			if(inv.id.Equals(invitationId)) {
				_invitations.Remove(inv);
				return;
			}
		}

		dispatch(ON_INVITATION_REMOVED, invitationId);
	}

	//--------------------------------------
	// STATIC
	//--------------------------------------

	public static byte[] ConvertStringToByteData(string data) {

	#if UNITY_ANDROID
		if(data == null) {
			return null;
		}
		
		data = data.Replace(AndroidNative.DATA_EOF, string.Empty);
		if(data.Equals(string.Empty)) {
			return null;
		}
		
		string[] array = data.Split("," [0]);
		
		List<byte> listOfBytes = new List<byte> ();
		foreach(string str in array) {
			int param = System.Convert.ToInt32(str);
			int temp_param = param < 0 ? BYTE_LIMIT + param : param;
			listOfBytes.Add (System.Convert.ToByte(temp_param));
		}
		
		return listOfBytes.ToArray ();


	#else
		return new byte[]{};
	#endif

	}

	public static string ConvertByteDataToString(byte[] data) {
		
		string b = "";
		for(int i = 0; i < data.Length; i++) {
			if(i != 0) {
				b += ",";
			}
			
			b += data[i].ToString();
		}

		return b;
		
	}





}
