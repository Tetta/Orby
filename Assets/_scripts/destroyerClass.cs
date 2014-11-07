using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class destroyerClass : MonoBehaviour {

	public GameObject divider;

	//private GameObject[] terrains;
	private string destroyerState = "";
	private Vector2 enterPoint;
	private Vector2 exitPoint;

	// Use this for initialization
	void Start () {
		enterPoint = transform.position;
		GameObject[] terrains = GameObject.FindGameObjectsWithTag("terrain");
		foreach (GameObject terrain in terrains) {
			Ferr2DT_PathTerrain pathTerrain = terrain.GetComponent<Ferr2DT_PathTerrain>();
			pathTerrain.RecreatePath();
			pathTerrain.RecreateCollider();	

		}
	}
	
	// Update is called once per frame
	void Update () {
		//if (destroyerState == "fly" && !divider.activeSelf) destroyerState = "";
		if (transform.position.magnitude > 10) {
			gameObject.SetActive(false);
		}	
	}

	void OnMouseDown() {

		if (destroyerState == "") {
			
			destroyerState = "active";
			//divider.SetActive(true);
			//divider.rigidbody2D.isKinematic = false;
		}
		
	}
	void OnMouseDrag() {
		if (destroyerState == "active") {
			//Debug.Log ("OnMouseDrag");
			
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
			Vector3 relative = transform.InverseTransformPoint(mousePosition);
			float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
			//transform.rotation = Quaternion.Euler(0, 0, 180 - angle);
			transform.Rotate(0, 0, 270 - angle);

		}
	}

	void OnMouseUp() {
		if (destroyerState == "active") {
			rigidbody2D.isKinematic = false;
			CircleCollider2D collider = gameObject.GetComponent<CircleCollider2D>();
			collider.radius = 0.001F;
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
			
			Vector3 diff = mousePosition - transform.position;
			//Debug.Log (diff);
			float pointBDiffC = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
			float maxDiffC = 500;
			float diffX = maxDiffC / pointBDiffC * diff.x;
			float diffY = maxDiffC / pointBDiffC * diff.y;
			
			
			rigidbody2D.AddForce( new Vector2(diffX, diffY));
			//Debug.Log (diffX);
			destroyerState = "fly";
		}
		
	}

	void OnTriggerStay2D(Collider2D collider) {
		//Debug.Log ("OnTriggerStay2D");
		//Time.timeScale = 0;

	}

	void OnTriggerExit2D(Collider2D collider) {
		if (collider.name == "terrain") {
			//Time.timeScale = 0;
			//Debug.Log ("exit: " + collider.name + transform.position);
			exitPoint = transform.position;
			//rigidbody2D.isKinematic = true;
			//transform.localPosition = new Vector2(0, 0);
			//gameObject.SetActive(false);
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

					//Debug.Log("enterPoint: " + enterPoint);
					//Debug.Log("exitPoint: " + exitPoint);
					//Debug.Log("posA: " + posA.magnitude);
					//Debug.Log("posB: " + posB.magnitude);
					//Debug.Log("pos: " + pos.magnitude);
					if (Mathf.Abs(pos.magnitude - posA.magnitude) <= 0.02F) { pos = posA; Debug.Log(123);}
					if (Mathf.Abs(pos.magnitude - posB.magnitude) <= 0.02F) { pos = posB; Debug.Log(222);}

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
				/*
				Debug.Log ("terrainCount: " + terrainCount);
				Debug.Log ("fp: " + firstPointA);
				Debug.Log ("sp: " + secondPointA);
				*/
				
			}
			if (firstPointA != -1 && secondPointA != -1) { 
				bool flag = true;
				i = -1;
				//Vector2[] pathVerts = terrain.pathVerts.ToArray();
				List<Vector2>  firstFigure = new List<Vector2>();
				List<Vector2>  secondFigure = new List<Vector2>();
				while (flag){
					if (i == -1) {
						//i = firstPointA;
						//Debug.Log (firstPointA);
						firstFigure.Add(firstPoint);
						//terrain.pathVerts.Add(firstPoint);
						//Debug.Log (secondPointA);
						firstFigure.Add(secondPoint);
						i = secondPointA + 1;
					}
					if (i >= terrainCount) i = 0;
					if (i == firstPointA) flag = false;
					//Debug.Log (i);
					if (terrain.pathVerts[i] != firstPoint && terrain.pathVerts[i] != secondPoint) firstFigure.Add(terrain.pathVerts[i]);
					i ++;
					
				}
				flag = true;
				i = -1;
				while (flag){
					if (i == -1) {
						secondFigure.Add(secondPoint);
						secondFigure.Add(firstPoint);
						i = firstPointA + 1;
					}
					if (i >= terrainCount) i = 0;
					if (i == secondPointA) flag = false;
					if (terrain.pathVerts[i] != firstPoint && terrain.pathVerts[i] != secondPoint) secondFigure.Add(terrain.pathVerts[i]);
					i ++;
					
				}
				terrain.pathVerts.Clear();
				if (getSq(firstFigure) >= getSq(secondFigure)) terrain.pathVerts.AddRange(firstFigure);
				else terrain.pathVerts.AddRange(secondFigure);

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

	// Расчет площади многоугольника через сумму площадей трапеций 
	public static float getSq (List<Vector2>  figure) {
		int n = figure.Count;
		float s = 0;
		float res = 0;
		for (int i = 0; i < n; i++) {
			if (i == 0) {
				s = figure[i].x*(figure[n-1].y - figure[i+1].y); //если i == 0, то y[i-1] заменяем на y[n-1]
				res += s;
			}	else
				if (i == n-1) {
				s = figure[i].x*(figure[i-1].y - figure[0].y); // если i == n-1, то y[i+1] заменяем на y[0]
					res += s;
				} else {
				s = figure[i].x*(figure[i-1].y - figure[i+1].y);
					res += s;
				}

		}
		return Mathf.Abs(res/2);
	}
}
