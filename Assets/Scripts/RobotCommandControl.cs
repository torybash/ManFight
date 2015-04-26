using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotCommandControl : MonoBehaviour {

	//References
	Robot selectedRobot;
	Transform currRobotGhost;
	Transform selectorTrans;
	LevelControl lvlCtrl;
	GameControl gameCtrl;
	GameGUIControl guiCtrl;
	PathArrowControl pathArwCtrl;

	public List<Robot> controlledRobots = new List<Robot>();

	//Prefabs
	[SerializeField] Transform robotGhostPrefab;
	[SerializeField] Transform selectorPrefab;


	//Variables

	void Awake(){
		lvlCtrl = GetComponent<LevelControl>();
		gameCtrl = GetComponent<GameControl>();
		guiCtrl = GetComponent<GameGUIControl>();
		pathArwCtrl = GetComponent<PathArrowControl>();
	}


	void Update () {
	
		if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

		bool selectedNew = Selecting();
		if (!selectedNew) CommandGiving();

	}


	bool Selecting(){


		if (Input.GetMouseButtonDown(0)){
			//get mouse pos
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit.collider != null) {
				if (hit.collider.tag.Equals("Robot") && controlledRobots.Contains(hit.collider.GetComponent<Robot>())){
					selectedRobot = hit.collider.GetComponent<Robot>();
					SetSelector(Tools.AddHalf(selectedRobot.transform.position));
					guiCtrl.UpdateRobotPanel(selectedRobot);
					return true;
				}
			}
		}
		return false;
	}


	void SetSelector(Vector2 pos){
		if (selectorTrans == null) selectorTrans = (Transform) Instantiate(selectorPrefab);
		selectorTrans.position = pos;
	}

	void CommandGiving(){
		
		if (selectedRobot != null){
//			Debug.Log("selected: " + selectedRobot);

			if (Input.GetMouseButtonDown(0)){
				Vector2 clickedTilePos = Tools.AddHalf(Camera.main.ScreenToWorldPoint(Input.mousePosition));

				List<Vector2> commandList = CalculateRoute(selectedRobot.ghostPosition, clickedTilePos);


//				string commandListString = "CommandList: ";
//				foreach (Vector2 item in commandList) commandListString += item + ", ";
//				print (commandListString + ", clickedPos: "  + clickPos + ", Tools.AddHalf(clickPos): "  + Tools.AddHalf(clickPos));

				if (commandList != null && commandList.Count <= selectedRobot.movesLeft){
					ValidMoveCommandGiven(commandList, clickedTilePos);
				}
			}
		}
	}


	void ValidMoveCommandGiven(List<Vector2> commandList, Vector2 endPos){
		selectedRobot.robotCommand.UpdateList(commandList);
		selectedRobot.movesLeft -= commandList.Count;
		selectedRobot.ghostPosition = endPos;

		if (currRobotGhost == null) currRobotGhost = (Transform) Instantiate(robotGhostPrefab);
		currRobotGhost.position = endPos;
		guiCtrl.UpdateRobotPanel(selectedRobot);

		pathArwCtrl.ShowPath(selectedRobot);

		Debug.Log(Tools.ListToString(selectedRobot.robotCommand.GetTurnCommands()));
	}


	public void AddRobot(Robot robot){
		controlledRobots.Add(robot);
	}


	public void StartTurn(){
		foreach (Robot robot in controlledRobots) {
			robot.StartTurn();
		}
	}

	public List<RobotCommand> GetRobotCommands(){
		List<RobotCommand> list = new List<RobotCommand>();
		foreach (Robot robot in controlledRobots) {
			list.Add(robot.robotCommand);
		}
		return list;
	}

	List<Vector2> CalculateRoute(Vector2 robotPos, Vector2 clickPos){
		List<Vector2> route = new List<Vector2>();


		route = lvlCtrl.AStarPath(robotPos, clickPos);

		//Debug
//		Vector2 diffVector = clickPos - robotPos;
//		int x = (int)diffVector.x;
//		int y = (int)diffVector.y;
//		while (x > 0 || y > 0 || x < 0 || y < 0){
//			if (Mathf.Abs(x) > Mathf.Abs(y)){
//				route.Add(new Vector2(Mathf.Sign(x), 0));
//		        x -= (int)Mathf.Sign(x);
//			}else{
//				route.Add(new Vector2(0, Mathf.Sign(y)));
//				y -= (int)Mathf.Sign(y);
//			}
//		}
		//debug

		return route;
	}
}
