#define FERR2D_TERRAIN

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(Ferr2D_Path))]
public class Ferr2D_PathEditor : Editor {
    Texture2D texMinus   = Ferr_EditorTools.GetGizmo("dot-minus.png");
    Texture2D texDot     = Ferr_EditorTools.GetGizmo("dot.png");
    Texture2D texDotPlus = Ferr_EditorTools.GetGizmo("dot-plus.png");

#if FERR2D_TERRAIN
    Texture2D texLeft   = Ferr_EditorTools.GetGizmo("dot-left.png"  );
    Texture2D texRight  = Ferr_EditorTools.GetGizmo("dot-right.png" );
    Texture2D texTop    = Ferr_EditorTools.GetGizmo("dot-top.png"   );
    Texture2D texBottom = Ferr_EditorTools.GetGizmo("dot-down.png");
    Texture2D texAuto   = Ferr_EditorTools.GetGizmo("dot-auto.png"  );
#endif 

    public static Vector3 offset = new Vector3(0, 0, -0.0f);
    bool showVerts = false;
    static int updateCount = 0;

	void                 OnSceneGUI    () {
		Ferr2D_Path  path      = (Ferr2D_Path)target;
        GUIStyle     iconStyle = new GUIStyle();
        iconStyle.alignment    = TextAnchor.MiddleCenter;

		// if this was an undo, refresh stuff too
        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    path.UpdateDependants();
                    return;
            }
        }
		
		// setup undoing things
		#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
		Undo.RecordObject(target, "Modified Path");
		#else
        Undo.SetSnapshotTarget(target, "Modified Path");
		Undo.CreateSnapshot();
		#endif
		
        // draw and interact with all the path handles
        DoHandles(path, iconStyle);

        // draw the path line
        if (Event.current.type == EventType.repaint)
            DoPath(path);
		
        // do adding verts in when the shift key is down!
		if (Event.current.shift) {
            DoShiftAdd(path, iconStyle);
		}
		
		// update everything that relies on this path, if the GUI changed
        if (GUI.changed || Event.current.type == EventType.mouseUp) {
			#if (UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
			Undo.RegisterSnapshot();
			#endif
            UpdateDependentsSmart(path, false);
            EditorUtility.SetDirty (target);
		}
	}

    private void UpdateDependentsSmart(Ferr2D_Path aPath, bool aForce) {
        if (aForce || Ferr_Menu.UpdateTerrainSkipFrames == 0 || updateCount % Ferr_Menu.UpdateTerrainSkipFrames == 0) {
            aPath.UpdateDependants();
        }
        updateCount += 1;
    }

    private void DoHandles (Ferr2D_Path path, GUIStyle iconStyle)
    {
#if FERR2D_TERRAIN
        Ferr2DT_PathTerrain terrain = path.gameObject.GetComponent<Ferr2DT_PathTerrain>();
        if (terrain) terrain.MatchOverrides();
#endif
        Quaternion inv = Quaternion.Inverse(path.transform.rotation);

        Handles.color = new Color(1, 1, 1, 0);
        for (int i = 0; i < path.pathVerts.Count; i++)
        {
            Vector3 pos = path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(path.pathVerts[i].x, path.pathVerts[i].y, 0), path.transform.localScale);

            // check if we want to remove points
            if (Event.current.control)
            {
                if (Handles.Button(pos + offset, Quaternion.identity, HandleScale(pos+offset), HandleScale(pos+offset), Handles.CircleCap))
                {
                    path.pathVerts.RemoveAt(i);
#if FERR2D_TERRAIN
                    if (terrain)
                        terrain.directionOverrides.RemoveAt(i);
#endif
                    i--;
                    GUI.changed = true;
                }
                if (SetScale(pos + offset, texMinus, ref iconStyle)) Handles.Label(pos + offset, new GUIContent(texMinus), iconStyle);
            }
            else
            {
                // check for moving the point
                if (SetScale(pos + offset, texMinus, ref iconStyle)) Handles.Label(pos + offset, new GUIContent(texDot), iconStyle);
                Vector3 result = Handles.FreeMoveHandle(
                    pos + offset,
                    Quaternion.identity,
                    HandleScale(pos+offset),
                    Vector3.zero, Handles.CircleCap);

                Vector3 global = (result - offset);
                if (Ferr_Menu.SnapMode == Ferr2DT_SnapMode.Global) global = SnapVector(global);
                Vector3 local = inv * (global - path.transform.position);
                if (Ferr_Menu.SnapMode == Ferr2DT_SnapMode.Local ) local  = SnapVector(local);
                path.pathVerts[i] = new Vector2(
                    local.x / path.transform.localScale.x,
                    local.y / path.transform.localScale.y);

                // if using terrain, check to see for any edge overrides
#if FERR2D_TERRAIN
                if (terrain && i+1 < path.pathVerts.Count) {
                    float scale = HandleScale(pos+offset) * 0.5f;
                    Vector3 dirOff = Vector3.zero;
                    if (i + 1 < path.pathVerts.Count || path.closed == true) {
                        int index = path.closed && i + 1 == path.pathVerts.Count ? 0 : i + 1;
                        Vector3 delta = path.pathVerts[index] - path.pathVerts[i];
                        delta.Normalize();
                        Vector3 norm = new Vector3(-delta.y, delta.x, 0);
                        dirOff = delta * scale * 3 + new Vector3(norm.x, norm.y, 0) * scale * 2;
                    }

                    if (SetScale(pos + offset + dirOff, texMinus, ref iconStyle, 0.5f)) Handles.Label(pos + offset + dirOff, new GUIContent(GetDirIcon(terrain.directionOverrides[i])), iconStyle);
                    if (Handles.Button( pos + offset + dirOff, Quaternion.identity, scale, scale, Handles.CircleCap)) {
                        terrain.directionOverrides[i] = NextDir(terrain.directionOverrides[i]);
                        GUI.changed = true;
                    }
                }
#endif

                // make sure we can add new point at the midpoints!
                if (i + 1 < path.pathVerts.Count || path.closed == true)
                {
                    int index = path.closed && i + 1 == path.pathVerts.Count ? 0 : i + 1;
                    Vector3 pos2 = path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(path.pathVerts[index].x, path.pathVerts[index].y, 0), path.transform.localScale);
                    Vector3 mid  = (pos + pos2) / 2;

                    if (Handles.Button(mid + offset, Quaternion.identity, HandleScale(mid+offset), HandleScale(mid+offset), Handles.CircleCap))
                    {
                        path.pathVerts.Insert(index, inv * new Vector2((mid.x - path.transform.position.x) / path.transform.localScale.x, (mid.y - path.transform.position.y) / path.transform.localScale.y));
#if FERR2D_TERRAIN
                        if (terrain)
                            terrain.directionOverrides.Insert(index, Ferr2DT_TerrainDirection.None);
#endif
                    }
                    if (SetScale(mid + offset, texDotPlus, ref iconStyle)) Handles.Label(mid + offset, new GUIContent(texDotPlus), iconStyle);
                }
            }
        }
    }
    private void DoShiftAdd(Ferr2D_Path path, GUIStyle iconStyle)
    {
#if FERR2D_TERRAIN
        Ferr2DT_PathTerrain terrain  = path.gameObject.GetComponent<Ferr2DT_PathTerrain>();
#endif
        Quaternion          inv      = Quaternion.Inverse(path.transform.rotation);
        Vector2             pos      = GetMousePos(Event.current.mousePosition, path.transform.position.z) - new Vector2(path.transform.position.x, path.transform.position.y);
        bool                hasDummy = path.pathVerts.Count <= 0;

        if (hasDummy) path.pathVerts.Add(Vector2.zero);

        int   closestID  = path.GetClosestSeg(inv * new Vector2(pos.x / path.transform.localScale.x, pos.y / path.transform.localScale.y));
        int   secondID   = closestID + 1 >= path.Count ? 0 : closestID + 1;

        float firstDist  = Vector2.Distance(pos, path.pathVerts[closestID]);
        float secondDist = Vector2.Distance(pos, path.pathVerts[secondID]);

        Vector3 local  = pos;
        if (Ferr_Menu.SnapMode == Ferr2DT_SnapMode.Local ) local  = SnapVector(local );
        Vector3 global = path.transform.position + local;
        if (Ferr_Menu.SnapMode == Ferr2DT_SnapMode.Global) global = SnapVector(global);

        Handles.color = Color.white;
        if (!(secondID == 0 && !path.closed && firstDist > secondDist))
        {
            Handles.DrawLine(
                global,
                path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(path.pathVerts[closestID].x, path.pathVerts[closestID].y, 0), path.transform.localScale));
        }
        if (!(secondID == 0 && !path.closed && firstDist < secondDist))
        {
            Handles.DrawLine(
                global,
                path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(path.pathVerts[secondID].x, path.pathVerts[secondID].y, 0), path.transform.localScale));
        }
        Handles.color = new Color(1, 1, 1, 0);

        Vector3 handlePos = new Vector3(pos.x, pos.y, 0) + path.transform.position + offset;
        if (Handles.Button(handlePos, Quaternion.identity, HandleScale(handlePos), HandleScale(handlePos), Handles.CircleCap))
        {
            Vector3    finalPos = inv * (new Vector3(global.x / path.transform.localScale.x, global.y / path.transform.localScale.y, 0) - path.transform.position);
            if (secondID == 0)
            {
                if (firstDist < secondDist)
                {
                    path.pathVerts.Add(finalPos);
#if FERR2D_TERRAIN
                    if (terrain)
                        terrain.directionOverrides.Add(Ferr2DT_TerrainDirection.None);
#endif
                }
                else
                {
                    path.pathVerts.Insert(0, finalPos);
#if FERR2D_TERRAIN
                    if (terrain)
                        terrain.directionOverrides.Insert(0, Ferr2DT_TerrainDirection.None);
#endif
                }
            }
            else
            {
                path.pathVerts.Insert(Mathf.Max(closestID, secondID), finalPos);
#if FERR2D_TERRAIN
                if (terrain)
                    terrain.directionOverrides.Insert(Mathf.Max(closestID, secondID), Ferr2DT_TerrainDirection.None);
#endif
            }
            GUI.changed = true;
        }
        if (SetScale(new Vector3(pos.x, pos.y, 0) + path.transform.position + offset, texDotPlus, ref iconStyle)) Handles.Label(new Vector3(pos.x, pos.y, 0) + path.transform.position + offset, new GUIContent(texDotPlus), iconStyle);

        if (hasDummy) path.pathVerts.RemoveAt(0);
    }
    private void DoPath    (Ferr2D_Path path)
    {
        Handles.color = Color.white;
        Ferr2DT_PathTerrain terrain = path.gameObject.GetComponent<Ferr2DT_PathTerrain>();
        List<Vector2> verts = terrain == null ? path.GetVertsRaw() : path.GetVerts(false, 2, terrain.splitCorners);
        for (int i = 0; i < verts.Count - 1; i++)
        {
            Vector3 pos  = path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(verts[i    ].x, verts[i    ].y, 0), path.transform.localScale);
            Vector3 pos2 = path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(verts[i + 1].x, verts[i + 1].y, 0), path.transform.localScale);
            Handles.DrawLine(pos + offset, pos2 + offset);
        }
        if (path.closed)
        {
            Vector3 pos  = path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(verts[0              ].x, verts[0              ].y, 0), path.transform.localScale);
            Vector3 pos2 = path.transform.position + path.transform.rotation * Vector3.Scale(new Vector3(verts[verts.Count - 1].x, verts[verts.Count - 1].y, 0), path.transform.localScale);
            Handles.DrawLine(pos + offset, pos2 + offset);
        }
    }

	public override void OnInspectorGUI() {
		#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
		Undo.RecordObject(target, "Modified Path");
		#else
        Undo.SetSnapshotTarget(target, "Modified Path");
		#endif

        Ferr2D_Path path = (Ferr2D_Path)target;

        path.closed = EditorGUILayout.Toggle ("Closed", path.closed);

        // display the path verts list info
        showVerts   = EditorGUILayout.Foldout(showVerts, "Path Vertices");
        EditorGUI.indentLevel = 2;
        if (showVerts)
        {
            int size = EditorGUILayout.IntField("Count: ", path.pathVerts.Count);
            while (path.pathVerts.Count > size) path.pathVerts.RemoveAt(path.pathVerts.Count - 1);
            while (path.pathVerts.Count < size) path.pathVerts.Add     (new Vector2(0, 0));
        }
        // draw all the verts! Long list~
        for (int i = 0; showVerts && i < path.pathVerts.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("#" + i, GUILayout.Width(60));
            path.pathVerts[i] = new Vector2(
                EditorGUILayout.FloatField(path.pathVerts[i].x),
                EditorGUILayout.FloatField(path.pathVerts[i].y));
            EditorGUILayout.EndHorizontal();
        }

        // button for updating the origin of the object
        if (GUILayout.Button("Center Position")) path.ReCenter();

        // update dependants when it changes
        if (GUI.changed)
        {
			Ferr2DT_PathTerrain terrain = path.GetComponent<Ferr2DT_PathTerrain>();
			if (!path.closed && terrain != null && (terrain.fill == Ferr2DT_FillMode.Closed || terrain.fill == Ferr2DT_FillMode.InvertedClosed || terrain.fill == Ferr2DT_FillMode.FillOnlyClosed)) 
					path.closed = true;
			path.UpdateDependants();
            EditorUtility.SetDirty(target);
        }
	}
