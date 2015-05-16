using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class LevelLoadingControl : MonoBehaviour {

	public bool hasLoadedLevel = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	NetworkView netView;


	string[] supportedNetworkLevels  = new[]{ "ManFight" };
	string disconnectedLevel = "loader";
	int lastLevelPrefix = 0;

	void Awake ()
	{
		netView = GetComponent<NetworkView>();
		netView.group = (int)NetGroup.LEVEL_LOAD;
//		networkView
		// Network level loading is done in a separate channel.
		DontDestroyOnLoad(this);
//		networkView = GetComponent<NetworkView>();
		
	}

	public void SendLoadLevel(){
		Application.LoadLevel(disconnectedLevel);

		if (Network.peerType != NetworkPeerType.Disconnected)
		{
//			Network.RemoveRPCsInGroup((int)NetGroup.DEFAULT);
//			Network.RemoveRPCsInGroup((int)NetGroup.PLAYER);
//			Network.RemoveRPCsInGroup((int)NetGroup.SERVER);

			netView.RPC( "LoadLevel", RPCMode.AllBuffered, "ManFight", lastLevelPrefix + 1);
		}
	}


	
	[RPC]
	IEnumerator LoadLevel (string level, int levelPrefix)
	{
		lastLevelPrefix = levelPrefix;
		
		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, thus all those objects will get deleted anyway
		Network.SetSendingEnabled((int)NetGroup.DEFAULT, false);
		Network.SetSendingEnabled((int)NetGroup.PLAYER, false);
		Network.SetSendingEnabled((int)NetGroup.SERVER, false);  
		
		// We need to stop receiving because first the level must be loaded first.
		// Once the level is loaded, rpc's and other state update attached to objects in the level are allowed to fire
		Network.isMessageQueueRunning = false;
		
		// All network views loaded from a level will get a prefix into their NetworkViewID.
		// This will prevent old updates from clients leaking into a newly created scene.
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);
		yield return typeof(WaitForEndOfFrame);
		
		// Allow receiving data again
		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data to clients
		Network.SetSendingEnabled((int)NetGroup.DEFAULT, true);
		Network.SetSendingEnabled((int)NetGroup.PLAYER, true);
		Network.SetSendingEnabled((int)NetGroup.SERVER, true);

		GameObject[] gameObjects = (GameObject[]) FindObjectsOfType(typeof(GameObject));
		foreach (var go in gameObjects)
			go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);

		hasLoadedLevel = true;
	}



	
	void OnDisconnectedFromServer ()
	{
		Application.LoadLevel(disconnectedLevel);
	}
}
