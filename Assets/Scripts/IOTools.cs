using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

public static class IOTools  {

	//Constants
	static string mapFolder = "Assets/MapFiles/";

	public static void WriteMap(MapTile[][] currLvl, string lvlTitle){
		
		string path = mapFolder + lvlTitle + ".txt";
		//		if (File.Exists(path)){
		//			Debug.Log("Level with that name already exists!");
		//		}
		
		StreamWriter sw = new StreamWriter(path);
		
		sw.Write(currLvl.Length + "," + currLvl[0].Length + "\n");
		
		for (int y = 0; y < currLvl[0].Length; y++) {
			for (int x = 0; x < currLvl.Length; x++) {
				string tileString = currLvl[x][y].ToString();
				
				if (x == currLvl.Length - 1) tileString += "\n";
				else tileString += ",";
				
				sw.Write(tileString);
			}
		}
		
		sw.Close();
		
	}



	public static MapTile[][] ReadMapString(string mapString){
		MapTile[][] lvl = null;
		int w, h;
		string[] mapStringParts = mapString.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		string[] firstLineParts = mapStringParts[0].Split(",".ToCharArray());

		w = int.Parse(firstLineParts[0]); h =  int.Parse(firstLineParts[1]);
		lvl = new MapTile[w][];
		for (int x = 0; x < w; x++) {
			lvl[x] = new MapTile[h];
			for (int y_ = 0; y_ < h; y_++) {
				lvl[x][y_] = new MapTile();
			}
		}

		for (int y = 1; y < mapStringParts.Length; y++) 
		{
			string[] mapStringPartParts = mapStringParts[y].Split(",".ToCharArray());
			for (int x = 0; x < mapStringPartParts.Length; x++) 
			{
				string tileLine = mapStringPartParts[x];
				string[] tileLineParts = tileLine.Split(":".ToCharArray());

				int playerID = int.Parse(tileLineParts[0]);
				
				List<SpriteType> sprites = new List<SpriteType>();
				string[] spLineSplit = tileLineParts[1].Trim("[]".ToCharArray()).Split("; ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
				//					Debug.Log("tileLine: " + tileLine + ", spLineSplit.Length: " + spLineSplit.Length + ", tileLineParts[1]: " + tileLineParts[1] + ", tileLineParts[1].Trim(.ToCharArray()): " +  tileLineParts[1].Trim("[]".ToCharArray()));
				for (int i = 0; i < spLineSplit.Length; i++) {
					sprites.Add((SpriteType) int.Parse(spLineSplit[i]));
				}

//				Debug.Log("x, y: "+ x + ", " + y);
				
				lvl[x][y-1].playerID = playerID;
				lvl[x][y-1].sprites = sprites;

//				lvl
			}
		}


		return lvl;
	}
	
	public static MapTile[][] ReadMapFile(string lvlTitle){
		MapTile[][] lvl = null;
		
		string path = mapFolder + lvlTitle + ".txt";
		try {
			StreamReader sr = new StreamReader(path);
			
			string line = sr.ReadLine();
			//parse first line
			string[] lineSplit = line.Split(",".ToCharArray());
			int w = int.Parse(lineSplit[0]), h = int.Parse(lineSplit[1]);
			lvl = new MapTile[w][];
			for (int x = 0; x < w; x++) {
				lvl[x] = new MapTile[h];
				for (int y_ = 0; y_ < h; y_++) {
					lvl[x][y_] = new MapTile();
				}
			}
			
			//parse level
			line = sr.ReadLine();
			int y = 0;
			while (line != null) {
				string[] lineParts = line.Split(",".ToCharArray());
				
				for (int x = 0; x < lineParts.Length; x++) {
					string tileLine = lineParts[x];
					string[] tileLineParts = tileLine.Split(":".ToCharArray());
					int playerID = int.Parse(tileLineParts[0]);
					
					List<SpriteType> sprites = new List<SpriteType>();
					string[] spLineSplit = tileLineParts[1].Trim("[]".ToCharArray()).Split("; ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
					//					Debug.Log("tileLine: " + tileLine + ", spLineSplit.Length: " + spLineSplit.Length + ", tileLineParts[1]: " + tileLineParts[1] + ", tileLineParts[1].Trim(.ToCharArray()): " +  tileLineParts[1].Trim("[]".ToCharArray()));
					for (int i = 0; i < spLineSplit.Length; i++) {
						sprites.Add((SpriteType) int.Parse(spLineSplit[i]));
					}
					
					lvl[x][y].playerID = playerID;
					lvl[x][y].sprites = sprites;
				}
				
				y++;
				line = sr.ReadLine();
			}
			sr.Close();
		}
		catch (IOException e) {
			//			// Let the user know what went wrong.
			Debug.Log("The file could not be read:");
			Debug.Log(e.Message);
		}
		//
		return lvl;
	}
}
