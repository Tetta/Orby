#pragma strict

import System.Collections.Generic;

public class ToolboxItemTypeJS extends MonoBehaviour
{
    public var count				: int;							//The number of items the toolbox holds
    public var width				: float;						//The width of the toolbox in units (100 pixel = 1 unit)
    public var normalPrefab			: ObjectBaseJS;					//A link to the prefab the item holds
    public var counter				: SpriteRenderer;				//A link to the counter
    public var toolboxNumbers		: Sprite[];						//Holds the toolbox numbers

    private var content				: List.<ObjectBaseJS>;			//The content the item holds
    private var placedContent		: List.<ObjectBaseJS>;			//The content the item had, but now are placed

    //Called at the beginning of the levels
    public function Start()
    {
        //Create the lists
        content = new List.<ObjectBaseJS>();
        placedContent = new List.<ObjectBaseJS>();
        
        //If the item in the toolbox is available more than once, enable the counter, and set its number
        if (count > 1)
        {
            counter.gameObject.SetActive(true);
            counter.sprite = toolboxNumbers[count - 2];
        }

        //Spawn the required number of clones from the prefab
        for (var i : int = 0; i < count; i++)
        {
            var go : ObjectBaseJS = Instantiate(normalPrefab, transform.position, Quaternion.identity);
            content.Add(go.GetComponent(ObjectBaseJS));

            go.transform.parent = this.transform;
            go.gameObject.SetActive(false);
        }
    }

    //Updates the position of the item
    public function SetPosition (pos : Vector3)
    {
        this.transform.position = pos;
    }
    //Adds a prefab clone to the item, and updates counter
    public function AddItem(item : ObjectBaseJS)
    {
        placedContent.Remove(item);
        content.Add(item);

        item.transform.eulerAngles = Vector3.zero;
        item.gameObject.SetActive(false);

        if (count == 1)
            counter.gameObject.SetActive(true);

        count++;

        if (count > 1)
            counter.sprite = toolboxNumbers[count - 2];
    }

    //Removes and returns the first prefab clone from the content
    public function RemoveItem()
    {
        var go : ObjectBaseJS = content[0];

        placedContent.Add(go);
        content.Remove(go);

        go.gameObject.SetActive(true);

        count--;
        if (count > 1)
            counter.sprite = toolboxNumbers[count - 2];
        else if (count == 1)
            counter.gameObject.SetActive(false);
        else
            ToolboxManagerJS.Instance().DisableItemType(this);

        return go;
    }

    //Resets the item
    public function Reset()
    {
        //Removes the placed elements from the map
        while (placedContent.Count > 0)
        {
            LevelManagerJS.Instance().RemoveItem(placedContent[0]);
            AddItem(placedContent[0]);        
        }

        //Update counter
        if (count > 2)
            counter.sprite = toolboxNumbers[count - 2];
    }
    //Returns the width of the item in units
    public function GetWidth()
    {
        return width;
    }
    //Returns the name of the prefab
    public function ContentName()
    {
        return normalPrefab.name;
    }
}
