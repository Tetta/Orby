using UnityEngine;
using System.Collections;

public class RcCar : PhysicsObject 
{
    public Rigidbody2D mainRigidbody;           //The main rigidbody
    public ParticleSystem particle;             //The radio wave particle   

    public float speed;                         //The max speed of the car

    public HingeJoint2D[] wheels;               //The wheels of the car [front, back]

    float signalStrength = 0;                   //The current signal strength from the controller
    float currentSpeed;
    bool activated = false;                     //Car activated/deactivated

    JointMotor2D motor;                         //The joint motor used by the wheels

    Vector2 frontWheelPos;                      //The original position of the front wheel
    Vector2 backWheelPos;                       //The original rotation of the front wheel

    Vector2 lastBodyPos;
    Vector2 lastBodyRot;

    //Called at the beginning of the game
    void Start()
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
    public void FixedUpdate()
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
    public override void Enable()
    {
        //Save positions and rotations
        lastBodyPos = mainRigidbody.transform.position;
        lastBodyRot = mainRigidbody.transform.eulerAngles;

        frontWheelPos = wheels[0].transform.position;
        backWheelPos = wheels[1].transform.position;

        mainRigidbody.fixedAngle = false;

        wheels[0].GetComponent<Rigidbody2D>().fixedAngle = false;
        wheels[1].GetComponent<Rigidbody2D>().fixedAngle = false;

        mainRigidbody.gravityScale = gravity;
        activated = true;
    }
    //Reset the object
    public override void Reset()
    {
        mainRigidbody.gravityScale = 0;
        activated = false;
        signalStrength = 0;
        particle.enableEmission = false;

        StopAllCoroutines();

        //Stop the rigidbodies
        mainRigidbody.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        wheels[0].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        wheels[1].GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        //Freeze rigidbody rotations
        mainRigidbody.fixedAngle = true;
        wheels[0].GetComponent<Rigidbody2D>().fixedAngle = true;
        wheels[1].GetComponent<Rigidbody2D>().fixedAngle = true;

        //Reset positions
        mainRigidbody.transform.position = lastBodyPos;
        mainRigidbody.transform.eulerAngles = lastBodyRot;
        
        wheels[0].transform.position = frontWheelPos;
        wheels[1].transform.position = backWheelPos;
    }
    //Receives the button signal strength
    public void SetAcceloration(float signal)
    {
        this.signalStrength = signal;
    }
}
