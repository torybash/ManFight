using UnityEngine;
using System.Collections;

public class RobotGhost : MonoBehaviour {

	SpriteRenderer[] srs;

	void Awake () {
		srs = GetComponentsInChildren<SpriteRenderer>();	
	}


	public void SetSprite(Color color){
		Color ghostColor = color;
		ghostColor.a = 0.4f;
		foreach (SpriteRenderer sr in srs) {
			sr.color = ghostColor;


		}

	}
}
