using UnityEngine;
using System.Collections;

public class Balloon : PhysicsObject
{
    public float upwardForce        = 2;                //The upward force of the balloon
	public float maxTorque			= 0.5f;             //Max torque force allowed to stop over rotating	
	public float maxRotation 		= 0;                //Maximum rotation allowed in degrees, so the rotation can only be between [-maxRotation, maxrotation]

    public Sprite[] popAnimations;                      //Holds the pop animation frames                   

    float zRot;                                         //The current rotation on the Z axis
    float rotatingForce;                                //The current torque applied in Z axis

    bool isPopped                   = false;            //Balloon popped
	bool onCollision 				= false;            //Balloon colliding

	// Update is called once per frame
	public void FixedUpdate () 
	{
		if (canMove)
		{
            this.rigidbody2D.AddForce(Vector2.up * upwardForce);

            //Calculate rotation angle
            zRot = transform.eulerAngles.z;
            if (zRot > 180) 
                zRot = -(360 - zRot);

            //Add rotation force in opposite direction to limit rotation
            rotatingForce = maxTorque * (-zRot / maxRotation);
            this.rigidbody2D.AddTorque(rotatingForce);
		}
	}
    //Called when the ballon is colliding with something
    public override void OnCollisionEnter2D(Collision2D other)
    {
        //If the balloon collided with the head of a dart, and the balloon is not exploded yet
        if (other.collider.gameObject.name == "HeadCollider" && !isPopped)
        {
            //Blow up the balloon
            isPopped = true;
            StartCoroutine("Explode");
            GoalManager.Instance.BalloonExloded(this.gameObject);
        }
    }

	//Called while the balloon is colliding with something
	void OnCollisionStay2D (Collision2D other)
	{
		onCollision = true;
	}
	//Called when the balloon leaves the collision
	void OnCollisionExit2D (Collision2D other)
	{
		onCollision = false;
		StartCoroutine("ReduceHorizontalSpeed", 0.5f);
	}

	//Enable the balloon
	public override void Enable ()
	{
        this.rigidbody2D.fixedAngle = false;
        canMove = true;
	}
    //Reset the balloon
	public override void Reset ()
    {
        //If the balloon is popped, reset it's renderer and texture
        if (isPopped)
        {
            isPopped = false;
            this.GetComponent<PolygonCollider2D>().enabled = true;
            this.GetComponent<SpriteRenderer>().sprite = popAnimations[0];
        }

        canMove = false;

        //Stop balloon movement
        this.rigidbody2D.velocity = new Vector2(0, 0);
        this.rigidbody2D.fixedAngle = true;

        StopAllCoroutines();
	}

	//Reduce horizontal speed based on time
	IEnumerator ReduceHorizontalSpeed (float time)
	{
		float startValue = this.rigidbody2D.velocity.x;
		float rate = 1.0f / time;
	    float t = 0.0f;
	    Vector2 vel = this.rigidbody2D.velocity;
		
	    while (t < 1.0f && !onCollision) 
	    {
	        t += Time.deltaTime * rate;
			vel = this.rigidbody2D.velocity;
	        vel.x = Mathf.Lerp(startValue, 0.0f, t);

			this.rigidbody2D.velocity = vel;
	        yield return new WaitForEndOfFrame();
	    }
	}
    //Explodes
    IEnumerator Explode()
    {
        //Play pop animation
        for (int i = 1; i < popAnimations.Length; i++)
        {
            this.GetComponent<SpriteRenderer>().sprite = popAnimations[i];
            yield return new WaitForSeconds(0.05f);
        }

        //Disable the renderer
        this.GetComponent<PolygonCollider2D>().enabled = false;
    }
}
