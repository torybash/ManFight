using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayoutControl : MonoBehaviour {

	[SerializeField] Transform bulletPrefab;

	GameControl gameCtrl;
	LevelControl lvlCtrl;

	// Use this for initialization
	void Awake () {
		gameCtrl = GetComponent<GameControl>();
		lvlCtrl = GetComponent<LevelControl>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public void PlayingOutCommandsUpdate(float playoutTimer, List<ServerRobotCommand>[] allPlayerRobotCommands, Dictionary<int, Robot>[] playerRobots){

//		int moveIdx = (int) ((playoutDuration - playoutTimer) * movesPerSec);
		
		bool allRobotsDone = true;
		for (int playerID = 0; playerID < allPlayerRobotCommands.Length; playerID++) { //Fore player
			List<ServerRobotCommand> playerRobotCommands = allPlayerRobotCommands[playerID];
			foreach (ServerRobotCommand robotCmd in playerRobotCommands) {
				Robot robot = playerRobots[playerID][robotCmd.robotID];

//				bool newCommand = false;
//
//				if (robotCmd.playbackCmdID == -1){ //first command
//					robotCmd.playbackCmdID = 0;
//				}

				//Get curr cmd
				Command currCmd = robotCmd.GetCurrentCommand();

				//Is done with curr command?
				bool isCmdDone = IsDoneWithCommand(robot, robotCmd, currCmd);

				if (isCmdDone){
					robotCmd.playbackCmdID++;
					currCmd = robotCmd.GetCurrentCommand();
					InitCommand(robot, robotCmd, currCmd);
				}

				//Perform command
				PerformCommand(robot, robotCmd, currCmd);

				



				//Detect and shoot at enemies
				bool hasTarget = false;
				if (currCmd.cmdTyp == CommandType.AGG_MOVE_TO || currCmd.cmdTyp == CommandType.GUARD){
					Robot closestRob = DetectEnemyRobots(robot, playerID, playerRobots);

					if (closestRob != null){
						hasTarget = true;
						Debug.DrawLine(robot.pos, closestRob.pos);

						//If not rotated, rotate towards enemy
						Vector2 diffVec = closestRob.pos - robot.pos;
						float angle = Mathf.Atan2(diffVec.y, diffVec.x) * Mathf.Rad2Deg - 90f;

						if (Mathf.DeltaAngle(robot.currAngle, angle) > 2f){
							robot.currAngle = Mathf.MoveTowardsAngle(robot.currAngle, angle, 90f * Time.fixedDeltaTime);
							robot.UpdateGunRotation();
						}else{
							robot.currAngle = Mathf.MoveTowardsAngle(robot.currAngle, angle, 90f * Time.fixedDeltaTime);
							robot.UpdateGunRotation();
							if (robot.shootCooldown < 0){ //If rotated, check if gun cooldowned
								//If cooldowned, shoot
								Bullet bullet = ((Transform)Network.Instantiate(bulletPrefab, robot.pos, Quaternion.identity, 0)).GetComponent<Bullet>();
								bullet.Init(playerID, robot.robotID, robot.weaponDmg, angle);

								robot.shootCooldown = robot.weaponCooldown;
							}
						}
					}
				}

				//Rotate aiming (when no target)
				if (!hasTarget && robot.currAngle != robot.goalAngle){
					robot.currAngle = Mathf.MoveTowardsAngle(robot.currAngle, robot.goalAngle, 90f * Time.fixedDeltaTime);
					
					robot.UpdateGunRotation();
				}


				//Check if robot is "done"
				if (!hasTarget && robotCmd.IsOnLastCommand() && currCmd.cmdTyp == CommandType.GUARD && currCmd.val > 1000){

				}else{
					allRobotsDone = false;
				}

				//Change timer(s)
				robot.shootCooldown -= Time.fixedDeltaTime;
			}
		}

		if (allRobotsDone)
		{
			gameCtrl.PlayedOutCommands();
		}

	}

	Robot DetectEnemyRobots(Robot robot, int playerID, Dictionary<int, Robot>[] playerRobots)
	{
		float range = robot.range;

		float closestDist = float.MaxValue;
		Robot closestRobot = null;

		for (int id = 0; id < playerRobots.Length; id++) {
			if (playerID == id) continue;

			foreach (var item in playerRobots[id]) {
				Robot rob = item.Value;

				float dist = Vector2.Distance(robot.pos, rob.pos);

				if (dist < range && dist < closestDist){
					closestDist = dist;
					closestRobot = rob;
				}
			}
		}

		//TODO CHECK IF ACTUALLY VISIBLE THROUGH FOG


		return closestRobot;
	}

	bool IsDoneWithCommand(Robot rob, ServerRobotCommand robCmd, Command cmd){

		if (cmd == null) return true;
//		rob.robotCommand.lastCmd

		switch (cmd.cmdTyp) {
		case CommandType.MOVE_TO:
			if (robCmd.currPathIncr == robCmd.currPath.Count - 1 && IsRobotAtGoal(rob.pos, robCmd.currPathNextPos, robCmd.currPath[robCmd.currPathIncr])) return true;
			break;
		case CommandType.CHANGE_HEIGHT:


			break;
		case CommandType.GUARD:
			float timeForGuarding = cmd.val;

			break;
		case CommandType.SET_ANGLE: //Instant
			return true;
		default:
			break;
		}

		return false;
	}



	void InitCommand(Robot rob, ServerRobotCommand robCmd, Command cmd){
		robCmd.lastCmd = cmd;


		switch (cmd.cmdTyp) {
		case CommandType.MOVE_TO:
			robCmd.currPath = lvlCtrl.AStarPath(rob.pos, new Vector2(cmd.goalPosX, cmd.goalPosY));
			robCmd.currPathIncr = -1;
			break;
		case CommandType.CHANGE_HEIGHT:
			
			
			break;
		case CommandType.GUARD:
//			float timeForGuarding = cmd.val;
			
			break;
		case CommandType.SET_ANGLE:
			rob.SetGoalAngle(cmd.val);
			break;
		default:
			break;
		}
	}



	void PerformCommand(Robot rob, ServerRobotCommand robCmd, Command cmd){
		switch (cmd.cmdTyp) {
		case CommandType.MOVE_TO:

//			Debug.Log("robCmd.currPathIncr: " + robCmd.currPathIncr + ", robCmd.currPath: " + robCmd.currPath + ", " + robCmd.currPath.Count);

			Vector2 nextMove = Vector2.zero;

			if (robCmd.currPathIncr >= 0) nextMove = robCmd.currPath[robCmd.currPathIncr];

//			Debug.Log("PerformCommand - IsRobotAtGoal: "+ IsRobotAtGoal(rob.pos, robCmd.currPathNextPos, nextMove) + ", rob.pos: " + rob.pos + ", robCmd.currPathNextPos: "+ robCmd.currPathNextPos + ", nextMove: "+  nextMove);

			if (robCmd.currPathIncr == -1 || IsRobotAtGoal(rob.pos, robCmd.currPathNextPos, nextMove)){
				robCmd.currPathIncr++;
				nextMove = robCmd.currPath[robCmd.currPathIncr];
				robCmd.currPathNextPos = Tools.CleanPos(new Vector2((int)rob.pos.x + nextMove.x, (int)rob.pos.y + nextMove.y));
			}else{
				nextMove = robCmd.currPath[robCmd.currPathIncr];
			}

			rob.transform.Translate(nextMove * 2f * Time.fixedDeltaTime);





			break;
		case CommandType.CHANGE_HEIGHT:
			
			
			break;
		case CommandType.GUARD:
			//			float timeForGuarding = cmd.val;
			
			break;
		case CommandType.SET_ANGLE:
//			if (rob.currAngle != rob.goalAngle){
//				rob.currAngle = Mathf.MoveTowardsAngle(rob.currAngle,rob. goalAngle, 90f * Time.fixedDeltaTime);
//				
//				rob.UpdateGunRotation();
//			}
////			rob.SetGoalAngle(cmd.val);
//
			break;
		default:
			break;
		}
	}




	private bool IsRobotAtGoal(Vector2 robPos, Vector2 goalPos, Vector2 dir){

		if (dir.x > 0 && robPos.x > goalPos.x) return true;
		else if (dir.x < 0 && robPos.x < goalPos.x) return true;
		else if (dir.y > 0 && robPos.y > goalPos.y) return true;
		else if (dir.y < 0 && robPos.y < goalPos.y) return true;

		return false;
	}
}
