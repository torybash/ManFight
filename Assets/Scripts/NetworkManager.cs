using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {

	private const string typeName = "UniqueGameName3";


	[SerializeField] Transform serverBtnPrefab;
	[SerializeField] Transform lobbyPlayerPrefab;

	[SerializeField] Transform playerInfoPrefab;


	//Server variables
	string gameRoomName = "Unnamed Room";


	//General use variables
	Transform guiCanvas;

	private HostData[] hostList;

	Transform[] serverButtons;


	string localPlayerName = "Unnamed Player";

	Button startGameButton;

	InputField nameInputField;
	InputField gameRoomInputField;

	Transform serverListPanel;
	Transform firstServerPos;

	Transform lobbyListPanel;
	Transform firstLobbyPlayerPos;


	List<NetworkPlayerInfo> playerInfos = new List<NetworkPlayerInfo>();

	List<Transform> lobbyPlayerBoxes = new List<Transform>();


	NetworkView netView;

	bool gameStarted = false;

	void Awake(){
		netView = GetComponent<NetworkView>();
		
		guiCanvas = GameObject.Find("GUICanvas").transform;
		foreach (Transform child in guiCanvas) {
			if (child.name.Equals("NameInputField")){
				nameInputField = child.GetComponent<InputField>();
			}else if (child.name.Equals("GameRoomInputField")){
				gameRoomInputField = child.GetComponent<InputField>();
			}else if (child.name.Equals("ServerListPanel")){
				serverListPanel = child;
				foreach (Transform gChild in child) {
					if (gChild.name.Equals("FirstServerPos")){
						firstServerPos = gChild;
					}
				}
			}else if (child.name.Equals("LobbyListPanel")){
				lobbyListPanel = child;
				foreach (Transform gChild in child) {
					if (gChild.name.Equals("FirstLobbyPlayerPos")){
						firstLobbyPlayerPos = gChild;
					}else if (gChild.name.Equals("StartGameButton")){
						startGameButton = gChild.GetComponent<Button>();
					}
				}
				lobbyListPanel.gameObject.SetActive(false);
			}
		}
	}



	void Update(){
//		Debug.Log("gameStarted: "+ gameStarted + ", Network.isClient: " + Network.isClient + ", Network.isServer: " + Network.isServer + ", pplication.CanStreamedLevelBeLoaded (ManFight: " + Application.CanStreamedLevelBeLoaded ("ManFight"));
		if (!gameStarted) return;

		if(Network.isClient || Network.isServer){
			if(Application.CanStreamedLevelBeLoaded ("ManFight")){
				Application.LoadLevel("ManFight");
			}else{
				
			}
		}
	}


	public void StartServer()
	{
//		MasterServer.ipAddress = "127.0.0.1";

		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameRoomName);


	
	}



	public void SetPlayerName(){
		localPlayerName = nameInputField.text;
	}

	public void SetRoomName(){
		gameRoomName = gameRoomInputField.text;
	}

	
	public void RefreshHostList()
	{
		Debug.Log("Refreshing host list");
		MasterServer.RequestHostList(typeName);

	}

	private void JoinServer(HostData hostData)
	{
		Debug.Log("Joining server");
		Network.Connect(hostData);

	}


	private void MakeServerList(){
		if (serverButtons != null) return;

		serverButtons = new Transform[hostList.Length];

		for (int i = 0; i < hostList.Length; i++) {
			HostData hostData = hostList[i];

			//Calculate pos
			Vector2 serverBtnPos = firstServerPos.localPosition;
			serverBtnPos.y -= i*50;

			//Make button
			Transform serverBtnTrans = (Transform) Instantiate(serverBtnPrefab);
			serverBtnTrans.SetParent(serverListPanel);
			serverBtnTrans.localPosition = serverBtnPos;
			serverBtnTrans.localScale = Vector3.one;
			serverBtnTrans.GetComponent<Button>().onClick.AddListener(() => JoinServer(hostData));

			//Set button text
			foreach (Transform child in serverBtnTrans) {
				if (child.name.Equals("ServerNameText")){
					child.GetComponent<Text>().text = hostData.gameName;
				}else if (child.name.Equals("ServerCapacityText")){
					child.GetComponent<Text>().text = hostData.connectedPlayers + "/" +  hostData.playerLimit;
				}
			}


			serverButtons[i] = serverBtnTrans;
		}
	}



	void OnGUI(){
		if (GUI.Button(new Rect(0, 0, 200, 40), "Refresh lobby")){
			RefreshLobbyList();
		}

		if (GUI.Button(new Rect(0, 45, 200, 40), "Print Network.connections")){
			foreach (NetworkPlayer np in Network.connections) {
				print ("NetworkPlayer: " + np);
			}
		}

	}


	public void StartGame(){
		netView.RPC("GameStarted", RPCMode.Others);
		GameStarted();
	}

	[RPC]
	public void GameStarted(){
		gameStarted = true;
	}


	public void RefreshLobbyList(){
		LoadLobby();
	}

	void LoadLobby(){
		lobbyListPanel.gameObject.SetActive(true);

		foreach (Transform cr in lobbyPlayerBoxes) {
			GameObject.Destroy(cr.gameObject);
		}

		lobbyPlayerBoxes.Clear();

		int c = 0;
		foreach (NetworkPlayerInfo npi in playerInfos) {

			//Calculate pos
			Vector2 lobbyPlayerBoxPos = firstLobbyPlayerPos.localPosition;
			lobbyPlayerBoxPos.y -= c*50;
			
			//Make boxes
			Transform lobbyPlayerBoxTrans = (Transform) Instantiate(lobbyPlayerPrefab);
			lobbyPlayerBoxTrans.SetParent(lobbyListPanel);
			lobbyPlayerBoxTrans.localPosition = lobbyPlayerBoxPos;
			lobbyPlayerBoxTrans.localScale = Vector3.one;

			//Set button text
			foreach (Transform child in lobbyPlayerBoxTrans) {
				if (child.name.Equals("PlayerNameText")){
					child.GetComponent<Text>().text = npi.playerName;
				}else if (child.name.Equals("PlayerPingText")){
//					child.GetComponent<Text>().text = hostData.connectedPlayers + "/" +  hostData.playerLimit;
				}
			}

			lobbyPlayerBoxes.Add(lobbyPlayerBoxTrans);
			c++;
		}


		if (!Network.isServer) startGameButton.enabled = false;

		
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{

		Debug.Log("OnMasterServerEvent");
		if (msEvent == MasterServerEvent.HostListReceived){
			hostList = MasterServer.PollHostList();
			MakeServerList();
		}
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");

		SpawnPlayerInfo();

		LoadLobby();
	}
	

	
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");

		SpawnPlayerInfo();
		LoadLobby();
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
}
