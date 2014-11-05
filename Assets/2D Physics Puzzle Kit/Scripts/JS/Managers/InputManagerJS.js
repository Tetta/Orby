#pragma strict
#pragma downcast

public class InputManagerJS extends MonoBehaviour 
{
    enum InputState { scrolling, moving, rotating, waitingForInput };				//The possible states for the toolbox

    public var mask 				: LayerMask = -1;								//Set input layer mask

    static var myInstance			: InputManagerJS;								//Hold a reference to this script

    private var inputState			: InputState = InputState.waitingForInput;     	//The state of the input
    private var hit					: RaycastHit2D;                                 //The raycast to detect the targetet item

    private var selectedItem		: Transform;                                 	//The transform of the selected item
    private var selectedObject		: ObjectBaseJS;                              	//The ObjectBase script of the selected item

    private var scrollStartingPos	: float;                                		//The starting position to the toolbox scrolling

    private var itemSelectionValid	: boolean = false;                        		//Selected item valid/invalid for dragging/rotating
    private var hasFeedback 		: boolean = false;                              //Feedback activated/deactivated

    private var inputPos			: Vector3;                                      //The position of the input
    private var offset				: Vector3;                                      //The starting offset between the selected item and the input position
    private var startingRot			: Vector3;                                    	//The starting rotation of the item
    private var startingVector		: Vector3;                                 		//The starting vector of the rotation (inputPos - selectedItem.position)
    private var currentVector		: Vector3;                                  	//The current vector of the rotation (inputPos - selectedItem.position)

    //Returns the instance
    public static function Instance() {  return myInstance; }

    //Called at the start of the level
    function Start()
    {
        myInstance = this;
        offset = Vector3.zero;
    }
    //Called at every frame, tracks input
    function Update()
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
    private function ScanForInput()
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

