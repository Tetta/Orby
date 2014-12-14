using UnityEngine;
using System.Collections;

public class gGrootClass : MonoBehaviour {

	public GameObject line;
	public GameObject chainPrefab;

	private string grootState = "";
	private float  chainLength = 0.3F;
	private int maxChainCount = 20;
	private int chainCount = 0;
	private GameObject[] chain;
	private float angle;
	private HingeJoint2D jointGroot;
	private float diffX;
	private float diffY;
	private GameObject[] terrains;

	// Use this for initialization
	void Start () {
		chain = new GameObject[100];
		jointGroot = GetComponent<HingeJoint2D> ();
		terrains = GameObject.FindGameObjectsWithTag("terrain");

	}
	
	// Update is called once per frame
	void Update () {
		if (grootState == "creating") {
			for (int j = 0; j < 8; j++) {
				if (chainCount < maxChainCount && grootState == "creating") {
					createRope();
					
				}
			}
		}

		if (grootState == "enable" || grootState == "creating") {
			GetComponent<LineRenderer>().SetVertexCount (chainCount);
			for(int i = 1; i <= chainCount; i++) {
				GetComponent<LineRenderer>().SetPosition (i - 1, new Vector3(chain[i].transform.position.x, chain[i].transform.position.y, 1.1F));
			}
		}
	}

	void OnMouseDown() {
		Debug.Log(23);
		if (grootState == "") {
			
			grootState = "drag";
			line.SetActive(true);
			gHintClass.checkHint(gameObject);
		} else if (grootState == "creating") {
			//grootState = "enable";
		} else if (grootState == "enable") {
		}
		
	}
	
	void OnMouseDrag() {
		if (grootState == "drag") {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(gHintClass.checkHint(gameObject, true));
			Vector3 relative = transform.InverseTransformPoint(mousePosition);
			angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
			line.transform.rotation = Quaternion.Euler(0, 0, - angle);
			
			
			//line.transform.Rotate(0, 0, angle);
			//line.transform.RotateAround(Vector3.zero, Vector3.zero, angle);
			
		}
	}

	void OnMouseUp() {
		if (grootState == "drag") {

			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(gHintClass.checkHint(gameObject, true));
			Vector3 diff = mousePosition - transform.position;
			//Debug.Log (diff);
			float pointBDiffC = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
			diffX = chainLength / pointBDiffC * diff.x;
			diffY = chainLength / pointBDiffC * diff.y;


			grootState = "creating";


			line.SetActive(false);
		}
	}

	void createRope () {
		chainCount ++; 
		int i = chainCount;
		chain[i] = Instantiate(chainPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		chain[i].transform.Rotate(0, 0, 180 - angle);
		chain[i].name = "chain " + i;
		chain[i].transform.parent = transform;
		chain[i].transform.localScale = new Vector3(1, 1, 1);
		if (i == 1) {
			jointGroot.connectedBody = chain[i].rigidbody2D;
			jointGroot.enabled = true;
		} else {
			//if (!berry.collider2D.OverlapPoint(chain[1].transform.position)) {
			for (int y = 1; y <= i; y++) {
				chain[y].transform.localPosition = new Vector3(diffX * (i - y), diffY * (i - y), 0);
				}
			//}
			Debug.Log(chain[i].transform.position.z);
			HingeJoint2D joint = chain[i].GetComponent<HingeJoint2D> ();
			joint.connectedBody = chain[i - 1].rigidbody2D;
			joint.enabled = true;
			jointGroot.connectedBody = chain[i].rigidbody2D;

			foreach (GameObject terrain in terrains) {
				if (terrain.collider2D.OverlapPoint(chain[1].transform.position)) {
					chain[1].rigidbody2D.isKinematic = true;
					grootState = "afterCollisionTerrain";
					grootState = "enable";
				} 
			}
		}
		if (chainCount == maxChainCount) {
			grootState = "noCollisions";
			//globalCounter = 1;
		}
		
	}

}
