using UnityEngine;
using UnityEditor;

using System;
using System.Collections;

public static class Ferr2DT_TerrainMaterialUtility {
    public static Rect AtlasField          (Ferr2DT_TerrainMaterial aMat, Rect aRect, Texture aTexture, float aWidth) {
		
		EditorGUILayout.BeginHorizontal(GUILayout.Height(64), GUILayout.Width(aWidth));
		GUILayout.Space(10);
		GUILayout.Space(64);
		
		Rect  r   = GUILayoutUtility.GetLastRect();
		float max = Mathf.Max (1, Mathf.Max ( aRect.width, aRect.height ));
		r.width   = Mathf.Max (1, (aRect.width  / max) * 64);
		r.height  = Mathf.Max (1, (aRect.height / max) * 64);
		
		GUI      .DrawTexture(new Rect(r.x-1,  r.y-1,    r.width+2, 1),          EditorGUIUtility.whiteTexture);
		GUI      .DrawTexture(new Rect(r.x-1,  r.yMax+1, r.width+2, 1),          EditorGUIUtility.whiteTexture);
		GUI      .DrawTexture(new Rect(r.x-1,  r.y-1,    1,         r.height+2), EditorGUIUtility.whiteTexture);
		GUI      .DrawTexture(new Rect(r.xMax, r.y-1,    1,         r.height+2), EditorGUIUtility.whiteTexture);
		GUI      .DrawTextureWithTexCoords(r, aTexture, aMat.ToUV(aRect));
		GUILayout.Space(10);
		
		Rect result = EditorGUILayout.RectField(aRect);
		EditorGUILayout.EndHorizontal();
		
		return result;
	}
    public static void ShowPreview         (Ferr2DT_TerrainMaterial aMat, Ferr2DT_TerrainDirection aDir, bool aSimpleUVs, bool aEditable, float aWidth) {
		if (aMat.edgeMaterial == null || aMat.edgeMaterial.mainTexture == null) return;
		
		GUILayout.Label(aMat.edgeMaterial.mainTexture, GUILayout.Width(Screen.width-aWidth));
		
		Rect texRect   = GUILayoutUtility.GetLastRect();
        texRect.width  = Mathf.Min(Screen.width-aWidth, aMat.edgeMaterial.mainTexture.width);
        texRect.height = (texRect.width / aMat.edgeMaterial.mainTexture.width) * aMat.edgeMaterial.mainTexture.height;
		
		ShowPreviewDirection(aMat, aDir, texRect, aSimpleUVs, aEditable);
	}
	public static void ShowPreviewDirection(Ferr2DT_TerrainMaterial aMat, Ferr2DT_TerrainDirection aDir, Rect aBounds, bool aSimpleUVs, bool aEditable) {
		Ferr2DT_SegmentDescription desc = aMat.descriptors[(int)aDir];
        if (!aMat.Has(aDir)) return;

        if (!aEditable) {
            for (int i = 0; i < desc.body.Length; i++)
            {
                Ferr_EditorTools.DrawRect(aMat.ToScreen( desc.body[i]  ), aBounds);    
            }
		    Ferr_EditorTools.DrawRect(aMat.ToScreen( desc.leftCap  ), aBounds);
		    Ferr_EditorTools.DrawRect(aMat.ToScreen( desc.rightCap ), aBounds);
        }
        else if (aSimpleUVs) {
            float   height    = MaxHeight(desc);
            float   capWidth  = Mathf.Max(desc.leftCap.width, desc.rightCap.width);
            float   bodyWidth = desc.body[0].width;
            int     bodyCount = desc.body.Length;
            Vector2 pos       = new Vector2(desc.leftCap.x, desc.leftCap.y);
            if (desc.leftCap.width == 0 && desc.leftCap.height == 0) pos = new Vector2(desc.body[0].x, desc.body[0].y);

            Rect bounds = new Rect(pos.x, pos.y, capWidth*2+bodyWidth*bodyCount, height);
            bounds = Ferr_EditorTools.UVRegionRect(bounds,  aBounds);
            bounds = ClampRect(bounds, (Texture2D)aMat.edgeMaterial.mainTexture);
            Ferr_EditorTools.DrawVLine(new Vector2(pos.x + capWidth + aBounds.x, pos.y+1), height);
            for (int i = 1; i <= desc.body.Length; i++) {
                Ferr_EditorTools.DrawVLine(new Vector2(pos.x + capWidth + bodyWidth*i + aBounds.x, pos.y+1), height);
            }

            height    = bounds.height;
            bodyWidth = (bounds.width - capWidth * 2) / bodyCount;
            pos.x     = bounds.x;
            pos.y     = bounds.y;

            float currX = pos.x;
            desc.leftCap.x      = currX;
            desc.leftCap.y      = pos.y;
            desc.leftCap.width  = capWidth;
            desc.leftCap.height = capWidth == 0 ? 0 : height;
            currX += capWidth;

            for (int i = 0; i < desc.body.Length; i++)
            {
                desc.body[i].x      = currX;
                desc.body[i].y      = pos.y;
                desc.body[i].width  = bodyWidth;
                desc.body[i].height = height;
                currX += bodyWidth;
            }

            desc.rightCap.x      = currX;
            desc.rightCap.y      = pos.y;
            desc.rightCap.width  = capWidth;
            desc.rightCap.height = capWidth == 0 ? 0 : height;

        } else {
            for (int i = 0; i < desc.body.Length; i++) {
                desc.body[i]  = ClampRect(Ferr_EditorTools.UVRegionRect(desc.body[i], aBounds), (Texture2D)aMat.edgeMaterial.mainTexture);
            }
            if (desc.leftCap.width != 0 && desc.leftCap.height != 0)
                desc.leftCap  = ClampRect(Ferr_EditorTools.UVRegionRect(desc.leftCap,  aBounds), (Texture2D)aMat.edgeMaterial.mainTexture);
            if (desc.rightCap.width != 0 && desc.rightCap.height != 0)
                desc.rightCap = ClampRect(Ferr_EditorTools.UVRegionRect(desc.rightCap, aBounds), (Texture2D)aMat.edgeMaterial.mainTexture);
        }
	}
    public static void ShowSample          (Ferr2DT_TerrainMaterial aMat, Ferr2DT_TerrainDirection aDir, float aWidth) {
        if (aMat.edgeMaterial == null || aMat.edgeMaterial.mainTexture == null)  return;

        Ferr2DT_SegmentDescription desc = aMat.descriptors[(int)aDir];
        float totalWidth                = desc.leftCap.width + desc.rightCap.width + (desc.body[0].width * 3);
        float sourceHeight              = MaxHeight(desc);

        float scale = Mathf.Min(aWidth/totalWidth, 64 / sourceHeight);

        GUILayout.Space(sourceHeight* scale);
        float x = 0;
        float y = GUILayoutUtility.GetLastRect().y;
        if (desc.leftCap.width != 0) {
            float yOff = ((sourceHeight - desc.leftCap.height) / 2) * scale;
            GUI.DrawTextureWithTexCoords(new Rect(0,y+yOff,desc.leftCap.width * scale, desc.leftCap.height * scale), aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture, aMat.ToUV(desc.leftCap));
            x += desc.leftCap.width * scale;
        }
        for (int i = 0; i < 3; i++)
        {
            int id = (2-i) % desc.body.Length;
            float yOff = ((sourceHeight - desc.body[id].height) / 2) * scale;
            GUI.DrawTextureWithTexCoords(new Rect(x,y+yOff,desc.body[id].width * scale, desc.body[id].height * scale), aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture, aMat.ToUV(desc.body[id]));
            x += desc.body[id].width * scale;
        }
        if (desc.leftCap.width != 0) {
            float yOff = ((sourceHeight - desc.rightCap.height) / 2) * scale;
            GUI.DrawTextureWithTexCoords(new Rect(x,y+yOff,desc.rightCap.width * scale, desc.rightCap.height * scale), aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture, aMat.ToUV(desc.rightCap));
        }
    }
    
