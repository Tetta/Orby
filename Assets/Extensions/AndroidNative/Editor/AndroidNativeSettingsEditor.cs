
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(AndroidNativeSettings))]
public class AndroidNativeSettingsEditor : Editor {


	GUIContent PlusApiLabel   = new GUIContent("Enable Plus API [?]:", "API used for account managment");
	GUIContent GamesApiLabel   = new GUIContent("Enable Games API [?]:", "API used for achivements and leaderboards");
	GUIContent AppSateApiLabel = new GUIContent("Enable App State API [?]:", "API used for cloud data save");
	GUIContent DriveApiLabel = new GUIContent("Enable Drive API [?]:", "API used for saved games");


	GUIContent Base64KeyLabel = new GUIContent("Base64 Key[?]:", "Base64 Key app key.");
	GUIContent SdkVersion   = new GUIContent("Plugin Version [?]", "This is Plugin version.  If you have problems or compliments please include this so we know exactly what version to look out for.");
	GUIContent GPSdkVersion   = new GUIContent("Google Play SDK Version [?]", "Version of Google Play SDK used by the plugin");
	GUIContent FBdkVersion   = new GUIContent("Facebook SDK Version [?]", "Version of Unity Facebook SDK Plugin");
	GUIContent SupportEmail = new GUIContent("Support [?]", "If you have any technical quastion, feel free to drop an e-mail");


	private AndroidNativeSettings settings;


	private const string version_info_file = "Plugins/StansAssets/Versions/AN_VersionInfo.txt"; 


	void Awake() {
		ApplaySettings();
	}

	public override void OnInspectorGUI() {
		#if UNITY_WEBPLAYER
		EditorGUILayout.HelpBox("Editing Android Native Settings not avaliable with web player platfrom. Please swith to any other platfrom under Build Seting menu", MessageType.Warning);
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.Space();
		if(GUILayout.Button("Switch To Android Platfrom",  GUILayout.Width(180))) {
			EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
		}
		EditorGUILayout.EndHorizontal();

		if(Application.isEditor) {
			return;
		}


		#endif


		settings = target as AndroidNativeSettings;

		GUI.changed = false;



		GeneralOptions();


		PlayServiceSettings();

		EditorGUILayout.Space();
		BillingSettings();
		EditorGUILayout.Space();
		GCM ();
		EditorGUILayout.Space();
		Other ();


		EditorGUILayout.Space();
		SocialPlatfromSettingsEditor.FacebookSettings();
		EditorGUILayout.Space();
		SocialPlatfromSettingsEditor.TwitterSettings();
		EditorGUILayout.Space();


		AboutGUI();


	

		if(GUI.changed) {
			DirtyEditor();
		}

	}

	public static bool IsInstalled {
		get {
			if(FileStaticAPI.IsFileExists(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "androidnative.jar") && FileStaticAPI.IsFileExists(version_info_file)) {
				return true;
			} else {
				return false;
			}
		}
	}

	public static bool IsUpToDate {
		get {
			if(AndroidNativeSettings.VERSION_NUMBER.Equals(DataVersion)) {
				return true;
			} else {
				return false;
			}
		}
	}


	public static float Version {
		get {
			return System.Convert.ToSingle(DataVersion);
		}
	}


	public static string DataVersion {
		get {
			if(FileStaticAPI.IsFileExists(version_info_file)) {
				return FileStaticAPI.Read(version_info_file);
			} else {
				return "Unknown";
			}
		}
	}

	public static void UpdateVersionInfo() {
		FileStaticAPI.Write(version_info_file, AndroidNativeSettings.VERSION_NUMBER);
	}

	private void GeneralOptions() {



		if(!IsInstalled) {
			EditorGUILayout.HelpBox("Install Required ", MessageType.Error);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			Color c = GUI.color;
			GUI.color = Color.cyan;
			if(GUILayout.Button("Install Plugin",  GUILayout.Width(120))) {
				PluginsInstalationUtil.Android_InstallPlugin();
				UpdateVersionInfo();
			}
			GUI.color = c;
			EditorGUILayout.EndHorizontal();
		}

		if(IsInstalled) {
			if(!IsUpToDate) {

				DrawUpdate();

				EditorGUILayout.HelpBox("Update Required \nResources version: " + DataVersion + " Plugin version: " + AndroidNativeSettings.VERSION_NUMBER, MessageType.Warning);

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				Color c = GUI.color;
				GUI.color = Color.cyan;
				if(GUILayout.Button("Update to " + AndroidNativeSettings.VERSION_NUMBER,  GUILayout.Width(250))) {
					AN_Plugin_Update();
					UpdateVersionInfo();
				}

				GUI.color = c;
				EditorGUILayout.Space();
				EditorGUILayout.EndHorizontal();

			} else {
				EditorGUILayout.HelpBox("Android Native Plugin v" + AndroidNativeSettings.VERSION_NUMBER + " is installed", MessageType.Info);
				PluginSetting();
			}
		}


		EditorGUILayout.Space();

	}


