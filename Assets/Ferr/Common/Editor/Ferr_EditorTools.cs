using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class Ferr_EditorTools {
    static int handleID       = 0;
    static int selectedHandle = -1;

    public static Texture2D GetGizmo(string aFileName) {
        Texture2D tex = AssetDatabase.LoadAssetAtPath("Assets/Ferr/Gizmos/" + aFileName, typeof(Texture2D)) as Texture2D;
        if (tex == null) {
            tex = EditorGUIUtility.whiteTexture;
            Debug.Log("Couldn't load Gizmo tex " + aFileName);
        }
        return tex;
    }
    public static void DrawRect(Rect aRect) {
        DrawRect(aRect, new Rect(0,0,1,1));
    }
    public static void DrawRect(Rect aRect, Rect aBounds) {
		float x      = aBounds.x + aRect.x * aBounds.width;
		float y      = aBounds.y + aRect.y * aBounds.height;
		float width  = aRect.width  * aBounds.width;
		float height = aRect.height * aBounds.height;
		
		GUI.DrawTexture(new Rect(x,       y,         width, 1     ), EditorGUIUtility.whiteTexture);
		GUI.DrawTexture(new Rect(x,      (y+height), width, 1     ), EditorGUIUtility.whiteTexture);
		GUI.DrawTexture(new Rect(x,       y,         1,     height), EditorGUIUtility.whiteTexture);
		GUI.DrawTexture(new Rect(x+width, y,         1,     height), EditorGUIUtility.whiteTexture);
	}
    public static void DrawHLine(Vector2 aPos, float aLength) {
        GUI.DrawTexture(new Rect(aPos.x, aPos.y, aLength, 1), EditorGUIUtility.whiteTexture);
    }
    public static void DrawVLine(Vector2 aPos, float aLength) {
        GUI.DrawTexture(new Rect(aPos.x, aPos.y, 1, aLength), EditorGUIUtility.whiteTexture);
    }

    public static Rect UVRegionRect(Rect aRect, Rect aBounds) {
        Vector2 pos = RectHandle(new Vector2(aBounds.x+aRect.x, aBounds.y+aRect.y), aRect, aBounds);
        aRect.x = pos.x - aBounds.x;
        aRect.y = pos.y - aBounds.y;

        float left  = MouseHandle(new Vector2(aBounds.x+aRect.x,   aBounds.y+aRect.y+aRect.height/2), 10).x - aBounds.x;
        float right = MouseHandle(new Vector2(aBounds.x+aRect.xMax,aBounds.y+aRect.y+aRect.height/2), 10).x - aBounds.x;

        float top    = MouseHandle(new Vector2(aBounds.x+aRect.x+aRect.width/2,aBounds.y+aRect.y   ), 10).y - aBounds.y;
        float bottom = MouseHandle(new Vector2(aBounds.x+aRect.x+aRect.width/2,aBounds.y+aRect.yMax), 10).y - aBounds.y;

        return new Rect(left, top, right-left, bottom-top);
    }
    public static Vector2 MouseHandle(Vector2 aPos, int aSize) {
        Rect button = new Rect(aPos.x-aSize/2, aPos.y-aSize/2, aSize, aSize);
        GUI.DrawTexture(button, EditorGUIUtility.whiteTexture);
        return RectHandle(aPos, button);
    }
    public static Vector2 RectHandle(Vector2 aPos, Rect aRect) {
        return RectHandle(aPos, aRect, new Rect(0,0,1,1));
    }
    public static Vector2 RectHandle(Vector2 aPos, Rect aRect, Rect aBounds) {
        handleID += 1;

        Ferr_EditorTools.DrawRect(new Rect(aBounds.x+aRect.x, aBounds.y+aRect.y, aRect.width, aRect.height));
        if (Event.current.type == EventType.MouseDown) {
            if (new Rect(aBounds.x+aRect.x, aBounds.y+aRect.y, aRect.width, aRect.height).Contains(Event.current.mousePosition)) {
                selectedHandle = handleID;
            }
        }
        if (selectedHandle == handleID && Event.current.type == EventType.MouseDrag) {
            aPos += Event.current.delta;
        }
        return aPos;
    }

    public static bool ResetHandles() {
        handleID = 0;
        if (Event.current.type == EventType.MouseUp) {
            selectedHandle = -1;
            return true;
        }
        return false;
    }
    public static bool HandlesMoving() {
        return selectedHandle != -1;
    }

    public static List<T> GetPrefabsOfType<T>() where T:Component {
        string[] fileNames  = System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories);
        int      pathLength = Application.dataPath.Length + 1;
        List<T>  result     = new List<T>();

        for (int i = fileNames.Length; i > 0; i--) {
            fileNames[i - 1] = "Assets\\" + fileNames[i - 1].Substring(pathLength);
            GameObject go = UnityEditor.AssetDatabase.LoadAssetAtPath(fileNames[i - 1], typeof(GameObject)) as GameObject;
            if (go != null) {
                T source = go.GetComponent<T>();
                if (source) result.Add(source);
            }
        }
        return result;
    }
}
