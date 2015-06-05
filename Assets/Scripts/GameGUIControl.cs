using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GameGUIControl : MonoBehaviour {

	//Prefabs
	[SerializeField] Transform panelRobotPrefab;
	[SerializeField] Transform floatingRobotPrefab;

	//Refernces
	GameControl gameCtrl;
	NetworkManagerControl netManCtrl;
	RobotCommandControl rCmdCtrl;
	LevelControl lvlCtrl;

	//GUI referrences
	Transform guiCanvas;

	Transform robotPanel;
	Text robotNameText;
	Text movesLeftText;
	Button shootModeButton;

	Transform generalInfoPanel;
	Text turnText;

	Transform robotPlacingPanel;
	Transform firstRobotPosition;

	[SerializeField] Slider prepTimerSlider;
	[SerializeField] Text prepTimerText;
	[SerializeField] Image aimImage;
	[SerializeField] Button endTurnButton;

	//Variables

	Transform[] placingRobots;

	Transform floatingRobotImage;
	Robot floatingRobot;

	bool endTurnClicked = false;

	void Awake () {
		gameCtrl = GetComponent<GameControl>();
		netManCtrl = GetComponent<NetworkManagerControl>();
		rCmdCtrl = GetComponent<RobotCommandControl>();
		lvlCtrl = GetComponent<LevelControl>();

		guiCanvas = GameObject.Find("GUICanvas").transform;
		foreach (Transform child in guiCanvas) {
			if (child.name.Equals("CurrRobotPanel")){
				robotPanel = child;

				foreach (Transform gChild in child) {
					if (gChild.name.Equals("MovesLeftText")){
						movesLeftText = gChild.GetComponent<Text>();
					}else if (gChild.name.Equals("RobotNameText")){
						robotNameText = gChild.GetComponent<Text>();	
					}else if (gChild.name.Equals("ShootModeButton")){
						shootModeButton = gChild.GetComponent<Button>();
					}
				}
			}else if (child.name.Equals("GeneralInfoPanel")){
				generalInfoPanel = child;

				foreach (Transform gChild in child) {
					if (gChild.name.Equals("TurnText")){
						turnText = gChild.GetComponent<Text>();
					}
				}
			}else if (child.name.Equals("RobotPlacingPanel")){
				robotPlacingPanel = child;
				foreach (Transform gChild in robotPlacingPanel) {
					if (gChild.name.Equals("FirstRobotPosition")){
						firstRobotPosition = gChild;
					}
				}
			}
		}

		robotPanel.gameObject.SetActive(false);
	}


	void Update(){


		FloatingPlacingRobot();
	}


	void FloatingPlacingRobot(){
		
		if (floatingRobotImage != null){
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			pos.z = 0;
			floatingRobotImage.position = pos;
			
			if (Input.GetMouseButtonUp(0)){

				int x = (int) pos.x; int y = (int) pos.y;

				if (lvlCtrl.IsValidRobotPlacement(x, y)){
					rCmdCtrl.CheckForAllRobotsPlaced();
					netManCtrl.RobotPlaced(floatingRobot, x, y);
//					floatingRobot.Placed(x, y);
					lvlCtrl.RobotAddedTo(x, y);
				}else{
					floatingRobot.needPlacing = true;
					UpdateRobotPlacingMenu();
				}

				Debug.Log("GameObject.Destroy(floatingRobotImage.gameObject))");
				GameObject.Destroy(floatingRobotImage.gameObject);
				floatingRobotImage = null;


			}
		}
	}



	//OUT 
	public void PlayerEndTurn(){
		if (endTurnClicked) return;
		endTurnClicked = true;
		netManCtrl.SendPlayerEndedTurn();
	}

//	public void ShootModeForCurrentRobot(bool on){
//		rCmdCtrl.ShootModeEnable(on);
//
//		if (on){
//
//
//			shootModeButton.GetComponentInChildren<Text>().text = "Shoot mode";
//			shootModeButton.onClick.AddListener(() => ShootModeForCurrentRobot(false));
//
//
//		}else{
//			shootModeButton.GetComponentInChildren<Text>().text = "Walk mode";
//			shootModeButton.onClick.AddListener(() => ShootModeForCurrentRobot(true));
//		}
//	}

	public void PrepTimerSliderChanged(){
//		prepTimerSlider
		rCmdCtrl.prepTimer = prepTimerSlider.value;
		prepTimerText.text = "Time: " + prepTimerSlider.value;
	}


	public void AimAdjusted(){
		//Detect click pos
		Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 aimImagePos = aimImage.rectTransform.position;

		//Calc aiming angle
		Vector2 diffVec = clickPos - aimImagePos;

		float angle = Mathf.Atan2(diffVec.y, diffVec.x) * Mathf.Rad2Deg - 90f;
		UpdateAimGUI(angle);
		rCmdCtrl.AimAdjustedForCurrRobot(angle);
	}


	//IN
	public void PrepTimerChanged(float timer){
		prepTimerSlider.value = timer;
		prepTimerText.text = "Time: " + timer;
	}

	public void TurnStarted(int turn){
		turnText.text = "Turn: " + turn;

		generalInfoPanel.gameObject.SetActive(true);
		endTurnButton.gameObject.SetActive(true);

		endTurnClicked = false;
	}

	
	public void UpdateSelectedRobot(Robot robot){
		robotNameText.text = robot.robName;
		robotPanel.gameObject.SetActive(true);
		UpdateAimGUI(robot.prepAngle);
//		movesLeftText.text = "Moves: " + robot.movesLeft + "/" + robot.maxMoves;
	}
	
	
	public void PlayingOutCommands()
	{
//		generalInfoPanel.
		
		generalInfoPanel.gameObject.SetActive(false);
		robotPanel.gameObject.SetActive(false);
		endTurnButton.gameObject.SetActive(false);

	}


	public void UpdateRobotPlacingMenu(){
		Debug.Log("UpdateRobotPlacingMenu");

		robotPlacingPanel.gameObject.SetActive(true);
		List<Robot> robots = rCmdCtrl.controlledRobots;

		if (placingRobots == null) placingRobots = new Transform[robots.Count]; //First update

		List<Robot> stillNeedPlacementRobots = new List<Robot>();

		foreach (Robot rob in robots) {
			if (rob.needPlacing) stillNeedPlacementRobots.Add(rob);
			else placingRobots[rob.robotID].gameObject.SetActive(false);
		}

		if (stillNeedPlacementRobots.Count == 0){
			robotPlacingPanel.gameObject.SetActive(false);
			return;
		}

		Debug.Log("stillNeedPlacementRobots size: " + stillNeedPlacementRobots.Count);
		int c = 0;
		foreach (Robot rob in stillNeedPlacementRobots) {
			Transform robTrans = null;
			if (placingRobots[rob.robotID] == null){
				robTrans = (Transform) Instantiate(panelRobotPrefab);
				placingRobots[rob.robotID] = robTrans;
			}else{
				robTrans = placingRobots[rob.robotID];
				robTrans.gameObject.SetActive(true);
			}

			robTrans.SetParent(robotPlacingPanel);
			robTrans.localPosition = firstRobotPosition.localPosition;
			robTrans.localPosition += Vector3.right * c * 60f;
			robTrans.localScale = Vector3.one;

			EventTrigger trigger = robTrans.GetComponent<EventTrigger>();
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerDown;
			entry.callback.AddListener( (eventData) => { PlacingRobotPointerDown(rob, robTrans); } );
			trigger.delegates.Clear();
			trigger.delegates.Add(entry);

			c++;
		}
	}

	void PlacingRobotPointerDown(Robot rob, Transform panelImage){
		Debug.Log("PlacingRobotPointerDown - rob.id: " + rob.robotID + ", floatingRobotImage: " + floatingRobotImage);
		if (floatingRobotImage != null){
			return;
		}

		rob.needPlacing = false;
		Vector3 pos = panelImage.position;
		pos.z = 0;
		floatingRobotImage = (Transform) Instantiate(floatingRobotPrefab, pos, Quaternion.identity);
		floatingRobot = rob;

		UpdateRobotPlacingMenu();
	}




	//INTERNAL
	void UpdateAimGUI(float angle){
		float rest = angle % 45f;
		if (rest > 22.5f){
			angle = angle - rest + 45f;
		}else{
			angle = angle - rest;
		}
		aimImage.rectTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
	}
}
