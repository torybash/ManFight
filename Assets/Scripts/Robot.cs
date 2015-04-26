using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour {

	SpriteRenderer[] srs;

	public NetworkView netView;

	bool hasMoved = false;
	public int movesLeft = 0;


	public RobotCommand robotCommand;

	public Vector2 ghostPosition = Vector2.zero;

	//ROBOT VALUES
	public int maxMoves = 15;
	public int range = 10;
	public int weaponDmg = 1;
	public float weaponCooldown = 0.5f;
	public int hp = 10;


	public int robotID = -1;


	// Use this for initialization
	void Awake () {
		netView = GetComponent<NetworkView>();

		srs = GetComponentsInChildren<SpriteRenderer>();
	}


	
	
	// Update is called once per frame
	void Update () {
	
		if (!netView.isMine) return;




		 
		//DEBUG OF DEATH
//		if (Input.GetKeyDown(KeyCode.UpArrow)){
//			transform.Translate(0, 1, 0);
//		}else if (Input.GetKeyDown(KeyCode.DownArrow)){
//			transform.Translate(0, -1, 0);
//		}else if (Input.GetKeyDown(KeyCode.LeftArrow)){
//			transform.Translate(-1, 0, 0);
//		}else if (Input.GetKeyDown(KeyCode.RightArrow)){
//			transform.Translate(1, 0, 0);
//		}
	}





	//GENERAL
	public void Init(int robotID, Color clr){
		this.robotID = robotID;
		foreach (SpriteRenderer sr in srs) {
			sr.color = clr;
		}
		netView.RPC("RPCInit", RPCMode.AllBuffered, robotID, clr.r, clr.g, clr.b);


	}

	public void StartTurn(){
		movesLeft = maxMoves;

		robotCommand = new RobotCommand(robotID);

		ghostPosition = transform.position;
	}

	
	

	//RPC
	[RPC]
	public void RPCInit(int robotID, float r, float g, float b){
//		Debug.Log("ROBOT INIT - rgb: "+  r + ", " + g + ", " + b);
		this.robotID = robotID;

		foreach (SpriteRenderer sr in srs) {
			sr.color = new Color(r, g, b);
		}



//		if (!Network.isServer) return;
//		GameControl gameCtrl = GameObject.Find("GameScripts").GetComponent<GameControl>();
//		gameCtrl.AddPlayerRobot(this, netView.owner.guid);
	}



	//Unity BUILT-IN
	void OnNetworkInstantiate(NetworkMessageInfo info) {
//		if (!Network.isServer) return;

//		GameControl gameCtrl = GameObject.Find("GameScripts").GetComponent<GameControl>();
//
//		gameCtrl.AddPlayerRobot(this, netView.owner.guid);
	}
}
