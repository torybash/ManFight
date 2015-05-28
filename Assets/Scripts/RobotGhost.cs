using UnityEngine;
using System.Collections;

public class RobotGhost : MonoBehaviour {

	public Robot rob;

	SpriteRenderer[] srs;

	void Awake () {
		srs = GetComponentsInChildren<SpriteRenderer>();	
	}


	public void Init(Color color, Robot rob){
		this.rob = rob;

		Color ghostColor = color;
		ghostColor.a = 1f;
		foreach (SpriteRenderer sr in srs) {
			sr.color = ghostColor;


		}

	}
}
