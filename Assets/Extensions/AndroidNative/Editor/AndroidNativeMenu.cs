////////////////////////////////////////////////////////////////////////////////
//  
// @module V2D
// @author Osipov Stanislav lacost.st@gmail.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;

public class AndroidNativeMenu : EditorWindow {
	
	//--------------------------------------
	//  PUBLIC METHODS
	//--------------------------------------

	#if UNITY_EDITOR

	[MenuItem("Window/Android Native/Edit Settings")]
	public static void Edit() {
		Selection.activeObject = AndroidNativeSettings.Instance;
	}

	[MenuItem("Window/Android Native/Online Documentation")]
	public static void Docs() {
		string url = "http://goo.gl/VmIFVQ";
		Application.OpenURL(url);
	}

	#endif

}
