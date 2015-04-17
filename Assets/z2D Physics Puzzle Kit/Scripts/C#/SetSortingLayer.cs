using UnityEngine;
using System.Collections;

public class SetSortingLayer : MonoBehaviour 
{
    public string layerName = "GameObject";
    public int sortingOrder = 5;

	void Start () 
    {
        this.GetComponent<Renderer>().sortingLayerName = layerName;
        this.GetComponent<Renderer>().sortingOrder = sortingOrder;
	}
}
