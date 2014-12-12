using UnityEngine;
using System.Collections;

public class gSluggishClass : MonoBehaviour {
	public GameObject line;
	private string sluggishState = "";
	private GameObject berry;

	// Use this for initialization
	void Start () {
		berry = GameObject.Find("berry");
	}
	
	// Update is called once per frame
	void Update () {
		if (sluggishState == "active") {

		}

	}

	void OnTriggerEnter2D(Collider2D collisionObject) {
		if (collisionObject.gameObject.name == "berry" && sluggishState == "") {
			berry.rigidbody2D.isKinematic = true;
			berry.transform.position = new Vector2(transform.position.x, transform.position.y);
			sluggishState = "collision";
			Debug.Log ("collision");
		}
	}

	void OnTriggerExit2D(Collider2D collisionObject) {
		if (collisionObject.gameObject.name == "berry" && sluggishState == "fly") {

			sluggishState = "";
			Debug.Log ("OnTriggerExit2D");
			
		}
	}

	void OnMouseDown() {
		if (sluggishState == "collision") {

			sluggishState = "active";
			line.SetActive(true);
		}
		
	}

	void OnMouseDrag() {
		if (sluggishState == "active") {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
			Vector3 relative = transform.InverseTransformPoint(mousePosition);
			float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
			line.transform.rotation = Quaternion.Euler(0, 0, 180 - angle);


			//line.transform.Rotate(0, 0, angle);
			//line.transform.RotateAround(Vector3.zero, Vector3.zero, angle);

		}
	}

	void OnMouseUp() {
		if (sluggishState == "active") {
			Debug.Log ("OnMouseUp");
			berry.rigidbody2D.isKinematic = false;
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));

			Vector3 diff = transform.position - mousePosition;
			Debug.Log (diff);
			float pointBDiffC = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
			float maxDiffC = 2000;
			if (gBerryClass.berryState == "") maxDiffC = 400;
			float diffX = maxDiffC / pointBDiffC * diff.x;
			float diffY = maxDiffC / pointBDiffC * diff.y;


			berry.rigidbody2D.AddForce( new Vector2(diffX, diffY));
			Debug.Log (gBerryClass.berryState);
			sluggishState = "fly";
			line.SetActive(false);
		}
		
	}


}
