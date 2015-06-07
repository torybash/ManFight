using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuGUIControl : MonoBehaviour {

	NetworkManager networkManager;


	//Prefabs
	[SerializeField] Transform serverBtnPrefab;
	[SerializeField] Transform lobbyPlayerPrefab;

	//GUI References

	//Panels
	[SerializeField] Transform serverListPanel;
	[SerializeField] Transform lobbyListPanel;
	[SerializeField] Transform settingsPanel;
	[SerializeField] Transform makeGamePanel;

	//Lobby
	[SerializeField] Button startGameButton;
	[SerializeField] Transform firstLobbyPlayerPos;

	//Make game
	[SerializeField] InputField roomNameInputField;
	[SerializeField] InputField gamePortInputField;

	//Settings
	[SerializeField] InputField nameInputField;

	//Server list
	[SerializeField] Transform firstServerPos;

	[SerializeField] Transform mainDisabler;



	//Generated GUIs
	Transform[] serverButtons;
	List<Transform> lobbyPlayerBoxes = new List<Transform>();

	void Awake(){
		networkManager = GetComponent<NetworkManager>();

		lobbyListPanel.gameObject.SetActive(false);
		settingsPanel.gameObject.SetActive(false);
		makeGamePanel.gameObject.SetActive(false);
		mainDisabler.gameObject.SetActive(false);
	}



	//INPUT

	//Main
	public void OpenStartGamePanel(bool open){
		makeGamePanel.gameObject.SetActive(open);
		mainDisabler.gameObject.SetActive(open);
	}

	public void OpenSettingsPanel(bool open){
		settingsPanel.gameObject.SetActive(open);
		mainDisabler.gameObject.SetActive(open);
	}

	public void RefreshHostList(){
		networkManager.RefreshHostList();
	}


	public void StartMapEditor(){
		Application.LoadLevel("MapEditor");
	}


	//Lobby
	public void OpenLobbyList(bool open){
		lobbyListPanel.gameObject.SetActive(open);
		mainDisabler.gameObject.SetActive(open);
	}

	public void StartGame(){
		networkManager.StartGame();
	}

	//Make game
	public void SetRoomName(){
		networkManager.gameRoomName = roomNameInputField.text;
	}
	
	public void SetGamePort(){
		networkManager.gamePort = gamePortInputField.text;
	}

	public void HostGame(){
		networkManager.HostGame();
		makeGamePanel.gameObject.SetActive(false);
	}



	//Settings
	public void SetPlayerName(){
		networkManager.localPlayerName = nameInputField.text;
	}
	





	//OUTPUT
	public void MakeServerList(HostData[] hostList){
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
			serverBtnTrans.GetComponent<Button>().onClick.AddListener(() => networkManager.JoinServer(hostData));
			
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
	

	public void LoadLobby(List<NetworkPlayerInfo> playerInfos){
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


}
