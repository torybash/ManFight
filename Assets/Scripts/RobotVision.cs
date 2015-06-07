using UnityEngine;
using System.Collections;

public class RobotVision : MonoBehaviour {

	bool on = true;

	DarknessFogControl fogCtrl;
		
	Robot robot;

	int lastX, lastY;

	// Use this for initialization
	void Awake () {
		fogCtrl = GameObject.Find("GameScripts").GetComponent<DarknessFogControl>();

		robot = GetComponent<Robot>();
	}

	void Start(){
		RobotCommandControl rCmdCtrl = GameObject.Find("GameScripts").GetComponent<RobotCommandControl>();

		if (!rCmdCtrl.controlledRobots.Contains(robot)) enabled = false;

		lastX = (int) transform.position.x;
		lastY = (int) transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (!on) return;

		int posX = (int)transform.position.x;
		int posY = (int)transform.position.y;

		if (posX != lastX || posY != lastY){
			fogCtrl.UpdateVisibility((int)transform.position.x, (int)transform.position.y, robot.range); 
		}
		lastX = posX;
		lastY = posY;
	}


	public void Enabled(bool on){
		this.on = on;
	}

	public void ForceUpdateVisibility(){
		fogCtrl.UpdateVisibility((int)transform.position.x, (int)transform.position.y, robot.range); 
	}
}
