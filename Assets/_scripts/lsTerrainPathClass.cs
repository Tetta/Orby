using UnityEngine;
using System.Collections;

public class lsTerrainPathClass : MonoBehaviour {
	public Transform cameraTransform;
	private Vector3 screenPoint;
	private Vector3 mouseDownPosition;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnPress() {
		screenPoint = cameraTransform.position;
		mouseDownPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
	}
	void OnDrag() {
		Vector3 offset = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)) - mouseDownPosition;
		Vector3 newPosition = screenPoint - offset;
		cameraTransform.position = new Vector3(newPosition.x, 0, 0);
	}

}
