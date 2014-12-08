using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToolboxItemType : MonoBehaviour
{
    public int count;                           //The number of items the toolbox holds
    public float width;                         //The width of the toolbox in units (100 pixel = 1 unit)
    public ObjectBase normalPrefab;             //A link to the prefab the item holds
    public SpriteRenderer counter;              //A link to the counter
    public Sprite[] toolboxNumbers;             //Holds the toolbox numbers

    List<ObjectBase> content;                   //The content the item holds
    List<ObjectBase> placedContent;             //The content the item had, but now are placed

    //Called at the beginning of the levels
    public void Start()
    {
        //Create the lists
        content = new List<ObjectBase>();
        placedContent = new List<ObjectBase>();
        
        //If the item in the toolbox is available more than once, enable the counter, and set its number
        if (count > 1)
        {
            counter.gameObject.SetActive(true);
            counter.sprite = toolboxNumbers[count - 2];
        }

        //Spawn the required number of clones from the prefab
        for (int i = 0; i < count; i++)
        {
            ObjectBase go = (ObjectBase)Instantiate(normalPrefab, transform.position, Quaternion.identity);
            content.Add(go.GetComponent<ObjectBase>());

            go.transform.parent = this.transform;
            go.gameObject.SetActive(false);
        }
    }

    //Updates the position of the item
    public void SetPosition (Vector3 pos)
    {
        this.transform.position = pos;
    }
    //Adds a prefab clone to the item, and updates counter
    public void AddItem(ObjectBase item)
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
    public ObjectBase RemoveItem()
    {
        ObjectBase go = content[0];

        placedContent.Add(go);
        content.Remove(go);

        go.gameObject.SetActive(true);

        count--;
        if (count > 1)
            counter.sprite = toolboxNumbers[count - 2];
        else if (count == 1)
            counter.gameObject.SetActive(false);
        else
            ToolboxManager.Instance.DisableItemType(this);

        return go;
    }

    //Resets the item
    public void Reset()
    {
        //Removes the placed elements from the map
        while (placedContent.Count > 0)
        {
            LevelManager.Instance.RemoveItem(placedContent[0]);
            AddItem(placedContent[0]);        
        }

        //Update counter
        if (count > 2)
            counter.sprite = toolboxNumbers[count - 2];
    }
    //Returns the width of the item in units
    public float GetWidth()
    {
        return width;
    }
    //Returns the name of the prefab
    public string ContentName()
    {
        return normalPrefab.name;
    }
}
