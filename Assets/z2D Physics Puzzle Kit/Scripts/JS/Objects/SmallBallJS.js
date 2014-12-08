#pragma strict

public class SmallBallJS extends PhysicsObjectJS
{
    private var friction	: float;				//The starting friction of the ball

    //Called at the beginning of the game
    function Start()
    {
        //Save starting friction
        friction = this.collider2D.sharedMaterial.friction;
    }

    //Called when the object enters a trigger zone
    public override function OnTriggerEnter2D(other : Collider2D)
    {
        //If we entered a pipe, remove friction behaviour
        if (other.name == "ColliderPipe")
            this.collider2D.sharedMaterial.friction = 0;

        super.OnTriggerEnter2D(other);
    }
    //Called when the object leaves a trigger zone
    public function OnTriggerExit2D(other : Collider2D)
    {
        //If we entered a pipe, add friction behaviour
        if (other.name == "ColliderPipe")
            this.collider2D.sharedMaterial.friction = friction;
    }
}
