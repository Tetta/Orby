using UnityEngine;
using System.Collections;

public class SmallBall : PhysicsObject
{
    float friction;                         //The starting friction of the ball

    //Called at the beginning of the game
    void Start()
    {
        //Save starting friction
        friction = this.GetComponent<Collider2D>().sharedMaterial.friction;
    }

    //Called when the object enters a trigger zone
    public override void OnTriggerEnter2D(Collider2D other)
    {
        //If we entered a pipe, remove friction behaviour
        if (other.name == "ColliderPipe")
            this.GetComponent<Collider2D>().sharedMaterial.friction = 0;

        base.OnTriggerEnter2D(other);
    }
    //Called when the object leaves a trigger zone
    public void OnTriggerExit2D(Collider2D other)
    {
        //If we entered a pipe, add friction behaviour
        if (other.name == "ColliderPipe")
            this.GetComponent<Collider2D>().sharedMaterial.friction = friction;
    }
}
