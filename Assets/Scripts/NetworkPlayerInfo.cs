using UnityEngine;
using System.Collections;

public class NetworkPlayerInfo : MonoBehaviour {

	public string playerName;

	public NetworkView netView;

	NetworkManager netManager;
	NetworkManagerControl netManCtrl;

//	public Color color;
	public Vector2 initPos;

	public int playerID = -1;


	public int robotIDIncr = 0;

	void Awake () {
		netView = GetComponent<NetworkView>();

		netManager = GameObject.Find("MenuScripts").GetComponent<NetworkManager>();

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
	public void ApplyStartingPosition(int playerID, Transform startPos){
//		color = startPos.color;
		this.playerID = playerID;
		initPos = new Vector2(startPos.position.x, startPos.position.y);

		netView.RPC("RPCApplyStartPosition", RPCMode.All, playerID, startPos.position);
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
	public void RPCApplyStartPosition(int playerID, Vector3 initPos){
//		Debug.Log("RPCApplyStartPosition - " + r + ", " + g + ", "  + b);
		this.playerID = playerID;
		this.initPos = initPos;

		if (netView.isMine) netManCtrl.PlayerHasAqcuiredStartPosition(this);
	}



	//UNITY FUNCTIONS
	void OnLevelWasLoaded(int level) {
		if (Application.loadedLevelName.Equals("ManFight")){
			netManCtrl = GameObject.Find("GameScripts").GetComponent<NetworkManagerControl>();
			netManCtrl.SetPlayer(this, netView.owner.guid);
		}
	}
}
