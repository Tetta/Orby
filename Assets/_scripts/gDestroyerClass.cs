using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gDestroyerClass : MonoBehaviour {

	private string destroyerState = "";
	private Vector2 enterPoint;
	private Vector2 exitPoint;
	private List<Vector3>  terrainsTransform = new List<Vector3>();

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
			gHintClass.checkHint(gameObject);
		}
		
	}
	void OnMouseDrag() {
		if (destroyerState == "active") {
			//Debug.Log ("OnMouseDrag");
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(gHintClass.checkHint(gameObject, true));

			Vector3 relative = transform.InverseTransformPoint(mousePosition);
			float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
			//transform.rotation = Quaternion.Euler(0, 0, 180 - angle);
			transform.Rotate(0, 0, 270 - angle);

		}
	}

	void OnMouseUp() {
		if (destroyerState == "active") {
			gRecHintClass.recHint(transform);
			rigidbody2D.isKinematic = false;
			CircleCollider2D collider = gameObject.GetComponent<CircleCollider2D>();
			collider.radius = 0.001F;
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(gHintClass.checkHint(gameObject, true));

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
		if (collider.tag == "terrain") {
			if ( terrainsTransform.Contains(collider.transform.position)) return;
			//for (int i = 0; i < terrainsTransform.Count; i++) {
			//
			//	if (collider.transform == terrainsTransform[i]) Debug.Log(123);
			//}
			terrainsTransform.Add(collider.transform.position);
			exitPoint = transform.position;
			Vector2 diff = exitPoint - enterPoint;
			float pointBDiffC = Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
			float diffX = 15 / pointBDiffC * diff.x;
			float diffY = 15 / pointBDiffC * diff.y;

			exitPoint = new Vector2(diffX, diffY) + enterPoint;
			//Debug.Log ("exit: " + collider.name + exitPoint);

			Ferr2D_Path terrain = collider.GetComponent<Ferr2D_Path>();
			List<Vector2>  pos = new List<Vector2>();
			List<int>  point = new List<int>();
			Vector2 firstPoint = new Vector2(0, 0);
			Vector2 secondPoint = new Vector2(0, 0);
			int firstPointA = -1, secondPointA = -1;
			Vector2 posTemp, posA, posB;
			int terrainCount = terrain.Count;
			for (int i = 0; i < terrainCount; i++) {
				
				if (i == terrainCount - 1) {
					posA = terrain.pathVerts[0] + new Vector2 (collider.transform.position.x, collider.transform.position.y);
					posB = terrain.pathVerts[i] + new Vector2 (collider.transform.position.x, collider.transform.position.y);
					posTemp = lineIntersectPos(enterPoint, exitPoint, posA, posB);
				} else {
					posA = terrain.pathVerts[i + 1] + new Vector2 (collider.transform.position.x, collider.transform.position.y);
					posB = terrain.pathVerts[i] + new Vector2 (collider.transform.position.x, collider.transform.position.y);
					posTemp = lineIntersectPos(enterPoint, exitPoint, posA, posB);
				}
				if (posTemp.x != 10000) {
					if (Mathf.Abs(posTemp.x - posA.x) <= 0.02F && Mathf.Abs(posTemp.y - posA.y) <= 0.02F) posTemp = posA;
					if (Mathf.Abs(posTemp.x - posB.x) <= 0.02F && Mathf.Abs(posTemp.y - posB.y) <= 0.02F) posTemp = posB;
					//Debug.Log("enterPoint: " + enterPoint);
					//Debug.Log("exitPoint: " + exitPoint);
					//Debug.Log("posA: " + posA);
					//Debug.Log("posB: " + posB);
					//Debug.Log("pos: " + posTemp);

					pos.Add(posTemp);
					point.Add(i);
				}

				//Debug.Log("pos.x: " + pos.x);

			}

			//sorting
			float minLength = 100;
			List<Vector2>  posSort = new List<Vector2>();
			List<int>  pointSort = new List<int>();
			int j = -1;
			for (int i = 0; i < pos.Count; i++ ) {
				for (int y = 0; y < pos.Count; y++ ) {
					if ((pos[y] - enterPoint).magnitude < minLength) {
						minLength = (pos[y] - enterPoint).magnitude;
						j = y;
					}
				}
				minLength = 100;
				posSort.Add(pos[j]);
				pos[j] = new Vector2(100, 100);
				pointSort.Add(point[j]);
			}

			for (int y = 0; y < posSort.Count - 1; y += 2 ) {
			
				terrain = collider.GetComponent<Ferr2D_Path>();
				int g = 0;
				if (y > 0) {
					//Debug.Log("terrain.pathVerts[1]: " + terrain.pathVerts[1]);
					//Debug.Log("posSort[y - 1]: " + posSort[y - 1]);
					if (terrain.pathVerts[1] == posSort[y - 1] - new Vector2(collider.transform.position.x, collider.transform.position.y)) g = y - 1;
					else g = y - 2;
					for (int q = y; q < posSort.Count; q++ ) {
						//Debug.Log ("pointSort[q]: " + pointSort[q]);
						//Debug.Log ("pointSort[g]: " + pointSort[g]);
						//Debug.Log ("g: " + g);
						pointSort[q] = pointSort[q] - pointSort[g] + 1;
						if (pointSort[q] < 0) pointSort[q] += terrainCount;
					}
				}
				firstPointA = pointSort[y];
				secondPointA = pointSort[y + 1];
				firstPoint = posSort[y] - new Vector2 (collider.transform.position.x, collider.transform.position.y);
				secondPoint = posSort[y + 1] - new Vector2 (collider.transform.position.x, collider.transform.position.y);
			
				//Debug.Log ("fp: " + firstPoint);
				//Debug.Log ("sp: " + secondPoint);
				bool flag = true;
				int i = -1;
				List<Vector2>  firstFigure = new List<Vector2>();
				List<Vector2>  secondFigure = new List<Vector2>();
				terrainCount = terrain.Count;
				//Debug.Log("pathVerts: ");
				//for (int e = 0; e < terrainCount; e++) Debug.Log(terrain.pathVerts[e]);

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
					//Debug.Log(i);
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
			for (int i = 0; i < gGrootClass.terrainGrootChains.Count; i++ ) {
				gGrootClass.terrainGrootChain terr = gGrootClass.terrainGrootChains[i];
				if (terr.terrain == collider.gameObject) {
					if (!terr.terrain.collider2D.OverlapPoint(terr.chain.transform.position)) {
						terr.chain.transform.parent.SendMessage("OnMouseDown");
						gGrootClass.terrainGrootChains.Remove(terr);
						i--;
					}
				}
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
		//Debug.Log("Figure");
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
			//Debug.Log(figure[i]);
		}
		//Debug.Log("sq: " + Mathf.Abs(res/2));
		return Mathf.Abs(res/2);
	}
}
