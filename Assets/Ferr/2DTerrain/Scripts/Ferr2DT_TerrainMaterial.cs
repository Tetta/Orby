using UnityEngine;
using System.Collections;

/// <summary>
/// A direction used to describe the surface of terrain.
/// </summary>
public enum Ferr2DT_TerrainDirection
{
	Top    = 0,
	Left   = 1,
	Right  = 2,
	Bottom = 3,
    None   = 100
}

/// <summary>
/// Describes a terrain segment, and how it should be drawn.
/// </summary>
[System.Serializable]
public class Ferr2DT_SegmentDescription {
    /// <summary>
    /// Applies only to terrain segments facing this direction.
    /// </summary>
    public Ferr2DT_TerrainDirection applyTo;
    /// <summary>
    /// Z Offset, for counteracting depth issues.
    /// </summary>
    public float  zOffset;
    /// <summary>
    /// Just in case you want to adjust the height of the segment
    /// </summary>
    public float  yOffset;
    /// <summary>
    /// UV coordinates for the left ending cap.
    /// </summary>
	public Rect   leftCap;
    /// <summary>
    /// UV coordinates for the right ending cap.
    /// </summary>
	public Rect   rightCap;
    /// <summary>
    /// A list of body UVs to randomly pick from.
    /// </summary>
	public Rect[] body;
    /// <summary>
    /// How much should the end of the path slide to make room for the caps? (Unity units)
    /// </summary>
    public float  capOffset = 0f;

	public Ferr2DT_SegmentDescription() {
		body    = new Rect[] { new Rect(0,0,50,50) };
		applyTo = Ferr2DT_TerrainDirection.Top;
	}

	public Ferr_JSONValue ToJSON  () {
		Ferr_JSONValue json = new Ferr_JSONValue();
		json["applyTo"      ] = (int)applyTo;
		json["zOffset"      ] = zOffset;
		json["yOffset"      ] = yOffset;
		json["capOffset"    ] = capOffset;
		json["leftCap.x"    ] = leftCap.x;
		json["leftCap.y"    ] = leftCap.y;
		json["leftCap.xMax" ] = leftCap.xMax;
		json["leftCap.yMax" ] = leftCap.yMax;
		json["rightCap.x"   ] = rightCap.x;
		json["rightCap.y"   ] = rightCap.y;
		json["rightCap.xMax"] = rightCap.xMax;
		json["rightCap.yMax"] = rightCap.yMax;

		json["body"] = 0;
		Ferr_JSONValue bodyArr = json["body"];
		for (int i = 0; i < body.Length; i++) {
			Ferr_JSONValue rect = new Ferr_JSONValue();
			rect["x"   ] = body[i].x;
			rect["y"   ] = body[i].y;
			rect["xMax"] = body[i].xMax;
			rect["yMax"] = body[i].yMax;

			bodyArr[i] = rect;
		}
		return json;
	}
	public void           FromJSON(Ferr_JSONValue aJSON) {
		Ferr_JSONValue json = new Ferr_JSONValue();
		applyTo = (Ferr2DT_TerrainDirection)aJSON["applyTo", (int)Ferr2DT_TerrainDirection.Top];
		zOffset   = aJSON["zOffset",0f];
		yOffset   = aJSON["yOffset",0f];
		capOffset = aJSON["capOffset",0f];
		leftCap = new Rect(
			aJSON["leftCap.x",     0f],
			aJSON["leftCap.y",     0f],
			aJSON["leftCap.xMax",  0f],
			aJSON["leftCap.yMax",  0f]);
		rightCap = new Rect(
			aJSON["rightCap.x",    0f],
			aJSON["rightCap.y",    0f],
			aJSON["rightCap.xMax", 0f],
			aJSON["rightCap.yMax", 0f]);

		Ferr_JSONValue bodyArr = json["body"];
		body = new Rect[bodyArr.Length];
		for (int i = 0; i < body.Length; i++) {
			body[i] = new Rect(
				bodyArr[i]["x",    0 ],
				bodyArr[i]["y",    0 ],
				bodyArr[i]["xMax", 50],
				bodyArr[i]["yMax", 50]);
		}
	}
}

