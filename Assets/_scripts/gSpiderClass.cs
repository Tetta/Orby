using UnityEngine;
using System.Collections;

public class gSpiderClass : MonoBehaviour {

	private GameObject[] guiStars = new GameObject[3];
	// Use this for initialization
	void Start () {
		guiStars[0] = GameObject.Find("gui star 1");
		guiStars[1] = GameObject.Find("gui star 2");
		guiStars[2] = GameObject.Find("gui star 3");	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D collisionObject) {
		if (collisionObject.gameObject.name == "star") {
			Destroy(collisionObject.gameObject);
			guiStars[gBerryClass.starsCounter].GetComponent<UISprite>().color =  new Color(1, 1, 1, 1);
			gBerryClass.starsCounter ++;
		}
		
	}
}
