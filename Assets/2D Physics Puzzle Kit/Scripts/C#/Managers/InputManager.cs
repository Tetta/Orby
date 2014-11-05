using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour 
{
    enum InputState { scrolling, moving, rotating, waitingForInput };   //The possible states for the toolbox

    public LayerMask mask = -1;					            //Set input layer mask

    static InputManager myInstance;                         //Hold a reference to this script

    InputState inputState = InputState.waitingForInput;     //The state of the input

    RaycastHit2D hit;                                       //The raycast to detect the targetet item

    Transform selectedItem;                                 //The transform of the selected item
    ObjectBase selectedObject;                              //The ObjectBase script of the selected item

    float scrollStartingPos;                                //The starting position to the toolbox scrolling

    bool itemSelectionValid = false;                        //Selected item valid/invalid for dragging/rotating
    bool hasFeedback = false;                               //Feedback activated/deactivated

    Vector3 inputPos;                                       //The position of the input
    Vector3 offset;                                         //The starting offset between the selected item and the input position
    Vector3 startingRot;                                    //The starting rotation of the item
    Vector2 startingVector;                                 //The starting vector of the rotation (inputPos - selectedItem.position)
    Vector2 currentVector;                                  //The current vector of the rotation (inputPos - selectedItem.position)

    //Returns the instance
    public static InputManager Instance { get { return myInstance; } }

    //Called at the start of the level
    void Start()
    {
        myInstance = this;
        offset = Vector3.zero;
    }
    //Called at every frame, tracks input
    void Update()
    {
        //Get the position of the input
        inputPos = Camera.main.ScreenToWorldPoint(GetInputPosition());
        inputPos.z = 0;

        //Cast a ray to detect objets
        hit = Physics2D.Raycast(inputPos, new Vector2(0, 0), 0.1f, mask);
        
        switch (inputState)
        {
            case InputState.scrolling:
                ScrollToolbox();
                break;

            case InputState.moving:
                MoveItem();
                break;

            case InputState.rotating:
                RotateItem();
                break;

            case InputState.waitingForInput:
                ScanForInput();
                break;
        }
    }

    //Called when there are no specific input and waiting for one
    void ScanForInput()
    {
        if (HasInput())
        {
            //If the input was registered
            if (hit.collider != null)
            {
                if (hit.transform.tag == "GUI")
                {
                    if (hasFeedback)
                        HideFeedback();

                    GUIManager.Instance.ReceiveInput(hit.transform);
                }

                else if (hit.transform.tag == "Toolbox")
                    PrepareScrolling(hit.transform);

                else if (hit.transform.tag == "GameObject")
                    PrepareToDrag(hit.transform, false);

                else if (hit.transform.tag == "Feedback")
                    PrepareToRotate();
            }
            //If we have an active feedback, and the input was in an empty space, hide the feedback
            else if (hasFeedback)
                HideFeedback();
        }
    }

    //Prepare the toolbox for scrolling
    void PrepareScrolling(Transform t)
    {
        //If the input was on a toolbox item, register it
        if (hit.transform.name != "background" && hit.transform.name != "backgroundCap")
        {
            selectedItem = hit.transform;
            itemSelectionValid = true;
        }

        //Prepare for scrolling
        ToolboxManager.Instance.PrepareScolling();
        scrollStartingPos = inputPos.x;
        inputState = InputState.scrolling;
    }
    //Scrolls toolbox content
    void ScrollToolbox()
    {
        //If the input is on a toolbox item
        if (itemSelectionValid)
        {
            //If we moved the input up while the distance to the starting input is not greater then 0.5
            if (Mathf.Abs(inputPos.x - scrollStartingPos) < 0.5f && inputPos.y > -2.8f)
            {
                //Prepare the item for dragging
                PrepareToDrag(selectedItem, true);
            }
            //If the distance to the starting input is greater than 0.5
            else if (Mathf.Abs(inputPos.x - scrollStartingPos) > 0.5f)
            {
                //Then the item is no longer valid for dragging
                itemSelectionValid = false;

                //Set the current position for the scroll starting position
                scrollStartingPos = inputPos.x;
            }
        }
        else
        {
            //Scroll the toolbox
            ToolboxManager.Instance.ScrollMode(inputPos.x - scrollStartingPos);
        }

        //If the input was released
        if (InputReleased())
        {
            //Finalise scrolling
            itemSelectionValid = false;
            ToolboxManager.Instance.FinaliseScrolling();
            inputState = InputState.waitingForInput;
        }
    }

    //Prepares the selected item for dragging
    void PrepareToDrag(Transform item, bool fromToolbox)
    {
        //If the level is in play mode, return to caller
        if (LevelManager.Instance.InPlayMode())
            return;

        //If we have an active feedback, hide it
        if (hasFeedback && selectedObject != null)
            HideFeedback();

        //If the object is from the toolbox
        if (fromToolbox)
        {
            //Remove the object from the toolbox, and select it
            selectedObject = item.GetComponent<ToolboxItemType>().RemoveItem();
            selectedObject.DragMode();
            selectedObject.PlayPickupAnimation();

            //Activate the feedback on the object
            FeedbackManager.Instance.Setup(selectedObject, FeedbackManager.TargetState.dragging);
            hasFeedback = true;

            //Render the object to the top 
            ChangeSortingOrderBy(selectedObject.gameObject, 3);
            inputState = InputState.moving;
        }
        //If the object is not from the toolbox, make sure it can be dragged
        else if (CanDragged(item))
        {
            //It is possible we have selected its child collider, so scan it for the ObjectBase script
            selectedObject = GetParent(item);
            
            selectedObject.DragMode();

            //Calculate offset based on input position
            offset = new Vector3(inputPos.x - selectedObject.transform.position.x, inputPos.y - selectedObject.transform.position.y, 0);

            //Activate feedback on the selected item
            FeedbackManager.Instance.Setup(selectedObject, FeedbackManager.TargetState.dragging);
            hasFeedback = true;

            //Render the object to the top 
            ChangeSortingOrderBy(selectedObject.gameObject, 3);
            inputState = InputState.moving;
        }
    }
    //Moves the selected item 
    void MoveItem()
    {
        //Move the item to the input position based on the starting offset
        selectedObject.transform.position = new Vector3(inputPos.x - offset.x, inputPos.y - offset.y , 0);
        
        if (InputReleased())
            DropItem();
    }
    //Drops the selected item
    void DropItem()
    {
        //Render the object on it's original order
        ChangeSortingOrderBy(selectedObject.gameObject, -3);

        //If the object is in a valid position
        if (selectedObject.GetValidPos())
        {
            //Drop it and add it to the active items
            selectedObject.Dropped();
            selectedObject.Setup();
            LevelManager.Instance.AddItem(selectedObject.GetComponent<ObjectBase>());

            //If the object can be rotated, change the feedback to rotation
            if (selectedObject.canRotate)
                FeedbackManager.Instance.Setup(selectedObject, FeedbackManager.TargetState.rotating);
            else
                selectedObject = null;
        }
        else
        {
            //Put the item back to the toolbox
            ToolboxManager.Instance.AddItem(selectedObject);
            LevelManager.Instance.RemoveItem(selectedObject.GetComponent<ObjectBase>());
            FeedbackManager.Instance.Disable(0);

            selectedObject = null;
        }

        //Reset item variables
        offset = Vector3.zero;
        selectedItem = null;
        itemSelectionValid = false;

        //Set input state
        inputState = InputState.waitingForInput;
    }

    //Prepares the item for rotating
    void PrepareToRotate()
    {
        //If the level is in play mode, return to caller
        if (LevelManager.Instance.InPlayMode())
            return;

        //If we clicked on the feedback, while it is not in rotation mode, return to caller
        if (!FeedbackManager.Instance.InRotation())
            return;

        //Put the selected object into drag mode, and calculate starting vector
        selectedObject.DragMode();
        startingVector = new Vector2(inputPos.x - selectedObject.transform.position.x, inputPos.y - selectedObject.transform.position.y);
        startingVector.Normalize();

        startingRot = selectedObject.transform.eulerAngles;

        //Make the feedback to rotate with the selected object
        FeedbackManager.Instance.RotateWith(selectedObject.transform);

        //Render the object on the top
        ChangeSortingOrderBy(selectedObject.gameObject, 3);
        inputState = InputState.rotating;
    }
    //Rotates the selected item
    void RotateItem()
    {
        //Calculate current rotation vector
        Vector3 currentVector = new Vector2(inputPos.x - selectedObject.transform.position.x, inputPos.y - selectedObject.transform.position.y);
        currentVector.Normalize();

        //Get the current rotation
        Vector3 currentRotation = selectedObject.transform.eulerAngles;

        //Calculate the angle between the starting and current vector
        float angle = Vector3.Angle(startingVector, currentVector);

        //Calculate a middle vector between the starting and current vector, and caclulate rotation based on it
        if (Vector3.Cross(startingVector, currentVector).z < 0)
            currentRotation.z = startingRot.z - angle;
        else
            currentRotation.z = startingRot.z + angle;

        //Apply rotation
        selectedObject.transform.eulerAngles = currentRotation;

        if (InputReleased())
            FinaliseRotation();
    }
    //Finalise rotation for the current input
    void FinaliseRotation()
    {
        //Render the object on it's original order
        ChangeSortingOrderBy(selectedObject.gameObject, -3);

        //Make the feedback rotate independently
        FeedbackManager.Instance.RotateAlone();

        //Stop rotation and add the item to the active items
        selectedObject.RotationEnded();
        LevelManager.Instance.AddItem(selectedObject.GetComponent<ObjectBase>());

        //Reset item variables
        offset = Vector3.zero;
        selectedItem = null;
        itemSelectionValid = false;

        inputState = InputState.waitingForInput;
    }

    //Hides active feedback
    void HideFeedback()
    {
        FeedbackManager.Instance.Disable(0.2f);
        hasFeedback = false;
    }

    //Returns true if there is an active input
    bool HasInput()
    {
        return Input.GetMouseButtonDown(0);
    }
    //Returns true, if the input was released
    bool InputReleased()
    {
        return Input.GetMouseButtonUp(0);
    }
    //Returns true, if the object can be dragged around by the player
    bool CanDragged(Transform item)
    {
        while (item != null)
        {
            if (item.name.Contains("(Clone)"))
                return true;

            item = item.parent;
        }

        return false;
    }
    //Change the sorting layer of obj and its children
    void ChangeSortingOrderBy(GameObject obj, int by)
    {
        if (obj.GetComponent<SpriteRenderer>())
            obj.GetComponent<SpriteRenderer>().sortingOrder += by;

        foreach(Transform child in obj.transform)
            ChangeSortingOrderBy(child.gameObject, by);
    }

    //Returns the input position
    Vector3 GetInputPosition()
    {
        return Input.mousePosition;
    }
    //Returns the ObjectBase parent of the item
    ObjectBase GetParent(Transform item)
    {
        while (item.GetComponent<ObjectBase>() == null)
        {
            item = item.parent;
        }

        return item.GetComponent<ObjectBase>();
    }
}
