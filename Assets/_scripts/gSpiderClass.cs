using UnityEngine;
using System.Collections;

public class gSpiderClass : MonoBehaviour {

	private GameObject[] guiStars = new GameObject[3];
	//private GameObject restart;
	private int fixedUpdateCount;
	private GameObject berry;
	private Animator currentSkinAnimator;
	//private GameObject completeMenu;
	//private GameObject berry;
	// Use this for initialization
	void Start () {
		guiStars[0] = GameObject.Find("gui star 1");
		guiStars[1] = GameObject.Find("gui star 2");
		guiStars[2] = GameObject.Find("gui star 3");	
		berry = GameObject.Find("berry");
		currentSkinAnimator = transform.GetChild(0).GetComponent<Animator>();
		//completeMenu = GameObject.Find("gui").transform.Find("complete menu").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.x < -4 || transform.position.x > 4 || transform.position.y < -6 || transform.position.y > 6) GameObject.Find("restart").SendMessage("OnClick");
		//if (completeMenu.activeSelf) transform.position = berry.transform.position;
	}

	void FixedUpdate () {
		fixedUpdateCount ++;
		if (fixedUpdateCount % 50 == 0 && gBerryClass.berryState != "start finish") {

			if ((berry.transform.position - transform.position).magnitude >= 0.35F) {
					if (currentSkinAnimator.GetCurrentAnimatorStateInfo(0).IsName("spider open month")) {
					currentSkinAnimator.Play("idle");
				}
				//if (currentSkinAnimator.GetCurrentAnimatorStateInfo(-1))
				//currentSkinAnimator.Play("spider idle", -1);
			}

			//check jump
			if (rigidbody2D.velocity == new Vector2(0, 0)) {
				if (transform.rotation.z > 0.4F || transform.rotation.z < -0.4F) {
					currentSkinAnimator.Play("spider jump");
					StartCoroutine(coroutineJump());
				} else {
					if (!currentSkinAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle") && !currentSkinAnimator.GetCurrentAnimatorStateInfo(0).IsName("spider open month")) {
						Debug.Log (123);
						if (currentSkinAnimator.GetCurrentAnimatorStateInfo(0).IsName("spider fly")) {
							currentSkinAnimator.transform.FindChild("leg left 2").GetComponent<UISprite>().depth = 2;
							currentSkinAnimator.transform.FindChild("leg right 2").GetComponent<UISprite>().depth = 2;
							currentSkinAnimator.Play("spider jump");
							StartCoroutine(coroutineJump());	
						} else
							currentSkinAnimator.Play("idle");
					}
				}
			} else {
				if (currentSkinAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
					currentSkinAnimator.transform.FindChild("leg left 2").GetComponent<UISprite>().depth = 1;
					currentSkinAnimator.transform.FindChild("leg right 2").GetComponent<UISprite>().depth = 1;

					currentSkinAnimator.Play("spider fly");
				}
			}

		} else if (fixedUpdateCount % 10 == 0 && gBerryClass.berryState == "") {
			//check mouth
			if ((berry.transform.position - transform.position).magnitude < 0.35F) 
				if (currentSkinAnimator.GetCurrentAnimatorStateInfo(0).IsName("idle")) currentSkinAnimator.Play("spider open month");
				
		}
	}
		
	void OnTriggerEnter2D(Collider2D collisionObject) {
		if (collisionObject.gameObject.name == "star") {
			Destroy(collisionObject.gameObject);
			guiStars[gBerryClass.starsCounter].GetComponent<UISprite>().color =  new Color(1, 1, 1, 1);
			gBerryClass.starsCounter ++;
		}
		
	}

	void OnClick () {
		Debug.Log (234);
		currentSkinAnimator.Play("spider see", -1);
	}

	public IEnumerator coroutineJump(){
		yield return new WaitForSeconds(0.1F);
		transform.rotation = new Quaternion(0, 0, 0, 1);

	}

	public static IEnumerator coroutineCry(){
		GameObject.Find("spider").transform.GetChild(0).GetComponent<Animator>().Play("spider cry");
		yield return new WaitForSeconds(2F);
		GameObject.Find("restart").SendMessage("OnClick");
	}

	
}
