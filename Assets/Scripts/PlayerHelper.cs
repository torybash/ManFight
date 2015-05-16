using UnityEngine;
using System.Collections;

public static class PlayerHelper {





	public static Color IDToColor(int id){
		switch (id) {
		case 0: return Color.red;
		case 1: return Color.blue;
		case 2: return Color.green;
		case 3: return Color.yellow;
		default:
			return Color.white;
		}
	}

}

public enum PlayerColor{
	RED = 0,
	BLUE,
	GREEN,
	YELLOW
}