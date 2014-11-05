using UnityEngine;
using System.Collections;

public class CustomPlayerUIRow : MonoBehaviour {


	public TextMesh playerId;
	public TextMesh playerName;
	public GameObject avatar;
	public TextMesh hasIcon;
	public TextMesh hasImage;

	void Awake() {
		avatar.renderer.material =  new Material(avatar.renderer.material);
	}


	public void Disable() {
		hasIcon.text = "";
		hasImage.text = "";
		playerId.text = "";
		playerName.text = "";

		avatar.renderer.enabled = false;
	}
}
