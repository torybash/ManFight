using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManagerControl : MonoBehaviour {



	//Variables
	List<NetworkPlayerInfo> netPlayers = new List<NetworkPlayerInfo>();
	int playerCount = 0;

	public Dictionary<string, NetworkPlayerInfo> playersGUIDToNPI = new Dictionary<string, NetworkPlayerInfo>();
	public Dictionary<string, int> playersGUIDToIDDict = new Dictionary<string, int>();
	public Dictionary<int, string> playersIDToGUID = new Dictionary<int, string>();

	int playerIncr = 0;

	public bool playersConnected = false;	

	public NetworkPlayerInfo localPlayer;


	//References
	GameControl gameCtrl;
	RobotCommandControl rCmdCtrl;
	CameraControl camCtrl;
	NetworkView netView;
	LevelLoadingControl lvlLoadCtrl;
	GameGUIControl guiCtrl;
	DarknessFogControl fogCtrl;

	//Prefabs
	[SerializeField] Transform robotPrefab;



	void Awake () {
		netView = GetComponent<NetworkView>();
		gameCtrl = GetComponent<GameControl>();
		rCmdCtrl = GetComponent<RobotCommandControl>();
		camCtrl = GetComponent<CameraControl>();
		guiCtrl = GetComponent<GameGUIControl>();
		fogCtrl = GetComponent<DarknessFogControl>();

		lvlLoadCtrl = GameObject.Find("LevelLoadingControl").GetComponent<LevelLoadingControl>();
//		GameObject[] networkPlayersObjs = GameObject.FindGameObjectsWithTag("NetworkPlayer");
//		playerCount = networkPlayersObjs.Length;
//		Debug.Log("networkPlayersObjs length: "  + networkPlayersObjs.Length + ", netPlayers[0]: " + netPlayers[0]);
//
//		for (int i = 0; i < networkPlayersObjs.Length; i++) {
////			netPlayers[i] = networkPlayersObjs[i].GetComponent<NetworkPlayerInfo>();
//		}
	}
	
	void Update () {
		if (!Network.isServer) return;

		if (playersConnected) return;
		if (HasAllPlayersConnected() && lvlLoadCtrl.hasLoadedLevel){
			foreach (string guid in playersGUIDToIDDict.Keys) {
				playersIDToGUID[playersGUIDToIDDict[guid]] = guid;
			}

			playersConnected = true;
			gameCtrl.PlayersConnected();
		}
	}

	//GENERAL
	public void SendPlayerEndedTurn(){
		byte[] srlzdRobotActs = Tools.SerializeObj(rCmdCtrl.GetRobotCommands());

		if (Network.isServer){
			RPCPlayerEndedTurn(netView.owner.guid, srlzdRobotActs);
		}else{
			netView.RPC("RPCPlayerEndedTurn", RPCMode.Server, Network.player.guid, srlzdRobotActs);
		}

	}


	public void PlayerHasAqcuiredStartPosition(NetworkPlayerInfo player){
		camCtrl.SetCameraPosition(player.initPos);

		//DEBUG
		fogCtrl.UpdateVisibility((int)player.initPos.x, (int)player.initPos.y, 3);
	}

	public void RobotPlaced(Robot rob, int x, int y){
		if (Network.isServer){
			RPCRobotPlaced(Network.player.guid, rob.robotID, x, y);
		}else{
			netView.RPC("RPCRobotPlaced", RPCMode.Server, Network.player.guid, rob.robotID, x, y);
		}

	}


	//SERVER
	public void SetPlayer(NetworkPlayerInfo npi, string playerGUID){
		if (npi.netView.isMine) localPlayer = npi;

		netPlayers.Add(npi);
		playersGUIDToNPI.Add(playerGUID, npi);
		playersGUIDToIDDict.Add(playerGUID, playerIncr++);

		Debug.Log("SetPlayer - npi: " + npi + ", playerGUID: " + playerGUID);
	}


	public List<NetworkPlayerInfo> GetPlayers(){
		return netPlayers;
	}

	public void PlayOutCommands(){
		netView.RPC("RPCPlayingOutCommands", RPCMode.AllBuffered);
	}


	bool HasAllPlayersConnected(){
		int amountPlayers = Network.connections.Length + 1;
		if (netPlayers.Count == amountPlayers) return true;
		return false;
	}

	public void ToAllTurnStarted(int turn){
		netView.RPC("RPCTurnStarted", RPCMode.AllBuffered, turn);
	}

	public void SpawnRobotsForPlayer(int playerIdx, int robotCnt){
		NetworkPlayerInfo player = netPlayers[playerIdx];

//		Debug.Log("SpawnRobotsForPlayer - playerIdx: " + playerIdx + ", player.color: " + player.color);



//		Vector3 spawnPos = Tools.AddHalf(new Vector3((int)player.initPos.x, (int)player.initPos.y, 0));
		Vector3 spawnPos = new Vector3(-100, -100, 0);

		Color clr = PlayerHelper.IDToColor(playerIdx);

		for (int i = 0; i < robotCnt; i++) {
			Robot robot = ((Transform)Network.Instantiate(robotPrefab, spawnPos, Quaternion.identity, (int)NetGroup.DEFAULT)).GetComponent<Robot>();
			robot.ServerInit(player.robotIDIncr++, clr, playersIDToGUID[playerIdx]);

			gameCtrl.AddPlayerRobot(robot, player.netView.owner.guid);
		}


//		SpawnedRobotForPlayer(robot, player);

//		StartCoroutine(SendSpawnedRobotForPlayer(robot, player));

//		netView.RPC("RPCSpawnedRobotForPlayer", RPCMode.AllBuffered, robot.netView.viewID, player.netView.owner);
	}


//	IEnumerator SendSpawnedRobotForPlayer(Robot robot, NetworkPlayerInfo player){
//		yield return null;
//		netView.RPC("RPCSpawnedRobotForPlayer", RPCMode.AllBuffered, robot.netView.viewID, player.netView.owner);
//	}

//	public void SpawnedRobotForPlayer(Robot robot, NetworkPlayerInfo player){
//		rCmdCtrl.AddRobot(robot);
//	}





	//RPCS

//	[RPC]
//	void RPCSpawnedRobotForPlayer(NetworkViewID robotViewID, NetworkPlayer networkPlayer){
//
//		if (networkPlayer.Equals(Network.player)){ //is mine?
//			Debug.Log("RPCSpawnedRobotForPlayer (is mine!) - robotViewID: "  +robotViewID);
//			NetworkView robotView = NetworkView.Find(robotViewID);
//			Robot robot = robotView.GetComponent<Robot>();
//			rCmdCtrl.AddRobot(robot);
//		}
//	}

	[RPC]
	void RPCRobotPlaced(string playerGUID, int roboID, int x, int y){
		if (!Network.isServer) return;

		gameCtrl.PlayerRobotPlaced(playerGUID, roboID, x, y);
	}

	[RPC]
	void RPCPlayerEndedTurn(string playerGUID, byte[] srlzdRobotActs){
		if (!Network.isServer) return;

		List<RobotCommand> robotCommands = Tools.DeserializeObj(srlzdRobotActs);

		gameCtrl.PlayerHasEndedTurn(playersGUIDToIDDict[playerGUID], robotCommands);

	}


	[RPC]
	void RPCPlayingOutCommands(){
		guiCtrl.PlayingOutCommands();

//		rCmdCtrl.Enable(false);

		rCmdCtrl.PlayingOutCommands();

	}

	[RPC]
	void RPCTurnStarted(int turn){
		guiCtrl.TurnStarted(turn);

		rCmdCtrl.TurnStarted(turn);

		if (turn == 1) guiCtrl.UpdateRobotPlacingMenu();
	}



//	void OnGUI(){
//		GUI.TextField(new Rect(Screen.width-250, 0, 250, 30), "guid: " + netView.owner.guid);
//		GUI.TextField(new Rect(Screen.width-250, 35, 250, 30), "guid: " + Network.player.guid);
//		GUI.TextField(new Rect(Screen.width-250, 70, 250, 30), "isServer: " + Network.isServer);
//		GUI.TextField(new Rect(Screen.width-250, 105, 250, 30), "isClient: " + Network.isClient);
//		GUI.TextField(new Rect(Screen.width-250, 140, 250, 30), "connections: " + Network.connections.Length);
//		GUI.TextField(new Rect(Screen.width-250, 175, 250, 30), "netPlayers: " + netPlayers.Count);
//		GUI.TextField(new Rect(Screen.width-250, 210, 250, 30), "playersConnected: " + playersConnected);
//
//
//	}
}
