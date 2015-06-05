using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class RobotCommand {

	public int robotID;
	public List<Command> turnCommands = new List<Command>();



	public RobotCommand(){}

	public RobotCommand(int robotID){
		this.robotID = robotID;
	}



	public void AddCommand(Command cmd){
		turnCommands.Add(cmd);
	}
	


	public static float MoveCommandsDuration(List<Vector2> commandList, RobotHeight robotHeight){
		float duration = 0;
		foreach (var item in commandList) {
			duration += CommandDuration(CommandType.MOVE_TO, robotHeight);
		}

		return duration;
	}

	public static float CommandDuration(CommandType cmd, RobotHeight rHeight){
		switch (cmd) {
		case CommandType.MOVE_TO:
			if (rHeight == RobotHeight.HIGH) return 0.5f;
			else return 0.8f;
		case CommandType.GUARD:
			return 0.5f;
		case CommandType.SET_ANGLE:
			return 0.0f;
		case CommandType.CHANGE_WEAPON:
			return 0.2f;
		case CommandType.CHANGE_HEIGHT:
			return 0.1f;
		case CommandType.AGG_MOVE_TO:
			if (rHeight == RobotHeight.HIGH) return 0.5f;
			else return 0.8f;
		default:
			break;
		}


		return float.MaxValue;
	}
}


public class ServerRobotCommand : RobotCommand{
	public int playbackCmdID;
	
	public Command lastCmd;
	
	public List<Vector2> currPath;
	public int currPathIncr;
	public Vector2 currPathNextPos;



//	public ServerRobotCommand(int robotID){
//
//		this.robotID = robotID;
//		//		this.ownerId = ownerId;
//		playbackCmdID = -1;
//		lastCmd = null;
//	}

	public ServerRobotCommand(RobotCommand robCmd){
		this.robotID = robCmd.robotID;
		this.turnCommands = robCmd.turnCommands;

		playbackCmdID = -1;
		lastCmd = null;
	}

	public Command GetCurrentCommand(){
		if (playbackCmdID < 0) return null; //<-- first command
		if (playbackCmdID >= turnCommands.Count) return new Command(CommandType.GUARD, -1, -1, 99999f); //Guard rest of turn
		return turnCommands[playbackCmdID];
	}


	public bool IsOnLastCommand(){
		if (playbackCmdID >= turnCommands.Count) return true;
		return false;
	}

}


[Serializable]
public class Command{

	public CommandType cmdTyp;
	public int goalPosX, goalPosY;
	public float val;

	public Command(CommandType cmdTyp, int goalPosX, int goalPosY, float val){
		this.cmdTyp = cmdTyp;
		this.goalPosX = goalPosX;
		this.goalPosY = goalPosY;
		this.val = val;
	}
}

[Serializable]
public enum CommandType : byte{
	MOVE_TO,
	AGG_MOVE_TO,
	GUARD,
	CHANGE_HEIGHT,
	CHANGE_WEAPON,
	SET_ANGLE,
	NONE
}


[Serializable]
public enum RobotHeight : byte{
	HIGH,
	LOW
}
