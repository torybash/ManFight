using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class MapEditorWindow : EditorWindow
{
	public const int TilerVersion = 1;
	
	public const string DataPath = @"/Tiler/Data/";
	
	private bool _isCompiling;
	private bool _isClosing;



	int tal = 0;
	string maString = "";


	Texture2D currSprite;


	
	public void Init(){

		Texture2D sprite = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Textures/rock.png",typeof(Texture2D));

		currSprite = sprite;

		Debug.Log("HIYOO");
	}


	void OnGUI(){
//		int tal = 0;
		tal = EditorGUILayout.IntField(tal);
		maString = EditorGUILayout.TextField(maString);
		
		float fieldW = 40f, fieldH = 40f;

		for (int i = 0; i < 10; i++) {
			for (int j = 0; j < 10; j++) {
//				EditorGUI.TextField(new Rect(i * fieldW, j * fieldH +  40f, fieldW, fieldH), "");

				EditorGUI.DrawPreviewTexture(new Rect(i * fieldW, j * fieldH +  40f, fieldW, fieldH), currSprite);
			}
		}
	}
}
