using UnityEngine;
using System.Collections;

public class initClass : MonoBehaviour {

	//private GameObject testLabel;
	public GameObject googlePlus;
	public GameObject googlePlay;
	public GameObject closeMenu;
	public GameObject mainMenu;
	// Use this for initialization
	void Start () {
		//testLabel = GameObject.Find("test label");

		//listen for GooglePlayConnection events
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_CONNECTED, OnPlayerConnected);
		GooglePlayConnection.instance.addEventListener (GooglePlayConnection.PLAYER_DISCONNECTED, OnPlayerDisconnected);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape) && mainMenu.activeSelf)
		{
			closeMenu.SetActive(true);
		}
	}

	private void OnPlayerConnected() {
		googlePlay.SetActive(true);
		googlePlus.SetActive(false);
	}

	private void OnPlayerDisconnected() {
		GooglePlayConnection.instance.disconnect ();
		googlePlay.SetActive(false);
		googlePlus.SetActive(true);
	}
}