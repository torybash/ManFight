using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DarknessFogControl : MonoBehaviour {

	[SerializeField] Transform darknessFogPrefab;

	DarknessFog darknessFog;

	LevelControl lvlCtrl;


	byte[,] tilesExplored;
	int w, h;

	SpriteType[][] lvl;


	void Awake(){
		lvlCtrl = GetComponent<LevelControl>();
	}

	// Use this for initialization
	void Start () {
	
		darknessFog = ((Transform) Instantiate(darknessFogPrefab)).GetComponent<DarknessFog>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void InitFog(SpriteType[][] lvl){
		this.lvl = lvl;

		w = lvl.Length; h = lvl[0].Length;
		tilesExplored = new byte[w, h];
		for (int x = 0; x < w; x++) {
			for (int y = 0; y < h; y++) {
				tilesExplored[x,y] = 0;
			}
		}
	}


	public void UpdateVisibility(int x, int y, int visionDist){

		ArrayList visibilityTiles = FindVisibleTiles(x, y, visionDist);

		foreach (TilePos tilePos in visibilityTiles) {
			tilesExplored[tilePos.x, tilePos.y] = 2;
		}

		darknessFog.UpdateDarknessFog(x, y, tilesExplored, w, h);
	}




	ArrayList FindVisibleTiles(int x0, int y0, int range){
		
//		float startTime = Time.realtimeSinceStartup;
		
		HashSet<int> visibleTiles = new HashSet<int>();
		HashSet<int> clearTiles = new HashSet<int>();
		
		
		
		visibleTiles.Add(x0 + (y0 * w));
		clearTiles.Add(x0 + (y0 * w));
		
		
		int numberOfAngles = 96;
		float angleInterval = 360f/numberOfAngles;
		
		for (int i = 0; i < numberOfAngles; i++) {
			float angle = i * angleInterval * Mathf.PI / 180f;
			int goalX = (int) (x0 + 0.5f + (Mathf.Cos(angle)*range));
			int goalY = (int) (y0 + 0.5f + (Mathf.Sin(angle)*range));
			
			ArrayList tilesOnLine = BresenhamLine(x0, y0, goalX, goalY);
			
			foreach (TilePos tilePos in tilesOnLine) {
				visibleTiles.Add(tilePos.x + (tilePos.y * w));
			}
		}
	
		ArrayList list = new ArrayList();
		foreach (int tileID in visibleTiles) {
			list.Add(new TilePos(tileID%w, tileID/h));
		}
		
//		float time = Time.realtimeSinceStartup - startTime;
		
		return list;
		
	}





	ArrayList BresenhamLine(int xStart, int yStart, int xEnd, int yEnd){
		ArrayList tilesOnLine = new ArrayList();
		
		int x, y, sx = 0, sy = 0;
		
		int dx = Mathf.Abs (xEnd - xStart);
		int dy = Mathf.Abs (yEnd - yStart);
		
		if (xStart < xEnd) sx = 1; 
		else if (xStart > xEnd) sx = -1;
		if (yStart < yEnd) sy = 1; 
		else if (yStart > yEnd) sy = -1;
		int err = dx - dy;
		int err2 = 0;
		
		x = xStart;
		y = yStart;
		
		while(true){
			tilesOnLine.Add(new TilePos(x, y));

//				if (x == xEnd && y == yEnd || lvl[x, y] >= (int)Constants.Tiles.METAL) break; //has reached goal?
			if (x == xEnd && y == yEnd || IsBlockingVisibility(x, y)) break; //has reached goal?

			err2 = err * 2;
			if (err2 > -dy){
				err = err - dy;
				x = x + sx;
				
				
				
				tilesOnLine.Add(new TilePos(x, y + sy));


//				if (map[x, y + sy] >= (int)Constants.Tiles.METAL) break;
				if (IsBlockingVisibility(x, y + sy)) break;
			}
//			if (x == xEnd && y == yEnd || map[x, y] >= (int)Constants.Tiles.METAL){ //has reached goal now???
			if (x == xEnd && y == yEnd || IsBlockingVisibility(x, y)){ //has reached goal now???
					tilesOnLine.Add(new TilePos(x, y));
				break;
			}
			if (err2 < dx){
				err = err + dx;
				y = y + sy;
				
				
				
				tilesOnLine.Add(new TilePos(x + sx, y));
				
				if (IsBlockingVisibility(x + sx, y)) break;
			}
		}
		return tilesOnLine;
	}



	bool IsBlockingVisibility(int x, int y){
		if (lvl[x][y] == SpriteType.ROCK) return true;

		return false;
	}
}
