#pragma strict
#pragma downcast

import System.Collections.Generic;

public class LevelManagerJS extends MonoBehaviour 
{
    public var levelNumber			: String;								//The current level number

    public var activeContainer 		: Transform;							//Holds the level elements
    public var  toolboxContainer	: Transform;							//Hold the toolbar elements at start

    static var myInstance			: LevelManagerJS;						//Hold a reference to this script

    private var activeObjects		: List.<ObjectBaseJS>;					//Hold every active object
    private var toolboxItemTypes	: List.<ToolboxItemTypeJS>;				//Hold every toolbar object

    private var starsCollected		: int = 0;								//Number of collected stars
    private var goalReached 		: boolean = false;						//Goal reached/not reached
    private var inPlayMode 			: boolean = false;						//The level is in play mode/plan mode

    public static function Instance() { return myInstance; }

    //Called at the start of the level
	function Start()
	{
        myInstance = this;

        //Create the lists
        activeObjects = new List.<ObjectBaseJS>();
        toolboxItemTypes = new List.<ToolboxItemTypeJS>();

        //Loop trough the active objects and add them to the active list, if they have an ObjectBase component
        for (var item : Transform in activeContainer)
        {
            if (item.GetComponent(ObjectBaseJS))
            {
                activeObjects.Add(item.GetComponent(ObjectBaseJS));
                item.GetComponent(ObjectBaseJS).Setup();
            }
        }

        //Loop trough the toolbox objects and add them to the toolbox list
        for (var item : Transform in toolboxContainer)
        {
            item.GetComponent(ToolboxItemTypeJS);
            toolboxItemTypes.Add(item.GetComponent(ToolboxItemTypeJS));
        }

        //And send them to the toolbox manager
        ToolboxManagerJS.Instance().SetupToolbar(toolboxItemTypes);
	}

    //Enable the scene
    public function EnableScene()
    {
        inPlayMode = true;

        for (var item : ObjectBaseJS in activeObjects)
            item.Enable();
    }
    //Resets the scene
    public function ResetScene()
    {
        inPlayMode = false;
        starsCollected = 0;
        
        for (var item : ObjectBaseJS in activeObjects)
            item.Reset();
    }
    //Restart the scene
    public function RestartScene()
    {
        inPlayMode = false;

        ToolboxManagerJS.Instance().ClearToolbox();
        ToolboxManagerJS.Instance().SetupToolbar(toolboxItemTypes);
        GUIManagerJS.Instance().Restart();
    }
    //Restarts the scene from the toolbox
    public function RestartFromFinish()
    {
        inPlayMode = false;

        ResetScene();
        goalReached = false;
    }

    //Called when a star is collected
    public function StarCollected()
    {
        starsCollected++;
    }
    //Returns the number of collected stars
    public function GetStarsCollected()
    {
        return starsCollected;
    }
    //Called by the goal manager, when the goal is reached by the target
    public function GoalReached()
    {
        if (!goalReached)
        {
            goalReached = true;
            GUIManagerJS.Instance().GoalReached();
        }
    }

    //Adds an item to the active object list
    public function AddItem(item : ObjectBaseJS)
    {
        if (!activeObjects.Contains(item))
            activeObjects.Add(item);

        item.transform.parent = activeContainer;
    }
    //Removes an item from the active object list
    public function RemoveItem(item : ObjectBaseJS)
    {
        activeObjects.Remove(item);
    }
    //Saves the current level data
    public function SaveData()
    {
        if (PlayerPrefs.GetInt(levelNumber) < starsCollected)
        {
            PlayerPrefs.SetInt(levelNumber, starsCollected);
            PlayerPrefs.Save();
        }
    }

    //Returns true if the level is in play mode
    public function InPlayMode()
    {
        return inPlayMode;
    }
}
