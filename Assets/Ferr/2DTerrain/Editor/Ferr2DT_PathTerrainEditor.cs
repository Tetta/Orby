using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Ferr2DT_PathTerrain))]
public class Ferr2DT_PathTerrainEditor : Editor {
    bool showVisuals     = true;
    bool showTerrainType = true;
    bool showCollider    = true;

    void OnEnable() {
        Ferr2DT_PathTerrain terrain = (Ferr2DT_PathTerrain)target;
        if (terrain.GetComponent<MeshFilter>().sharedMesh == null)
        	terrain.RecreatePath();
    }

    void OnSceneGUI()
    {
        Ferr2DT_PathTerrain collider = (Ferr2DT_PathTerrain)target;
        Ferr2D_Path         path     = collider.gameObject.GetComponent<Ferr2D_Path>();

        EditorUtility.SetSelectedWireframeHidden(collider.gameObject.renderer, Ferr_Menu.HideMeshes);

        if (collider.enabled == false || path == null || path.pathVerts.Count <= 1 || !collider.createCollider) return;
        
        Handles.color = new Color(0, 1, 0, 1);

		DrawColliderEdge(collider);
    }
	public override void OnInspectorGUI() {
		#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
		Undo.RecordObject(target, "Modified Path Terrain");
		#else
        Undo.SetSnapshotTarget(target, "Modified Path Terrain");
		#endif

        Ferr2DT_PathTerrain sprite = (Ferr2DT_PathTerrain)target;

        // render the material selector!
        EditorGUILayout.BeginHorizontal();
        GUIContent button = sprite.TerrainMaterial != null && sprite.TerrainMaterial.edgeMaterial != null && sprite.TerrainMaterial.edgeMaterial.mainTexture != null ? new GUIContent(sprite.TerrainMaterial.edgeMaterial.mainTexture) : new GUIContent("Pick");
        if (GUILayout.Button(button, GUILayout.Width(48f),GUILayout.Height(48f))) Ferr2DT_MaterialSelector.Show(sprite.SetMaterial);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField   ("Terrain Material:");
        EditorGUI      .indentLevel = 2;
        EditorGUILayout.LabelField   (sprite.TerrainMaterial == null ? "None" : sprite.TerrainMaterial.name);
        EditorGUI      .indentLevel = 0;
        EditorGUILayout.EndVertical  ();
        EditorGUILayout.EndHorizontal();

        showVisuals = EditorGUILayout.Foldout(showVisuals, "VISUALS");

        
        if (showVisuals) {
            EditorGUI.indentLevel = 2;
            // other visual data
            sprite.vertexColor      = EditorGUILayout.ColorField("Vertex Color",       sprite.vertexColor             );
            sprite.pixelsPerUnit    = EditorGUILayout.FloatField("Pixels Per Unit",    sprite.pixelsPerUnit           );
			sprite.stretchThreshold = EditorGUILayout.Slider    ("Stretch Threshold",  sprite.stretchThreshold, 0f, 1f);
            sprite.splitMiddle      = EditorGUILayout.Toggle    ("Split Middle",       sprite.splitMiddle             );
            sprite.createTangents   = EditorGUILayout.Toggle    ("Create Tangents",    sprite.createTangents          );
            sprite.randomByWorldCoordinates = EditorGUILayout.Toggle("Randomize by World Coordinates", sprite.randomByWorldCoordinates);
        }
        EditorGUI.indentLevel = 0;


        showTerrainType = EditorGUILayout.Foldout(showTerrainType, "TERRAIN TYPE");
        if (showTerrainType) {
            EditorGUI.indentLevel = 2;
            sprite.fill          = (Ferr2DT_FillMode)EditorGUILayout.EnumPopup("Fill Type", sprite.fill);
		    if (sprite.fill == Ferr2DT_FillMode.Closed || sprite.fill == Ferr2DT_FillMode.InvertedClosed || sprite.fill == Ferr2DT_FillMode.FillOnlyClosed && sprite.GetComponent<Ferr2D_Path>() != null) sprite.GetComponent<Ferr2D_Path>().closed = true;
            if (sprite.fill != Ferr2DT_FillMode.None && (sprite.TerrainMaterial != null && sprite.TerrainMaterial.fillMaterial == null)) sprite.fill = Ferr2DT_FillMode.None;
            if (sprite.fill != Ferr2DT_FillMode.None ) sprite.fillZ = EditorGUILayout.FloatField("Fill Z Offset", sprite.fillZ);
            if (sprite.fill == Ferr2DT_FillMode.Skirt) sprite.fillY = EditorGUILayout.FloatField("Skirt Y Value", sprite.fillY);
            
            sprite.splitCorners  = EditorGUILayout.Toggle     ("Split Corners", sprite.splitCorners );
            sprite.smoothPath    = EditorGUILayout.Toggle     ("Smooth Path",   sprite.smoothPath   );
            EditorGUI.indentLevel = 3;
            if (sprite.smoothPath)
            {
                sprite.splitCount = EditorGUILayout.IntField  ("Edge Splits", sprite.splitCount);
                sprite.splitDist  = EditorGUILayout.Slider    ("Fill Split",  sprite.splitDist, 0.1f, 3);
                if (sprite.splitCount < 1) sprite.splitCount = 2;
            } else {
				sprite.splitCount = 0;
                sprite.splitDist  = 1;
			}
            EditorGUI.indentLevel = 2;
			
        }
        EditorGUI.indentLevel = 0;
        

        showCollider = EditorGUILayout.Foldout(showCollider, "COLLIDER");
        // render collider options
        if (showCollider) {
            EditorGUI.indentLevel = 2;
            sprite.createCollider = EditorGUILayout.Toggle("Create Collider", sprite.createCollider);
            if (sprite.createCollider) {
				#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
				sprite.create3DCollider = EditorGUILayout.Toggle("Use 3D Collider", sprite.create3DCollider);

                EditorGUI.indentLevel = 3;
				if (sprite.create3DCollider) {
					sprite.physicsMaterial        = (PhysicMaterial)EditorGUILayout.ObjectField("Physics Material", sprite.physicsMaterial,typeof(PhysicMaterial), true);
					sprite.depth                  = EditorGUILayout.FloatField("Collider Width",           sprite.depth);
                    sprite.smoothSphereCollisions = EditorGUILayout.Toggle    ("Smooth Sphere Collisions", sprite.smoothSphereCollisions);
				} else {
					sprite.physicsMaterial2D = (PhysicsMaterial2D)EditorGUILayout.ObjectField("Physics Material", sprite.physicsMaterial2D,typeof(PhysicsMaterial2D), true);
				}
                EditorGUI.indentLevel = 2;
				#else
				sprite.physicsMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Physics Material", sprite.physicsMaterial,typeof(PhysicMaterial), true);
				sprite.depth           = EditorGUILayout.FloatField("Collider Width", sprite.depth);
				#endif
                if (sprite.fill == Ferr2DT_FillMode.None)
                {
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ] = EditorGUILayout.FloatField("Thickness Top",    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ]);
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom] = EditorGUILayout.FloatField("Thickness Bottom", sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom]);
                }
                else
                {
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ] = EditorGUILayout.FloatField("Offset Top",    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ]);
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Left  ] = EditorGUILayout.FloatField("Offset Left",   sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Left  ]);
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Right ] = EditorGUILayout.FloatField("Offset Right",  sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Right ]);
                    sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom] = EditorGUILayout.FloatField("Offset Bottom", sprite.surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom]);
                }

                EditorGUI.indentLevel = 0;
                EditorGUILayout.LabelField("Generate colliders along:");
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Top",GUILayout.Width(25));
                sprite.collidersTop = EditorGUILayout.Toggle(sprite.collidersTop,GUILayout.Width(25));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Left",GUILayout.Width(25));
                sprite.collidersLeft = EditorGUILayout.Toggle(sprite.collidersLeft,GUILayout.Width(25));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Right",GUILayout.Width(35));
                sprite.collidersRight = EditorGUILayout.Toggle(sprite.collidersRight,GUILayout.Width(35));
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical();
                EditorGUILayout.LabelField("Bottom",GUILayout.Width(45));
                sprite.collidersBottom = EditorGUILayout.Toggle(sprite.collidersBottom,GUILayout.Width(45));
                EditorGUILayout.EndVertical();

                EditorGUILayout.EndHorizontal();

				#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
				if (!sprite.collidersBottom || !sprite.collidersLeft || !sprite.collidersRight || !sprite.collidersTop) {
					EditorGUI.indentLevel = 2;
                    sprite.colliderThickness = EditorGUILayout.FloatField("Collider Thickness", sprite.colliderThickness);
					EditorGUI.indentLevel = 0;
				}
				#endif
            }
        }
        EditorGUI.indentLevel = 0;

		if (GUI.changed) {
            EditorUtility.SetDirty(target);
			sprite.RecreatePath();
		}
        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    sprite.ForceMaterial(sprite.TerrainMaterial, true);
                    sprite.RecreatePath();
                    break;
            }
        }
	}

    bool ColliderNormValid(Ferr2DT_PathTerrain aSprite, Vector2 aOne, Vector2 aTwo) {
        Ferr2DT_TerrainDirection dir = Ferr2D_Path.GetDirection(aTwo, aOne);
        if (dir == Ferr2DT_TerrainDirection.Top    && aSprite.collidersTop   ) return true;
        if (dir == Ferr2DT_TerrainDirection.Left   && aSprite.collidersLeft  ) return true;
        if (dir == Ferr2DT_TerrainDirection.Right  && aSprite.collidersRight ) return true;
        if (dir == Ferr2DT_TerrainDirection.Bottom && aSprite.collidersBottom) return true;
        return false;
    }
	void DrawColliderEdge(Ferr2DT_PathTerrain aTerrain) {
		List<List<Vector2>> verts = aTerrain.GetColliderVerts();
        for (int t = 0; t < verts.Count; t++) {
            for (int i = 0; i < verts[t].Count - 1; i++) {
                Handles.DrawLine(aTerrain.transform.position + aTerrain.transform.rotation * (Vector3)verts[t][i], aTerrain.transform.position + aTerrain.transform.rotation * (Vector3)verts[t][i + 1]);
            }
        }
        if (verts.Count > 0 && verts[verts.Count - 1].Count > 0) {
            Handles.color = Color.yellow;
            Handles.DrawLine(aTerrain.transform.position + aTerrain.transform.rotation * (Vector3)verts[0][0], aTerrain.transform.position + aTerrain.transform.rotation * (Vector3)verts[verts.Count - 1][verts[verts.Count - 1].Count - 1]);
            Handles.color = Color.green;
        }
	}
    
}