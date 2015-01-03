using UnityEngine;
using System.Collections;

public class gHandClass : MonoBehaviour {

	private static GameObject hand;
	private static string level;

	// Use this for initialization
	void Start () {
		hand = GameObject.Find("hand");
		level = Application.loadedLevelName;
		if (level == "level1" || level == "level15" || level == "level16" || level == "level19") addHand();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void addHand() {
		if (level == "level1") {
			hand.transform.position = new Vector2 (-0.4F, 0.2F);
			hand.animation.Play("hand enable");
		} else if (level == "level5") {
			hand.animation.Play("hand sluggish");
		} else if (level == "level15") {
			hand.animation.Play("hand destroyer");
		} else if (level == "level16") {
			hand.transform.position = new Vector2 (0.27F, 0.2F);
			hand.animation.Play("hand enable");
		} else if (level == "level19") {
			hand.animation.Play("hand groot");
		}
	}

	public static void delHand() {
		if (level == "level1") {
			hand.transform.position = new Vector2 (-4, 1);
			hand.animation.Stop("hand enable");
		} else if (level == "level5") {
			hand.transform.position = new Vector2 (-4, 1);
			hand.animation.Stop("hand sluggish");
		} else if (level == "level15") {
			hand.transform.position = new Vector2 (-4, 1);
			hand.animation.Stop("hand destroyer");
		} else if (level == "level16") {
			hand.transform.position = new Vector2 (-4, 1);
			hand.animation.Stop("hand enable");
		} else if (level == "level19") {
			hand.transform.position = new Vector2 (-4, 1);
			hand.animation.Stop("hand groot");
		}
	}
}
