#pragma strict

public class SetSortingLayerJS extends MonoBehaviour 
{
    public var layerName 		: String = "GameObject";
    public var sortingOrder 	: int = 5;

	function Start () 
    {
        this.renderer.sortingLayerName = layerName;
        this.renderer.sortingOrder = sortingOrder;
	}
}