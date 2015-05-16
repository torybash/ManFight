using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayoutControl : MonoBehaviour {

	GameControl gameCtrl;


	// Use this for initialization
	void Awake () {
		gameCtrl = GetComponent<GameControl>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public void PlayingOutCommandsUpdate(float playoutTimer, List<RobotCommand>[] allPlayerRobotCommands, Dictionary<int, Robot>[] playerRobots){

//		int moveIdx = (int) ((playoutDuration - playoutTimer) * movesPerSec);
		
		
		for (int playerID = 0; playerID < allPlayerRobotCommands.Length; playerID++) { //Fore player
			List<RobotCommand> playerRobotCommands = allPlayerRobotCommands[playerID];
			foreach (RobotCommand robotCmd in playerRobotCommands) {
				Robot robot = playerRobots[playerID][robotCmd.robotID];

				//Get curr cmd
				Command currCmd = robotCmd.GetCurrentCommand();

				//Is done with curr command?
				bool isCmdCone = IsDoneWithCommand(robot, currCmd);

				//Perform command


				//Check if anything to do under command (aggro move / guard)


				//Get new command if last command over




//				if (robotCmd.turnCommands.Count > moveIdx){ //has a move for this move/time idx
//					Command cmd = robotCmd.turnCommands[moveIdx];
					
					//					robot.transform.Translate(Tools.CommandToVector(cmd) * movesPerSec * Time.fixedDeltaTime);

			}
		}
		if (playoutTimer < 0)
		{
			gameCtrl.PlayedOutCommands();
		}

	}



	bool IsDoneWithCommand(Robot rob, Command cmd){

		return false;
	}
}
