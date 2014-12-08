using UnityEngine;
using System.Collections;

public class gRopeClass : MonoBehaviour {

	private InteractiveCloth cloth;
	private ClothRenderer clothRenderer;
	private Vector3[] verts;
	// Use this for initialization
	void Start () {
		verts = new Vector3[8];
		cloth = gameObject.GetComponent<InteractiveCloth>();
		clothRenderer = gameObject.GetComponent<ClothRenderer>();
		//cloth.enabled = false;
		//clothRenderer.enabled = false;
		//transform.position = new Vector3(1,0.3F, 0.1F);
		int i = 0;

		//Debug.Log(cloth.mesh.vertices[0]);
		verts[0] = new Vector3(0, 0, 0);
		verts[1] = new Vector3(1, 0, 0);
		verts[2] = new Vector3(0, 1, 0);
		//verts[3] = new Vector3(1, 1, 0);
		/*
		verts[0] = new Vector3(0, 0, 0);
		verts[1] = new Vector3(1, 1, 0);
		verts[2] = new Vector3(0, 1, 0);
		verts[3] = new Vector3(1, 0, 0);
		verts[4] = new Vector3(0, 0, 0);
		verts[5] = new Vector3(1, 1, 0);
		verts[6] = new Vector3(0, 1, 0);
		verts[7] = new Vector3(1, 0, 0);
		*/
		int[] ints = new int[0];
		foreach (Vector3 vert in cloth.mesh.vertices ) {
			Debug.Log(cloth.mesh.vertices[i]);
			i++;
			
		}
		//cloth.mesh.vertices = verts;
		//cloth.mesh.normals = verts;
		//cloth.mesh.triangles = ints;
	}
	
	// Update is called once per frame
	void Update () {

		//cloth.enabled = true;

		//clothRenderer.enabled = true;
		int i = 0;

		foreach (Vector3 vert in cloth.mesh.vertices ) {
			//Debug.Log(cloth.mesh.vertices[i]);
			i++;

		}
		//cloth.mesh.vertices = verts;
		//Debug.Log(verts[0]);
		//Debug.Log(cloth.mesh.vertices[0]);

		
	}
}
