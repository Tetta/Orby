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
			if (Mathf.Round((Time.time - time) * 10) == Mathf.Round(actions[counter].time * 10)) {
				hintState = "pause";
				Time.timeScale = 0;
				hint.transform.position = actions[counter].id;
				//Debug.Log(123);
			}

		}


		//if (Input.GetMouseButtonDown(0)) {

			//Debug.Log( Time.time);


			//web.SendMessage("OnMouseDown");
			//destroyer.SendMessage("doOnMouseUp");
			//Debug.Log(destroyer.GetInstanceID());
			
			
		//}
	}
	void OnMouseUp() {
		Application.LoadLevel(Application.loadedLevel);
		actions = new action[2];

		actions[0].id = new Vector3(-1.6338F, 1.9594F, 0F);
		actions[0].time = 2.099313F;
		actions[0].mouse = new Vector3(206, 251, 0);
		actions[1].id = new Vector3(1.3F, 2F, 0F);
		actions[1].time = 1.062672F;
		actions[1].mouse = new Vector3(163, 225, 0);

		hintState = "start";
		time = Time.time;
		counter = 0;
	}

	public static Vector3 checkHint(GameObject obj, bool flag = false) {
		Time.timeScale = 1;
		if (gHintClass.hintState == "start") {
			gHintClass.hintState = "";
			hint.transform.position = new Vector3(-4, 0, 0);
		}

		if (hintState == "pause") { 
			if (actions[counter].id == obj.transform.position) {
				if (flag) return actions[counter].mouse;
				hint.transform.position = new Vector3(-4, 0, 0);
				obj.SendMessage("OnMouseDrag");
				obj.SendMessage("OnMouseUp");
				counter ++;
				time = Time.time;
				hintState = "start";
			} else {
				gHintClass.hintState = "";
				hint.transform.position = new Vector3(-4, 0, 0);
			}
		}

		return new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z);
	}
	
	public struct action {
		public Vector3 id;
		public Vector3 mouse;
		public float time;
	}
}
