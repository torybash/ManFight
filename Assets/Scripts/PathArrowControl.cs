using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathArrowControl : MonoBehaviour {


	[SerializeField] Transform pathArrowPrefab;
	[SerializeField] PathSprite[] pathSprites;
	//-->
	Dictionary<PathType, PathSprite> pathSpriteDict = new Dictionary<PathType, PathSprite>();

	Dictionary<int, List<MoveDirection>> paths = new Dictionary<int, List<MoveDirection>>();


//	List<Transform> pathArrows = new List<Transform>();


	Dictionary<int, List<Transform>> pathArrows = new Dictionary<int, List<Transform>>();

	GameObject cont;

	void Awake(){
		foreach (PathSprite pathSprite in pathSprites) {
			pathSpriteDict.Add(pathSprite.pathType, pathSprite);
		}
	}

	void Start(){
		cont = new GameObject();
		cont.name = "PathArrowContainer";
	}

	public void UpdatePath(Robot robot, List<Vector2> moveList){
		DestroyArrows(robot);

		//New path addition
		List<MoveDirection> moves = null;
		if (paths.ContainsKey(robot.robotID)) moves = paths[robot.robotID];
		else moves = new List<MoveDirection>();
		List<MoveDirection> newMoves = Tools.VectorListToMoves(moveList);
		foreach (MoveDirection move in newMoves) {
			moves.Add(move);
		}

		Vector2 posIncr = robot.transform.position;
		int c = 0;
		List<Transform> robotPathArrows = new List<Transform>();
//		foreach (Command command in path) {
		for (int i = 0; i < moves.Count; i++) {
			MoveDirection command = moves[i];
			MoveDirection nextCommand = (i == moves.Count - 1 ? MoveDirection.NONE : moves[i + 1]);

		
			


			Quaternion rot = Tools.MoveToRotation(command);
//			Vector2 pos = posIncr; // + 0.5f * Tools.CommandToVector(command);
			Vector2 pos = posIncr + 1f * Tools.CommandToVector(command);

			Transform pathArrow = (Transform) Instantiate(pathArrowPrefab, pos, rot);
			pathArrow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);


			//Set sprite
			PathType pathType = CalculatePathType(command, nextCommand);

			if (i < moves.Count - 1) 
				pathArrow.GetComponent<SpriteRenderer>().sprite = pathSpriteDict[pathType].sprite;
			else 
				pathArrow.GetComponent<SpriteRenderer>().sprite = pathSpriteDict[PathType.END].sprite;

			pathArrow.SetParent(cont.transform);

			posIncr += Tools.CommandToVector(command);
			robotPathArrows.Add(pathArrow);
			c++;
		}

		if (paths.ContainsKey(robot.robotID)){
			paths[robot.robotID] = moves;
		}else{
			paths.Add(robot.robotID, moves);
		}
		pathArrows.Add(robot.robotID, robotPathArrows);
	}



	private PathType CalculatePathType(MoveDirection cmd, MoveDirection nextCmd){

		if (cmd == nextCmd) return PathType.STRAIGHT;

		if (cmd == MoveDirection.UP && nextCmd == MoveDirection.RIGHT) return PathType.RIGHT;
		if (cmd == MoveDirection.RIGHT && nextCmd == MoveDirection.DOWN) return PathType.RIGHT;
		if (cmd == MoveDirection.DOWN && nextCmd == MoveDirection.LEFT) return PathType.RIGHT;
		if (cmd == MoveDirection.LEFT && nextCmd == MoveDirection.UP) return PathType.RIGHT;

		if (cmd == MoveDirection.UP && nextCmd == MoveDirection.LEFT) return PathType.LEFT;
		if (cmd == MoveDirection.RIGHT && nextCmd == MoveDirection.UP) return PathType.LEFT;
		if (cmd == MoveDirection.DOWN && nextCmd == MoveDirection.RIGHT) return PathType.LEFT;
		if (cmd == MoveDirection.LEFT && nextCmd == MoveDirection.DOWN) return PathType.LEFT;

		if (cmd == MoveDirection.UP && nextCmd == MoveDirection.DOWN) return PathType.CIRCLE;
		if (cmd == MoveDirection.RIGHT && nextCmd == MoveDirection.LEFT) return PathType.CIRCLE;
		if (cmd == MoveDirection.DOWN && nextCmd == MoveDirection.UP) return PathType.CIRCLE;
		if (cmd == MoveDirection.LEFT && nextCmd == MoveDirection.RIGHT) return PathType.CIRCLE;

		return PathType.STRAIGHT;
	}

	public void Enable(bool on){
		if(on){

		}else{
			DestroyArrows();
		}
	}

	void DestroyArrows(){
		foreach (List<Transform> pathList in pathArrows.Values){
			if (pathList == null) continue;
			foreach (Transform arw in pathList) {
				if (arw != null) GameObject.Destroy(arw.gameObject);
			}
		}
		
	}

	void DestroyArrows(Robot robot){
		if (!pathArrows.ContainsKey(robot.robotID)) return;
		foreach (Transform arw in pathArrows[robot.robotID]) if (arw != null) GameObject.Destroy(arw.gameObject);
		pathArrows[robot.robotID].Clear();
		pathArrows.Remove(robot.robotID);
	}
}

public enum MoveDirection{
	UP,
	DOWN,
	LEFT,
	RIGHT,
	NONE
}

public enum PathType{
	STRAIGHT,
	RIGHT,
	LEFT,
	CIRCLE,
	END
}

[System.Serializable]
public class PathSprite{
	public PathType pathType;
	public Sprite sprite;
}
