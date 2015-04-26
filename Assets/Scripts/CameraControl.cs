using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	[SerializeField] float camMoveSpeed;

	Vector2 cameraMoveInput = new Vector2();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CameraMovement();
	}


	public void InitCamera(int mapWidth, int mapHeight){

	}

	public void SetCameraPosition(Vector3 pos){
		Debug.Log("SetCameraPosition! - pos: " + pos);

		Vector3 newPos = pos;
		newPos.z = -10;
		Camera.main.transform.position = newPos;
	}



	void CameraMovement(){
		cameraMoveInput.x = Input.GetAxis("Horizontal");
		cameraMoveInput.y = Input.GetAxis("Vertical");


		Camera.main.transform.Translate(cameraMoveInput * camMoveSpeed * Time.deltaTime);
	}

}
