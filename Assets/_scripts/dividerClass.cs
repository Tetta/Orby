using UnityEngine;
using System.Collections;

public class dividerClass : MonoBehaviour {

	private string dividerState = "";
	private Vector2 enterPoint;
	private Vector2 exitPoint;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.magnitude > 10) {
			dividerState = "";
			rigidbody2D.isKinematic = true;
			transform.localPosition = new Vector2(0, 0);
			gameObject.SetActive(false);

		}
	}

	
	void OnTriggerEnter2D(Collider2D collider) {
		/*
		if (dividerState == "" && collider.name == "terrain") {
			enterPoint = transform.position;
			//Debug.Log ("enter: " + collider.name + enterPoint);
			dividerState = "enter";

		}
		*/
	}
	void OnTriggerExit2D(Collider2D collider) {
		//if (dividerState == "enter" && collider.name == "terrain") {
		if (collider.name == "terrain") {
			//Debug.Log ("exit: " + collider.name + transform.position);
			exitPoint = transform.position;
			dividerState = "";
			rigidbody2D.isKinematic = true;
			transform.localPosition = new Vector2(0, 0);
			gameObject.SetActive(false);
			Ferr2D_Path terrain = collider.GetComponent<Ferr2D_Path>();
			Vector2 pos;
			Vector2 firstPoint = new Vector2(0, 0);
			Vector2 secondPoint = new Vector2(0, 0);
			int firstPointA = -1;
			int secondPointA = -1;
			int i;
			int terrainCount = terrain.Count;
			for (i = 0; i < terrainCount; i++) {
			
				if (i == terrainCount - 1) {
					Vector2 posA = terrain.pathVerts[0] + new Vector2 (collider.transform.position.x, collider.transform.position.y);
					Vector2 posB = terrain.pathVerts[i] + new Vector2 (collider.transform.position.x, collider.transform.position.y);
					pos = lineIntersectPos(enterPoint, exitPoint, posA, posB);
				} else {
					Vector2 posA = terrain.pathVerts[i + 1] + new Vector2 (collider.transform.position.x, collider.transform.position.y);
					Vector2 posB = terrain.pathVerts[i] + new Vector2 (collider.transform.position.x, collider.transform.position.y);
					pos = lineIntersectPos(enterPoint, exitPoint, posA, posB);
					//Debug.Log("posA: " + posA);
					//Debug.Log("posB: " + posB);
				}
				if (pos.x != 10000) {
					if (firstPointA == -1) {
						firstPointA = i;
						firstPoint = pos - new Vector2 (collider.transform.position.x, collider.transform.position.y);
					} else {
						secondPointA = i;
						secondPoint = pos - new Vector2 (collider.transform.position.x, collider.transform.position.y);
					}
				}
			}
			if (secondPointA == -1) {
				Debug.Log ("terrainCount: " + terrainCount);
				Debug.Log ("fp: " + firstPointA);
				Debug.Log ("sp: " + secondPointA);

			}
			bool flag = true;
			i = -1;
			Vector2[] pathVerts = terrain.pathVerts.ToArray();
			if (firstPointA != -1) { 
				terrain.pathVerts.Clear();
				while (flag){
					if (i == -1) {
						//i = firstPointA;
						//Debug.Log (firstPointA);
						terrain.pathVerts.Add(firstPoint);
						//Debug.Log (secondPointA);
						terrain.pathVerts.Add(secondPoint);
						i = secondPointA + 1;
					}
					if (i >= terrainCount) i = 0;
					if (i == firstPointA) flag = false;
					//Debug.Log (i);
					terrain.pathVerts.Add(pathVerts[i]);
					i ++;

				}
				//Debug.Log (terrain.pathVerts[0]);
				//terrain.UpdateDependants();
				Ferr2DT_PathTerrain pathTerrain = collider.GetComponent<Ferr2DT_PathTerrain>();
				pathTerrain.RecreatePath();
				pathTerrain.RecreateCollider();			
			}


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
