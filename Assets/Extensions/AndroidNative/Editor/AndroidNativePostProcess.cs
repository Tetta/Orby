
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

				//checking for bundle change
				if(OldBundle != string.Empty) {
					if(OldBundle != PlayerSettings.bundleIdentifier) {
						int result = EditorUtility.DisplayDialogComplex("Andrid Native: bundle id change detected", "Project bundle Identifier changed, do you wnat to replase old bundle: " + OldBundle + "with new one: " + PlayerSettings.bundleIdentifier, "Yes", "No", "Later");


						switch(result) {
						case 0:
							Manifest = Manifest.Replace(QUOTE +  OldBundle + QUOTE, QUOTE +  PlayerSettings.bundleIdentifier + QUOTE);
							Manifest = Manifest.Replace(QUOTE +  OldBundle + ".fileprovider" + QUOTE, QUOTE +  PlayerSettings.bundleIdentifier + ".fileprovider" + QUOTE);
							OldBundle = PlayerSettings.bundleIdentifier;
							break;
						case 1:
							OldBundle = PlayerSettings.bundleIdentifier;
							break;

						}

					}



				} else {
					OldBundle = PlayerSettings.bundleIdentifier;
				}

				FileStaticAPI.Write(file, Manifest);
				Debug.Log("AN Post Process Done");
			}

		}
	}


	private static string OldBundle {
		get {
			if(EditorPrefs.HasKey(BUNLDE_KEY)) {
				return EditorPrefs.GetString(BUNLDE_KEY);
			} else {
				return string.Empty;
			}
		}

		set {
			EditorPrefs.SetString(BUNLDE_KEY, value);
		}
	}

	private static string QUOTE {
		get {
			return "\"";
		}
	}

	private static string BUNLDE_KEY {
		get {
			return "SA_PP_BUNLDE_KEY" + PlayerSettings.productName;
		}
	}



}
