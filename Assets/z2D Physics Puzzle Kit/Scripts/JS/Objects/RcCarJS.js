#pragma strict

public class RcCarJS extends PhysicsObjectJS 
{
    public var mainRigidbody			: Rigidbody2D;			//The main rigidbody
    public var particle					: ParticleSystem;		//The radio wave particle   

    public var speed					: float;				//The max speed of the car

    public var wheels					: HingeJoint2D[];		//The wheels of the car [front, back]

    private var signalStrength 			: float = 0;			//The current signal strength from the controller
    private var currentSpeed			: float;
    private var activated 				: boolean = false;		//Car activated/deactivated

    private var motor					: JointMotor2D;			//The joint motor used by the wheels

    private var frontWheelPos			: Vector2;				//The original position of the front wheel
    private var backWheelPos			: Vector2;				//The original rotation of the front wheel

    private var lastBodyPos				: Vector2;
    private var lastBodyRot				: Vector2;

    //Called at the beginning of the game
    function Start()
    {
        //Save starting parameters
        lastBodyPos = mainRigidbody.transform.position;
        lastBodyRot = mainRigidbody.transform.eulerAngles;

        frontWheelPos = wheels[0].transform.position;
        backWheelPos = wheels[1].transform.position;

        motor = new JointMotor2D();
        motor.maxMotorTorque = 1000;
    }
    //Called at a fixed interval, updates vehicle physics
    public function FixedUpdate()
    {
        //If the car is activated, process and apply signal strength
        if (activated)
        {
            if (signalStrength > 10)
                particle.enableEmission = true;
            else
                particle.enableEmission = false;

            currentSpeed = -speed * signalStrength / 100;

            motor.motorSpeed = currentSpeed;
            wheels[0].motor = motor;
            wheels[1].motor = motor;
        }
    }

    //Enabled the object
    public override function Enable()
    {
        //Save positions and rotations
        lastBodyPos = mainRigidbody.transform.position;
        lastBodyRot = mainRigidbody.transform.eulerAngles;

        frontWheelPos = wheels[0].transform.position;
        backWheelPos = wheels[1].transform.position;

        mainRigidbody.fixedAngle = false;

        wheels[0].GetComponent.<Rigidbody2D>().fixedAngle = false;
        wheels[1].GetComponent.<Rigidbody2D>().fixedAngle = false;

        mainRigidbody.gravityScale = gravity;
        activated = true;
    }
    //Reset the object
    public override function Reset()
    {
        mainRigidbody.gravityScale = 0;
        activated = false;
        signalStrength = 0;
        particle.enableEmission = false;

        StopAllCoroutines();

        //Stop the rigidbodies
        mainRigidbody.GetComponent.<Rigidbody2D>().velocity = Vector2.zero;
        wheels[0].GetComponent.<Rigidbody2D>().velocity = Vector2.zero;
        wheels[1].GetComponent.<Rigidbody2D>().velocity = Vector2.zero;

        //Freeze rigidbody rotations
        mainRigidbody.fixedAngle = true;
        wheels[0].GetComponent.<Rigidbody2D>().fixedAngle = true;
        wheels[1].GetComponent.<Rigidbody2D>().fixedAngle = true;

        //Reset positions
        mainRigidbody.transform.position = lastBodyPos;
        mainRigidbody.transform.eulerAngles = lastBodyRot;
        
        wheels[0].transform.position = frontWheelPos;
        wheels[1].transform.position = backWheelPos;
    }
    //Receives the button signal strength
    public function SetAcceloration(signal : float)
    {
        this.signalStrength = signal;
    }
}
