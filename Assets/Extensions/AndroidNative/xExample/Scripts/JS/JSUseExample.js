#pragma strict

function Start () {

}

function OnGUI () {

	  if (GUI.Button(new Rect(10, 70, 200, 70), "Click")) {
	  
	  		//calling ConnectToPlaySertivce function of CSharpAPIHelper
	  		//CSharpAPIHelper should be attached to the same gameobject
	  		gameObject.SendMessage ("ConnectToPlaySertivce");
	  }
}


function PlayerConnectd() {
	Debug.Log("Player Connected Event received");
}


function PlayerDisconected() {
	Debug.Log("Player Disconected Event received");
}