    public static bool IsSimple      (Ferr2DT_SegmentDescription aDesc) {
        float height = aDesc.leftCap.height;
        bool noCaps = aDesc.leftCap.width == 0 && aDesc.rightCap.width == 0;
        
        bool heightSame = noCaps ? 
            aDesc.body[0] .height == aDesc.body[aDesc.body.Length-1].height :
            aDesc.rightCap.height == height && aDesc.body[0].height == height;
        bool capSame    = aDesc.leftCap.width   == aDesc.rightCap.width;
        bool ySame      = noCaps ?
            aDesc.body[0].y == aDesc.body[aDesc.body.Length-1].y :
            aDesc.leftCap.y == aDesc.rightCap.y && aDesc.leftCap.y == aDesc.body[0].y && aDesc.leftCap.y == aDesc.body[aDesc.body.Length-1].y;
        bool xSpaced    = noCaps ? 
            (aDesc.body[0].x - aDesc.body[aDesc.body.Length-1].x) % aDesc.body[0].width == 0:
            aDesc.leftCap.xMax == aDesc.body[0].x  && aDesc.rightCap.x == aDesc.body[aDesc.body.Length-1].xMax;

        return heightSame && capSame && ySame && xSpaced;
    }
    public static void EditUVsSimple (Ferr2DT_SegmentDescription desc)
    {
        float   height    = MaxHeight(desc);
        float   capWidth  = Mathf.Max(desc.leftCap.width, desc.rightCap.width);
        float   bodyWidth = desc.body[0].width;
        int     bodyCount = desc.body.Length;
        Vector2 pos       = new Vector2(desc.leftCap.x, desc.leftCap.y);
        if (desc.leftCap.width == 0 && desc.leftCap.height == 0) pos = new Vector2(desc.body[0].x, desc.body[0].y);

        pos       = EditorGUILayout.Vector2Field("Position",    pos      );
        height    = EditorGUILayout.FloatField  ("Height",      height   );
        capWidth  = EditorGUILayout.FloatField  ("Cap Width",   capWidth );
        bodyWidth = EditorGUILayout.FloatField  ("Body Width",  bodyWidth);
        bodyCount = Mathf.Max(1, EditorGUILayout.IntField    ("Body slices", bodyCount));

        if (bodyCount != desc.body.Length) {
            Array.Resize<Rect>(ref desc.body, bodyCount);
        }

        float currX = pos.x;
        desc.leftCap.x      = currX;
        desc.leftCap.y      = pos.y;
        desc.leftCap.width  = capWidth;
        desc.leftCap.height = capWidth == 0 ? 0 : height;
        currX += capWidth;

        for (int i = 0; i < desc.body.Length; i++)
        {
            desc.body[i].x      = currX;
            desc.body[i].y      = pos.y;
            desc.body[i].width  = bodyWidth;
            desc.body[i].height = height;
            currX += bodyWidth;
        }

        desc.rightCap.x      = currX;
        desc.rightCap.y      = pos.y;
        desc.rightCap.width  = capWidth;
        desc.rightCap.height = capWidth == 0 ? 0 : height;
    }
    public static void EditUVsComplex(Ferr2DT_TerrainMaterial    aMat, Ferr2DT_SegmentDescription desc, float aWidth, ref int aCurrBody)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Body", GUILayout.Width(40f));

