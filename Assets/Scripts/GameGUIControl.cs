using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameGUIControl : MonoBehaviour {

	//Refernces
	GameControl gameCtrl;
	NetworkManagerControl netManCtrl;

	Transform guiCanvas;

	Transform robotPanel;
	Text robotNameText;
	Text movesLeftText;

	void Awake () {
		gameCtrl = GetComponent<GameControl>();
		netManCtrl = GetComponent<NetworkManagerControl>();

		guiCanvas = GameObject.Find("GUICanvas").transform;
		foreach (Transform child in guiCanvas) {
			if (child.name.Equals("CurrRobotPanel")){
				robotPanel = child;

				foreach (Transform gChild in child) {
					if (gChild.name.Equals("MovesLeftText")){
						movesLeftText = gChild.GetComponent<Text>();
					}else if (gChild.name.Equals("RobotNameText")){
						robotNameText = gChild.GetComponent<Text>();	
					}
				}
			}
		}

		robotPanel.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	public void PlayerEndTurn(){
		netManCtrl.SendPlayerEndedTurn();
	}

	public void UpdateRobotPanel(Robot robot){
		robotPanel.gameObject.SetActive(true);

		movesLeftText.text = "Moves: " + robot.movesLeft + "/" + robot.maxMoves;
	}

}
