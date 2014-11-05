﻿
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;

using System.Collections;

public class AndroidNativePostProcess : MonoBehaviour {



	[PostProcessBuild(99)]
	public static void OnPostProcessBuild(BuildTarget target, string path) {
		if(target == BuildTarget.Android) {
			if(!AndroidNativeSettingsEditor.IsInstalled) {
				EditorUtility.DisplayDialog(
					"Android Native Resrouces not found",
					"Looks like Android Native Resurces wasn't imported to your plugins folder. Please hit 'Install' button in plugin settings",
					"Ok");
			} else {

				string file = PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "AndroidManifest.xml";
				string Manifest = FileStaticAPI.Read(file);
				Manifest = Manifest.Replace("%APP_BUNDLE_ID%", PlayerSettings.bundleIdentifier);
				FileStaticAPI.Write(file, Manifest);
				Debug.Log("AN Post Process Done");
			}

		}
	}
}
