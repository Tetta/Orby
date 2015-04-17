using UnityEngine;
using System.Collections;

public class RcController : PhysicsObject
{
    public RcCar car;                           //Holds a link to the RC car
    public Transform knob;                      //Holds a link to the controller's knob
    public Transform body;                      //Holds a link to the body of the controller
    public ParticleSystem particle;             //Holds the radio wave particle
    
    float maxKnobMovement = 0.11f;              //The max vertical movement the knob is allowed to do
    float signalStrength;                       //Holds the knob pressed percentage [0% .. 100%]
    bool inPlayMode;                            //The level is in play mode/test mode

    Vector2 knobStartDist;                      //The starting distance between the knob and the body
    Vector2 bodyStartPos;                       //The starting position of the body

    void Awake()
    {
        knobStartDist = knob.localPosition - body.localPosition;
        bodyStartPos = body.transform.localPosition;
    }

    //Called at a fixed interval
    public void FixedUpdate()
    {
        UpdateKnobPosition();
        CalculateSignalStrength();

        if (inPlayMode)  
            SendSignal();
    }

    //Called when the level is enabled
    public override void Enable()
    {
        inPlayMode = true;

        body.GetComponent<Rigidbody2D>().gravityScale = gravity;
    }
    //Called when the level is disabled
    public override void Reset()
    {
        inPlayMode = false;

        body.GetComponent<Rigidbody2D>().gravityScale = 0;
        body.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        body.localPosition = bodyStartPos;
        knob.localPosition = bodyStartPos + knobStartDist;
    }

    //Updates the position of the knob
    void UpdateKnobPosition()
    {
        //Make sure that the knob stays in the right position on the y axis
        knob.transform.localPosition = new Vector2(body.localPosition.x + knobStartDist.x, knob.transform.localPosition.y);

        //If the knob is pushed down by an object, apply upward force on it
        if (knob.localPosition.y - body.localPosition.y < knobStartDist.y)
            knob.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5);
        else
        {
            knob.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            knob.transform.localPosition = new Vector2(knob.transform.localPosition.x, body.localPosition.y + knobStartDist.y);
        }
    }
    //Calculates signal strength
    void CalculateSignalStrength()
    {
        signalStrength = Mathf.Abs((knob.localPosition.y - body.localPosition.y) - knobStartDist.y) / (maxKnobMovement / 100);
        
        //If the signal is stronger than 1%, activate emission      
        if (signalStrength > 1)
        {
            particle.enableEmission = true;
        }
        //If the signal is too weak, disable emission
        else
        {
            particle.enableEmission = false;
            signalStrength = 0;
        }
    }
    //Send the signal to the car
    void SendSignal()
    {
        //If the car object is not set, look for it
        if (car == null)
            car = GameObject.FindObjectOfType<RcCar>();
        //If we have the car, send the signal strength to it
        else
            car.SetAcceloration(signalStrength);
    }
}
