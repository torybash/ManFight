using UnityEngine;
using System.Collections;

public class NetworkPlayerInfo : MonoBehaviour {

	public string playerName;

	public NetworkView netView;

	NetworkManager netManager;
	NetworkManagerControl netManCtrl;

	public Color color;
	public Vector2 initPos;



	public int robotIDIncr = 0;

	void Awake () {
		netView = GetComponent<NetworkView>();

		netManager = GameObject.Find("GameScripts").GetComponent<NetworkManager>();

		DontDestroyOnLoad(gameObject);
	}




	void Update(){
		//DEBUG

		if (Input.GetMouseButtonDown(1)){
			Vector2 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int x = (int) clickPos.x;
			int y = (int) clickPos.y;
			SpawnRobot(x, y);
		}

	}
		


	//GENERAL
	public void Initialize(string localPlayerName){
//		this.playerName = localPlayerName;
		RPCTellOthersName(localPlayerName);

		if (netView.isMine)
			netView.RPC("RPCTellOthersName", RPCMode.OthersBuffered, localPlayerName);


	}


	public void SpawnRobot(int x, int y){
		netView.RPC("RPCSpawnRobot", RPCMode.Server, x, y);
	}

	public void SpawnRobot(){
		netView.RPC("RPCSpawnRobot", RPCMode.Server, (int)initPos.x, (int)initPos.y);
	}




	//SERVER
	public void ApplyStartingPosition(StartPosition startPos){
		color = startPos.color;
		initPos = new Vector2(startPos.transform.position.x, startPos.transform.position.y);

		netView.RPC("RPCApplyStartPosition", RPCMode.All, color.r, color.g, color.b, startPos.transform.position);
	}



	





	//RPCs
//	[RPC]
//	public void RPCSpawnRobot(int posX, int posY){
//		if (!netView.isMine) return;
//
//		Vector3 spawnPos = Tools.AddHalf(new Vector3(posX, posY, 0));
//		Robot robot = ((Transform)Network.Instantiate(robotPrefab, spawnPos, Quaternion.identity, 0)).GetComponent<Robot>();
//		robot.Init(robotIDIncr++, color);
//
//		netManCtrl.SpawnedRobot(robot);
//	}


	[RPC]
	public void RPCTellOthersName(string playerName){
		this.playerName = playerName;

		netManager.PlayerConnected(this);
	}



	[RPC]
	public void RPCApplyStartPosition(float r, float g, float b, Vector3 initPos){
//		Debug.Log("RPCApplyStartPosition - " + r + ", " + g + ", "  + b);
		this.initPos = initPos;
		color = new Color(r, g, b);

		netManCtrl.PlayerHasAqcuiredStartPosition(this);
	}



	//UNITY FUNCTIONS
	void OnLevelWasLoaded(int level) {
		if (Application.loadedLevelName.Equals("ManFight")){
			netManCtrl = GameObject.Find("GameScripts").GetComponent<NetworkManagerControl>();
			netManCtrl.SetPlayer(this, netView.owner.guid);
		}
	}
}
