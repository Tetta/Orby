using UnityEngine;
using System.Collections;

public class CustomLeaderboardFiledsHolder : MonoBehaviour {

	public TextMesh rank;
	public TextMesh score;
	public TextMesh playerId;
	public TextMesh playerName;
	public GameObject avatar;

	void Awake() {
		avatar.renderer.material =  new Material(avatar.renderer.material);
	}


	public void Disable() {
		rank.text = "";
		score.text = "";
		playerId.text = "";
		playerName.text = "";

		avatar.renderer.enabled = false;
	}
}