#if FERR2D_TERRAIN
    private Texture2D GetDirIcon(Ferr2DT_TerrainDirection aDir) {
        if      (aDir == Ferr2DT_TerrainDirection.Top   ) return texTop;
        else if (aDir == Ferr2DT_TerrainDirection.Right ) return texRight;
        else if (aDir == Ferr2DT_TerrainDirection.Left  ) return texLeft;
        else if (aDir == Ferr2DT_TerrainDirection.Bottom) return texBottom;
        return texAuto;
    }
    private Ferr2DT_TerrainDirection NextDir(Ferr2DT_TerrainDirection aDir) {
        if      (aDir == Ferr2DT_TerrainDirection.Top   ) return Ferr2DT_TerrainDirection.Right;
        else if (aDir == Ferr2DT_TerrainDirection.Right ) return Ferr2DT_TerrainDirection.Bottom;
        else if (aDir == Ferr2DT_TerrainDirection.Left  ) return Ferr2DT_TerrainDirection.Top;
        else if (aDir == Ferr2DT_TerrainDirection.Bottom) return Ferr2DT_TerrainDirection.None;
        return Ferr2DT_TerrainDirection.Left;
    }
#endif
	public Vector2 GetMousePos  (Vector2 aMousePos, float aZOffset) {
		
		//aMousePos.y = Screen.height - (aMousePos.y + 25);
		Ray   ray   = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(aMousePos.x, aMousePos.y, 0));
		Plane plane = new Plane(new Vector3(0,0,-1), aZOffset);
		float dist  = 0;
		Vector3 result = new Vector3(0,0,0);
		
		ray = HandleUtility.GUIPointToWorldRay(aMousePos);
		if (plane.Raycast(ray, out dist)) {
			result = ray.GetPoint(dist);
		}
		return new Vector2(result.x, result.y);
	}
    public static float   GetCameraDist(Vector3 aPt) {
        return Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, aPt);
    }
    public static bool    SetScale     (Vector3 aPos, Texture aIcon, ref GUIStyle aStyle, float aScaleOverride = 1) {
        if (Vector3.Dot(SceneView.lastActiveSceneView.camera.transform.forward, aPos - SceneView.lastActiveSceneView.camera.transform.position) > 0) {
            float max      = (Screen.width + Screen.height) / 2;
			float dist     = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.5f : GetCameraDist(aPos);

			aStyle.fixedWidth  = (aIcon.width  / (dist / (max / 160))) * Ferr_Menu.PathScale * aScaleOverride;
            aStyle.fixedHeight = (aIcon.height / (dist / (max / 160))) * Ferr_Menu.PathScale * aScaleOverride;

            return true;
        }
        return false;
    }
    public static float HandleScale(Vector3 aPos) {
		float dist = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.45f : GetCameraDist(aPos);
		return Mathf.Min(0.4f * Ferr_Menu.PathScale, (dist/5.0f) * 0.4f * Ferr_Menu.PathScale);
    }
    private static Vector3 SnapVector(Vector3 aVector) {
        return new Vector3(
            ((int)(aVector.x / Ferr_Menu.SnapGrid + (aVector.x>0?0.5f:-0.5f))) * Ferr_Menu.SnapGrid,
            ((int)(aVector.y / Ferr_Menu.SnapGrid + (aVector.y > 0 ? 0.5f : -0.5f))) * Ferr_Menu.SnapGrid,
            ((int)(aVector.z / Ferr_Menu.SnapGrid + (aVector.z > 0 ? 0.5f : -0.5f))) * Ferr_Menu.SnapGrid);
    }
    private static Vector2 SnapVector(Vector2 aVector) {
        return new Vector2(
            ((int)(aVector.x / Ferr_Menu.SnapGrid + (aVector.x > 0 ? 0.5f : -0.5f))) * Ferr_Menu.SnapGrid,
            ((int)(aVector.y / Ferr_Menu.SnapGrid + (aVector.y > 0 ? 0.5f : -0.5f))) * Ferr_Menu.SnapGrid);
    }
}