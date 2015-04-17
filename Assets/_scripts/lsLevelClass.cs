using UnityEngine;
using System.Collections;

public class lsLevelClass : MonoBehaviour {

	//public GameObject spider;
	//public Transform cameraTransform;
	public GameObject islandInactive;
	public GameObject gem1Inactive;
	public GameObject gem2Inactive;
	public GameObject gemBottom1Inactive;
	public GameObject gemBottom2Inactive;
	public int prevLevel = 0;

	//private float maxDistance = 3.5F;
	private int level;

	// Use this for initialization
	void Start () {
		//path = new NavMeshPath();
		level = int.Parse(gameObject.name.Substring(6));
		//levelLabel.text = level.ToString();
		if (initClass.progress.Count == 0) initClass.getProgress();
		int levelProgress = initClass.progress["level" + level];
		int lastLevel = initClass.progress["lastLevel"];
		if (!((prevLevel == 0 && lastLevel + 1 >= level) || (prevLevel != 0 && lastLevel >= prevLevel)) && staticClass.levelBlocks[level] <= initClass.progress["gems"]) islandInactive.SetActive(true);
		if (levelProgress == 0 || levelProgress == 2) {
			gem1Inactive.SetActive(true);
			gemBottom1Inactive.SetActive(true);
		}
		if (levelProgress == 0 || levelProgress == 1) {
			gem2Inactive.SetActive(true);
			gemBottom2Inactive.SetActive(true);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnClick() {
		int lastLevel = initClass.progress["lastLevel"];
		if (initClass.progress["currentLevel"] == level) {
			GameObject.Find("spider").transform.GetChild(0).GetComponent<Animator>().Play("spider jump");
			GameObject.Find("spider").SendMessage("selectLevelMenuCorourine");

		} else {
			if ((prevLevel == 0 && lastLevel + 1 >= level) || (prevLevel != 0 && lastLevel >= prevLevel)) {
				initClass.progress["currentLevel"] = level;
				initClass.saveProgress();
			}
			GameObject.Find("spider").SendMessage("clickLevel", transform.position);
		}

	}

	/// <summary>
	/// Checks if a line between p0 to p1 intersects a line between p2 and p3
	///
	/// returns the point of intersection
	/// </summary>
	public static Vector2 lineIntersectPos(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	{
		float s1_x, s1_y, s2_x, s2_y;
		s1_x = p1.x - p0.x;
		s1_y = p1.y - p0.y;
		s2_x = p3.x - p2.x;
		s2_y = p3.y - p2.y;
		float s, t;
		s = (-s1_y * (p0.x - p2.x) + s1_x * (p0.y - p2.y)) / (-s2_x * s1_y + s1_x * s2_y);
		t = (s2_x * (p0.y - p2.y) - s2_y * (p0.x - p2.x)) / (-s2_x * s1_y + s1_x * s2_y);
		if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
		{
			// Collision detected
			return new Vector2(p0.x + (t * s1_x), p0.y + (t * s1_y));
		}
		return new Vector2(10000, 10000); // No collision
	}

}
