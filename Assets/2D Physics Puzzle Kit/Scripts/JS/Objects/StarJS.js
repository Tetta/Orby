#pragma strict

public class StarJS extends ObjectBaseJS
{
    public var particle			: ParticleSystem;			//The pickup particle
    public var mainRenderer		: Renderer;					//The main renderer of the star

    private var canCollect 		: boolean = true;			//The star can/can't be collected
    private var inPlayMode 		: boolean = false;			//The level is/is not in play mode

    //Called when the object enters a trigger zone
    function OnTriggerEnter2D(other : Collider2D)
    {
        //If the level is in play mode, the star can be collected and the other object is a gameobject
        if (inPlayMode && canCollect && other.gameObject.tag == "GameObject")
            //Collect the star
            Collected();
    }

    //Called when the level enters play mode
    public override function Enable()
    {
        inPlayMode = true;
        this.collider2D.isTrigger = true;
    }
    //Called when the level leaves play mode
    public override function Reset()
    {
        inPlayMode = false;

        canCollect = true;
        mainRenderer.enabled = true;
    }
   
    //Called when the star is collected
    private function Collected()
    {
        canCollect = false;

        mainRenderer.enabled = false;
        LevelManagerJS.Instance().StarCollected();

        particle.Play();
    }
}
