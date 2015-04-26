using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System; // We will need the Serializable Attribute from here
using System.IO; // This line will enable you the usage of MemoryStreams
using System.Runtime.Serialization.Formatters.Binary; // Finally we add BinaryFormatters

public static class Tools {

	private static Vector2 up = Vector2.up;
	private static Vector2 right = Vector2.right;
	private static Quaternion upRot = Quaternion.AngleAxis(0, Vector3.forward);
	private static Quaternion downRot = Quaternion.AngleAxis(180, Vector3.forward);
	private static Quaternion leftRot = Quaternion.AngleAxis(90, Vector3.forward);
	private static Quaternion rightRot = Quaternion.AngleAxis(270, Vector3.forward);

	public static float RndRange(float x1, float x2){
		return UnityEngine.Random.Range(x1, x2);
	}

	public static int RndRange(int x1, int x2){
		return UnityEngine.Random.Range(x1, x2);
	}
	
	public static Vector2 AddHalf(Vector2 vec){


		int x = 0;
		if (vec.x > 0) x = (int) vec.x;
		else x = (int) vec.x -1;
		int y = 0;
		if (vec.y > 0) y = (int) vec.y;
		else y = (int) vec.y -1;

		return new Vector2(x + 0.5f, y + 0.5f);
	}

	public static T GenericMethod<T>(T param)
	{
		return param;
	}


	public static string ListToString<T>(List<T> list){
		string result = "List: ";

		foreach (var item in list) {
			result += item.ToString() + ", ";
		}

		return result;
	}



	public static Command VectorToCommand(Vector2 vec){
		if (vec.Equals(up)) return Command.MOVE_UP;
		else if (vec.Equals(-up)) return Command.MOVE_DOWN;
		else if (vec.Equals(right)) return Command.MOVE_RIGHT;
		else if (vec.Equals(-right)) return Command.MOVE_LEFT;
		else return Command.NONE;
	}

	public static Vector2 CommandToVector(Command cmd){
		switch (cmd) {
		case Command.MOVE_UP: return up;
		case Command.MOVE_DOWN: return -up;
		case Command.MOVE_RIGHT: return right;
		case Command.MOVE_LEFT: return -right;
		default: return Vector2.zero;
		}
	}

	public static Quaternion CommandToRotation(Command cmd){
		switch (cmd) {
		case Command.MOVE_UP: return upRot;
		case Command.MOVE_DOWN: return downRot;
		case Command.MOVE_RIGHT: return rightRot;
		case Command.MOVE_LEFT: return leftRot;
		default: return Quaternion.identity;
		}
	}



	public static byte[] SerializeObj(List<RobotCommand> robotCommandList){

		BinaryFormatter binFormatter = new BinaryFormatter(); 
		MemoryStream memStream = new MemoryStream();

//		foreach (RobotCommand robotCmd in robotCommandList) {
//			binFormatter.Serialize(memStream, robotCmd);
//		}
		binFormatter.Serialize(memStream, robotCommandList);
		byte[] serializedRobotCommands = memStream.ToArray();

		memStream.Close();

//		PlayerInfo plInfo = new PlayerInfo {nickname = "Alex"}; // Instantiate an object of our type and fill it with data.
//		
//		BinaryFormatter binFormatter = new BinaryFormatter(); // Create Formatter and Stream to process our data
//		MemoryStream memStream = new MemoryStream();
//		
//		binFormatter.Serialize(memStream, plInfo); // We Serialize our plInfo object using the memStream
//		byte[] serializedPlInfo = memStream.ToArray(); // We convert the contents of the stream (which now contains our object) into a byte array.
//		
//		memStream.Close() // Close our stream!

		return serializedRobotCommands;
	}


	public static List<RobotCommand> DeserializeObj(byte[] serializedRobotCommands){
		BinaryFormatter binFormatter = new BinaryFormatter(); // Create Formatter and Stream to process our data
		MemoryStream memStream = new MemoryStream();
		
		/* This line will write the byte data we received into the stream The second parameter specifies the offset, since we want to start at the beginning of the stream we set this to 0.
    * The third   parameter specifies the maximum number of bytes to be written into the stream, so we use the amount of bytes that our data contains by passing the length of our byte array. */
		memStream.Write(serializedRobotCommands,0,serializedRobotCommands.Length); 
		
		/* After writing our data, our streams internal "reader" is now at the last position of the stream. We shift it back to the beginning of our stream so we can start reading from the very    
     *  beginning */
		memStream.Seek(0, SeekOrigin.Begin); 
		
		List<RobotCommand> robotCommandList = (List<RobotCommand>)binFormatter.Deserialize(memStream); // Deserialize our data and Cast it into a PlayerInfo object

		return robotCommandList;
	}



}
