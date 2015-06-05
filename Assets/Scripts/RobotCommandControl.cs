using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotCommandControl : MonoBehaviour {

	//References
	Robot selectedRobot;
//	Transform currRobotGhost;
	Dictionary<int, RobotGhost> currRobotGhosts = new Dictionary<int, RobotGhost>();
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
	bool robotsNeedPlacing = false;

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
		if (robotsNeedPlacing) return;

		bool selectedNew = Selecting();
		if (!selectedNew) CommandGiving();

	}


	bool Selecting()
	{
		if (Input.GetMouseButtonDown(0)){
			//get mouse pos
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit.collider != null && hit.collider.tag.Equals("RobotGhost")){

				//Get Robot and check if belonging to player (controlledRobots)
				Robot rob = hit.collider.GetComponent<RobotGhost>().rob;

				if (controlledRobots.Contains(rob)){
					RobotSelected(rob);
				}

				return true;
			}
		}
		return false;
	}

	void RobotSelected(Robot rob)
	{
		Debug.Log("RobotSelected - robID: "+ rob.robotID + ", prep timer: " + rob.robotPrepTimer);

		//Get latest prep time for robot
		float latestMoveTime = rob.GetLatestMoveTime();
		if (rob.robotPrepTimer < latestMoveTime){
			prepTimer = rob.robotPrepTimer;
		}else{
			prepTimer = latestMoveTime;
		}

		selectedRobot = rob;

		UpdateAllRobotsPrepValues();

		guiCtrl.UpdateSelectedRobot(selectedRobot);
		guiCtrl.PrepTimerChanged(prepTimer);
		SetSelector(Tools.CleanPos(selectedRobot.ghostPos));
	}

	void UpdateAllRobotsPrepValues(){
		foreach (Robot robot in controlledRobots) {
			robot.UpdatePrepValues(prepTimer);
		}
	}


	void SetSelector(Vector2 pos){
		if (selectorTrans == null) selectorTrans = (Transform) Instantiate(selectorPrefab);
		selectorTrans.gameObject.SetActive(true);
		selectorTrans.position = pos;
	}

	void CommandGiving()
	{	
		if (selectedRobot != null)
		{

			if (Input.GetMouseButtonDown(0))
			{
				Vector2 clickedTilePos = Tools.CleanPos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
				List<Vector2> commandList = CalculateRoute(selectedRobot.ghostPos, clickedTilePos);

				if (commandList != null)
				{
					float timeForMove = RobotCommand.MoveCommandsDuration(commandList, selectedRobot.robotHeight);
					if (timeForMove + prepTimer <= gameCtrl.playoutDuration){
						ValidMoveCommandGiven(commandList, clickedTilePos, timeForMove);
					}
				}
			}
		}
	}


	void ValidMoveCommandGiven(List<Vector2> commandList, Vector2 endPos, float timeForMove)
	{
		//Is overriding commands?
		bool isOverriding = selectedRobot.GetLatestMoveTime() < prepTimer; //TODO Use in dialog box

		//Add commands 
		Vector2 simulPos = selectedRobot.ghostPos;
		float simulTime = prepTimer;
		float endRot = selectedRobot.ghostRot;
		foreach (Vector2 move in commandList) {
//			Vector2 lastPos = simulPos;
			simulPos += move;
			simulTime += RobotCommand.CommandDuration(CommandType.MOVE_TO, selectedRobot.prepRobotHeight);
			Command cmd = new Command(CommandType.MOVE_TO, (int)simulPos.x, (int)simulPos.y, -1);
			selectedRobot.robotCommand.AddCommand(cmd);
			selectedRobot.SetStateAtTime(simulTime, simulPos);
		}



		//Update prep timer and prep values
		UpdatePrepValues(prepTimer + timeForMove);
		selectedRobot.robotPrepTimer = prepTimer;

		//Visual stuff (and ghostPos)
		guiCtrl.UpdateSelectedRobot(selectedRobot);
		pathArwCtrl.UpdatePath(selectedRobot, commandList, isOverriding);
		SetGhost(selectedRobot, endPos, endRot);
		SetSelector(Tools.CleanPos(selectedRobot.ghostPos));
	}

	public void SetGhost(Robot rob, Vector2 pos, float angle)
	{
		rob.ghostPos = pos;
		rob.ghostRot = angle;
		int robID = rob.robotID;

		if (!currRobotGhosts.ContainsKey(robID)){
			RobotGhost ghost = ((Transform) Instantiate(robotGhostPrefab)).GetComponent<RobotGhost>();
            currRobotGhosts.Add(robID, ghost);
		}
		currRobotGhosts[robID].Refresh(rob.color, rob, pos, angle);
	}

	void UpdatePrepValues(float newPrepTime)
	{
		prepTimer = newPrepTime;
		UpdateAllRobotsPrepValues();

		guiCtrl.PrepTimerChanged(prepTimer);
	}

	public void AddRobot(Robot robot)
	{
		controlledRobots.Add(robot);
	}


	public void TurnStarted(int turn)
	{
		on = true;


		foreach (Robot robot in controlledRobots) {
			robot.TurnStarted();
		}

		UpdatePrepValues(0);
		pathArwCtrl.TurnStarted();
	}

	public List<RobotCommand> GetRobotCommands(){
		List<RobotCommand> list = new List<RobotCommand>();
		foreach (Robot robot in controlledRobots) {
			list.Add(robot.robotCommand);
		}
		return list;
	}




	List<Vector2> CalculateRoute(Vector2 robotPos, Vector2 clickPos)
	{
		List<Vector2> route = new List<Vector2>();
		route = lvlCtrl.AStarPath(robotPos, clickPos);
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
	

	//From GUI
	public void AimAdjustedForCurrRobot(float angle)
	{
		Command cmd = new Command(CommandType.SET_ANGLE, -1, -1, angle);

		//Just overwrite if last command was also SET_ANGLE
		if (selectedRobot.robotCommand.turnCommands.Count > 0 && selectedRobot.robotCommand.turnCommands[selectedRobot.robotCommand.turnCommands.Count - 1].cmdTyp == CommandType.SET_ANGLE){
			selectedRobot.robotCommand.turnCommands[selectedRobot.robotCommand.turnCommands.Count - 1].val = angle;
		}else{
			selectedRobot.robotCommand.AddCommand(cmd);
		}
	}

	public void CheckForAllRobotsPlaced(){
		bool hasPlacedAll = true;
		foreach (Robot rob in controlledRobots) {
			if (rob.needPlacing) hasPlacedAll = false;
		}
		robotsNeedPlacing = !hasPlacedAll;
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

			foreach (RobotGhost roboGhost in currRobotGhosts.Values) {
				GameObject.Destroy(roboGhost.gameObject);
			}
			currRobotGhosts.Clear();
//			pathArwCtrl.Enable(false);
		}
	}
}

