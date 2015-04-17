#pragma strict

public class BalloonJS extends PhysicsObjectJS
{
    public var upwardForce			: float = 2;                //The upward force of the balloon
	public var maxTorque			: float	= 0.5f;             //Max torque force allowed to stop over rotating	
	public var maxRotation			: float = 0;                //Maximum rotation allowed in degrees, so the rotation can only be between [-maxRotation, maxrotation]

    public var popAnimations		: Sprite[];					//Holds the pop animation frames                   

    private var zRot				: float;					//The current rotation on the Z axis
    private var rotatingForce		: float;					//The current torque applied in Z axis

    private var isPopped			: boolean = false;			//Balloon popped
	private var onCollision			: boolean = false;			//Balloon colliding

	// Update is called once per frame
	public function FixedUpdate () 
	{
		if (canMove)
		{
            this.GetComponent.<Rigidbody2D>().AddForce(Vector2.up * upwardForce);

            //Calculate rotation angle
            zRot = transform.eulerAngles.z;
            if (zRot > 180) 
                zRot = -(360 - zRot);

            //Add rotation force in opposite direction to limit rotation
            rotatingForce = maxTorque * (-zRot / maxRotation);
            this.GetComponent.<Rigidbody2D>().AddTorque(rotatingForce);
		}
	}
    //Called when the ballon is colliding with something
    public override function OnCollisionEnter2D(other : Collision2D)
    {
        //If the balloon collided with the head of a dart, and the balloon is not exploded yet
        if (other.collider.gameObject.name == "HeadCollider" && !isPopped)
        {
            //Blow up the balloon
            isPopped = true;
            StartCoroutine("Explode");
            GoalManagerJS.Instance().BalloonExloded(this.gameObject);
        }
    }

	//Called while the balloon is colliding with something
	function OnCollisionStay2D (other : Collision2D)
	{
		onCollision = true;
	}
	//Called when the balloon leaves the collision
	function OnCollisionExit2D (other : Collision2D)
	{
		onCollision = false;
		StartCoroutine("ReduceHorizontalSpeed", 0.5f);
	}

	//Enable the balloon
	public override function Enable ()
	{
        this.GetComponent.<Rigidbody2D>().fixedAngle = false;
        canMove = true;
	}
    //Reset the balloon
	public override function Reset ()
    {
        //If the balloon is popped, reset it's renderer and texture
        if (isPopped)
        {
            isPopped = false;
            this.GetComponent(PolygonCollider2D).enabled = true;
            this.GetComponent(SpriteRenderer).sprite = popAnimations[0];
        }

        canMove = false;

        //Stop balloon movement
        this.GetComponent.<Rigidbody2D>().velocity = new Vector2(0, 0);
        this.GetComponent.<Rigidbody2D>().fixedAngle = true;

        StopAllCoroutines();
	}

	//Reduce horizontal speed based on time
	public function ReduceHorizontalSpeed (time : float)
	{
		var startValue : float = this.GetComponent.<Rigidbody2D>().velocity.x;
		var rate : float = 1.0f / time;
	    var t : float = 0.0f;
	    var vel : Vector2 = this.GetComponent.<Rigidbody2D>().velocity;
		
	    while (t < 1.0f && !onCollision) 
	    {
	        t += Time.deltaTime * rate;
			vel = this.GetComponent.<Rigidbody2D>().velocity;
	        vel.x = Mathf.Lerp(startValue, 0.0f, t);

			this.GetComponent.<Rigidbody2D>().velocity = vel;
	        yield WaitForEndOfFrame();
	    }
	}
    //Explodes
    public function Explode()
    {
        //Play pop animation
        for (var i : int = 1; i < popAnimations.Length; i++)
        {
            this.GetComponent(SpriteRenderer).sprite = popAnimations[i];
            yield WaitForSeconds(0.05f);
        }

        //Disable the renderer
        this.GetComponent(PolygonCollider2D).enabled = false;
    }
}
