using UnityEngine;

[AddComponentMenu("NGUI/Examples/Load Level On Click")]
public class LoadLevelOnClick : MonoBehaviour
{
	public string levelName;

	void OnClick ()
	{
		if (levelName == "") Application.LoadLevel(Application.loadedLevel);
		else Application.LoadLevel(levelName);
	}
}