using UnityEngine;
using System.Collections;

public class gWebClass2 : MonoBehaviour {
	// Use this for initialization
	public GameObject holder;
	public GameObject web;
	public GameObject web2;
	public GameObject chainPrefab;
	public GameObject pointB;
	public Vector3 pointBAchor;
	public int drag = 2;

	private int globalCounter = 0;
	private int maxChainCount2 = 80;
	private int maxChainCount = 40;
	private int chainCount = 0;
	private float  chainLength = 0.078F;
	private GameObject[] chain;
	private float maxDiffC;
	private Vector2 diff;
	private string webState = "";
	private float diffX;
	private float diffY;
	private float chainPositionZ = 1;
	private HingeJoint2D jointWeb;
	private int normalChainCount = 20;
	private GameObject berry;

	void Start () {
		berry = GameObject.Find("berry");

		chain = new GameObject[100];
		maxDiffC = chainLength * maxChainCount;
		jointWeb = web.GetComponent<HingeJoint2D> ();
		if (pointB != null) {
			diff = pointB.transform.position + pointBAchor - web.transform.position;
			float pointBDiffC = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
			diffX = maxDiffC / pointBDiffC * diff.x / maxChainCount;
			diffY = maxDiffC / pointBDiffC * diff.y / maxChainCount;
			webState = "creatingWeb";
			while (webState == "creatingWeb") {
				createWeb(pointB.transform.position, pointBAchor, true);

				int i = chainCount;
				//chain[i].rigidbody2D.fixedAngle = true;
				if (pointB.collider2D.OverlapPoint(chain[1].transform.position + new Vector3(diffX, diffY, 0))) {

					chainCount = i;
					HingeJoint2D jointChain = chain[1].GetComponent<HingeJoint2D> ();
					jointChain.connectedBody = pointB.rigidbody2D;
					jointChain.connectedAnchor = pointBAchor;
					jointChain.enabled = true;	
					jointChain.useLimits = false;
					for (int y = 1; y <= chainCount; y ++){
						chain[y].rigidbody2D.drag = drag;
						//chain[y].rigidbody2D.angularDrag = 2;
						//chain[y].rigidbody2D.centerOfMass = new Vector2(0, -0.3F);
					}

					//i = maxChainCount / 2;
					webState = "enableWeb";

				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log(Time.time);
		if (webState == "creatingWeb") {
			for (int j = 0; j < 8; j++) {
				if (chainCount < maxChainCount && webState == "creatingWeb") {
					createWeb(berry.transform.position, new Vector3(0, 0, 0), false);
			
				}
			}
		}
		/*
		if (webState == "collisionOrby1") {
			HingeJoint2D jointChain = chain[1].GetComponent<HingeJoint2D> ();

			jointChain.useLimits = false;
			//jointChain.limits = new JointAngleLimits2D { min = 180, max = 0};
			jointChain.connectedBody = orby.rigidbody2D;
			jointChain.connectedAnchor = orby.transform.position - chain[1].transform.position;
			//jointChain.connectedAnchor = new Vector2(0, 0);
			jointChain.enabled = true;	
			webState = "afterCollisionOrby";
		}
		*/
		if (webState == "afterCollisionOrby") {
			HingeJoint2D jointWeb = web.GetComponent<HingeJoint2D> ();
			for (int j = 0; j < 2; j++) {
				if (normalChainCount < chainCount) {
					Destroy(chain[chainCount], 0);

					for (int y = 1; y <= chainCount; y++) {
						chain[y].transform.position =  new Vector3(web.transform.position.x + diffX * (chainCount - y), web.transform.position.y + diffY * (chainCount - y), chainPositionZ);
					}
					//orby.transform.position =  new Vector3(web.transform.position.x + diffX * chainCount, web.transform.position.y + diffY * chainCount, chainPositionZ);
					jointWeb.connectedBody = chain[chainCount - 1].rigidbody2D;
					chainCount--;
				} else {
					webState = "enableWeb";
					gBerryClass.berryState = "enableWeb";
				}
			}

		}

		if (webState == "noCollisions" || webState == "destroyingWeb" || webState == "collisionBlock") {
			gBerryClass.berryState = "";
			for (int j = 0; j < 2; j++) {
				if (chainCount > 0) {
					Destroy(chain[globalCounter], 0);
					chainCount--;
					globalCounter ++;
				
				} else {
					webState = "";
				}
			}
			holder.GetComponent<LineRenderer>().SetVertexCount (chainCount);
			for(int i = 0; i < chainCount; i++)
				holder.GetComponent<LineRenderer>().SetPosition (i, chain[i + globalCounter].transform.position);


		}
		if (webState == "enableWeb" || webState == "creatingWeb") {
			holder.GetComponent<LineRenderer>().SetVertexCount (chainCount);
			for(int i = 1; i <= chainCount; i++) {
				holder.GetComponent<LineRenderer>().SetPosition (i - 1, chain[i].transform.position);
			}
		}

	}

	void createWeb (Vector3 pointBPosition, Vector3 pointBAchor, bool start) {
		chainCount ++; 
		int i = chainCount;
		Vector3 pos = new Vector3(transform.position.x, transform.position.y, 1);
		chain[i] = Instantiate(chainPrefab, pos, Quaternion.identity) as GameObject;
		chain[i].GetComponent<SpriteRenderer> ().sortingOrder = i;
		Vector3 relative = transform.InverseTransformPoint(pointBPosition + pointBAchor);
		float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
		chain[i].transform.Rotate(0, 0, 180 - angle);
		chain[i].name = "chain " + i;
		chain[i].transform.parent = holder.transform;
		//chain[i].rigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
		if (i == 1) {
			jointWeb.connectedBody = chain[i].rigidbody2D;
			jointWeb.enabled = true;
			
			//chainClass script = chain[i].GetComponent <chainClass>();
		} else {
			if (!berry.collider2D.OverlapPoint(chain[1].transform.position)) {
				for (int y = 1; y <= i; y++) {
					chain[y].transform.position = new Vector3(web.transform.position.x + diffX * (i - y), web.transform.position.y + diffY * (i - y), chainPositionZ);
				}
			}
			HingeJoint2D joint = chain[i].GetComponent<HingeJoint2D> ();
			joint.connectedBody = chain[i - 1].rigidbody2D;
			joint.enabled = true;
			jointWeb.connectedBody = chain[i].rigidbody2D;
			if (berry.collider2D.OverlapPoint(chain[1].transform.position) && i >= 20) {
				HingeJoint2D jointChain = chain[1].GetComponent<HingeJoint2D> ();

				jointChain.useLimits = false;
				//jointChain.limits = new JointAngleLimits2D { min = 180, max = 0};
				//jointChain.connectedAnchor = berry.transform.InverseTransformPoint(chain[1].transform.position);
				jointChain.connectedAnchor = new Vector2(0, 0);

				jointChain.connectedBody = berry.rigidbody2D;
				jointChain.enabled = true;	
				webState = "afterCollisionOrby";

				//diff = orby.transform.position - web.transform.position;
				//float orbyDiffC = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
				//diffX = maxDiffC / orbyDiffC * diff.x / maxChainCount;
				//diffY = maxDiffC / orbyDiffC * diff.y / maxChainCount;

				//chain[99] = Instantiate(web2, chain[1].transform.position, Quaternion.identity) as GameObject;
				//chain[99].transform.parent = orby.transform;
				//chain[99].transform.Rotate(0, 0, - angle);
				//webState = "collisionOrby";
				//Debug.Log ("webState = collisionOrby");
				//Debug.Log (Time.time + ": chainCount = " + chainCount);
			} 
		}
		if ((chainCount == maxChainCount2 && start) || (chainCount == maxChainCount && !start)) {
			webState = "noCollisions";
			globalCounter = 1;
		}

	}
	
	void OnMouseDown () {
		GetComponent<Animator>().Play("web");
		gRecHintClass.recHint(transform);
		gHintClass.checkHint(gameObject);
		if (webState == "") {
			audio.Play();
			staticClass.useWeb ++;
			if (GooglePlayConnection.state == GPConnectionState.STATE_CONNECTED) GooglePlayManager.instance.IncrementAchievement("achievement_use_web_50_times", 1);

			diff = berry.transform.position - web.transform.position;
			float orbyDiffC = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
			diffX = maxDiffC / orbyDiffC * diff.x / maxChainCount;
			diffY = maxDiffC / orbyDiffC * diff.y / maxChainCount;
			webState = "creatingWeb";
		}
		if (webState == "enableWeb") {
			audio.Play();
			staticClass.useWeb ++;
			webState = "destroyingWeb";
			globalCounter = 1;
		}
	}

}
