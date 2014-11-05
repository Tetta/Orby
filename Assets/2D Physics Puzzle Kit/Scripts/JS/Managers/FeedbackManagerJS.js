#pragma strict

public class FeedbackManagerJS extends MonoBehaviour
{
    public enum TargetState { dragging, rotating};

    public var target					: ObjectBaseJS;					//The target object
    public var feedback					: SpriteRenderer;				//The feedback renderer

    public var rotationSpeed			: float;                  		//The rotation speed of the feedback

    public var dragGreenTexture			: Sprite;               		//Drag valid texture
    public var dragRedTexture			: Sprite;                 		//Drag invalid texture

    public var RotationGreenTexture		: Sprite;           			//Rotation valid texture
    public var RotationRedTexture		: Sprite;             			//Rotation invalid texture

    static var myInstance				: FeedbackManagerJS;			//Hold a reference to this instance

    private var targetState   			: TargetState = TargetState.dragging;
    private var originalSpeed			: float;                  		//The original rotation speed

    //Returns the instance
    public static function Instance() { return myInstance; }

    //Called at the start of the level
    function Start()
    {
        myInstance = this;
    }
	// Update is called once per frame
	function Update ()
    {
        if (target)
        {
            UpdateTexture();

            feedback.transform.position = target.GetFeedbackPos();
            feedback.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }
	}

    //Set taget object and state, then scales the feedback to the target
    function Setup(target : ObjectBaseJS, state : TargetState)
    {
        targetState = state;

        this.target = target;
        feedback.sortingOrder = 4;

        StopCoroutine("Rescale");
        StartCoroutine(Rescale(feedback.transform, target.GetFeedbackSize(), 0.2f));
    }
    //Scales the feedback to zero in time
    function Disable(time : float)
    {
        feedback.sortingOrder = 1;

        StopCoroutine("Rescale");
        StartCoroutine(Rescale(feedback.transform, new Vector3(0, 0, 1), time));
    }

    //Stop the feedback rotation, and make it to rotate with the target 
    function RotateWith(item : Transform)
    {
        originalSpeed = rotationSpeed;
        rotationSpeed = 0; 

        feedback.transform.parent = item;
    }
    //Makes the feedback to rotate on its own
    function RotateAlone()
    {
        feedback.transform.parent = this.transform;
        rotationSpeed = originalSpeed;
    }

    //Returns true, if the feedback is in rotating state
    function InRotation()
    {
        return targetState == TargetState.rotating;
    }
    //Change the state to received state
    function ChangeState(state : TargetState)
    {
        targetState = state;
    }
    //Update circle texture based on state
    private function UpdateTexture()
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
    private function Rescale(obj : Transform, endScale : Vector3, time : float)
    {
        var startScale : Vector3 = obj.localScale;

        var rate : float = 1.0f / time;
        var t : float = 0.0f;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            obj.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield;
        }
    }
}
