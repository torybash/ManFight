using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class RobotCommand {

	public int robotID;
	public List<Command> turnCommands = new List<Command>();

	public int playbackCmdID = 0;


	public RobotCommand(int robotID){
		this.robotID = robotID;
//		this.ownerId = ownerId;
	}



	public void AddCommand(Command cmd, Vector2 pos){
		
	}

	public void AddCommand(Command cmd, float val){
		
	}


	public Command GetCurrentCommand(){
		if (playbackCmdID >= turnCommands.Count) return Command.NONE;
		return turnCommands[playbackCmdID];
	}
	


	public static float MoveCommandsDuration(List<Vector2> commandList, RobotHeight robotHeight){
		float duration = 0;
		foreach (var item in commandList) {
			duration += CommandDuration(Command.MOVE_TO, robotHeight);
		}

		return duration;
	}

	public static float CommandDuration(Command cmd, RobotHeight rHeight){
		switch (cmd) {
		case Command.MOVE_TO:
			if (rHeight == RobotHeight.HIGH) return 0.5f;
			else return 0.8f;
		case Command.GUARD:
			return 0.5f;
		case Command.SET_ANGLE:
			return 0.0f;
		case Command.CHANGE_WEAPON:
			return 0.2f;
		case Command.CHANGE_HEIGHT:
			return 0.1f;
		case Command.AGG_MOVE_TO:
			if (rHeight == RobotHeight.HIGH) return 0.5f;
			else return 0.8f;
		default:
			break;
		}


		return float.MaxValue;
	}
}



//[Serializable]
//public enum Command : byte{
//	MOVE_RIGHT,
//	MOVE_LEFT,
//	MOVE_UP,
//	MOVE_DOWN,
//	GUARD_RIGHT,
//	GUARD_LEFT,
//	GUARD_UP,
//	GUARD_DOWN,
//	NONE
//}

[Serializable]
public enum Command : byte{
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