                    GUIManagerJS.Instance().ReceiveInput(hit.transform);
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
    private function PrepareScrolling(t : Transform)
    {
        //If the input was on a toolbox item, register it
        if (hit.transform.name != "background" && hit.transform.name != "backgroundCap")
        {
            selectedItem = hit.transform;
            itemSelectionValid = true;
        }

        //Prepare for scrolling
        ToolboxManagerJS.Instance().PrepareScolling();
        scrollStartingPos = inputPos.x;
        inputState = InputState.scrolling;
    }
    //Scrolls toolbox content
    private function ScrollToolbox()
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
            ToolboxManagerJS.Instance().ScrollMode(inputPos.x - scrollStartingPos);
        }

        //If the input was released
        if (InputReleased())
        {
            //Finalise scrolling
            itemSelectionValid = false;
            ToolboxManagerJS.Instance().FinaliseScrolling();
            inputState = InputState.waitingForInput;
        }
    }

    //Prepares the selected item for dragging
    private function PrepareToDrag(item : Transform, fromToolbox : boolean)
    {
        //If the level is in play mode, return to caller
        if (LevelManagerJS.Instance().InPlayMode())
            return;

        //If we have an active feedback, hide it
        if (hasFeedback && selectedObject != null)
            HideFeedback();

        //If the object is from the toolbox
        if (fromToolbox)
        {
            //Remove the object from the toolbox, and select it
            selectedObject = item.GetComponent(ToolboxItemTypeJS).RemoveItem();
            selectedObject.DragMode();
            selectedObject.PlayPickupAnimation();

            //Activate the feedback on the object
            FeedbackManagerJS.Instance().Setup(selectedObject, FeedbackManagerJS.TargetState.dragging);
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
            FeedbackManagerJS.Instance().Setup(selectedObject, FeedbackManagerJS.TargetState.dragging);
            hasFeedback = true;

            //Render the object to the top 
            ChangeSortingOrderBy(selectedObject.gameObject, 3);
            inputState = InputState.moving;
        }
    }
    //Moves the selected item 
    private function MoveItem()
    {
        //Move the item to the input position based on the starting offset
        selectedObject.transform.position = new Vector3(inputPos.x - offset.x, inputPos.y - offset.y , 0);
        
        if (InputReleased())
            DropItem();
    }
    //Drops the selected item
    private function DropItem()
    {
        //Render the object on it's original order
        ChangeSortingOrderBy(selectedObject.gameObject, -3);

        //If the object is in a valid position
        if (selectedObject.GetValidPos())
        {
            //Drop it and add it to the active items
            selectedObject.Dropped();
            selectedObject.Setup();
            LevelManagerJS.Instance().AddItem(selectedObject.GetComponent(ObjectBaseJS));

            //If the object can be rotated, change the feedback to rotation
            if (selectedObject.canRotate)
                FeedbackManagerJS.Instance().Setup(selectedObject, FeedbackManagerJS.TargetState.rotating);
            else
                selectedObject = null;
        }
        else
        {
            //Put the item back to the toolbox
            ToolboxManagerJS.Instance().AddItem(selectedObject);
            LevelManagerJS.Instance().RemoveItem(selectedObject.GetComponent("ObjectBase"));
            FeedbackManagerJS.Instance().Disable(0);

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
    private function PrepareToRotate()
    {
        //If the level is in play mode, return to caller
        if (LevelManagerJS.Instance().InPlayMode())
            return;

        //If we clicked on the feedback, while it is not in rotation mode, return to caller
        if (!FeedbackManagerJS.Instance().InRotation())
            return;

        //Put the selected object into drag mode, and calculate starting vector
        selectedObject.DragMode();
        startingVector = new Vector2(inputPos.x - selectedObject.transform.position.x, inputPos.y - selectedObject.transform.position.y);
        startingVector.Normalize();

        startingRot = selectedObject.transform.eulerAngles;

        //Make the feedback to rotate with the selected object
        FeedbackManagerJS.Instance().RotateWith(selectedObject.transform);

        //Render the object on the top
        ChangeSortingOrderBy(selectedObject.gameObject, 3);
        inputState = InputState.rotating;
    }
    //Rotates the selected item
    private function RotateItem()
    {
        //Calculate current rotation vector
        var currentVector : Vector3 = new Vector2(inputPos.x - selectedObject.transform.position.x, inputPos.y - selectedObject.transform.position.y);
        currentVector.Normalize();

        //Get the current rotation
        var currentRotation : Vector3 = selectedObject.transform.eulerAngles;

        //Calculate the angle between the starting and current vector
        var angle : float = Vector3.Angle(startingVector, currentVector);

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
    private function FinaliseRotation()
    {
        //Render the object on it's original order
        ChangeSortingOrderBy(selectedObject.gameObject, -3);

        //Make the feedback rotate independently
        FeedbackManagerJS.Instance().RotateAlone();

        //Stop rotation and add the item to the active items
        selectedObject.RotationEnded();
        LevelManagerJS.Instance().AddItem(selectedObject.GetComponent(ObjectBaseJS));

        //Reset item variables
        offset = Vector3.zero;
        selectedItem = null;
        itemSelectionValid = false;

        inputState = InputState.waitingForInput;
    }

    //Hides active feedback
    private function HideFeedback()
    {
        FeedbackManagerJS.Instance().Disable(0.2f);
        hasFeedback = false;
    }

    //Returns true if there is an active input
    private function HasInput()
    {
        return Input.GetMouseButtonDown(0);
    }
    //Returns true, if the input was released
    private function InputReleased()
    {
        return Input.GetMouseButtonUp(0);
    }
    //Returns true, if the object can be dragged around by the player
    private function CanDragged(item : Transform)
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
    private function ChangeSortingOrderBy(obj : GameObject, by : int)
    {
        if (obj.GetComponent(SpriteRenderer))
            obj.GetComponent(SpriteRenderer).sortingOrder += by;

        for(var child : Transform in obj.transform)
            ChangeSortingOrderBy(child.gameObject, by);
    }

    //Returns the input position
    private function GetInputPosition()
    {
        return Input.mousePosition;
    }
    //Returns the ObjectBase parent of the item
    private function GetParent(item : Transform)
    {
        while (item.GetComponent(ObjectBaseJS) == null)
        {
            item = item.parent;
        }

        return item.GetComponent(ObjectBaseJS);
    }
}