	private void DrawUpdate() {
		if(Version <= 4.4f) {
			EditorGUILayout.HelpBox("AndroidManifest.xml was updated in 4.5 \nNew version contains AndroidManifest.xml chnages, Please remove Assets/Plugins/Android/AndroidManifest.xml file before update or add manualy File Sharing Block from Assets/Plugins/StansAssets/Android/AndroidManifest.xml", MessageType.Warning);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Remove AndroidManifest and Update to " + AndroidNativeSettings.VERSION_NUMBER,  GUILayout.Width(250))) {
				
				string file = "AndroidManifest.xml";
				FileStaticAPI.DeleteFile(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + file);
				
				AN_Plugin_Update();
				UpdateVersionInfo();
			}
			
			
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
		}


		if(Version <= 4.5f) {
			EditorGUILayout.HelpBox("AndroidManifest.xml was updated in 4.6 \nNew version contains AndroidManifest.xml chnages, Please remove Assets/Plugins/Android/AndroidManifest.xml file before update or add manualy %APP_BUNDLE_ID% tockens from Assets/Plugins/StansAssets/Android/AndroidManifest.xml", MessageType.Warning);
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			
			if(GUILayout.Button("Remove AndroidManifest and Update to " + AndroidNativeSettings.VERSION_NUMBER,  GUILayout.Width(250))) {
				
				string file = "AndroidManifest.xml";
				FileStaticAPI.DeleteFile(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + file);
				
				AN_Plugin_Update();
				UpdateVersionInfo();
			}
			
			
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			
		}
	}

	private void Actions() {
		EditorGUILayout.Space();
		AndroidNativeSettings.Instance.ShowActions = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowActions, "More Actions");
		if(AndroidNativeSettings.Instance.ShowActions) {

			if(!FileStaticAPI.IsFolderExists("Facebook")) {
				GUI.enabled = false;
			}	

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();

			if(GUILayout.Button("Remove Facebook SDK",  GUILayout.Width(160))) {
				bool result = EditorUtility.DisplayDialog(
					"Removing Facebook SDK",
					"Warning action can not be undone without reimporting the plugin",
					"Remove",
					"Cansel");

				if(result) {
					FileStaticAPI.DeleteFolder(PluginsInstalationUtil.ANDROID_DESTANATION_PATH + "facebook");
					FileStaticAPI.DeleteFolder("Facebook");
					FileStaticAPI.DeleteFolder("Extensions/GooglePlayCommon/Social/Facebook");
					FileStaticAPI.DeleteFile("Extensions/AndroidNative/xExample/Scripts/Social/FacebookAndroidUseExample.cs");
					FileStaticAPI.DeleteFile("Extensions/AndroidNative/xExample/Scripts/Social/FacebookAnalyticsExample.cs");
				}

			}

			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Reset Settings",  GUILayout.Width(160))) {

				SocialPlatfromSettingsEditor.ResetSettings();

				FileStaticAPI.DeleteFile("Extensions/AndroidNative/Resources/AndroidNativeSettings.asset");
				AndroidNativeSettings.Instance.ShowActions = true;
				Selection.activeObject = AndroidNativeSettings.Instance;

				return;
			}

			if(GUILayout.Button("Load Example Settings",  GUILayout.Width(160))) {
				SocialPlatfromSettingsEditor.LoadExampleSettings();
				FileStaticAPI.DeleteFile("Extensions/AndroidNative/Resources/AndroidNativeSettings.asset");
				FileStaticAPI.CopyFile("Extensions/AndroidNative/Resources/AndroidNativeSettings_example.asset", "Extensions/AndroidNative/Resources/AndroidNativeSettings.asset");
			}


			EditorGUILayout.EndHorizontal();

		}
	}


	private void PluginSetting() {
		EditorGUILayout.Space();
		EditorGUILayout.HelpBox("Plugin Settings", MessageType.None);




		AndroidNativeSettings.Instance.ShowPluginSettings = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowPluginSettings, "Android Native APIs");
		if(AndroidNativeSettings.Instance.ShowPluginSettings) {

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField("Enable Native API");
			GUI.enabled = false;
			EditorGUILayout.Toggle(true);
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();



			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Enable Billing API");
			settings.EnableBillingAPI	 	= EditorGUILayout.Toggle(settings.EnableBillingAPI);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Enable Google Play API");
			settings.EnablePSAPI	 	= EditorGUILayout.Toggle(settings.EnablePSAPI);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Enable Social API");
			settings.EnableSocialAPI	 	= EditorGUILayout.Toggle(settings.EnableSocialAPI);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Enable Camera API");
			settings.EnableCameraAPI	 	= EditorGUILayout.Toggle(settings.EnableCameraAPI);
			EditorGUILayout.EndHorizontal();

		
		}

		if(GUI.changed) {
			UpdateAPIsInstalation();
		}


		Actions();

		EditorGUILayout.Space();
	}

	public static void UpdateAPIsInstalation() {


		if(AndroidNativeSettings.Instance.EnableBillingAPI) {
			PluginsInstalationUtil.EnableBillingAPI();
		} else {
			PluginsInstalationUtil.DisableBillingAPI();
		}
		
		if(AndroidNativeSettings.Instance.EnablePSAPI) {
			PluginsInstalationUtil.EnableGooglePlayAPI();
		} else {
			PluginsInstalationUtil.DisableGooglePlayAPI();
		}


		if(AndroidNativeSettings.Instance.EnableSocialAPI) {
			PluginsInstalationUtil.EnableSocialAPI();
		} else {
			PluginsInstalationUtil.DisableSocialAPI();
		}


		if(AndroidNativeSettings.Instance.EnableCameraAPI) {
			PluginsInstalationUtil.EnableCameraAPI();
		} else {
			PluginsInstalationUtil.DisableCameraAPI();
		}

	}

	private void PlayServiceSettings() {
		EditorGUILayout.HelpBox("Google API Settings", MessageType.None);
		AndroidNativeSettings.Instance.ShowPSSettings = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowPSSettings, "PlayService Settings");
		if(AndroidNativeSettings.Instance.ShowPSSettings) {


			EditorGUILayout.LabelField("API:");
			EditorGUI.indentLevel++;


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(PlusApiLabel);
			settings.EnablePlusAPI	 	= EditorGUILayout.Toggle(settings.EnablePlusAPI);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(GamesApiLabel);
			settings.EnableGamesAPI	 	= EditorGUILayout.Toggle(settings.EnableGamesAPI);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(DriveApiLabel);
			settings.EnableDriveAPI	 	= EditorGUILayout.Toggle(settings.EnableDriveAPI);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(AppSateApiLabel);
			settings.EnableAppStateAPI	 	= EditorGUILayout.Toggle(settings.EnableAppStateAPI);
			EditorGUILayout.EndHorizontal();




			EditorGUI.indentLevel--;


			EditorGUILayout.LabelField("Auto Image Loading:");

			EditorGUI.indentLevel++;
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Profile Icons");
			settings.LoadProfileIcons	 	= EditorGUILayout.Toggle(settings.LoadProfileIcons);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Profile Hi-res Images");
			settings.LoadProfileImages	 	= EditorGUILayout.Toggle(settings.LoadProfileImages);
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Event Icons");
			settings.LoadEventsIcons	 	= EditorGUILayout.Toggle(settings.LoadEventsIcons);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Quest Icons");
			settings.LoadQuestsIcons	 	= EditorGUILayout.Toggle(settings.LoadQuestsIcons);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Quest Banners");
			settings.LoadQuestsImages	 	= EditorGUILayout.Toggle(settings.LoadQuestsImages);
			EditorGUILayout.EndHorizontal();
			EditorGUI.indentLevel--;
		}

	}

	private void BillingSettings() {
	//	EditorGUILayout.HelpBox("(Optional) In-app Billing Parameters", MessageType.None);
		AndroidNativeSettings.Instance.ShowStoreKitParams = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowStoreKitParams, "Billing Settings");
		if(AndroidNativeSettings.Instance.ShowStoreKitParams) {

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(Base64KeyLabel);
			settings.base64EncodedPublicKey	 	= EditorGUILayout.TextField(settings.base64EncodedPublicKey);
			if(settings.base64EncodedPublicKey.Length > 0) {
				settings.base64EncodedPublicKey 	= settings.base64EncodedPublicKey.Trim();
			}

			EditorGUILayout.EndHorizontal();


			if(settings.InAppProducts.Count == 0) {
				EditorGUILayout.HelpBox("No products added", MessageType.Warning);
			}
		

			int i = 0;
			foreach(string str in settings.InAppProducts) {
				EditorGUILayout.BeginHorizontal();
				settings.InAppProducts[i]	 	= EditorGUILayout.TextField(settings.InAppProducts[i]);
				if(settings.InAppProducts[i].Length > 0) {
					settings.InAppProducts[i]		= settings.InAppProducts[i].Trim();
				}
				if(GUILayout.Button("Remove",  GUILayout.Width(80))) {
					settings.InAppProducts.Remove(str);
					break;
				}
				EditorGUILayout.EndHorizontal();
				i++;
			}


			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if(GUILayout.Button("Add",  GUILayout.Width(80))) {
				settings.InAppProducts.Add("");
			}
			EditorGUILayout.EndHorizontal();


			EditorGUILayout.Space();
		}
	}




	private void GCM() {
		AndroidNativeSettings.Instance.GCMSettingsActinve = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.GCMSettingsActinve, "Google Cloud Messaging  Settings");
		if (AndroidNativeSettings.Instance.GCMSettingsActinve) {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Sender Id");
			settings.GCM_SenderId	 	= EditorGUILayout.TextField(settings.GCM_SenderId);
			if(settings.GCM_SenderId.Length > 0) {
				settings.GCM_SenderId		= settings.GCM_SenderId.Trim();
			}

			EditorGUILayout.EndHorizontal();
		}
	}



	private void Other() {
		AndroidNativeSettings.Instance.ShowCameraAndGalleryParams = EditorGUILayout.Foldout(AndroidNativeSettings.Instance.ShowCameraAndGalleryParams, "Camera And Gallery");
		if (AndroidNativeSettings.Instance.ShowCameraAndGalleryParams) {
			CameraAndGalleryParams();
		}
	}

	public static void CameraAndGalleryParams() {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Camera Capture Mode");
		AndroidNativeSettings.Instance.CameraCaptureMode	 	= (AN_CameraCaptureType) EditorGUILayout.EnumPopup(AndroidNativeSettings.Instance.CameraCaptureMode);
		EditorGUILayout.EndHorizontal();
		
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Max Loaded Image Size");
		AndroidNativeSettings.Instance.MaxImageLoadSize	 	= EditorGUILayout.IntField(AndroidNativeSettings.Instance.MaxImageLoadSize);
		EditorGUILayout.EndHorizontal();
		
		
		
		GUI.enabled = !AndroidNativeSettings.Instance.UseProductNameAsFolderName;
		if(AndroidNativeSettings.Instance.UseProductNameAsFolderName) {
			if(PlayerSettings.productName.Length > 0) {
				AndroidNativeSettings.Instance.GalleryFolderName = PlayerSettings.productName.Trim();
			}


		}
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("App Gallery Folder");
		AndroidNativeSettings.Instance.GalleryFolderName	 	= EditorGUILayout.TextField(AndroidNativeSettings.Instance.GalleryFolderName);
		if(AndroidNativeSettings.Instance.GalleryFolderName.Length > 0) {
			AndroidNativeSettings.Instance.GalleryFolderName		= AndroidNativeSettings.Instance.GalleryFolderName.Trim();
			AndroidNativeSettings.Instance.GalleryFolderName		= AndroidNativeSettings.Instance.GalleryFolderName.Trim('/');
		}

		EditorGUILayout.EndHorizontal();
		
		GUI.enabled = true;
		
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Use Product Name As Folder Name");
		AndroidNativeSettings.Instance.UseProductNameAsFolderName	 	= EditorGUILayout.Toggle(AndroidNativeSettings.Instance.UseProductNameAsFolderName);
		EditorGUILayout.EndHorizontal();
	}




	
	
	private void AboutGUI() {

		EditorGUILayout.HelpBox("About the Plugin", MessageType.None);
		
		SelectableLabelField(SdkVersion,   AndroidNativeSettings.VERSION_NUMBER);
		if(FileStaticAPI.IsFolderExists("Facebook")) {
			SelectableLabelField(FBdkVersion, SocialPlatfromSettings.FB_SDK_VERSION_NUMBER);
		}	
		SelectableLabelField(GPSdkVersion, AndroidNativeSettings.GOOGLE_PLAY_SDK_VERSION_NUMBER);



		SelectableLabelField(SupportEmail, "stans.assets@gmail.com");
		
		
	}
	
	private void SelectableLabelField(GUIContent label, string value) {
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(label, GUILayout.Width(180), GUILayout.Height(16));
		EditorGUILayout.SelectableLabel(value, GUILayout.Height(16));
		EditorGUILayout.EndHorizontal();
	}

	private void ApplaySettings() {
		if(AndroidNativeSettings.Instance.UseProductNameAsFolderName) {
			AndroidNativeSettings.Instance.GalleryFolderName = PlayerSettings.productName;
		}
	}

	public static void AN_Plugin_Update() {
		PluginsInstalationUtil.Android_UpdatePlugin();
		AndroidNativeSettingsEditor.UpdateAPIsInstalation();
	}



	private static void DirtyEditor() {
		#if UNITY_EDITOR
		EditorUtility.SetDirty(SocialPlatfromSettings.Instance);
		EditorUtility.SetDirty(AndroidNativeSettings.Instance);
		#endif
	}
	
	
}
