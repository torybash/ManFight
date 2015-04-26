using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {

	LevelControl lvlCtrl;
	CameraControl camCtrl;

	NetworkManagerControl netManCtrl;
	RobotCommandControl rCmdCtrl;

	int turn = 0;
	float turnTimer = 0;



	bool[] playersEndedTurn;

	List<RobotCommand>[] allPlayerRobotCommands;

	Dictionary<int, Robot>[] playerRobots;

	public bool playingOutCommands = false;
	public float playoutTimer = 0;


	//Constants
	const float playoutDuration = 10f;
	const float movesPerSec = 2f;

	void Awake () {
		lvlCtrl = GetComponent<LevelControl>();
		camCtrl = GetComponent<CameraControl>();

		netManCtrl = GetComponent<NetworkManagerControl>();
		rCmdCtrl = GetComponent<RobotCommandControl>();
	}

	void Start(){
		lvlCtrl.SpawnLevel();


		camCtrl.InitCamera(lvlCtrl.width, lvlCtrl.height);
//		camCtrl.SetCameraPosition();



	}





	void FixedUpdate(){
		if (!Network.isServer) return;
		if (!netManCtrl.playersConnected) return;

		if (playingOutCommands){
			PlayingOutCommandsUpdate();
		}
	}

	//GENERAL

	public void TurnStarted(){
		rCmdCtrl.StartTurn();
	}


	//SERVER

	void PlayingOutCommandsUpdate(){
		Debug.Log("PlayingOutCommandsUpdate - playoutTimer: " + playoutTimer);

		playoutTimer -= Time.fixedDeltaTime;

		int moveIdx = (int) ((playoutDuration - playoutTimer) * movesPerSec);


		for (int i = 0; i < allPlayerRobotCommands.Length; i++) { //Fore player
			List<RobotCommand> playerRobotCommands = allPlayerRobotCommands[i];
			
			foreach (RobotCommand robotCmd in playerRobotCommands) {
				Debug.Log("robotCmd.robotID: "+ robotCmd.robotID);
				Robot robot = playerRobots[i][robotCmd.robotID];
				//				robot.PlayCommands(robotCmd);

				if (robotCmd.turnCommands.Count > moveIdx){ //has a move for this move/time idx
					Command cmd = robotCmd.turnCommands[moveIdx];
					
					robot.transform.Translate(Tools.CommandToVector(cmd) * movesPerSec * Time.fixedDeltaTime);
				}
//				



			}
			
		}

		if (playoutTimer < 0) playingOutCommands = false;
	}


	void ApplyStartingPositions(){
		for (int i = 0; i < netManCtrl.GetPlayers().Count; i++) {
			StartPosition startPos = lvlCtrl.PollRandomStartingPos();
			netManCtrl.GetPlayers()[i].ApplyStartingPosition (startPos);
		}
	}

	void SpawnRobots(){
		for (int i = 0; i < netManCtrl.GetPlayers().Count; i++) {
			netManCtrl.SpawnRobotForPlayer(i);
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
//		netManCtrl.PlayOutCommands();
	
	
		PlayOutCommands();
	}


	void PlayOutCommands(){


		playingOutCommands = true;
		playoutTimer = playoutDuration;
	}

	
	public void PlayersConnected(){

		InitGame();

	}

	void InitGame(){


		int playerCount = netManCtrl.GetPlayers().Count;
		playersEndedTurn = new bool[playerCount];
		allPlayerRobotCommands = new List<RobotCommand>[playerCount];
		playerRobots = new Dictionary<int, Robot>[playerCount];
		for (int i = 0; i < playerCount; i++) {
			allPlayerRobotCommands[i] = new List<RobotCommand>();
			playerRobots[i] = new Dictionary<int, Robot>();
		}

		ApplyStartingPositions();
		SpawnRobots();

		StartTurn();
	}

	void StartTurn(){
		netManCtrl.ToAllTurnStarted();
	}


	public void PlayerHasEndedTurn(int playerID, List<RobotCommand> robotCommands){
//		Debug.Log("PlayerHasEndedTurn - playerGUID: " + playerGUID);
		playersEndedTurn[playerID] = true;
		allPlayerRobotCommands[playerID] = robotCommands;
		CheckForEndTurn();
	}


	public void AddPlayerRobot(Robot robot, string playerGUID){
		Debug.Log("AddPlayerRobot - playerRobots: " + playerRobots + ", playerRobots.Length: " + playerRobots.Length + ", robot: "  + robot + ", robot.robotID: " + robot.robotID + ", playerGUID: "+ playerGUID);
		int playerID = netManCtrl.netPlayerIDDict[playerGUID];
		playerRobots[playerID].Add(robot.robotID, robot);

	}
	
}
