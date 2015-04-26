using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathArrowControl : MonoBehaviour {


	[SerializeField] Transform pathArrowPrefab;


	List<Transform> pathArrows = new List<Transform>();

	public void ShowPath(Robot robot){

		List<Command> path = robot.robotCommand.GetTurnCommands();

		Vector2 posIncr = robot.transform.position;
		foreach (Command command in path) {


			Quaternion rot = Tools.CommandToRotation(command);
			Vector2 pos = posIncr + 0.5f * Tools.CommandToVector(command);

			Transform pathArrow = (Transform) Instantiate(pathArrowPrefab, pos, rot);


			posIncr += Tools.CommandToVector(command);
			pathArrows.Add(pathArrow);
		}

	}
}
