using UnityEditor;
using System;
using System.Linq;
using UnityEngine;
using Unity;
using UnityEditorInternal;
using System.Collections;

public class MapEditorMenu {



	[MenuItem("MapEditor/Open Editor")]
	private static void OpenEditor()
	{
//		MapEditorWindow window = EditorWindow.GetWindow<MapEditorWindow>();

		MapEditorWindow window = (MapEditorWindow)EditorWindow.GetWindow (typeof (MapEditorWindow));
		window.Init();
		window.Show();
		// Prevent the window from being destroyed when a new
		// scene is loaded into the editor.
//		UnityEngine.Object.DontDestroyOnLoad(window);
		//		MapEditorWindow.Create();
	}




}
