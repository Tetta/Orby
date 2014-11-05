using UnityEngine;
using System.Collections;

public class PhysicsObject : MonoBehaviour
{
    public float gravity;                                   //The pull force on the y axis
    public float springBounciness;                          //How much should the object bounce when it hits the trampoline

    protected bool canMove = false;                         //Object movement enabled/disabled 

    //Called when the object collides with an other object
    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        //If we hit another object which has an ObjectBase script, notify the goal manager
        if (GetParent(other.transform))
            GoalManager.Instance.CollisionEvent(this.gameObject, GetParent(other.transform).gameObject);

        //If we hit a trampoline, bounce back
        if (other.collider.gameObject .name == "TrampolineTop")
            this.rigidbody2D.AddForce(Vector2.up * rigidbody2D.velocity.y * springBounciness);

        //If the y velocity is smaller than 1.25, make the object stop vertically
        if (Mathf.Abs(this.rigidbody2D.velocity.y) < 1.25f)
            this.rigidbody2D.velocity = new Vector2(this.rigidbody2D.velocity.x, 0);
    }
    //Called when the object enters a trigger zone
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        //If we hit another object which has an ObjectBase script, notify the goal manager
        if (GetParent(other.transform))
            GoalManager.Instance.TriggerEvent(this.gameObject, GetParent(other.transform).gameObject);
        //If we hit the empty target object, notify the goal manager
        else if (other.name == "EmptyTarget")
            GoalManager.Instance.TriggerEvent(this.gameObject, other.gameObject);
    }

    //Called when the level enters play mode
    public virtual void Enable()
    {
        canMove = true;
        this.rigidbody2D.gravityScale = gravity;
        this.rigidbody2D.fixedAngle = false;
    }
    //Called when the level leaves play mode
    public virtual void Reset()
    {
        canMove = false;
        this.rigidbody2D.gravityScale = 0;
        this.rigidbody2D.velocity = new Vector2(0, 0);
        this.rigidbody2D.fixedAngle = true;
    }

    //Returns the ObjectBase parent of the item
    protected ObjectBase GetParent(Transform item)
    {
        while (item != null && item.GetComponent<ObjectBase>() == null)
            item = item.parent;

        if (item == null)
            return null;
        else
            return item.GetComponent<ObjectBase>();
    }
}