        int bodyID = Mathf.Clamp(aCurrBody, 0, desc.body.Length);
        if (GUILayout.Button("<", GUILayout.Width(20f))) aCurrBody = Mathf.Clamp(aCurrBody - 1, 0, desc.body.Length - 1);
        EditorGUILayout.LabelField("" + (bodyID + 1), GUILayout.Width(12f));
        if (GUILayout.Button(">", GUILayout.Width(20f))) aCurrBody = Mathf.Clamp(aCurrBody + 1, 0, desc.body.Length - 1);
        bodyID = Mathf.Clamp(aCurrBody, 0, desc.body.Length - 1);
        int length = Math.Max(1, EditorGUILayout.IntField(desc.body.Length, GUILayout.Width(32f)));
        EditorGUILayout.LabelField("Total", GUILayout.Width(40f));
        if (length != desc.body.Length) Array.Resize<Rect>(ref desc.body, length);

        EditorGUILayout.EndHorizontal();

        desc.body[bodyID] = AtlasField(aMat, desc.body[bodyID], aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture, aWidth);
        if (desc.leftCap.width == 0 && desc.leftCap.height == 0)
        {
            if (EditorGUILayout.Toggle("Left Cap", false))
            {
                desc.leftCap = new Rect(0, 0, 30, 30);
            }
        }
        else
        {
            if (EditorGUILayout.Toggle("Left Cap", true))
            {
                desc.leftCap = AtlasField(aMat, desc.leftCap, aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture, aWidth);
            }
            else
            {
                desc.leftCap = new Rect(0, 0, 0, 0);
            }

        }
        if (desc.rightCap.width == 0 && desc.rightCap.height == 0)
        {
            if (EditorGUILayout.Toggle("Right Cap", false))
            {
                desc.rightCap = new Rect(0, 0, 30, 30);
            }
        }
        else
        {
            if (EditorGUILayout.Toggle("Right Cap", true))
            {
                desc.rightCap = AtlasField(aMat, desc.rightCap, aMat.edgeMaterial != null ? aMat.edgeMaterial.mainTexture : EditorGUIUtility.whiteTexture, aWidth);
            }
            else
            {
                desc.rightCap = new Rect(0, 0, 0, 0);
            }
        }
    }

    public static float MaxHeight (Ferr2DT_SegmentDescription aDesc) {
        float sourceHeight = Mathf.Max( aDesc.leftCap.height, aDesc.rightCap.height );
        float max          = 0;
        for (int i = 0; i < aDesc.body.Length; i++)
        {
            if (aDesc.body[i].height > max) max = aDesc.body[i].height;
        }
        return Mathf.Max(max, sourceHeight);
    }
    public static Rect  ClampRect (Rect aRect, Texture2D aTex) {
        if (aRect.width  > aTex.width ) aRect.width  = aTex.width;
        if (aRect.height > aTex.height) aRect.height = aTex.height;
        if (aRect.xMax   > aTex.width ) aRect.x      = aTex.width -aRect.width;
        if (aRect.yMax   > aTex.height) aRect.y      = aTex.height-aRect.height;
        if (aRect.x      < 0          ) aRect.x      = 0;
        if (aRect.y      < 0          ) aRect.y      = 0;
        return aRect;
    }
}

