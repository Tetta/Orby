#pragma strict

public class GoalManagerJS extends MonoBehaviour 
{
    public var target		: GameObject;
    public var goal			: GameObject;

    public var onTrigger	: boolean;

    static var myInstance	: GoalManagerJS;
    
    public static function Instance() { return myInstance; }

    function Start()
    {
        myInstance = this;
    }

    //Called when an object collides with another object
    function CollisionEvent(sender : GameObject, collidedWith : GameObject)
    {
        if (sender == target && collidedWith == goal && !onTrigger)
            LevelManagerJS.Instance().GoalReached();
    }
    //Called when an object enters a trigger zone
    function TriggerEvent(sender : GameObject, triggeredWith : GameObject)
    {
        if (sender == target && triggeredWith == goal && onTrigger)
            LevelManagerJS.Instance().GoalReached();
    }
    //Called when a balloon explodes
    function BalloonExloded(sender : GameObject)
    {
        if (sender == target && goal == null)
            LevelManagerJS.Instance().GoalReached();
    }
}
