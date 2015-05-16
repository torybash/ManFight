using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapTile {
	public List<SpriteType> sprites = new List<SpriteType>();

	//For startPos/startField
	public int playerID = -1;


//	public MapTile(List<SpriteType> sprites, int playerID){
//		this.sprites = sprites;
//		this.playerID = playerID;
//	}

	public MapTile(){
		sprites.Add(SpriteType.EMPTY);
	}

	public override string ToString(){
		string str = "";
		str += playerID + ":";

		for (int i = 0; i < sprites.Count; i++) {
			str += (int) sprites[i];
			if (i < sprites.Count - 1) str += ";";
		}
		return str;
	}
}
