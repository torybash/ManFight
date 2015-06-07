using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelCreatorControl : MonoBehaviour {


	//Prefabs
	[SerializeField] Transform tileSpritePrefab;
//	[SerializeField] Transform rockPrefab;
//	[SerializeField] Transform startPosPrefab;
//	[SerializeField] Transform startFieldPrefab;

	[SerializeField] List<TileSprite> tileSprites;
	//-->
	Dictionary<SpriteType, TileSprite> tileSpriteDict = new Dictionary<SpriteType, TileSprite>();

	GameObject lvlCont;

	LevelControl lvlCtrl;

	void Awake(){
		lvlCtrl = GetComponent<LevelControl>();

		lvlCont = new GameObject();
		lvlCont.name = "LevelContainer";

		foreach (TileSprite tileSprite in tileSprites) {
			tileSpriteDict.Add(tileSprite.spriteType, tileSprite);
		}
	}

	void Start(){




	}

	public void SpawnLevel(MapTile[][] lvl){
		//TODO Shud maek mesj of dis
		for (int y = 0; y < lvl[0].Length; y++) {
			for (int x = 0; x < lvl.Length; x++) {
				foreach (SpriteType spTyp in lvl[x][y].sprites) 
				{
					//Create prefab
					Transform spTrans = (Transform) Instantiate(tileSpritePrefab, LevelControl.LevelToWorldPos(x, y), Quaternion.identity);

//					Debug.Log("spTrans: " + spTrans + ", lvlCont: " +  lvlCont + ", spTyp: " + spTyp);
					spTrans.SetParent(lvlCont.transform);

					//Set sprite
					//TODO MAKE BETTER

					if (spTyp == SpriteType.EMPTY){
						spTrans.GetComponent<SpriteRenderer>().sortingOrder = -1;
					}
						
					if (spTyp == SpriteType.ROCK){
						//if rock exist below, use sprite 2
						if (y - 1 > 0 && lvl[x][y-1].sprites.Contains(SpriteType.ROCK)){
							spTrans.GetComponent<SpriteRenderer>().sprite = tileSpriteDict[spTyp].sprite2;
						}else{
							spTrans.GetComponent<SpriteRenderer>().sprite = tileSpriteDict[spTyp].sprite;
						}
					}else{
						spTrans.GetComponent<SpriteRenderer>().sprite = tileSpriteDict[spTyp].sprite;
					}


					if (spTyp == SpriteType.STARTING_POS)
					{
						spTrans.GetComponent<SpriteRenderer>().color = PlayerHelper.IDToColor(lvl[x][y].playerID);
//						spTrans.GetComponent<StartPosition>().Init(PlayerHelper.IDToColor(lvl[x][y].playerID));
						lvlCtrl.startingPositions.Add(lvl[x][y].playerID, spTrans);
					}else if (spTyp == SpriteType.STARTING_FIELD)
					{
						spTrans.GetComponent<SpriteRenderer>().color = PlayerHelper.IDToColor(lvl[x][y].playerID);
					}
				}
			}
		}
	}
}


[System.Serializable]
public class TileSprite{
	public SpriteType spriteType;
	public Sprite sprite;
	public Sprite sprite2;
}
