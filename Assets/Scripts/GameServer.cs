using UnityEngine;
using System.Collections;

public class GameServer : MonoBehaviour {

	private const string typeName = "UniqueGameName3";

	//Server variables
	string gameRoomName = "Unnamed Room";

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 30;

		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameRoomName);
	}

}
