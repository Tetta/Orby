using UnityEngine;
using System.Collections;

public class SetSortingLayer : MonoBehaviour 
{
    public string layerName = "GameObject";
    public int sortingOrder = 5;

	void Start () 
    {
        this.renderer.sortingLayerName = layerName;
        this.renderer.sortingOrder = sortingOrder;
	}
}
