using UnityEngine;
using UnityEditor;
using System.Collections;

[InitializeOnLoad]
public static class Menus {


	[MenuItem("ManFight/Run from menu %#r")]
	private static void RunFromMenu()
	{
		if ( EditorApplication.isPlaying == true )
		{
			EditorApplication.isPlaying = false;
			return;
		}
		EditorApplication.SaveCurrentSceneIfUserWantsTo();
		EditorApplication.OpenScene("Assets/Menu.unity");
		EditorApplication.isPlaying = true;
	}
}
