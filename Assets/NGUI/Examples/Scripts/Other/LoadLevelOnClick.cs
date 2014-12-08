using UnityEngine;


public class LoadLevelOnClick : MonoBehaviour
{
	public string levelName;

	void OnClick ()
	{



		if (levelName == "") Application.LoadLevel(Application.loadedLevel);
		else if (levelName == "next") Application.LoadLevel(Application.loadedLevel + 1);
		else Application.LoadLevel(levelName);
	}
}