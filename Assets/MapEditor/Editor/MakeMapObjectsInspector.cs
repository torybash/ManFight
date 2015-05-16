using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(MakeMapObjects))]
public class MakeMapObjectsInspector : Editor {



	public override void OnInspectorGUI()
	{

		DrawDefaultInspector();
		if (GUILayout.Button("Make obj")){
			MakeObjects();
		}
	}



	void MakeObjects(){
		TextAsset[] lvlTxts = ((MakeMapObjects) target).lvlTxts;


		for (int i = 0; i < lvlTxts.Length; i++) {
			
		}

//		var obj = ScriptableObject.CreateInstance<MyNewType>();
//		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/" + "assetName.asset");
//		AssetDatabase.CreateAsset (asset, assetPathAndName);
//		
//		AssetDatabase.SaveAssets ();
//		EditorUtility.FocusProjectWindow ();
//		Selection.activeObject = asset;
	}
}
