#pragma strict

public class TriggerRegistererJS extends MonoBehaviour 
{
    public var objectBase		: ObjectBaseJS;

    function OnCollisionEnter2D(other : Collision2D)
    {
        if (GetParent(other.transform))
            GoalManagerJS.Instance().CollisionEvent(objectBase.gameObject, GetParent(other.transform).gameObject);
    }
    function OnTriggerEnter2D(other : Collider2D)
    {
        if (GetParent(other.transform))
            GoalManagerJS.Instance().TriggerEvent(this.gameObject, GetParent(other.transform).gameObject);
        else if (other.name == "EmptyTargetJS")
            GoalManagerJS.Instance().TriggerEvent(objectBase.gameObject, other.gameObject);
    }

    //Called when the object hit something in trigger mode
    function OnTriggerStay2D(other : Collider2D)
    {
        objectBase.SetValidPos(false);
    }
    //Called when the object leaves trigger mode
    function OnTriggerExit2D(other : Collider2D)
    {
        objectBase.SetValidPos(true);
    }

    //Returns the ObjectBase parent of the item
    function GetParent(item : Transform)
    {
        while (item != null && item.GetComponent(ObjectBaseJS) == null)
            item = item.parent;

        if (item == null)
            return null;
        else
            return item.GetComponent(ObjectBaseJS);
    }
}
