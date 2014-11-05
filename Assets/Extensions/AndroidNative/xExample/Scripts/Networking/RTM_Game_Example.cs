//#define SA_DEBUG_MODE
using UnityEngine;
using System.Collections;

public class RTM_Game_Example : AndroidNativeExampleBase {


	public GameObject avatar;
	public GameObject hi;
	public SA_Label playerLabel;
	public SA_Label gameState;
	public SA_Label parisipants;

	public DefaultPreviewButton connectButton;


	public DefaultPreviewButton helloButton;
	public DefaultPreviewButton leaveRoomButton;
	public DefaultPreviewButton showRoomButton;


	public DefaultPreviewButton[] ConnectionDependedntButtons;
	public SA_PartisipantUI[] patrisipants;


	private Texture defaulttexture;


	void Start() {
		
		playerLabel.text = "Player Diconnected";
		defaulttexture = avatar.renderer.material.mainTexture;
		
		//listen for GooglePlayConnection events
		GooglePlayRTM.instance.addEventListener (GooglePlayRTM.ON_INVITATION_RECEIVED, OnInvite);
		GooglePlayRTM.instance.addEventListener (GooglePlayRTM.ON_ROOM_CREATED, OnRoomCreated);


		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
		GooglePlayConnection.instance.addEventListener(GooglePlayConnection.CONNECTION_RESULT_RECEIVED, OnConnectionResult);

		
		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			//checking if player already connected
			OnPlayerConnected ();
		} 

		//networking eventd
		GooglePlayRTM.instance.addEventListener(GooglePlayRTM.DATA_RECIEVED, OnGCDataReceived);


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

	
	private void ShowWatingRoom() {
		GooglePlayRTM.instance.ShowWaitingRoomIntent();
	}


	private void findMatch() {
		GooglePlayRTM.instance.FindMatch(1, 2);
	}

	private void InviteFriends() {
		GooglePlayRTM.instance.OpenInvitationBoxUI(1, 2);
	}


	private void SendHello() {
		#if (UNITY_ANDROID && !UNITY_EDITOR) || SA_DEBUG_MODE
		string msg = "hello world";
		System.Text.UTF8Encoding  encoding = new System.Text.UTF8Encoding();
		byte[] data = encoding.GetBytes(msg);

		GooglePlayRTM.instance.SendDataToAll(data, GP_RTM_PackageType.RELIABLE);
		#endif

	}

	private void LeaveRoom() {
		GooglePlayRTM.instance.LeaveRoom();
	}



	private void DrawPartisipants() {

		UpdateGameState("Room State: " + GooglePlayRTM.instance.currentRoom.status.ToString());
		parisipants.text = "Total Room Partisipants: " + GooglePlayRTM.instance.currentRoom.partisipants.Count;
			


		foreach(SA_PartisipantUI p in patrisipants) {
			p.gameObject.SetActive(false);
		}
		
		int i = 0;
		foreach(GP_Partisipant p in GooglePlayRTM.instance.currentRoom.partisipants) {
			patrisipants[i].gameObject.SetActive(true);
			patrisipants[i].SetPartisipant(p);
			i++;
		}
	

	}

	private void UpdateGameState(string msg) {
		gameState.text = msg;
	}

	void FixedUpdate() {
		DrawPartisipants();

		if(GooglePlayRTM.instance.currentRoom.status!= GP_RTM_RoomStatus.ROOM_VARIANT_DEFAULT && GooglePlayRTM.instance.currentRoom.status!= GP_RTM_RoomStatus.ROOM_STATUS_ACTIVE) {
			showRoomButton.EnabledButton();
		} else {
			showRoomButton.DisabledButton();
		}



		if(GooglePlayRTM.instance.currentRoom.status == GP_RTM_RoomStatus.ROOM_VARIANT_DEFAULT) {
			leaveRoomButton.DisabledButton();
		} else {
			leaveRoomButton.EnabledButton();
		}

		if(GooglePlayRTM.instance.currentRoom.status == GP_RTM_RoomStatus.ROOM_STATUS_ACTIVE) {
			helloButton.EnabledButton();
			hi.SetActive(true);
		} else {
			helloButton.DisabledButton();
			hi.SetActive(false);
		}

		if(GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) {
			if(GooglePlayManager.instance.player.icon != null) {
				avatar.renderer.material.mainTexture = GooglePlayManager.instance.player.icon;
			}
		}  else {
			avatar.renderer.material.mainTexture = defaulttexture;
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

	private string invitationId;
	private void OnInvite(CEvent e) {
		invitationId = (string) e.data;

		AndroidDialog dialog =  AndroidDialog.Create("Invite", "You have new invite from firend", "Start Playing", "Open Inbox");
		dialog.addEventListener(BaseEvent.COMPLETE, OnInvDialogComplete);


	}

	private void OnRoomCreated(CEvent e) {
		GP_GamesStatusCodes code = (GP_GamesStatusCodes) e.data;
		

		SA_StatusBar.text = "Room Create Result:  " + code.ToString();
		
	}



	private void OnInvDialogComplete(CEvent e) {
		
		//removing listner
		(e.dispatcher as AndroidDialog).removeEventListener(BaseEvent.COMPLETE, OnInvDialogComplete);
		
		//parsing result
		switch((AndroidDialogResult)e.data) {
		case AndroidDialogResult.YES:
			GooglePlayRTM.instance.AcceptInviteToRoom(invitationId);
			break;
		case AndroidDialogResult.NO:
			GooglePlayRTM.instance.OpenInvitationInBoxUI();
			break;
			
		}
	}


	private void OnGCDataReceived(CEvent e) {
		#if (UNITY_ANDROID && !UNITY_EDITOR) || SA_DEBUG_MODE

		GP_RTM_Network_Package package = e.data as GP_RTM_Network_Package;
		
		System.Text.UTF8Encoding enc = new System.Text.UTF8Encoding();
		string str = enc.GetString(package.buffer);


		string name = package.participantId;

	
		GP_Partisipant p =  GooglePlayRTM.instance.currentRoom.GetPartisipantById(package.participantId);
		if(p != null) {
			GooglePlayerTemplate player = GooglePlayManager.instance.GetPlayerById(p.playerId);
			if(player != null) {
				name = player.name;
			}
		}

		AndroidMessage.Create("Data Eeceived", "player " + name + " \n " + "data: " + str);

		#endif
		
	}



}
