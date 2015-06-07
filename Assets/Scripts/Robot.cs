using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour {

	SpriteRenderer[] srs;

	RobotVision robotVision;

	[SerializeField] Transform gunTurretContainer;

	public NetworkView netView;

//	public int movesLeft = 0;


	public RobotCommand robotCommand;


	public Color color;

	//ROBOT VALUES
	public string robName = "UNNAMED ROBOT";
	public int maxMoves = 15;
	public int range = 10;
	public int weaponDmg = 2;
	public float weaponCooldown = 1.0f;
	public float speed = 4f;
	public int initHP = 10;


	public int robotID = -1;

	public int playerId;

	public RobotHeight robotHeight = RobotHeight.HIGH;


	RobotCommandControl rCmdCtrl;
	GameControl gameCtrl;

	bool isMine;


	public float currAngle;
	public float goalAngle;


	//Preperation
	public float prepAngle;
	public bool needPlacing;
	public float robotPrepTimer;
	public RobotHeight prepRobotHeight;

	public float ghostRot;
	public Vector2 ghostPos = Vector2.zero;

	public Vector2 startTurnPos = Vector2.zero;


	public List<RobotHistory> robotHistory = new List<RobotHistory>();
	
	//Playout
	public float shootCooldown;
	public int hp;


	public Vector2 pos {
		get{ return transform.position;}
	}
	
	void Awake () {
		netView = GetComponent<NetworkView>();
		srs = GetComponentsInChildren<SpriteRenderer>();
		robotVision = GetComponent<RobotVision>();
	}
	

	//GENERAL
	public void TurnStarted()
	{
		transform.position = Tools.CleanPos(transform.position);
		startTurnPos = pos;

		robotCommand = new RobotCommand(robotID);
		robotHistory.Clear();
		rCmdCtrl.SetGhost(this, pos, currAngle);
	}	




	//PLAYER
	void Init(int playerId, int robotID)
	{
		robName = "Robot " + (robotID + 1);
		this.playerId = playerId;

		hp = initHP;

		this.robotID = robotID;
		needPlacing = true;
		
		robotCommand = new RobotCommand(robotID);

		rCmdCtrl = GameObject.Find("GameScripts").GetComponent<RobotCommandControl>();
		gameCtrl = GameObject.Find("GameScripts").GetComponent<GameControl>();
		rCmdCtrl.AddRobot(this);
	}

	public void Placed(int x, int y){
		Debug.Log("Robot " + robotID + " placed (name: " + robName + ") , pos: " + pos + ", time: " + Network.time);

		transform.position = Tools.CleanPos(new Vector2(x, y));

		needPlacing = false;
		robotVision.Enabled(true);
		ForceUpdateVisibility();

		ghostPos = pos;
		startTurnPos = pos;

		rCmdCtrl.SetGhost(this, pos, currAngle);

		EnableTransparency(true);

		Debug.Log("Placed - startTurnPos: " + startTurnPos);
	}


	void EnableTransparency(bool on){
		Color col = color;
		col.a = on ? 0.5f : 1f;
		foreach (SpriteRenderer sr in srs) {
			sr.color = col;
		}
	}

	public void ForceUpdateVisibility(){
		robotVision.ForceUpdateVisibility();
	}

	public void SetStateAtTime(float time, Vector2 pos){
		robotHistory.Add(new RobotHistory(time, pos));
	}

	public void UpdatePrepValues(float prepTimer, bool setTime = false)
	{
//		Debug.Log("/////////////////UpdatePrepValues///////////// --- prepTimer: " + prepTimer + " --- id: " + robotID);
	
		if (setTime) robotPrepTimer = prepTimer;

		Vector2 newGhostPos = ghostPos;
		float newGhostRot = ghostRot;
		RobotHistory lastStory = new RobotHistory(0, startTurnPos);
		foreach (RobotHistory robHist in robotHistory) {

//			Debug.Log("robHist: " + robHist.pos + ", " + robHist.time);

			if (robHist.time >= prepTimer){
				if (robHist.time == prepTimer){
					newGhostPos = robHist.pos;
				}else{
					float timeFrac = (prepTimer - lastStory.time) / (robHist.time - lastStory.time);
					Vector2 vecDiff = robHist.pos - lastStory.pos;
					newGhostPos = lastStory.pos + vecDiff * timeFrac;
				}
				break;
			}
			lastStory.time = robHist.time;
			lastStory.pos = robHist.pos;
		}

//		Debug.Log("UpdatePrepValues - newGhostPos: " + newGhostPos + ", ghostPosition: " + ghostPos + ", robotHistory: " + robotHistory.Count);

		rCmdCtrl.SetGhost(this, newGhostPos, newGhostRot); //TODO Simulate angle!

		//Simulate from start
//		Vector2 simulPos = pos;
//		float simulTimer = 0;
//		RobotHeight simulRobotHeight = robotHeight;
//		foreach (Command cmd in robotCommand.turnCommands) {
//			switch (cmd.cmdTyp) {
//			case CommandType.MOVE_TO:
//				float timeForCmd = RobotCommand.MoveCommandsDuration(new List<Vector2>{Vector2.right}, simulRobotHeight);
//				simulTimer += timeForCmd;
//				break;
//			case CommandType.GUARD:
//
//				break;
//			case CommandType.SET_ANGLE:
//
//				break;
//			
//			default:
//			break;
//			}
////			float timeForCmd = RobotCommand.MoveCommandsDuration(
//		}

	}


	public float GetLatestMoveTime(){
		float latestMoveTime = 0;
		if (robotHistory.Count > 0) latestMoveTime = robotHistory[robotHistory.Count - 1].time;
		return latestMoveTime;
	}

	//SERVER
	public void ServerInit(int robotID, Color clr, int playerId, string ownerGUID){
		this.robotID = robotID;
		foreach (SpriteRenderer sr in srs) {
			sr.color = clr;
		}
		netView.RPC("RPCInit", RPCMode.AllBuffered, playerId, robotID, clr.r, clr.g, clr.b, ownerGUID);
	}
	
	public void SetPosition(int x, int y)
	{
		Debug.Log("Server - set rob " + robotID + " from pos " + pos + ", to: " + x + ", " +  y + ", time: " + Network.time);
		transform.position = Tools.CleanPos(new Vector2(x, y));
		ghostPos = transform.position;
	}


	public void SetGoalAngle(float angle){
		goalAngle = angle;
	}


	public void UpdateGunRotation(){
		gunTurretContainer.rotation = Quaternion.AngleAxis(currAngle, Vector3.forward);
	}


//	public void PlayoutUpdate(){
//		if (currAngle != goalAngle){
//			currAngle = Mathf.MoveTowardsAngle(currAngle, goalAngle, 45.00f * Time.fixedDeltaTime);
//
//			gunTurretContainer.rotation = Quaternion.AngleAxis(currAngle, Vector3.forward);
//		}
//	}

	//RPC
	[RPC]
	public void RPCInit(int playerId, int robotID, float r, float g, float b, string ownerGUID){ 
//		Debug.Log("RPCInit (Robot) - rgb: "+  r + ", " + g + ", " + b + ", robotID: " + robotID + ", ownerGUID: " + ownerGUID);

		color = new Color(r, g, b);
		foreach (SpriteRenderer sr in srs) {
			sr.color = color;
		}

		//IS MINE?
		if (ownerGUID.Equals(Network.player.guid)){
			isMine = true;
			Init(playerId, robotID);
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



	void OnTriggerEnter2D(Collider2D other){
		Debug.Log("OnTriggerEnter2D - isServer?: "+  Network.isServer);

		if (!Network.isServer) return;

		if (this == null || gameCtrl == null) return;

		if (other.tag.Equals("Bullet")){

			Bullet bullet = other.GetComponent<Bullet>();

			if (bullet.playerID == playerId) return;

			hp -= bullet.dmg;
			if (hp < 0){
				gameCtrl.DestroyRobot(this);
			}

			Network.Destroy(other.gameObject);


		}
	}
}

public class RobotHistory{
	public float time;
	public Vector2 pos;

	public RobotHistory(float time, Vector2 pos){
		this.time = time;
		this.pos = pos;
	}
}
