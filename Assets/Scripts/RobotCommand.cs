using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[Serializable]
public class RobotCommand {

	public int robotID;
	public List<Command> turnCommands = new List<Command>();




	public RobotCommand(int robotID){
		this.robotID = robotID;
//		this.ownerId = ownerId;
	}





	public void UpdateList(List<Vector2> path){
		foreach (Vector2 vec in path) {
			turnCommands.Add(Tools.VectorToCommand(vec));
		}
	}




	public List<Command> GetTurnCommands(){
		return turnCommands;
	}
}

[Serializable]
public enum Command : byte{
	MOVE_RIGHT,
	MOVE_LEFT,
	MOVE_UP,
	MOVE_DOWN,
	GUARD_RIGHT,
	GUARD_LEFT,
	GUARD_UP,
	GUARD_DOWN,
	NONE

}