using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class MapEditorControl : MonoBehaviour {

	MapTile[][] currLvl;





	//Prefabs
	[SerializeField] Transform rockPrefab;
	[SerializeField] Transform startPosPrefab;
	[SerializeField] Transform startFieldPrefab;

	//Variables
	Transform currBrushPrefab;
	SpriteType currBrushType;

	List<Transform>[][] currLvlObjs;

	string currLvlTitle = "map0";
	string loadLvlTitle = "map0";

	bool hoveringGUI = false;


	int w = 32, h = 32;


	//Inspector values
	[SerializeField] PlayerColor currPlayerID;




	void Start(){
		currBrushPrefab = rockPrefab;
		currPlayerID = PlayerColor.RED;

		currLvl = new MapTile[w][];
		currLvlObjs = new List<Transform>[w][];
		for (int i = 0; i < currLvl.Length; i++) {
			currLvl[i] = new MapTile[h];
			currLvlObjs[i] = new List<Transform>[h];
			for (int j = 0; j < currLvl[i].Length; j++) {
				currLvl[i][j] = new MapTile();
				currLvlObjs[i][j] = new List<Transform>();
			}
		}
	}

	void Update () {



		BrushSelection();
		Drawing();

		hoveringGUI = false;
	}


	void BrushSelection(){

		//Debug
		if (Input.GetKeyDown(KeyCode.Alpha0)){
			currBrushType = SpriteType.EMPTY;
			currBrushPrefab = null;
		}
		if (Input.GetKeyDown(KeyCode.Alpha1)){
			currBrushType = SpriteType.ROCK;
			currBrushPrefab = rockPrefab;
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)){
			currBrushType = SpriteType.STARTING_POS;
			currBrushPrefab = startPosPrefab;
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)){
			currBrushType = SpriteType.STARTING_FIELD;
			currBrushPrefab = startFieldPrefab;
		}

	}


	void Drawing(){

		if (hoveringGUI) return;
//		if ( UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
//		if ( UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1)) return;

		if (Input.GetMouseButton(0)){
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			int posX = (int)mousePos.x; int posY = (int)mousePos.y;
			if (posX < 0 || posX >= currLvl.Length || posY < 0 || posY > currLvl[0].Length) return;
			MapTile mapTile = currLvl[posX][posY];
			
//			Debug.Log("spritesAtPos: " + mapTile);

			if (currBrushType == SpriteType.EMPTY){

				mapTile.sprites.Clear();
				foreach (Transform spTrans in currLvlObjs[posX][posY]) {
					GameObject.Destroy(spTrans.gameObject);
				}
				currLvlObjs[posX][posY].Clear();
			}else{

				if (!mapTile.sprites.Contains(currBrushType)){
					mapTile.sprites.Add(currBrushType);
					mapTile.playerID = -1;
					if (currBrushType == SpriteType.STARTING_POS || currBrushType == SpriteType.STARTING_FIELD)
						mapTile.playerID = (int) currPlayerID;
					
					SpawnSprite(currBrushPrefab, mousePos, mapTile.playerID);
				

				}
			}

		}
	}


	void SpawnSprite(SpriteType spTyp, Vector2 pos, int playerID){
		Transform prefab = null;
		switch (spTyp) {
		case SpriteType.ROCK: prefab = rockPrefab; break;
		case SpriteType.STARTING_POS: prefab = startPosPrefab; break;
		case SpriteType.STARTING_FIELD: prefab = startFieldPrefab; break;
		default: break;
		}

		SpawnSprite(prefab, pos, playerID);
	}

	void SpawnSprite(Transform prefab, Vector2 pos, int playerID){
		Transform sprite = (Transform) Instantiate(prefab, Tools.CleanPos(pos), Quaternion.identity);
		currLvlObjs[(int)pos.x][(int)pos.y].Add(sprite);
		
		if (playerID >= 0){
			sprite.GetComponent<SpriteRenderer>().color = PlayerHelper.IDToColor((int)playerID);
		}
	}


	void SaveMap(){
		IOTools.WriteMap(currLvl, currLvlTitle);
	}



	void LoadMap(){
		ClearMap();

		currLvl = IOTools.ReadMapFile(loadLvlTitle);

		SpawnLevelObjects(currLvl);
	}


	void ClearMap(){
		for (int x = 0; x < currLvlObjs.Length; x++) {
			for (int y = 0; y < currLvlObjs[0].Length; y++) {
				foreach (Transform spTrans in currLvlObjs[x][y]) {
					GameObject.Destroy(spTrans.gameObject);
				}
				currLvlObjs[x][y].Clear();
				currLvl[x][y] = new MapTile();
			}
		}
	}

	void SpawnLevelObjects(MapTile[][] lvl){
		for (int x = 0; x < lvl.Length; x++) {
			for (int y = 0; y < lvl[0].Length; y++) {
				MapTile mapTile = lvl[x][y];


				for (int i = 0; i < mapTile.sprites.Count; i++) {
					SpriteType spTyp = mapTile.sprites[i];
					SpawnSprite(spTyp, new Vector2(x, y), mapTile.playerID);
				}
			}
		}

	}




//	function Start () {
//		try {
//			// Create an instance of StreamReader to read from a file.
//			sr = new StreamReader("TestFile.txt");
//			// Read and display lines from the file until the end of the file is reached.
//			line = sr.ReadLine();
//			while (line != null) {
//				print(line);
//				line = sr.ReadLine();
//			}
//			sr.Close();
//		}
//		catch (e) {
//			// Let the user know what went wrong.
//			print("The file could not be read:");
//			print(e.Message);
//		}
//	}

//	function Start () {
//		// Create an instance of StreamWriter to write text to a file.
//		sw = new StreamWriter("TestFile.txt");
//		// Add some text to the file.
//		sw.Write("This is the ");
//		sw.WriteLine("header for the file.");
//		sw.WriteLine("-------------------");
//		// Arbitrary objects can also be written to the file.
//		sw.Write("The date is: ");
//		sw.WriteLine(DateTime.Now);
//		sw.Close();
//	}

	void OnGUI(){


		if (GUILayout.Button("Clear Map")){
			ClearMap();
		}
		if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) hoveringGUI = true;

		if (GUILayout.Button("Save Map")){
			SaveMap();
		}
		if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) hoveringGUI = true;

		currLvlTitle = GUILayout.TextField(currLvlTitle);
		if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) hoveringGUI = true;

		if (GUILayout.Button("Load Map")){
			LoadMap();
		}
		if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) hoveringGUI = true;

		loadLvlTitle = GUILayout.TextField(loadLvlTitle);
		if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) hoveringGUI = true;

		if (currBrushPrefab != null){
			Texture2D brushTex = currBrushPrefab.GetComponent<SpriteRenderer>().sprite.texture;
			GUILayout.Box(brushTex);
		}else{
			GUILayout.TextField("Erase");
		}
		if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) hoveringGUI = true;

		int playerIDparse = (int) currPlayerID;
		if (int.TryParse(GUILayout.TextField(""+(int)currPlayerID), out playerIDparse)){
			currPlayerID = (PlayerColor) playerIDparse;
		}
		if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition)) hoveringGUI = true;


	}
}


