using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	[SerializeField] float camMoveSpeed;
	[SerializeField] float camZoomSpeed;
	[SerializeField] float minCamZoom;
	[SerializeField] float maxCamZoom;

	Vector2 cameraMoveInput = new Vector2();

	float initCamSize;

	
	// Use this for initialization
	void Start () {
	
		initCamSize = Camera.main.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
		CameraMovement();

		CameraZoom();
	}


	public void InitCamera(int mapWidth, int mapHeight){

	}

	public void SetCameraPosition(Vector3 pos){
//		Debug.Log("SetCameraPosition! - pos: " + pos);

		Vector3 newPos = pos;
		newPos.z = -10;
		Camera.main.transform.position = newPos;
	}



	void CameraMovement(){
		cameraMoveInput.x = Input.GetAxis("Horizontal");
		cameraMoveInput.y = Input.GetAxis("Vertical");


		Camera.main.transform.Translate(cameraMoveInput * camMoveSpeed * Time.deltaTime);
	}


	void CameraZoom(){
		float camZoomInput = Input.GetAxis("Mouse ScrollWheel");

		float newCamSize = Camera.main.orthographicSize + camZoomInput * camZoomSpeed * Time.deltaTime;

		if (newCamSize < minCamZoom) newCamSize = minCamZoom;
		if (newCamSize > maxCamZoom) newCamSize = maxCamZoom;

		Camera.main.orthographicSize = newCamSize;
	}

}
