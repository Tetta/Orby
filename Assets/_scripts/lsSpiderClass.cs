using UnityEngine;
using System.Collections;

public class lsSpiderClass : MonoBehaviour {

	public GameObject levelMenu;
	public UILabel titleNumberLevel;
	public GameObject stars;
	public GameObject time;
	public GameObject web;
	public GameObject sluggish;
	public GameObject destroyer;
	public GameObject yeti;
	public GameObject groot;
	public GameObject cameraUI;
	public GameObject gem1Inactive;
	public GameObject gem2Inactive;

	private string spiderState = "";
	//private NavMeshAgent spiderAgent;
	//private GameObject spider2;
	private Animator currentSkinAnimator;
	//private Vector3 velSpider;
	private Vector3 velCamera;
	private Vector3 startPosSpider;
	private Vector3 posIsland;
	private float timerJump;
	private float timeJumpConst = 0.5F;
	private Vector3 tan1;
	private Vector3 tan2;


	// Use this for initialization
	void Start () {
		if (initClass.progress.Count == 0) initClass.getProgress();
		transform.localPosition = GameObject.Find("level " + initClass.progress["currentLevel"]).transform.localPosition + new Vector3(0, 104, 0);
		if (transform.position.x > 0) cameraUI.transform.position = new Vector3(transform.position.x, cameraUI.transform.position.y, cameraUI.transform.position.z);
		currentSkinAnimator = transform.GetChild(0).GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape)) Application.LoadLevel("menu");

		if (spiderState == "jump up") {
			if (Time.time - timerJump > 0.33F) {
				spiderState = "jump";
				currentSkinAnimator.speed = 0;
			}

		}
		if (spiderState == "jump") {

			//двигаем камеру
			cameraUI.transform.position += velCamera * Time.deltaTime;
			//t
			float t = (Time.time - timerJump - 0.33F) / timeJumpConst;
			//двигаем паука

			//transform.position += velSpider * Time.deltaTime;
			transform.position = сalculateBezierPoint(t, startPosSpider, tan1, tan2, posIsland);
			//заканчиваем прыжок
			if (t >= 1) {
				if (velCamera != Vector3.zero) cameraUI.transform.position = new Vector3 (posIsland.x, 0, 0);
				spiderState = "";
				currentSkinAnimator.speed = 1;
				StartCoroutine(selectLevelMenuCorourine());
			}
		}

	}

	public void clickLevel (Vector3 posIslandParam) {
		posIsland = posIslandParam + new Vector3(0, 0.22F, 0);
		startPosSpider = transform.position;
		//velSpider = (posIsland - startPosSpider) / timeJumpConst;
		if (Mathf.Abs(cameraUI.transform.position.x - startPosSpider.x) > 1) {
			velCamera = new Vector3(posIsland.x - startPosSpider.x, 0, 0) / timeJumpConst;
			cameraUI.transform.position = new Vector3 (transform.position.x, 0, 0);
		} else 
			velCamera = Vector3.zero;

		//касательные
		tan1.x = startPosSpider.x + (posIsland.x - startPosSpider.x) / 4;
		tan2.x = posIsland.x - (posIsland.x - startPosSpider.x) / 4;
		tan1.y = startPosSpider.y + Mathf.Clamp(Mathf.Abs(posIsland.y - startPosSpider.y), 0.5F, 1.8F);
		tan2.y = posIsland.y + Mathf.Clamp(Mathf.Abs(posIsland.y - startPosSpider.y), 0.5F, 1.8F);

		spiderState = "jump up";
		//Debug.Log("tan1: " + tan1);
		//Debug.Log("tan2: " + tan2);

		timerJump = Time.time;
		currentSkinAnimator.Play("spider jump");
	}

	public IEnumerator selectLevelMenuCorourine () {
		yield return new WaitForSeconds(0.3F);
		levelMenu.SetActive(true);

		// to default
		levelMenu.transform.GetChild(0).transform.GetChild(0).GetComponent<UIToggle>().value = true;
		time.SetActive(false);
		web.SetActive(false);
		sluggish.SetActive(false);
		destroyer.SetActive(false);
		yeti.SetActive(false);
		groot.SetActive(false);
		for (int i = 1; i <= 3; i++) {
			stars.transform.GetChild(i - 1).GetComponent<UISprite>().color = new Color(0, 0, 0, 1);
		}
		gem1Inactive.SetActive(false);
		gem2Inactive.SetActive(false);

		//init gems
		int levelProgress = initClass.progress["level" + initClass.progress["currentLevel"]];
		if (levelProgress == 0 || levelProgress == 2) gem1Inactive.SetActive(true);
		if (levelProgress == 0 || levelProgress == 1) gem2Inactive.SetActive(true);

		titleNumberLevel.text = initClass.progress["currentLevel"].ToString();

		//init content2.stars
		int levelDemandsStars = staticClass.levels[initClass.progress["currentLevel"], 0];
		for (int i = 1; i <= levelDemandsStars; i++) {
			stars.transform.GetChild(i - 1).GetComponent<UISprite>().color = new Color(1, 1, 1, 1);
		}
		stars.transform.GetChild(3).GetComponent<UILabel>().text = levelDemandsStars.ToString();

		int levelDemands = staticClass.levels[initClass.progress["currentLevel"], 1];
		//если условие только одно (про звезды)
		if (levelDemands == 0) {
			stars.transform.localPosition = new Vector3(stars.transform.localPosition.x, -30, stars.transform.localPosition.z);
		//остальные условия
		} else if (levelDemands >= 1 && levelDemands <=99){
			time.SetActive(true);
			if (levelDemands < 10) time.transform.GetChild(0).GetComponent<UILabel>().text = "0" + levelDemands.ToString();
			else time.transform.GetChild(0).GetComponent<UILabel>().text = levelDemands.ToString();
		}	else if (levelDemands >= 100 && levelDemands <=199){
			web.SetActive(true);
			web.transform.GetChild(0).GetComponent<UILabel>().text = (levelDemands - 100).ToString();
			web.transform.GetChild(1).GetComponent<UILabel>().text = (levelDemands - 100).ToString();

		}	else if (levelDemands == 201){
			sluggish.SetActive(true);
		}	else if (levelDemands == 202){
			destroyer.SetActive(true);
		}	else if (levelDemands == 203){
			yeti.SetActive(true);
		}	else if (levelDemands == 204){
			groot.SetActive(true);
		}
	}

	Vector3 сalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3) {

		float u = 1 - t;
		float tt = t*t;
		float uu = u*u;
		float uuu = uu * u;
		float ttt = tt * t;
		
		Vector3 p = uuu * p0;    //first term
		p += 3 * uu * t * p1;    //second term
		p += 3 * u * tt * p2;    //third term
		p += ttt * p3;           //fourth term
		
		return p;
	}


}
