using UnityEngine;
using System.Collections;

public class MapMaster : MonoBehaviour {

	public TextAsset[] lvlTxts;



	public MapTile[][] GetLevel(int idx){
		return IOTools.ReadMapString(lvlTxts[idx].text);
	}

}
