using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {


	LevelControl lvlCtrl;
	CameraControl camCtrl;

	NetworkManagerControl netManCtrl;
	RobotCommandControl rCmdCtrl;
	GameGUIControl guiCtrl;
	PlayoutControl playCtrl;

	int turn = 0;
	float turnTimer = 0;



	bool[] playersEndedTurn;

	List<ServerRobotCommand>[] allPlayerRobotCommands;

	Dictionary<int, Robot>[] playerRobots;


//	delegate void State();
//	State prepState;
//	State playoutState;

	public bool playingOutCommands = false;
	public float playoutTimer = 0;


	//Constants
	public float playoutDuration = 10f;
	const float movesPerSec = 2f;

	void Awake () {
		lvlCtrl = GetComponent<LevelControl>();
		camCtrl = GetComponent<CameraControl>();
		playCtrl = GetComponent<PlayoutControl>();

		netManCtrl = GetComponent<NetworkManagerControl>();
		rCmdCtrl = GetComponent<RobotCommandControl>();
	}

	void Start(){
		lvlCtrl.SpawnLevel();
//		camCtrl.InitCamera(lvlCtrl.width, lvlCtrl.height);
	}





	void FixedUpdate(){
		if (!Network.isServer) return;
		if (!netManCtrl.playersConnected) return;

		if (playingOutCommands){
			playoutTimer -= Time.fixedDeltaTime;
			playCtrl.PlayingOutCommandsUpdate(playoutTimer, allPlayerRobotCommands, playerRobots);
		}
	}
	

	//SERVER
	


	void ApplyStartingPositions(){
		for (int id = 0; id < netManCtrl.GetPlayers().Count; id++) {

			Transform startPos = lvlCtrl.GetStartingPosition(id);
			netManCtrl.GetPlayers()[id].ApplyStartingPosition (id, startPos);
		}
	}

	void SpawnRobots(){
		for (int id = 0; id < netManCtrl.GetPlayers().Count; id++) {
			
			netManCtrl.SpawnRobotsForPlayer(id, 3);
//			netManCtrl.GetPlayers()[i].SpawnRobot();
		}
	}


	void CheckForEndTurn(){
		bool allEnded = true;
		for (int i = 0; i < playersEndedTurn.Length; i++) {
			if (!playersEndedTurn[i]) allEnded = false;
		}

		if (allEnded) TurnEnded();

	}


	void TurnEnded(){
		netManCtrl.PlayOutCommands();
	
	
		PlayOutCommands();
	}


	void PlayOutCommands(){


		playingOutCommands = true;
		playoutTimer = playoutDuration;
	}

	public void PlayedOutCommands()
	{
		playingOutCommands = false;
		StartNextTurn();
	}
	
	public void PlayersConnected(){

		InitGame();

	}

	void InitGame(){


		int playerCount = netManCtrl.GetPlayers().Count;
		playersEndedTurn = new bool[playerCount];
		allPlayerRobotCommands = new List<ServerRobotCommand>[playerCount];
		playerRobots = new Dictionary<int, Robot>[playerCount];
		for (int i = 0; i < playerCount; i++) {
			allPlayerRobotCommands[i] = new List<ServerRobotCommand>();
			playerRobots[i] = new Dictionary<int, Robot>();
		}

		ApplyStartingPositions();
		SpawnRobots();

		StartNextTurn();
	}

	void StartNextTurn(){
		turn++;
		netManCtrl.ToAllTurnStarted(turn);
	}


	public void PlayerHasEndedTurn(int playerID, List<RobotCommand> robotCommands){
//		Debug.Log("PlayerHasEndedTurn - playerGUID: " + playerGUID);
		playersEndedTurn[playerID] = true;

		List<ServerRobotCommand> srvRobotCommands = new List<ServerRobotCommand>();
		foreach (RobotCommand robCmd in robotCommands) {
			srvRobotCommands.Add(new ServerRobotCommand(robCmd));
		}
		allPlayerRobotCommands[playerID] = srvRobotCommands;
		CheckForEndTurn();
	}


	public void AddPlayerRobot(Robot robot, string playerGUID){
		Debug.Log("AddPlayerRobot - playerRobots: " + playerRobots + ", playerRobots.Length: " + playerRobots.Length + ", robot: "  + robot + ", robot.robotID: " + robot.robotID);
		int playerID = netManCtrl.playersGUIDToIDDict[playerGUID];
		playerRobots[playerID].Add(robot.robotID, robot);

	}


	public void PlayerRobotPlaced(string playerGUID, int roboID, int x, int y){
		int playerID = netManCtrl.playersGUIDToIDDict[playerGUID];
		Robot rob = playerRobots[playerID][roboID];
		rob.SetPosition(x, y);

		netManCtrl.UpdateRobotForPlayer(playerGUID, roboID, x, y);
	}



	public void DestroyRobot(Robot rob){

		playerRobots[rob.playerId].Remove(rob.robotID);

		foreach (ServerRobotCommand item in allPlayerRobotCommands[rob.playerId]) {
			if (item.robotID == rob.robotID){
				allPlayerRobotCommands[rob.playerId].Remove(item);
				break;
			}
		}

		netManCtrl.RobotDestroyed(rob);

		Network.Destroy(rob.gameObject);
	}





}
