using UnityEngine;
using System.Collections;

public class lsLevelClass : MonoBehaviour {

	public NavMeshAgent spider;
	public Transform cameraTransform;
	public GameObject[] stars;
	public int prevLevel = 0;

	private NavMeshPath path;
	private float maxDistance = 3.5F;
	private int level;

	// Use this for initialization
	void Start () {
		path = new NavMeshPath();
		level = int.Parse(gameObject.name.Substring(6));

		if (initClass.progress.Count == 0) initClass.getProgress();
		int levelProgress = initClass.progress["level" + level];
		int lastLevel = initClass.progress["lastLevel"];
		if ((prevLevel == 0 && lastLevel + 1 >= level) || (prevLevel != 0 && lastLevel >= prevLevel)) GetComponent<SpriteRenderer>().color = Color.white;

		for (int i = levelProgress; i <= 2; i++) {
			stars[i].SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnClick() {
		int lastLevel = initClass.progress["lastLevel"];
		//if (initClass.progress["currentLevel"] == level) Application.LoadLevel("level" + level);
		if (initClass.progress["currentLevel"] == level) {
			spider.gameObject.SendMessage("selectLevelMenu");

		}
		if ((prevLevel == 0 && lastLevel + 1 >= level) || (prevLevel != 0 && lastLevel >= prevLevel)) {

			if (Mathf.Abs(spider.transform.position.x - cameraTransform.position.x) > maxDistance) {
				spider.CalculatePath(transform.position, path);
				Debug.Log("path.corners.Length: " + path.status );
				for (int i = 1; i < path.corners.Length; i++) {
					Vector2 v = new Vector2(10000, 10000);
					if (spider.transform.position.x < cameraTransform.position.x) v = lineIntersectPos(new Vector2(path.corners[i - 1].x, path.corners[i - 1].z), new Vector2(path.corners[i].x, path.corners[i].z), new Vector2 (cameraTransform.position.x - maxDistance, 3), new Vector2 (cameraTransform.position.x - maxDistance, -3));
					else v = lineIntersectPos(new Vector2(path.corners[i - 1].x, path.corners[i - 1].z), new Vector2(path.corners[i].x, path.corners[i].z), new Vector2 (cameraTransform.position.x + maxDistance, 3), new Vector2 (cameraTransform.position.x + maxDistance, -3));
					if (v.x != 10000) {
						Debug.Log(44);
						spider.enabled = false;
						spider.transform.position = new Vector3(v.x, 0, v.y);
						spider.enabled = true;
						break;
					}
				}

			}
			spider.SetDestination(transform.position);
			initClass.progress["currentLevel"] = level;
			initClass.saveProgress();
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
