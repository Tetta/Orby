using UnityEngine;
using System.Collections;

public class gSpiderClass : MonoBehaviour {

	private GameObject[] guiStars = new GameObject[3];
	private GameObject restart;
	//private GameObject completeMenu;
	//private GameObject berry;
	// Use this for initialization
	void Start () {
		guiStars[0] = GameObject.Find("gui star 1");
		guiStars[1] = GameObject.Find("gui star 2");
		guiStars[2] = GameObject.Find("gui star 3");	
		restart = GameObject.Find("restart");
		//berry = GameObject.Find("berry");
		//completeMenu = GameObject.Find("gui").transform.Find("complete menu").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x < -4 || transform.position.x > 4 || transform.position.y < -6 || transform.position.y > 6) restart.SendMessage("OnClick");
		//if (completeMenu.activeSelf) transform.position = berry.transform.position;
	}

	void OnTriggerEnter2D(Collider2D collisionObject) {
		if (collisionObject.gameObject.name == "star") {
			Destroy(collisionObject.gameObject);
			guiStars[gBerryClass.starsCounter].GetComponent<UISprite>().color =  new Color(1, 1, 1, 1);
			gBerryClass.starsCounter ++;
		}
		
	}

	void OnClick () {
		transform.GetChild(0).GetComponent<Animator>().Play("spider jump", -1);
	}
}
