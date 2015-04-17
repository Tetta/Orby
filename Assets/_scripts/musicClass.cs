using UnityEngine;
using System.Collections;

public class musicClass : MonoBehaviour {
	public static musicClass instance = null;
	// Use this for initialization
	void Start () {
		if(instance!=null){
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad (gameObject);
		GetComponent<AudioSource>().Play();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
