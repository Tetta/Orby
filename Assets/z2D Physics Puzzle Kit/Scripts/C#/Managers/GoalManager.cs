using UnityEngine;
using System.Collections;

public class GoalManager : MonoBehaviour 
{
    public GameObject target;
    public GameObject goal;

    public bool onTrigger;

    static GoalManager myInstance;
    public static GoalManager Instance { get { return myInstance; } }

    void Start()
    {
        myInstance = this;
    }

    //Called when an object collides with another object
    public void CollisionEvent(GameObject sender, GameObject collidedWith)
    {
        if (sender == target && collidedWith == goal && !onTrigger)
            LevelManager.Instance.GoalReached();
    }
    //Called when an object enters a trigger zone
    public void TriggerEvent(GameObject sender, GameObject triggeredWith)
    {
        if (sender == target && triggeredWith == goal && onTrigger)
            LevelManager.Instance.GoalReached();
    }
    //Called when a balloon explodes
    public void BalloonExloded(GameObject sender)
    {
        if (sender == target && goal == null)
            LevelManager.Instance.GoalReached();
    }
}
