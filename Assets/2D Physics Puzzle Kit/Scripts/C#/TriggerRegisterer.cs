using UnityEngine;
using System.Collections;

public class TriggerRegisterer : MonoBehaviour 
{
    public ObjectBase objectBase;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (GetParent(other.transform))
            GoalManager.Instance.CollisionEvent(objectBase.gameObject, GetParent(other.transform).gameObject);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (GetParent(other.transform))
            GoalManager.Instance.TriggerEvent(this.gameObject, GetParent(other.transform).gameObject);
        else if (other.name == "EmptyTarget")
            GoalManager.Instance.TriggerEvent(objectBase.gameObject, other.gameObject);
    }

    //Called when the object hit something in trigger mode
    void OnTriggerStay2D(Collider2D other)
    {
        objectBase.SetValidPos(false);
    }
    //Called when the object leaves trigger mode
    void OnTriggerExit2D(Collider2D other)
    {
        objectBase.SetValidPos(true);
    }

    //Returns the ObjectBase parent of the item
    ObjectBase GetParent(Transform item)
    {
        while (item != null && item.GetComponent<ObjectBase>() == null)
            item = item.parent;

        if (item == null)
            return null;
        else
            return item.GetComponent<ObjectBase>();
    }
}
