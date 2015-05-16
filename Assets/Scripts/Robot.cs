using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour {

	SpriteRenderer[] srs;

	RobotVision robotVision;

	public NetworkView netView;

//	public int movesLeft = 0;


	public RobotCommand robotCommand;

	public Vector2 ghostPosition = Vector2.zero;

	public Color color;

	//ROBOT VALUES
	public int maxMoves = 15;
	public int range = 10;
	public int weaponDmg = 1;
	public float weaponCooldown = 0.5f;
	public int hp = 10;


	public int robotID = -1;

	public RobotHeight robotHeight = RobotHeight.HIGH;


	RobotCommandControl rCmdCtrl;

	bool isMine;

	//Preperation
	public bool needPlacing;
	public float robotPrepTimer;
	
	void Awake () {
		netView = GetComponent<NetworkView>();
		srs = GetComponentsInChildren<SpriteRenderer>();
		robotVision = GetComponent<RobotVision>();
	}
	

	//GENERAL
	public void StartTurn(){
//		movesLeft = maxMoves;
		ghostPosition = transform.position;
		robotCommand = new RobotCommand(robotID);
	}	




	//PLAYER
	void Init(){
		needPlacing = true;
		
		robotCommand = new RobotCommand(robotID);
		ghostPosition = transform.position;		

		rCmdCtrl = GameObject.Find("GameScripts").GetComponent<RobotCommandControl>();
		rCmdCtrl.AddRobot(this);


	}

	public void Placed(){
		needPlacing = false;
		robotVision.Enabled(true);
		ForceUpdateVisibility();
	}

	public void ForceUpdateVisibility(){
		robotVision.ForceUpdateVisibility();
	}

	public void UpdatePositionToTime(float timer){

	}

	//SERVER
	public void ServerInit(int robotID, Color clr, string ownerGUID){
		this.robotID = robotID;
		foreach (SpriteRenderer sr in srs) {
			sr.color = clr;
		}
		netView.RPC("RPCInit", RPCMode.AllBuffered, robotID, clr.r, clr.g, clr.b, ownerGUID);
	}
	
	public void SetPosition(int x, int y){
		transform.position = Tools.AddHalf(new Vector2(x, y));
		ghostPosition = transform.position;
	}


	//RPC
	[RPC]
	public void RPCInit(int robotID, float r, float g, float b, string ownerGUID){ 
//		Debug.Log("RPCInit (Robot) - rgb: "+  r + ", " + g + ", " + b + ", robotID: " + robotID + ", ownerGUID: " + ownerGUID);
		this.robotID = robotID;

		color = new Color(r, g, b);
		foreach (SpriteRenderer sr in srs) {
			sr.color = color;
		}

		//IS MINE?
		if (ownerGUID.Equals(Network.player.guid)){
			isMine = true;
			Init();
		}


//		if (!Network.isServer) return;
//		GameControl gameCtrl = GameObject.Find("GameScripts").GetComponent<GameControl>();
//		gameCtrl.AddPlayerRobot(this, netView.owner.guid);
	}



	//Unity BUILT-IN
	void OnNetworkInstantiate(NetworkMessageInfo info) {

//		netManCtrl = GameObject.Find("GameScripts").GetComponent<NetworkManagerControl>();
//
//
//		netManCtrl.ReceivedNetworkInstantiate(this);
//
//		Debug.Log("OnNetworkInstantiate - Robot  - sender: "  + info.sender.guid);
//		if (!Network.isServer) return;

//		GameControl gameCtrl = GameObject.Find("GameScripts").GetComponent<GameControl>();
//
//		gameCtrl.AddPlayerRobot(this, netView.owner.guid);
	}
}
