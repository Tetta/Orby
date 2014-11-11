using UnityEngine;
using System.Collections;

public class iLevelClass : MonoBehaviour {
	public GameObject[] stars;
	public GameObject locked;
	public int level;

	// Use this for initialization
	void Start () {
		if (initClass.progress.Count == 0) initClass.getProgress();
		int levelProgress = initClass.progress["level" + level];
		int currentLevel = initClass.progress["currentLevel"];
		if (currentLevel >= level) locked.SetActive(false);
		//Debug.Log("level: " + level);
		//Debug.Log("levelProgress: " + levelProgress);
		for (int i = levelProgress; i <= 2; i++) {
			stars[i].SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick ()
	{
		if ((int) initClass.progress["currentLevel"] >= level) Application.LoadLevel("level" + level);
	}
}