/// <summary>
/// Describes a material that can be applied to a Ferr2DT_PathTerrain
/// </summary>
public class Ferr2DT_TerrainMaterial : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The material of the interior of the terrain.
    /// </summary>
    public Material                     fillMaterial;
    /// <summary>
    /// The material of the edges of the terrain.
    /// </summary>
    public Material                     edgeMaterial;
    /// <summary>
    /// These describe all four edge options, how the top, left, right, and bottom edges should be drawn.
    /// </summary>
    public Ferr2DT_SegmentDescription[] descriptors = new Ferr2DT_SegmentDescription[4];
    #endregion

    #region Constructor
    public Ferr2DT_TerrainMaterial() {
        for (int i = 0; i < descriptors.Length; i++) {
            descriptors[i] = new Ferr2DT_SegmentDescription();
        }
    }
    #endregion

    #region Methods
	/// <summary>
	/// Creates a JSON string from this TerrainMaterial, edgeMaterial and fillMaterial are stored by name only.
	/// </summary>
	/// <returns>JSON Value object, can put it into a larger JSON object, or just ToString it.</returns>
	public Ferr_JSONValue ToJSON  () {
		Ferr_JSONValue json = new Ferr_JSONValue();
		json["fillMaterialName"] = fillMaterial.name;
		json["edgeMaterialName"] = edgeMaterial.name;

		json["descriptors"     ] = 0;
		Ferr_JSONValue descArr = json["descriptors"];
		for (int i = 0; i < descriptors.Length; i++) {
			descArr[i] = descriptors[i].ToJSON();
		}

		return json;
	}
	/// <summary>
	/// Creates a TerrainMaterial from a JSON string, does -not- link edgeMaterial or fillMaterial, you'll have to do that yourself!
	/// </summary>
	/// <param name="aJSON">A JSON string, gets parsed and sent to FromJSON(Ferr_JSONValue)</param>
	public void           FromJSON(string aJSON) {
		FromJSON(Ferr_JSON.Parse(aJSON));
	}
	/// <summary>
	/// Creates a TerrainMaterial from a JSON object, does -not- link edgeMaterial or fillMaterial, you'll have to do that yourself!
	/// </summary>
	/// <param name="aJSON">A parsed JSON value</param>
	public void           FromJSON(Ferr_JSONValue aJSON) {
		Ferr_JSONValue descArr = aJSON["descriptors"];
		for (int i = 0; i < descArr.Length; i++) {
			descriptors[i] = new Ferr2DT_SegmentDescription();
			descriptors[i].FromJSON(descArr[i]);
		}
	}
    /// <summary>
    /// Gets the edge descriptor for the given edge, defaults to the Top, if none by that type exists, or an empty one, if none are defined at all.
    /// </summary>
    /// <param name="aDirection">Direction to get.</param>
    /// <returns>The given direction, or the first direction, or a default, based on what actually exists.</returns>
    public Ferr2DT_SegmentDescription GetDescriptor(Ferr2DT_TerrainDirection aDirection) {
        for (int i = 0; i < descriptors.Length; i++) {
            if (descriptors[i].applyTo == aDirection) return descriptors[i];
        }
        if (descriptors.Length > 0) {
            return descriptors[0];
        }
        return new Ferr2DT_SegmentDescription();
    }
    /// <summary>
    /// Finds out if we actually have a descriptor for the given direction
    /// </summary>
    /// <param name="aDirection">Duh.</param>
    /// <returns>is it there, or is it not?</returns>
	public bool                       Has          (Ferr2DT_TerrainDirection aDirection) {
		for (int i = 0; i < descriptors.Length; i++) {
            if (descriptors[i].applyTo == aDirection) return true;
        }
		return false;
	}
    /// <summary>
    /// Sets a particular direction as having a valid descriptor. Or not. That's a bool.
    /// </summary>
    /// <param name="aDirection">The direction!</param>
    /// <param name="aActive">To active, or not to active? That is the question!</param>
	public void                       Set          (Ferr2DT_TerrainDirection aDirection, bool aActive) {
		if (aActive) {
			if (descriptors[(int)aDirection].applyTo != aDirection) {
				descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
				descriptors[(int)aDirection].applyTo = aDirection;
			}
		} else if (descriptors[(int)aDirection].applyTo != Ferr2DT_TerrainDirection.Top) {
			descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
		}
	}
    /// <summary>
    /// Converts our internal pixel UV coordinates to UV values Unity will recognize.
    /// </summary>
    /// <param name="aPixelUVs">A UV rect, using pixels.</param>
    /// <returns>A UV rect using Unity coordinates.</returns>
	public Rect                       ToUV    (Rect aPixelUVs) {
		if (edgeMaterial == null) return aPixelUVs;
        return new Rect(
            aPixelUVs.x        / edgeMaterial.mainTexture.width,
            (1.0f - (aPixelUVs.height / edgeMaterial.mainTexture.height)) - (aPixelUVs.y / edgeMaterial.mainTexture.height),
            aPixelUVs.width    / edgeMaterial.mainTexture.width,
            aPixelUVs.height   / edgeMaterial.mainTexture.height);
	}
    /// <summary>
    /// Converts our internal pixel UV coordinates to UV values we can use on the screen! As 0-1.
    /// </summary>
    /// <param name="aPixelUVs">A UV rect, using pixels.</param>
    /// <returns>A UV rect using standard UV coordinates.</returns>
	public Rect                       ToScreen(Rect aPixelUVs) {
		if (edgeMaterial == null) return aPixelUVs;
        return new Rect(
            aPixelUVs.x        / edgeMaterial.mainTexture.width,
            aPixelUVs.y        / edgeMaterial.mainTexture.height,
            aPixelUVs.width    / edgeMaterial.mainTexture.width,
            aPixelUVs.height   / edgeMaterial.mainTexture.height);
    }
    #endregion
}
