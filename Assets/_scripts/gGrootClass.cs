using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gGrootClass : MonoBehaviour {

	public GameObject line;
	public GameObject chainPrefab;

	private GameObject berry;
	private GameObject spider;
	private int globalCounter = 0;
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
	public struct terrainGrootChain {
		public GameObject terrain;
		public GameObject chain;
	}
	public static  List<terrainGrootChain>  terrainGrootChains = new List<terrainGrootChain>();

	// Use this for initialization
	void Start () {
		chain = new GameObject[100];
		jointGroot = GetComponent<HingeJoint2D> ();
		terrains = GameObject.FindGameObjectsWithTag("terrain");
		spider = GameObject.Find("spider");
		berry = GameObject.Find("berry");

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

		if (grootState == "noCollisions" || grootState == "destroying") {
			for (int j = 0; j < 2; j++) {
				if (chainCount > 0) {
					Destroy(chain[globalCounter], 0);
					chainCount--;
					globalCounter ++;
					
				} else {
					grootState = "";
				}
			}
			GetComponent<LineRenderer>().SetVertexCount (chainCount);
			for(int i = 0; i < chainCount; i++)
				GetComponent<LineRenderer>().SetPosition (i, new Vector3(chain[i + globalCounter].transform.position.x, chain[i + globalCounter].transform.position.y, 1.1F)); 
			
			
		}
	}

	void OnMouseDown() {
		if (grootState == "") {
			grootState = "drag";
			line.SetActive(true);
			gHintClass.checkHint(gameObject);
		}		
		if (grootState == "enable") {
			grootState = "destroying";
			gHintClass.checkHint(gameObject);
			gRecHintClass.recHint(transform);
			globalCounter = 1;
			line.SetActive(false);

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
			gRecHintClass.recHint(transform);

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
			for (int y = 1; y <= i; y++) {
				chain[y].transform.localPosition = new Vector3(diffX * (i - y), diffY * (i - y), 0);
			}
			HingeJoint2D joint = chain[i].GetComponent<HingeJoint2D> ();
			joint.connectedBody = chain[i - 1].rigidbody2D;
			joint.enabled = true;
			jointGroot.connectedBody = chain[i].rigidbody2D;

		}
		foreach (GameObject terrain in terrains) {
			if (terrain.collider2D.OverlapPoint(chain[1].transform.position)) {
				chain[1].rigidbody2D.isKinematic = true;
				grootState = "enable";
				terrainGrootChains.Add(new terrainGrootChain() {terrain = terrain, chain = chain[1]});
			} 
		}
		if (chainCount == maxChainCount || spider.collider2D.OverlapPoint(chain[1].transform.position) || berry.collider2D.OverlapPoint(chain[1].transform.position)) {
			grootState = "noCollisions";
			globalCounter = 1;
		}
	}

}
