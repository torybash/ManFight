using UnityEngine;
using System.Collections;

public class RobotVision : MonoBehaviour {

	bool isMine = false;

	DarknessFogControl fogCtrl;

	int visionDist;

	// Use this for initialization
	void Start () {
		RobotCommandControl rCmdCtrl = GameObject.Find("GameScripts").GetComponent<RobotCommandControl>();
		fogCtrl = GameObject.Find("GameScripts").GetComponent<DarknessFogControl>();
		Robot robot = GetComponent<Robot>();
		visionDist = robot.range;
		if (rCmdCtrl.controlledRobots.Contains(robot)) isMine = true;
		else gameObject.SetActive(false);

	}
	
	// Update is called once per frame
	void Update () {
		fogCtrl.UpdateVisibility((int)transform.position.x, (int)transform.position.y, visionDist); 
	}
}
