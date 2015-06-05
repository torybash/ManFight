using UnityEngine;
using System.Collections;

public class RobotGhost : MonoBehaviour {

	public Robot rob;

	SpriteRenderer[] srs;
	[SerializeField] Transform gunContainer;

	void Awake () {
		srs = GetComponentsInChildren<SpriteRenderer>();	
	}


	public void Refresh(Color color, Robot rob, Vector2 pos, float angle){
		this.rob = rob;

		Color ghostColor = color;
		ghostColor.a = 1f;
		foreach (SpriteRenderer sr in srs) {
			sr.color = ghostColor;
		}

		transform.position = pos;
		gunContainer.rotation = Quaternion.AngleAxis(angle, Vector3.forward);



	}

	public void RotateGun(float angle){

	}
}
