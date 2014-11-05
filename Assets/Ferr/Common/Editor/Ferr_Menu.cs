using UnityEngine;
using UnityEditor;
using System.Collections;

public enum Ferr2DT_SnapMode {
    None,
    Local,
    Global
}

public static class Ferr_Menu {
    static bool             prefsLoaded = false;
    static bool             hideMeshes  = true;
    static float            pathScale   = 1;
    static Ferr2DT_SnapMode snapMode    = Ferr2DT_SnapMode.None;
    static float            snapGrid    = 0.5f;
    static int              updateTerrainSkipFrames = 0;

    public static bool HideMeshes {
        get { LoadPrefs(); return hideMeshes; }
    }
    public static float PathScale {
        get { LoadPrefs(); return pathScale;  }
    }
    public static Ferr2DT_SnapMode SnapMode {
        get { LoadPrefs(); return snapMode;   }
    }
    public static float SnapGrid {
        get { LoadPrefs(); return snapGrid;   }
    }
    public static int UpdateTerrainSkipFrames {
        get { LoadPrefs(); return updateTerrainSkipFrames; }
    }

    [PreferenceItem("Ferr")]
    static void Ferr2DT_PreferencesGUI() 
    {
        LoadPrefs();

        hideMeshes = EditorGUILayout.Toggle    ("Hide terrain meshes", hideMeshes);
        pathScale  = EditorGUILayout.FloatField("Path vertex scale",   pathScale );
        updateTerrainSkipFrames = EditorGUILayout.IntField("Update Terrain Every X Frames", updateTerrainSkipFrames);
        snapMode   = (Ferr2DT_SnapMode)EditorGUILayout.EnumPopup("Snap Mode (very alpha)", snapMode);
        snapGrid   = EditorGUILayout.FloatField("Snap Grid Size",      snapGrid  );

        if (GUI.changed) {
            SavePrefs();
        }
    }

    static void LoadPrefs() {
        if (prefsLoaded) return;
        prefsLoaded = true;
        hideMeshes  = EditorPrefs.GetBool ("Ferr_hideMeshes", true);
        pathScale   = EditorPrefs.GetFloat("Ferr_pathScale",  1   );
        updateTerrainSkipFrames = EditorPrefs.GetInt("Ferr_updateTerrainAlways", 0);
        snapMode    = (Ferr2DT_SnapMode)EditorPrefs.GetInt("Ferr_snapMode", (int)Ferr2DT_SnapMode.None);
        snapGrid    = EditorPrefs.GetFloat("Ferr_snapGrid",   0.5f);
    }
    static void SavePrefs() {
        if (!prefsLoaded) return;
        EditorPrefs.SetBool ("Ferr_hideMeshes", hideMeshes);
        EditorPrefs.SetFloat("Ferr_pathScale",  pathScale );
        EditorPrefs.SetInt  ("Ferr_updateTerrainAlways", updateTerrainSkipFrames);
        EditorPrefs.SetInt  ("Ferr_snapMode",   (int)snapMode);
        EditorPrefs.SetFloat("Ferr_snapGrid",   snapGrid);
    }
}
