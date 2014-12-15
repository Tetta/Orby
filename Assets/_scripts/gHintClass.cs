using UnityEngine;
using System.Collections;


public class gHintClass : MonoBehaviour {


	public static int counter = 0;
	public static action[] actions;
	public static float time;
	public static string hintState = "";

	private static GameObject hint;

	// Use this for initialization
	void Start () {
		hint = GameObject.Find("hint");
	}
	
	// Update is called once per frame
	void Update () {
		if (hintState == "start" && counter <= actions.Length - 1) {

			//Debug.Log(Time.time - time);
			hint.transform.position = new Vector3(-4, 0, 0);
			if (Mathf.Round((Time.unscaledTime - time) * 10) == Mathf.Round(actions[counter].time * 10)) {
				hintState = "pause";
				Time.timeScale = 0;
				hint.transform.position = actions[counter].id;
				//Debug.Log(123);
			}

		}

	}
	void OnMouseUp() {
		Application.LoadLevel(Application.loadedLevel);
		SendMessage(Application.loadedLevelName); 


		Time.timeScale = 1;
		hintState = "start";
		time = Time.unscaledTime;
		counter = 0;
	}

	public static Vector3 checkHint(GameObject obj, bool flag = false) {
		Time.timeScale = 1;

		if (hintState == "pause") { 
			if (actions[counter].id == obj.transform.position) {
				if (flag) return actions[counter].mouse;
				hint.transform.position = new Vector3(-4, 0, 0);

				if (obj.name == "destroyer" || obj.name == "sluggish" || obj.name == "groot") {
					obj.SendMessage("OnMouseDrag");
					obj.SendMessage("OnMouseUp");
				}
				counter ++;
				time = Time.unscaledTime;
				hintState = "start";
			} else {
				gHintClass.hintState = "";
				hint.transform.position = new Vector3(-4, 0, 0);
			} 
		}	else if (hintState == "start" && !flag) {
			hintState = "";
			hint.transform.position = new Vector3(-4, 0, 0);
		}


		return new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
	}
	
	public struct action {
		public Vector3 id;
		public Vector3 mouse;
		public float time;
	}

	void level1 () {
		actions = new action[2];
		actions[0].id = new Vector3(0.051342F, 0.49288F, 0F);
		actions[0].time = 0.1F;
		actions[0].mouse = new Vector3(209, 386, 0);
		actions[1].id = new Vector3(0.051342F, 0.49288F, 0F);
		actions[1].time = 1.094343F;
		actions[1].mouse = new Vector3(209, 386, 0);
	}

	void level20 () {
		actions = new action[8];
		actions[0].id = new Vector3(-1.707522F, -1.798797F, -0.1F);
		actions[0].time = 1.49423F;
		actions[0].mouse = new Vector3(85, 205, 0);
		actions[1].id = new Vector3(0.051342F, 0.49288F, 0F);
		actions[1].time = 0.6248536F;
		actions[1].mouse = new Vector3(199, 379, 0);
		actions[2].id = new Vector3(-2.2448F, 1.1385F, 0F);
		actions[2].time = 1.624653F;
		actions[2].mouse = new Vector3(53, 413, 0);
		actions[3].id = new Vector3(-1.6338F, 1.9594F, 0F);
		actions[3].time = 1.689129F;
		actions[3].mouse = new Vector3(282, 196, 0);
		actions[4].id = new Vector3(1.3F, 2F, 0F);
		actions[4].time = 1.248973F;
		actions[4].mouse = new Vector3(192, 265, 0);
		actions[5].id = new Vector3(2.053608F, 0.27878F, 0F);
		actions[5].time = 0.9984751F;
		actions[5].mouse = new Vector3(366, 371, 0);
		actions[6].id = new Vector3(2.053608F, 0.27878F, 0F);
		actions[6].time = 0.6565351F;
		actions[6].mouse = new Vector3(365, 371, 0);
		actions[7].id = new Vector3(0.051342F, 0.49288F, 0F);
		actions[7].time = 1.199567F;
		actions[7].mouse = new Vector3(208, 379, 0);
	}

	void level75 () {
		actions = new action[3];
		actions[0].id = new Vector3(2.1232F, 0.1807F, 0F);
		actions[0].time = 0.644765F;
		actions[0].mouse = new Vector3(321, 329, 0);
		actions[1].id = new Vector3(-1.026575F, 0.7992552F, -0.1F);
		actions[1].time = 1.100898F;
		actions[1].mouse = new Vector3(125, 376, 0);
		actions[2].id = new Vector3(2.1232F, 0.1807F, 0F);
		actions[2].time = 1.292115F;
		actions[2].mouse = new Vector3(351, 363, 0);
	}
}
