#pragma strict

public class PhysicsObjectJS extends MonoBehaviour
{
    public var gravity					: float;				//The pull force on the y axis
    public var springBounciness			: float;				//How much should the object bounce when it hits the trampoline

    protected var canMove 				: boolean = false;		//Object movement enabled/disabled 

    //Called when the object collides with an other object
    public function OnCollisionEnter2D(other : Collision2D)
    {
        //If we hit another object which has an ObjectBase script, notify the goal manager
        if (GetParent(other.transform))
            GoalManagerJS.Instance().CollisionEvent(this.gameObject, GetParent(other.transform).gameObject);

        //If we hit a trampoline, bounce back
        if (other.collider.gameObject .name == "TrampolineTop")
            this.GetComponent.<Rigidbody2D>().AddForce(Vector2.up * GetComponent.<Rigidbody2D>().velocity.y * springBounciness);

        //If the y velocity is smaller than 1.25, make the object stop vertically
        if (Mathf.Abs(this.GetComponent.<Rigidbody2D>().velocity.y) < 1.25f)
            this.GetComponent.<Rigidbody2D>().velocity = new Vector2(this.GetComponent.<Rigidbody2D>().velocity.x, 0);
    }
    //Called when the object enters a trigger zone
    public function OnTriggerEnter2D(other : Collider2D)
    {
        //If we hit another object which has an ObjectBase script, notify the goal manager
        if (GetParent(other.transform))
            GoalManagerJS.Instance().TriggerEvent(this.gameObject, GetParent(other.transform).gameObject);
        //If we hit the empty target object, notify the goal manager
        else if (other.name == "EmptyTargetJS")
            GoalManagerJS.Instance().TriggerEvent(this.gameObject, other.gameObject);
    }

    //Called when the level enters play mode
    public function Enable()
    {
        canMove = true;
        this.GetComponent.<Rigidbody2D>().gravityScale = gravity;
        this.GetComponent.<Rigidbody2D>().fixedAngle = false;
    }
    //Called when the level leaves play mode
    public function Reset()
    {
        canMove = false;
        this.GetComponent.<Rigidbody2D>().gravityScale = 0;
        this.GetComponent.<Rigidbody2D>().velocity = new Vector2(0, 0);
        this.GetComponent.<Rigidbody2D>().fixedAngle = true;
    }

    //Returns the ObjectBase parent of the item
    protected function GetParent(item : Transform)
    {
        while (item != null && item.GetComponent(ObjectBaseJS) == null)
            item = item.parent;

        if (item == null)
            return null;
        else
            return item.GetComponent(ObjectBaseJS);
    }
}