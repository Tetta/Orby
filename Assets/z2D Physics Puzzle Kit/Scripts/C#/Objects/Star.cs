using UnityEngine;
using System.Collections;

public class Star : ObjectBase
{
    public ParticleSystem particle;                     //The pickup particle
    public Renderer mainRenderer;                       //The main renderer of the star

    bool canCollect = true;                             //The star can/can't be collected
    bool inPlayMode = false;                            //The level is/is not in play mode

    //Called when the object enters a trigger zone
    void OnTriggerEnter2D(Collider2D other)
    {
        //If the level is in play mode, the star can be collected and the other object is a gameobject
        if (inPlayMode && canCollect && other.gameObject.tag == "GameObject")
            //Collect the star
            Collected();
    }

    //Called when the level enters play mode
    public override void Enable()
    {
        inPlayMode = true;
        this.collider2D.isTrigger = true;
    }
    //Called when the level leaves play mode
    public override void Reset()
    {
        inPlayMode = false;

        canCollect = true;
        mainRenderer.enabled = true;
    }
   
    //Called when the star is collected
    void Collected()
    {
        canCollect = false;

        mainRenderer.enabled = false;
        LevelManager.Instance.StarCollected();

        particle.Play();
    }
}
