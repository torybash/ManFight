using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotCommandControl : MonoBehaviour {

	//References
	Robot selectedRobot;
//	Transform currRobotGhost;
	Dictionary<int, Transform> currRobotGhosts = new Dictionary<int, Transform>();
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
	bool on = true;
	bool shootMode = false;

	public float prepTimer;

	void Awake(){
		lvlCtrl = GetComponent<LevelControl>();
		gameCtrl = GetComponent<GameControl>();
		guiCtrl = GetComponent<GameGUIControl>();
		pathArwCtrl = GetComponent<PathArrowControl>();
	}


	void Update () {
		if (!on) return;
		if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

		bool selectedNew = Selecting();
		if (!selectedNew) CommandGiving();

	}


	bool Selecting(){


		if (Input.GetMouseButtonDown(0)){
			//get mouse pos
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit.collider != null && hit.collider.tag.Equals("Robot") && controlledRobots.Contains(hit.collider.GetComponent<Robot>())){
				RobotSelected(hit.collider.GetComponent<Robot>());
				return true;
			}
		}
		return false;
	}

	void RobotSelected(Robot rob){
		selectedRobot = rob;
		SetSelector(Tools.AddHalf(selectedRobot.transform.position));
		guiCtrl.UpdateRobotPanel(selectedRobot);

		UpdatePrepTimer(rob.robotPrepTimer);
	}

	void UpdateRobotPositions(){
		foreach (Robot rob in controlledRobots) {
			rob.UpdatePositionToTime(prepTimer);
		}
	}


	void SetSelector(Vector2 pos){
		if (selectorTrans == null) selectorTrans = (Transform) Instantiate(selectorPrefab);
		selectorTrans.gameObject.SetActive(true);
		selectorTrans.position = pos;
	}

	void CommandGiving(){
		
		if (selectedRobot != null){
//			Debug.Log("selected: " + selectedRobot);
			if (shootMode){
				Vector2 hoverTilePos = Tools.AddHalf(Camera.main.ScreenToWorldPoint(Input.mousePosition));


			}else{

				if (Input.GetMouseButtonDown(0)){
					Vector2 clickedTilePos = Tools.AddHalf(Camera.main.ScreenToWorldPoint(Input.mousePosition));
					List<Vector2> commandList = CalculateRoute(selectedRobot.ghostPosition, clickedTilePos);

					if (commandList != null){
						ValidMoveCommandGiven(commandList, clickedTilePos);
					}
				}
			}
		}
	}


	void ValidMoveCommandGiven(List<Vector2> commandList, Vector2 endPos){

		Command cmd = Command.MOVE_TO;
		float timeForMove = RobotCommand.MoveCommandsDuration(commandList, selectedRobot.robotHeight);

		UpdatePrepTimer(prepTimer + timeForMove);

		selectedRobot.robotCommand.AddCommand(cmd, endPos);

		SetGhostPosition(endPos);

		guiCtrl.UpdateRobotPanel(selectedRobot);
		pathArwCtrl.UpdatePath(selectedRobot, commandList);

	}

	void SetGhostPosition(Vector2 pos){
		selectedRobot.ghostPosition = pos;
		int id = selectedRobot.robotID;
		if (!currRobotGhosts.ContainsKey(id)) currRobotGhosts.Add(id, (Transform) Instantiate(robotGhostPrefab));
		currRobotGhosts[id].GetComponent<RobotGhost>().SetSprite(selectedRobot.color);
		currRobotGhosts[id].position = pos;
	}

	void UpdatePrepTimer(float newPrepTime){
		prepTimer = newPrepTime;
		selectedRobot.robotPrepTimer = prepTimer;
		guiCtrl.PrepTimerChanged(prepTimer);
	}

	public void AddRobot(Robot robot){
		controlledRobots.Add(robot);
	}


	public void StartTurn(int turn){
		on = true;

		foreach (Robot robot in controlledRobots) {
			robot.StartTurn();
		}

		pathArwCtrl.Enable(false);
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


	public void ForceUpdateVisibility(){
		foreach (Robot robot in controlledRobots) {
			robot.ForceUpdateVisibility();
		}
	}

	public void PlayingOutCommands(){
		Enable(false);
	}

	public void ShootModeEnable(bool on){
		shootMode = on;
	}

	public void Enable(bool on){
		this.on = on;
		if (on){
			selectorTrans.gameObject.SetActive(true);

//			currRobotGhost.gameObject.SetActive(true);
//			pathArwCtrl.Enable(true);
		}else{
			selectorTrans.gameObject.SetActive(false);
//			currRobotGhost.gameObject.SetActive(false);

			foreach (Transform roboGhost in currRobotGhosts.Values) {
				GameObject.Destroy(roboGhost.gameObject);
			}
			currRobotGhosts.Clear();
//			pathArwCtrl.Enable(false);
		}
	}
}
