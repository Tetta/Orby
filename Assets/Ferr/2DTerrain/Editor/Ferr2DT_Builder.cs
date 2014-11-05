using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

[InitializeOnLoad]
public class Ferr2DT_Builder {
    static bool wasPlaying;

    static Ferr2DT_Builder() {
        EditorApplication.playmodeStateChanged += StateChanged;
    }

    static void StateChanged() {
        if ((!wasPlaying && EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying) || EditorApplication.isCompiling) {
            SaveTerrains();
        }
        wasPlaying = EditorApplication.isPlayingOrWillChangePlaymode;
    }

    public static void SaveTerrains() {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        List<Ferr2DT_PathTerrain> terrains = Ferr_EditorTools.GetPrefabsOfType<Ferr2DT_PathTerrain>();
        for (int i = 0; i < terrains.Count; i++) {
            terrains[i].RecreatePath();
            SaveMesh(terrains[i]);
        }

        sw.Stop();
        if (terrains.Count > 0) {
            Debug.Log("Prebuilding terrain prefabs ("+terrains.Count+"): " + Mathf.Round((float)sw.Elapsed.TotalMilliseconds) + "ms");
        }
    }
    public static void SaveMesh    (Ferr2DT_PathTerrain aTerrain) {
        MeshFilter mesh = aTerrain     .GetComponent<MeshFilter>();
        string     path = AssetDatabase.GetAssetPath(mesh.sharedMesh);

        if ((path.Contains(".assets") && File.Exists(path)) || mesh == null || mesh.sharedMesh == null) {
            return;
        }
        path = "Assets/Ferr2DTerrainMeshes";

        string assetName = "/" + mesh.sharedMesh.name + ".assets";
        if (!Directory.Exists(path)) {
            Directory .CreateDirectory(path);
        }
        try {
            AssetDatabase.CreateAsset(mesh.sharedMesh, path + assetName);
            AssetDatabase.Refresh();
        } catch {
            Debug.LogError("Unable to save terrain prefab mesh! Likely, you deleted the mesh files, and the prefab is still referencing them. Restarting your Unity editor should solve this minor issue.");
        }
    }
}
