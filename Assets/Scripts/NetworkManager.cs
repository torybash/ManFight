//#define torybashPCMaster


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	NetworkView netView;


	//Constants
	private const string typeName = "TheManFight";

	//Prefabs
	[SerializeField] Transform playerInfoPrefab;


	//Input variables
	public string gameRoomName = "Unnamed Room";
	public string localPlayerName = "Unnamed Player";
	public string gamePort = "25000";

	//Network variables
	private HostData[] hostList;
	List<NetworkPlayerInfo> playerInfos = new List<NetworkPlayerInfo>();
	bool gameStarted = false;
	bool loadingLevel = false;

	//References
	LevelLoadingControl lvlLoadCtrl;
	MenuGUIControl menuGUICtrl;


	void Awake(){
		menuGUICtrl = GetComponent<MenuGUIControl>();
		netView = GetComponent<NetworkView>();
		lvlLoadCtrl = GameObject.Find("LevelLoadingControl").GetComponent<LevelLoadingControl>();

#if torybashPCMaster
		MasterServer.ipAddress = "87.73.120.12";
		MasterServer.port = 23466;
		Network.natFacilitatorIP = "87.73.120.12";
		Network.natFacilitatorPort = 50005;
#endif
	}

	void Start(){
		RefreshHostList();
	}

	void Update(){
		if (!gameStarted) return;

		if(Network.isClient || Network.isServer){
			if(Application.CanStreamedLevelBeLoaded ("ManFight")){
				lvlLoadCtrl.SendLoadLevel();
				loadingLevel = true;
			}else{
				
			}
		}
	}




	public void HostGame(){
		Network.InitializeServer(4, int.Parse(gamePort), !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameRoomName);
	}


	public void RefreshHostList(){
		Debug.Log("Refreshing host list");
		MasterServer.RequestHostList(typeName);
	}

	public void JoinServer(HostData hostData){
		Debug.Log("Joining server");
		Network.Connect(hostData);
	}

	
	public void StartGame(){
		MasterServer.UnregisterHost();

		netView.RPC("GameStarted", RPCMode.Others);
		GameStarted();
	}


	public void RefreshLobbyList(){
		menuGUICtrl.LoadLobby(playerInfos);
	}


	void SpawnPlayerInfo(){
		Vector3 pos = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0);
		NetworkPlayerInfo npi = ((Transform) Network.Instantiate(playerInfoPrefab, pos, Quaternion.identity, 0)).GetComponent<NetworkPlayerInfo>();
		npi.Initialize(localPlayerName);
	}


	public void PlayerConnected(NetworkPlayerInfo npi){
		playerInfos.Add(npi);
		RefreshLobbyList();
	}


	//RPCs
	[RPC]
	public void GameStarted(){
		gameStarted = true;
	}


	//Unity built-in
	void OnPlayerConnected(){
		Debug.Log("OnPlayerConnected");
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent){
		Debug.Log("OnMasterServerEvent");
		if (msEvent == MasterServerEvent.HostListReceived){
			hostList = MasterServer.PollHostList();
			menuGUICtrl.MakeServerList(hostList);
		}
	}
	
	void OnServerInitialized(){
		Debug.Log("Server Initializied");
		
		SpawnPlayerInfo();
		menuGUICtrl.LoadLobby(playerInfos);
	}

	void OnConnectedToServer(){
		Debug.Log("Server Joined");
		
		SpawnPlayerInfo();
		menuGUICtrl.LoadLobby(playerInfos);
	}
}
