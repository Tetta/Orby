#pragma strict

function Start () {
}

function Update () {
}
function OnClick () {
	Application.Quit();
	Debug.Log("exitGame");
}

function OnKey (key:KeyCode) {
	//Application.Quit();
	Debug.Log(key);
}
