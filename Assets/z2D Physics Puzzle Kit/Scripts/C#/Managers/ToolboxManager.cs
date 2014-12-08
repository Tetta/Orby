using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToolboxManager : MonoBehaviour 
{
    enum ToolboxState { open, close, inTransit };           //The possible states for the toolbox
    ToolboxState toolboxState = ToolboxState.close;         //The current state of the toolbox

    public float maxToolboxMovement;                        //The maximum toolbox number
    public float spaceBetweenItems = 0;                     //The space between the items in the toolbox
    public Transform button;                                //The toolbox button
    public Transform strip;                                 //The toolbox strip
    public Transform itemContainer;                         //The main container for the toolbox items
    public Transform tempContainer;                         //The temp container for item movement

    static ToolboxManager myInstance;                       //Holds a reference to this script

    List<ToolboxItemType> items;                            //Holds the item types in the toolbox
    List<ToolboxItemType> inactiveItems;                    //Holds the inactive item types

    float totalWidth;                                       //The total width of the toolbox
    float toolboxMovement;                                  //The currently allowed toolbox movement
    float scrollAmmount;                                    //The currently allowed scrolling
    float itemContainerStartPos;                            //The starting position of the container at the beginning of the scroll

    bool transitInProgress = false;                         //Toolbox transit in progress/not in progress
    Vector3 itemContainerPos;                               //The position of the container when scrolling

    //Returns the instance
    public static ToolboxManager Instance { get { return myInstance; } }

    //Called at the beginning of the level
    void Start()
    {
        myInstance = this;
        button.GetComponent<SpriteRenderer>().sortingLayerName = "GUI";
        itemContainerPos = itemContainer.transform.position;

        items = new List<ToolboxItemType>();
        inactiveItems = new List<ToolboxItemType>();
    }

    //Add the items for the container and arrange them
    public void SetupToolbar(List<ToolboxItemType> toolbarItems)
    {
        totalWidth = 0;
        
        float lastWidth = 0;
        float currentWidth = 0;
        float lastPos = button.transform.position.x;
        Vector3 newPos;

        //Loops through the received items
        for (int i = 0; i < toolbarItems.Count; i++)
        {
            //Gets the current item width and calculates the next position
            currentWidth = toolbarItems[i].GetWidth();
            newPos = new Vector3(lastPos + (lastWidth / 2) + spaceBetweenItems + (currentWidth / 2), button.transform.position.y, 0);

            //Set the item to its position
            toolbarItems[i].transform.parent = itemContainer;
            toolbarItems[i].SetPosition(newPos);
            items.Add(toolbarItems[i]);

            lastWidth = currentWidth;
            lastPos = newPos.x;
        }

        //Calculate toolbox movement variable
        totalWidth = lastPos - button.transform.position.x + (lastWidth / 2) + (4 * spaceBetweenItems);
        toolboxMovement = totalWidth;
        scrollAmmount = 0;

        if (toolboxMovement > maxToolboxMovement)
        {
            toolboxMovement = maxToolboxMovement;
            scrollAmmount = totalWidth - toolboxMovement;
        }
    }

    //Called when an input is registered on the toolbox button
    public void ButtonPressed()
    {
        if (toolboxState == ToolboxState.close && items.Count > 0)
            StartCoroutine(MoveHorizontalBy(strip.transform, -toolboxMovement, 0.2f, true));
        else if (toolboxState == ToolboxState.open)
            StartCoroutine(MoveHorizontalBy(strip.transform, toolboxMovement, 0.2f, true));
    }
    //Called by the GUI manager, shows the toolbox
    public void ShowToolbox()
    {
        StartCoroutine("Show");
    }
    //Called by the GUI manager, hides the toolbox
    public void HideToolbox()
    {
        StartCoroutine("Hide");
    }

    //Prepare the toolbox for scrolling
    public void PrepareScolling()
    {
        itemContainerStartPos = itemContainer.position.x;
    }
    //Scrolls the toolbox
    public void ScrollMode(float value)
    {
        itemContainerPos.x = itemContainerStartPos + value;
        itemContainer.position = itemContainerPos;
    }
    //Finalise toolbox scrolling
    public void FinaliseScrolling()
    {
        float moveBy = 0;
        
        //If the toolbox is scrolled out of its zone, move it back
        if (itemContainer.localPosition.x > 0)
            moveBy = -itemContainer.localPosition.x;
        else if (itemContainer.localPosition.x  < -scrollAmmount)
            moveBy = -(itemContainer.localPosition.x + scrollAmmount);

        if (moveBy != 0)
            StartCoroutine(MoveHorizontalBy(itemContainer, moveBy, 0.2f, false));
    }

    //Put an item to the toolbox
    public void AddItem(ObjectBase item)
    {
        //Get the name of the item
        string itemName = item.name.Substring(0, item.name.IndexOf('('));

        //If its toolbox counterpart is active, add the item to it
        foreach (ToolboxItemType itemType in items)
        {
            if (itemType.ContentName() == itemName)
            {
                itemType.AddItem(item);
                return;
            }
        }

        //If we did not find the counterpart in the active objects, scan the inactive items
        foreach (ToolboxItemType itemType in inactiveItems)
        {
            //If we found it, activate it
            if (itemType.ContentName() == itemName)
            {
                StartCoroutine(AddItemType(itemType, item));
                return;
            }
        }
    }
    //Removes every element from the toolbox
    public void ClearToolbox()
    {
        //Resets the toolbox elements
        while (items.Count > 0)
        {
            items[0].Reset();
            items.RemoveAt(0);
        }

        while (inactiveItems.Count > 0)
        {
            inactiveItems[0].gameObject.SetActive(true);
            inactiveItems[0].Reset();

            inactiveItems.RemoveAt(0);
        }

        //Reset the variables
        totalWidth = 0;
        toolboxMovement = 0;
        scrollAmmount = 0;
    }
    //Disables and removes an item type from the toolbox
    public void DisableItemType(ToolboxItemType item)
    {
        //If this is the last item in the toolbox, simply remove it
        if (items.Count == 1)
        {
            inactiveItems.Add(items[0]);

           totalWidth = 0;
            toolboxMovement = 0;
            scrollAmmount = 0;

            items[0].gameObject.SetActive(false);

            items.Remove(items[0]);
            Close();
        }
        //If there is more than one item left in the toolbox, "remove" animation is played
        else
            StartCoroutine("RemoveItemType", item);
    }

    //Opens the toolbox
    void Open()
    {
        if (toolboxState == ToolboxState.close && items.Count > 0)
            StartCoroutine(MoveHorizontalBy(strip.transform, -toolboxMovement, 0.2f, true));
    }
    //Closes the toolbox
    void Close()
    {
        if (toolboxState == ToolboxState.open)
        {
            if (items.Count == 0)
                StartCoroutine(MoveHorizontalBy(strip.transform, -strip.transform.localPosition.x, 0.2f, true));
            else
                StartCoroutine(MoveHorizontalBy(strip.transform, toolboxMovement, 0.2f, true));
        }
    }
    //Returns the index of t in the items array
    int GetIndex(ToolboxItemType t)
    {
        int i = 0;
        while (i < items.Count && t.ContentName() != items[i].ContentName())
            i++;

        return i;
    }
    //Recalculated toolbox movement related variables based
    void ModifyToolboxValues(float addToWidth)
    {
        totalWidth += addToWidth;

        if (totalWidth > maxToolboxMovement)
        {
            toolboxMovement = maxToolboxMovement;
            scrollAmmount = totalWidth - toolboxMovement;
        }
        else
        {
            scrollAmmount = 0;

            if (items.Count > 0)
            {
                toolboxMovement = totalWidth;
            }
            else
            {
                toolboxMovement = 0;
                totalWidth = 0;
            }
        }
    }   
    //Makes the items a child of the item container
    void ResetParents()
    {
        foreach (ToolboxItemType item in items)
            item.transform.parent = itemContainer;
    }
    //Makes the content of the received list a child of the temp container
    void MoveToTemp(List<ToolboxItemType> l)
    {
        foreach (ToolboxItemType item in l)
            item.transform.parent = tempContainer;
    }
    //Returns the items between items[index+1] and items[items.Count]
    List<ToolboxItemType> GetItemsAfter(int index)
    {
        List<ToolboxItemType> rightItems = new List<ToolboxItemType>();

        for (int i = index+1; i < items.Count; i++)
            rightItems.Add(items[i]);

        return rightItems;
    }

    //Adds an item type to the toolbox
    IEnumerator AddItemType(ToolboxItemType t, ObjectBase item)
    {
        t.AddItem(item);

        while (transitInProgress)
            yield return new WaitForEndOfFrame();

        transitInProgress = true;

        float newTypePos;
        float addWidth;

        //If the toolbox is empty, calculate new position from the toolbox button
        if (items.Count == 0)
        {
            newTypePos = button.transform.position.x + (t.GetWidth() / 2) + (spaceBetweenItems);
            addWidth = t.GetWidth() + (spaceBetweenItems * 5);
        }
        //Else, calculate new position from the last item
        else
        {
            newTypePos = items[items.Count - 1].transform.position.x + (items[items.Count - 1].GetWidth() / 2) + spaceBetweenItems + (t.GetWidth() / 2);
            addWidth = t.GetWidth() + spaceBetweenItems;
        }

        Vector3 newPos = new Vector3(newTypePos, button.transform.position.y, t.transform.position.z);

        //Reset the type 
        t.SetPosition(newPos);
        t.gameObject.SetActive(true);

        //Remove the type from the inactive list and add it to the active list
        inactiveItems.Remove(t);
        items.Add(t);

        ModifyToolboxValues(addWidth);

        yield return new WaitForEndOfFrame();

        //If the newly added item is the only item, open the toolbox
        if (items.Count == 1)
        {
            Open();
            yield return new WaitForSeconds(0.2f);
        }
        //If the item can fit into the toolbox without scrolling, extend the toolbox
        else if (scrollAmmount == 0)
        {
            StartCoroutine(MoveHorizontalBy(strip, -(t.GetWidth() + spaceBetweenItems), 0.2f, false));
            yield return new WaitForSeconds(0.2f);
        }

        transitInProgress = false;
    }
    //Removes an item type from the toolbox
    IEnumerator RemoveItemType(ToolboxItemType t)
    {
        while (transitInProgress)
            yield return new WaitForEndOfFrame();

        transitInProgress = true;

        //Get the index of the item and store the items to its right
        int index = GetIndex(t);
        List<ToolboxItemType> right = GetItemsAfter(index);

        //Remove the item type from the items list
        items.Remove(t);
        inactiveItems.Add(t);
        t.gameObject.SetActive(false);

        //Place the right items to the temp, and move them to their new place
        MoveToTemp(right);
        StartCoroutine(MoveHorizontalBy(tempContainer, -(t.GetWidth() + spaceBetweenItems), 0.2f, false));

        //Modify toolbox movement variables
        ModifyToolboxValues(-(t.GetWidth() + spaceBetweenItems));

        //StartCoroutine("Test");
        yield return new WaitForSeconds(0.2f);

        //Copy back the items from the temp to their original position
        ResetParents();

        //If the items can fit into the whole toolbox
        if (totalWidth < maxToolboxMovement && items.Count > 0)
        {
            //Move the toolbox to the right
            float movementAmmount = button.transform.position.x - (3 * spaceBetweenItems) - (items[items.Count - 1].GetWidth() / 2);
            movementAmmount -= items[items.Count - 1].transform.position.x;

            StartCoroutine(MoveHorizontalBy(strip, movementAmmount, 0.2f, false));
            yield return new WaitForSeconds(0.2f);
        }

        //Finalise item scrolling
        FinaliseScrolling();

        transitInProgress = false;
    }
    //Hides the toolbox
    IEnumerator Hide()
    {
        //If the toolbox is open
        if (toolboxState == ToolboxState.open)
        {
            //Close it and wait for it
            StartCoroutine(MoveHorizontalBy(strip, toolboxMovement, 0.4f, false));
            yield return new WaitForSeconds(0.4f);
            strip.transform.localPosition = new Vector2(0, strip.transform.localPosition.y);
        }

        //Hide the toolbox
        strip.gameObject.SetActive(false);
        StartCoroutine(MoveHorizontalBy(this.transform, 1.5f, 0.35f, false));
    }
    //Shows the toolbox
    IEnumerator Show()
    {
        transitInProgress = true;

        //Show the toolbox and wait for it
        StartCoroutine(MoveHorizontalBy(this.transform, -1.5f, 0.35f, false));
        yield return new WaitForSeconds(0.35f);

        //Reactivate toolbox strip
        strip.gameObject.SetActive(true);

        //If the toolbox state is open
        if (toolboxState == ToolboxState.open)
        {
            //Open it
            StartCoroutine(MoveHorizontalBy(strip, -toolboxMovement, 0.4f, true));
        }
    }
    //Moves an object by an ammount, under time
    IEnumerator MoveHorizontalBy(Transform obj, float moveBy, float time, bool changeToolboxState)
    {
        //If changeing state is allowed, change to in transit state
        if (changeToolboxState)
            toolboxState = ToolboxState.inTransit;

        //Move the menu to the designated position under time
        float i = 0.0f;
        float rate = 1.0f / time;

        Vector3 startPos = obj.localPosition;
        Vector3 endPos = startPos;
        endPos.x += moveBy;

        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            obj.localPosition = Vector3.Lerp(startPos, endPos, i);
            yield return 0;
        }

        //If changing state is allowed, change to the right state
        if (changeToolboxState)
        {
            if (moveBy < 0)
                toolboxState = ToolboxState.open;
            else
                toolboxState = ToolboxState.close;
        }
    }
}
