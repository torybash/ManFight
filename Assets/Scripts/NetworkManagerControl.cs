using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkManagerControl : MonoBehaviour {



	//Variables
	List<NetworkPlayerInfo> netPlayers = new List<NetworkPlayerInfo>();
	int playerCount = 0;

	public Dictionary<string, NetworkPlayerInfo> netPlayerDict = new Dictionary<string, NetworkPlayerInfo>();
	public Dictionary<string, int> netPlayerIDDict = new Dictionary<string, int>();

	int playerIncr = 0;

	public bool playersConnected = false;	

	NetworkPlayerInfo localPlayer;

	//References
	GameControl gameCtrl;
	RobotCommandControl rCmdCtrl;
	CameraControl camCtrl;
	NetworkView netView;


	//Prefabs
	[SerializeField] Transform robotPrefab;

	// Use this for initialization
	void Awake () {
		netView = GetComponent<NetworkView>();
		gameCtrl = GetComponent<GameControl>();
		rCmdCtrl = GetComponent<RobotCommandControl>();
		camCtrl = GetComponent<CameraControl>();
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

		if (HasAllPlayersConnected()){
			playersConnected = true;
			gameCtrl.PlayersConnected();
		}
	}

	//GENERAL
	public void SendPlayerEndedTurn(){
		byte[] srlzdRobotActs = Tools.SerializeObj(rCmdCtrl.GetRobotCommands());

		netView.RPC("RPCPlayerEndedTurn", RPCMode.Server, Network.player.guid, srlzdRobotActs);

		if (Network.isServer){
			RPCPlayerEndedTurn(netView.owner.guid, srlzdRobotActs);
		}
	}


	public void PlayerHasAqcuiredStartPosition(NetworkPlayerInfo player){
		camCtrl.SetCameraPosition(player.initPos);
	}


	//SERVER
	public void SetPlayer(NetworkPlayerInfo npi, string playerGUID){
		if (npi.netView.isMine) localPlayer = npi;

		netPlayers.Add(npi);
		netPlayerDict.Add(playerGUID, npi);
		netPlayerIDDict.Add(playerGUID, playerIncr++);

		Debug.Log("SetPlayer - npi: " + npi + ", playerGUID: " + playerGUID);
	}


	public List<NetworkPlayerInfo> GetPlayers(){
		return netPlayers;
	}

	public void PlayOutCommands(){
		netView.RPC("RPCPlayOutCommands", RPCMode.AllBuffered);
	}


	public bool HasAllPlayersConnected(){
		int amountPlayers = Network.connections.Length + 1;
		if (netPlayers.Count == amountPlayers) return true;
		return false;
	}

	public void ToAllTurnStarted(){
		netView.RPC("RPCTurnStarted", RPCMode.AllBuffered);
	}

	public void SpawnRobotForPlayer(int playerIdx){
		NetworkPlayerInfo player = netPlayers[playerIdx];

		Vector3 spawnPos = Tools.AddHalf(new Vector3((int)player.initPos.x, (int)player.initPos.y, 0));
		Robot robot = ((Transform)Network.Instantiate(robotPrefab, spawnPos, Quaternion.identity, 0)).GetComponent<Robot>();
		robot.Init(player.robotIDIncr++, player.color);

		gameCtrl.AddPlayerRobot(robot, player.netView.owner.guid);
//		SpawnedRobotForPlayer(robot, player);

		netView.RPC("RPCSpawnedRobotForPlayer", RPCMode.AllBuffered, robot.netView.viewID, player.netView.owner);
	}

//	public void SpawnedRobotForPlayer(Robot robot, NetworkPlayerInfo player){
//		rCmdCtrl.AddRobot(robot);
//	}





	//RPCS

	[RPC]
	void RPCSpawnedRobotForPlayer(NetworkViewID robotViewID, NetworkPlayer networkPlayer){

		if (networkPlayer.Equals(Network.player)){ //is mine?
			Debug.Log("RPCSpawnedRobotForPlayer (is mine!) - robotViewID: "  +robotViewID);
			NetworkView robotView = NetworkView.Find(robotViewID);
			Robot robot = robotView.GetComponent<Robot>();
			rCmdCtrl.AddRobot(robot);
		}
	}


	[RPC]
	void RPCPlayerEndedTurn(string playerGUID, byte[] srlzdRobotActs){
		if (!Network.isServer) return;

		List<RobotCommand> robotCommands = Tools.DeserializeObj(srlzdRobotActs);

		gameCtrl.PlayerHasEndedTurn(netPlayerIDDict[playerGUID], robotCommands);

	}


	[RPC]
	void RPCPlayOutCommands(){

	}

	[RPC]
	void RPCTurnStarted(){
		gameCtrl.TurnStarted();
	}



	void OnGUI(){
		GUI.TextField(new Rect(0, 0, 250, 30), "guid: " + netView.owner.guid);
		GUI.TextField(new Rect(0, 35, 250, 30), "guid: " + Network.player.guid);
		GUI.TextField(new Rect(0, 70, 250, 30), "isServer: " + Network.isServer);
		GUI.TextField(new Rect(0, 105, 250, 30), "isClient: " + Network.isClient);
		GUI.TextField(new Rect(0, 140, 250, 30), "connections: " + Network.connections.Length);
		GUI.TextField(new Rect(0, 175, 250, 30), "netPlayers: " + netPlayers.Count);
		GUI.TextField(new Rect(0, 210, 250, 30), "playersConnected: " + playersConnected);


	}
}
