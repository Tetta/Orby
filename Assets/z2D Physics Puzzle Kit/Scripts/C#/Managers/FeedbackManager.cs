using UnityEngine;
using System.Collections;

public class FeedbackManager : MonoBehaviour 
{
    public enum TargetState { dragging, rotating};

    public ObjectBase       target;                         //The target object
    public SpriteRenderer   feedback;                       //The feedback renderer

    public float            rotationSpeed;                  //The rotation speed of the feedback

    public Sprite           dragGreenTexture;               //Drag valid texture
    public Sprite           dragRedTexture;                 //Drag invalid texture

    public Sprite           RotationGreenTexture;           //Rotation valid texture
    public Sprite           RotationRedTexture;             //Rotation invalid texture

    static FeedbackManager  myInstance;                     //Hold a reference to this instance

    TargetState             targetState = TargetState.dragging;
    float                   originalSpeed;                  //The original rotation speed

    //Returns the instance
    public static FeedbackManager Instance { get { return myInstance; } }

    //Called at the start of the level
    void Start()
    {
        myInstance = this;
    }
	// Update is called once per frame
	void Update ()
    {
        if (target)
        {
            UpdateTexture();

            feedback.transform.position = target.GetFeedbackPos();
            feedback.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
	}

    //Set taget object and state, then scales the feedback to the target
    public void Setup(ObjectBase target, TargetState state)
    {
        targetState = state;

        this.target = target;
        feedback.sortingOrder = 4;

        StopCoroutine("Rescale");
        StartCoroutine(Rescale(feedback.transform, target.GetFeedbackSize(), 0.2f));
    }
    //Scales the feedback to zero in time
    public void Disable(float time)
    {
        feedback.sortingOrder = 1;

        StopCoroutine("Rescale");
        StartCoroutine(Rescale(feedback.transform, new Vector3(0, 0, 1), time));
    }

    //Stop the feedback rotation, and make it to rotate with the target 
    public void RotateWith(Transform item)
    {
        originalSpeed = rotationSpeed;
        rotationSpeed = 0; 

        feedback.transform.parent = item;
    }
    //Makes the feedback to rotate on its own
    public void RotateAlone()
    {
        feedback.transform.parent = this.transform;
        rotationSpeed = originalSpeed;
    }

    //Returns true, if the feedback is in rotating state
    public bool InRotation()
    {
        return targetState == TargetState.rotating;
    }
    //Change the state to received state
    public void ChangeState(TargetState state)
    {
        targetState = state;
    }
    //Update circle texture based on state
    void UpdateTexture()
    {
        if (target.GetValidPos())
        {
            if (targetState == TargetState.dragging)
                feedback.sprite = dragGreenTexture;
            else
                feedback.sprite = RotationGreenTexture;
        }
        else
        {
            if (targetState == TargetState.dragging)
                feedback.sprite = dragRedTexture;
            else
                feedback.sprite = RotationRedTexture;
        }
    }

    //Rescale the circle
    private IEnumerator Rescale(Transform obj, Vector3 endScale, float time)
    {
        Vector3 startScale = obj.localScale;

        float rate = 1.0f / time;
        float t = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            obj.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return new WaitForEndOfFrame();
        }
    }
}
