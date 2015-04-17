using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectBase : MonoBehaviour 
{
    public float animationStartScale;                       //The starting scale in the "toolbox remove animation"
    public float feedbackSize;                              //The size of the feedback for this item

    public bool canRotate;                                  //Object rotation allowed/not allowed

    public Transform feedbackPosition;                      //The position of the feedback for this object
    public Collider2D[] childColliders;                     //The colliders of the child objects
    public Collider2D inputCollider;                        //The input collider

    protected Vector3 originalPos;                          //The original position of the object
    protected Vector3 originalRot;                          //The original rotation of the object

    protected Vector3 lastPos;                              //The last valid position of the object
    protected Vector3 lastRot;                              //The last valid rotation of the object

    protected PhysicsObject physicsScript;                  //A link to the physics script of the object, if exists

    protected bool validPos;                                //The current position is valid/not valid
    protected List<Collider2D> colliders;                   //The colliders of this object and the child objects

    //Called when the object is enabled 
    void Awake()
    {
        //Create and load the colliders list
        colliders = new List<Collider2D>();

        foreach (var collider in GetComponents<Collider2D>())
            colliders.Add(collider);

        colliders.AddRange(childColliders);
    }

    //Called when the object hit something in trigger mode
    void OnTriggerStay2D(Collider2D other)
    {
        validPos = false;
    }
    //Called when the object leaves trigger mode
    void OnTriggerExit2D(Collider2D other)
    {
        validPos = true;
    }

    //Called by the level manager at the start of the game
    public virtual void Setup()
    {
        //Save starting position and rotation
        originalPos = this.transform.position;
        originalRot = this.transform.eulerAngles;

        lastPos = originalPos;
        lastRot = originalRot;

        //Try to find the physicsObject script
        physicsScript = this.GetComponent<PhysicsObject>();
    }
    //Called when the level enters play mode
    public virtual void Enable()
    {
        //If the object has a physics script, call its enable function
        if (physicsScript)
            physicsScript.Enable();
        //If the object does not have a physics script, but has a rigidbody, make it kinematic
        else if (this.GetComponent<Rigidbody2D>())
            this.GetComponent<Rigidbody2D>().isKinematic = true;

        foreach (var collider in colliders)
            collider.isTrigger = false;
    }
    //Called when the level leaves play mode
    public virtual void Reset()
    {
        //If the object has a physics script, call its reset function
        if (physicsScript)
            physicsScript.Reset();
        //If the object does not have a physics script, but has a rigidbody, disable its kinematic state
        else if (this.GetComponent<Rigidbody2D>())
            this.GetComponent<Rigidbody2D>().isKinematic = false;

        foreach (var collider in colliders)
            collider.isTrigger = true;

        this.transform.position = lastPos;
        this.transform.eulerAngles = lastRot;
    }
    //Called when the level is restarted
    public virtual void Restart()
    {
        //Reset position and rotation
        this.transform.position = originalPos;
        this.transform.eulerAngles = originalRot;
    }

    //Called by the input manager, when the item is selected
    public void DragMode()
    {
        this.transform.parent = null;
        validPos = true;

        if (inputCollider)
            inputCollider.gameObject.SetActive(false);

        foreach (var collider in colliders)
            collider.isTrigger = true;
    }
    //Called by the input manager, when the item is dropped
    public void Dropped()
    {
        lastPos = this.transform.position;

        if (inputCollider)
            inputCollider.gameObject.SetActive(true);

        foreach (var collider in colliders)
            collider.isTrigger = false;
    }
    //Called by the input manager, when the item is no longer rotated
    public void RotationEnded()
    {
        if (validPos)
            lastRot = this.transform.eulerAngles;
        else
            this.transform.eulerAngles = lastRot;

        validPos = true;

        if (inputCollider)
            inputCollider.gameObject.SetActive(true);

        foreach (var collider in colliders)
            collider.isTrigger = false;
    }

    //Sets the validPos to newValue
    public void SetValidPos(bool newValue)
    {
        validPos = newValue;
    }
    //Return the state of validPos
    public bool GetValidPos()
    {
        return validPos;
    }
    //Return the size of the feedback
    public Vector3 GetFeedbackSize()
    {
        return new Vector3(feedbackSize, feedbackSize, 1);
    }
    //Returns the position of the feedback
    public Vector3 GetFeedbackPos()
    {
        if (feedbackPosition == null)
            return this.transform.position;
        else
            return feedbackPosition.transform.position;
    }

    //Called when the item is picked up from the toolbox
    public void PlayPickupAnimation()
    {
        StartCoroutine(Rescale(0.2f));
    }
    //Rescale the object under time
    protected IEnumerator Rescale(float time)
    {
        Vector3 startScale = new Vector3(animationStartScale, animationStartScale, 1);
        Vector3 endScale = new Vector3(1, 1, 1);

        float rate = 1.0f / time;
        float t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            this.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return new WaitForEndOfFrame();
        }
    }
}