public class Ferr2DT_TerrainMaterialWindow : EditorWindow {
    private Ferr2DT_TerrainMaterial material;
    
    int                      currBody  = 0;
    bool                     simpleUVs = true;
    const int                width     = 250;
    Ferr2DT_TerrainDirection currDir   = Ferr2DT_TerrainDirection.Top;
    GUIStyle                 foldoutStyle;

    public static void Show(Ferr2DT_TerrainMaterial aMaterial) {
        Ferr2DT_TerrainMaterialWindow window = EditorWindow.GetWindow<Ferr2DT_TerrainMaterialWindow>();
        window.material       = aMaterial;
        window.wantsMouseMove = true;
        window.title          = "Ferr2DT Editor";
        if (aMaterial != null && aMaterial.edgeMaterial != null) {
            window.minSize = new Vector2(width + aMaterial.edgeMaterial.mainTexture.width +10, aMaterial.edgeMaterial.mainTexture.height+10);
        }
        window.foldoutStyle           = EditorStyles.foldout;
        window.foldoutStyle.fontStyle = FontStyle.Bold;
        window.currDir                = Ferr2DT_TerrainDirection.None;
    }

    void OnGUI        () {
        if (material == null) return;

        // if this was an undo, repaint it
        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    Repaint ();
                    return;
            }
        }

		#if !(UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
		Undo.RecordObject(material, "Modified Terrain Material");
		#else
        Undo.SetSnapshotTarget(material, "Modified Terrain Material");
        if (!Ferr_EditorTools.HandlesMoving()) {
            Undo.CreateSnapshot();
        }
		#endif
        if (Ferr_EditorTools.ResetHandles()) {
            GUI.changed = true;
        }
        
        EditorGUILayout .BeginHorizontal ();
        EditorGUILayout .BeginVertical   (GUILayout.Width(width));

        if (EditorGUILayout.Foldout(currDir == Ferr2DT_TerrainDirection.Top,    "Top",    foldoutStyle)) {
            if (currDir != Ferr2DT_TerrainDirection.Top) simpleUVs = Ferr2DT_TerrainMaterialUtility.IsSimple(material.descriptors[(int)Ferr2DT_TerrainDirection.Top]);
            currDir = Ferr2DT_TerrainDirection.Top;
            bool showTop = GUILayout.Toggle(material.Has(Ferr2DT_TerrainDirection.Top), "Use Top");
            material.Set(Ferr2DT_TerrainDirection.Top, showTop);
            if (showTop) ShowDirection(material, currDir);
        }
        if (EditorGUILayout.Foldout(currDir == Ferr2DT_TerrainDirection.Left,   "Left",   foldoutStyle)) {
            if (currDir != Ferr2DT_TerrainDirection.Left) simpleUVs = Ferr2DT_TerrainMaterialUtility.IsSimple(material.descriptors[(int)Ferr2DT_TerrainDirection.Left]);
            currDir = Ferr2DT_TerrainDirection.Left;
            bool showLeft = GUILayout.Toggle(material.Has(Ferr2DT_TerrainDirection.Left), "Use Left");
            material.Set(Ferr2DT_TerrainDirection.Left, showLeft);
            if (showLeft) ShowDirection(material, currDir);
        }
        if (EditorGUILayout.Foldout(currDir == Ferr2DT_TerrainDirection.Right,  "Right",  foldoutStyle)) {
            if (currDir != Ferr2DT_TerrainDirection.Right) simpleUVs = Ferr2DT_TerrainMaterialUtility.IsSimple(material.descriptors[(int)Ferr2DT_TerrainDirection.Right]);
            currDir = Ferr2DT_TerrainDirection.Right;
            bool showRight = GUILayout.Toggle(material.Has(Ferr2DT_TerrainDirection.Right), "Use Right");
            material.Set(Ferr2DT_TerrainDirection.Right, showRight);
            if (showRight) ShowDirection(material, currDir);
        }
        if (EditorGUILayout.Foldout(currDir == Ferr2DT_TerrainDirection.Bottom, "Bottom", foldoutStyle)) {
            if (currDir != Ferr2DT_TerrainDirection.Bottom) simpleUVs = Ferr2DT_TerrainMaterialUtility.IsSimple(material.descriptors[(int)Ferr2DT_TerrainDirection.Bottom]);
            currDir = Ferr2DT_TerrainDirection.Bottom;
            bool showBottom = GUILayout.Toggle(material.Has(Ferr2DT_TerrainDirection.Bottom), "Use Bottom");
            material.Set(Ferr2DT_TerrainDirection.Bottom, showBottom);
            if (showBottom) ShowDirection(material, currDir);
        }
        EditorGUILayout.EndVertical  ();
        EditorGUILayout.BeginVertical();
        if (currDir != Ferr2DT_TerrainDirection.None) {
            Ferr2DT_TerrainMaterialUtility.ShowPreview(material, currDir, simpleUVs, true, width);
        }
        EditorGUILayout.EndVertical  ();
        EditorGUILayout.EndHorizontal();

        if (Event.current.type == EventType.MouseMove || Event.current.type == EventType.MouseDrag)
			Repaint ();

        if (GUI.changed) {
            EditorUtility.SetDirty(material);
			#if (UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
            Undo.RegisterSnapshot();
			#endif

            Ferr2DT_PathTerrain[] terrain = GameObject.FindObjectsOfType(typeof(Ferr2DT_PathTerrain)) as Ferr2DT_PathTerrain[];
            for (int i = 0; i < terrain.Length; i++)
            {
                if(terrain[i].TerrainMaterial == material)
                    terrain[i].RecreatePath();
            }
		}
		#if (UNITY_4_2 || UNITY_4_1 || UNITY_4_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_1 || UNITY_3_0)
        Undo.ClearSnapshotTarget();
		#endif
    }
    void ShowDirection(Ferr2DT_TerrainMaterial aMat, Ferr2DT_TerrainDirection aDir) {
		Ferr2DT_SegmentDescription desc = aMat.descriptors[(int)aDir];

		desc.zOffset   = EditorGUILayout.FloatField( "Z Offset",   desc.zOffset  );
		desc.yOffset   = EditorGUILayout.FloatField( "Y Offset",   desc.yOffset  );
        desc.capOffset = EditorGUILayout.FloatField( "Cap Offset", desc.capOffset);

        Ferr2DT_TerrainMaterialUtility.ShowSample(aMat, aDir, width);

        simpleUVs = EditorGUILayout.Toggle("Simple", simpleUVs);
        if (simpleUVs) {
            Ferr2DT_TerrainMaterialUtility.EditUVsSimple(desc);
        } else {
            Ferr2DT_TerrainMaterialUtility.EditUVsComplex(aMat, desc, width, ref currBody);
        }
	}
}