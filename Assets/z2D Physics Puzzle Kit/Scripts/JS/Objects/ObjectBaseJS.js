#pragma strict
#pragma downcast

import System.Collections.Generic;

public class ObjectBaseJS extends MonoBehaviour 
{
    public var animationStartScale			: float;					//The starting scale in the "toolbox remove animation"
    public var feedbackSize					: float;					//The size of the feedback for this item

    public var canRotate					: boolean;					//Object rotation allowed/not allowed

    public var feedbackPosition				: Transform;				//The position of the feedback for this object
    public var childColliders				: Collider2D[];				//The colliders of the child objects
    public var inputCollider				: Collider2D;				//The input collider

    protected var originalPos				: Vector3;					//The original position of the object
    protected var originalRot				: Vector3;					//The original rotation of the object

    protected var lastPos					: Vector3;					//The last valid position of the object
    protected var lastRot					: Vector3;					//The last valid rotation of the object

    protected var physicsScript				: PhysicsObjectJS;			//A link to the physics script of the object, if exists

    protected var validPos					: boolean;					//The current position is valid/not valid
    protected var colliders					: List.<Collider2D> ;		//The colliders of this object and the child objects

    //Called when the object is enabled 
    function Awake()
    {
        //Create and load the colliders list
        colliders = new List.<Collider2D>();

        for (var collider in GetComponents(Collider2D))
            colliders.Add(collider);

        colliders.AddRange(childColliders);
    }

    //Called when the object hit something in trigger mode
    function OnTriggerStay2D(other : Collider2D)
    {
        validPos = false;
    }
    //Called when the object leaves trigger mode
    function OnTriggerExit2D(other : Collider2D)
    {
        validPos = true;
    }

    //Called by the level manager at the start of the game
    public function Setup()
    {
        //Save starting position and rotation
        originalPos = this.transform.position;
        originalRot = this.transform.eulerAngles;

        lastPos = originalPos;
        lastRot = originalRot;

        //Try to find the physicsObject script
        physicsScript = this.GetComponent(PhysicsObjectJS);
    }
    //Called when the level enters play mode
    public function Enable()
    {
        //If the object has a physics script, call its enable function
        if (physicsScript)
            physicsScript.Enable();
        //If the object does not have a physics script, but has a rigidbody, make it kinematic
        else if (this.GetComponent.<Rigidbody2D>())
            this.GetComponent.<Rigidbody2D>().isKinematic = true;

        for (var collider in colliders)
            collider.isTrigger = false;
    }
    //Called when the level leaves play mode
    public function Reset()
    {
        //If the object has a physics script, call its reset function
        if (physicsScript)
            physicsScript.Reset();
        //If the object does not have a physics script, but has a rigidbody, disable its kinematic state
        else if (this.GetComponent.<Rigidbody2D>())
            this.GetComponent.<Rigidbody2D>().isKinematic = false;

        for (var collider in colliders)
            collider.isTrigger = true;

        this.transform.position = lastPos;
        this.transform.eulerAngles = lastRot;
    }
    //Called when the level is restarted
    public function Restart()
    {
        //Reset position and rotation
        this.transform.position = originalPos;
        this.transform.eulerAngles = originalRot;
    }

    //Called by the input manager, when the item is selected
    public function DragMode()
    {
        this.transform.parent = null;
        validPos = true;

        if (inputCollider)
            inputCollider.gameObject.SetActive(false);

        for (var collider in colliders)
            collider.isTrigger = true;
    }
    //Called by the input manager, when the item is dropped
    public function Dropped()
    {
        lastPos = this.transform.position;

        if (inputCollider)
            inputCollider.gameObject.SetActive(true);

        for (var collider in colliders)
            collider.isTrigger = false;
    }
    //Called by the input manager, when the item is no longer rotated
    public function RotationEnded()
    {
        if (validPos)
            lastRot = this.transform.eulerAngles;
        else
            this.transform.eulerAngles = lastRot;

        validPos = true;

        if (inputCollider)
            inputCollider.gameObject.SetActive(true);

        for (var collider in colliders)
            collider.isTrigger = false;
    }

    //Sets the validPos to newValue
    public function SetValidPos(newValue : boolean)
    {
        validPos = newValue;
    }
    //Return the state of validPos
    public function GetValidPos()
    {
        return validPos;
    }
    //Return the size of the feedback
    public function GetFeedbackSize()
    {
        return new Vector3(feedbackSize, feedbackSize, 1);
    }
    //Returns the position of the feedback
    public function GetFeedbackPos()
    {
        if (feedbackPosition == null)
            return this.transform.position;
        else
            return feedbackPosition.transform.position;
    }

    //Called when the item is picked up from the toolbox
    public function PlayPickupAnimation()
    {
        StartCoroutine(Rescale(0.2f));
    }
    //Rescale the object under time
    protected function Rescale(time : float)
    {
        var startScale : Vector3 = new Vector3(animationStartScale, animationStartScale, 1);
        var endScale : Vector3 = new Vector3(1, 1, 1);

        var rate : float = 1.0f / time;
        var t : float = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            this.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield WaitForEndOfFrame();
        }
    }
}
