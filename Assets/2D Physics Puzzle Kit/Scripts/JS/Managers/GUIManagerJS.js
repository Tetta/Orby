#pragma strict

public class GUIManagerJS extends MonoBehaviour 
{
    enum LevelState { inPlanning, inPlaying, inPause, inFinish, inGoal, inChange };
    var levelState 					: LevelState  = LevelState.inPlanning;

    public var overlay				: SpriteRenderer;

    public var playButton			: Transform;                        //The play button
    public var pauseButton			: Transform;                       //The pause button
    public var toolbox				: Transform;                           //The toolbox parent object
    public var pauseMenu			: Transform;                         //The pause menu parent object
    public var finishMenu			: Transform;                        //The finish menu parent object
    
    public var finishStars			: Transform[];                     //The stars in the finish screen
    public var markers				: Transform[];                         //Goal marker, Target marker, Goal check  
    public var playButtonTextures	: Sprite[];                 //Play and Stop texture

    private var markersVisible 		: boolean = false;
    static var myInstance			: GUIManagerJS;                       //Hold a reference to this script
    
    public static function Instance() { return myInstance; }

    //Called at the begining of the level
    function Start()
    {
        myInstance = this;
        StartCoroutine(ShowMarkers(2));
    }

    //Called from the input manager, when a button is pressed
    function ReceiveInput(button : Transform)
    {
        if (levelState == LevelState.inChange)
            return;

        switch (button.name)
        {
            case "Play":
                if (levelState == LevelState.inPlanning)
                    PlayLevel();
                else if (levelState == LevelState.inPlaying)
                    StopLevel();
                break;

            case "Pause":
                if (levelState == LevelState.inPlanning)
                    PauseLevel();
                else if (levelState == LevelState.inPause)
                    UnpauseLevel();
                break;

            case "ToolboxButton":
                ToolboxManagerJS.Instance().ButtonPressed();
                break;

            case "Restart":
                LevelManagerJS.Instance().RestartScene();
                break;

            case "FinishRestart":
                RestartFromFinish();
                break;

            case "LevelSelect":
                Application.LoadLevel(0);
                break;

            case "NextLevel":
                if (Application.loadedLevel < 8)
                    Application.LoadLevel(Application.loadedLevel + 1);
                break;
        }

        StartCoroutine(Animate(button, 0.1f, 0.15f));
    }
    //Called from the level manager, when the goal is reached
    function GoalReached()
    {
        StartCoroutine(ShowFinishScreen());
    }
    //Called from the level manager, when the level is restarted
    function Restart()
    {
        UnpauseLevel();
    }

    //Puts the level into play mode
    private function PlayLevel()
    {
        levelState = LevelState.inChange;
        if (markersVisible) HideMarkers();

        //Change play button texture, enable the level objects, and hide the toolbox
        playButton.GetComponent(SpriteRenderer).sprite = playButtonTextures[1];
        LevelManagerJS.Instance().EnableScene();
        ToolboxManagerJS.Instance().HideToolbox();

        //Hide the pause menu and change the level state after it
        StartCoroutine(MoveMenuElementBy(pauseButton, -1.5f, 0.4f, 0));
        StartCoroutine(ChangeStateAfter(LevelState.inPlaying, 0.8f));
    }
    //Puts the level into planning mode
    private function StopLevel()
    {
        levelState = LevelState.inChange;

        //Reset the level and show the toolbox
        playButton.GetComponent(SpriteRenderer).sprite = playButtonTextures[0];
        LevelManagerJS.Instance().ResetScene();
        ToolboxManagerJS.Instance().ShowToolbox();

        //Show the pause menu and change the level state after it
        StartCoroutine(MoveMenuElementBy(pauseButton, 1.5f, 0.4f, 0));
        StartCoroutine(ChangeStateAfter(LevelState.inPlanning, 0.8f));
    }
    //Puts the level into pause mode
    private function PauseLevel()
    {
        levelState = LevelState.inChange;
        if (markersVisible) HideMarkers();

        //Hide the toolbox and fade the screen
        ToolboxManagerJS.Instance().HideToolbox();
        StartCoroutine(FadeScreen(0.4f, 0.6f));

        //Hide the play button and bring the pause menu to middle position
        StartCoroutine(MoveMenuElementBy(playButton, 1.5f, 0.4f, 0));
        StartCoroutine(MoveMenuElementBy(pauseMenu, 0.75f, 0.2f, 0));

        //Move the pause menu to the extended position with the pause button
        StartCoroutine(MoveMenuElementBy(pauseMenu, 0.75f, 0.2f, 0.2f));
        StartCoroutine(MoveMenuElementBy(pauseButton, 0.75f, 0.2f, 0.2f));

        //Change level state once finished
        StartCoroutine(ChangeStateAfter(LevelState.inPause, 0.8f));
    }
    //Puts the level into planning mode
    private function UnpauseLevel()
    {
        levelState = LevelState.inChange;

        //Show the toolbox and unfade the screen
        ToolboxManagerJS.Instance().ShowToolbox();
        StartCoroutine(FadeScreen(0.4f, 0));

        //Move the pause menu along with the pause button to middle position
        StartCoroutine(MoveMenuElementBy(pauseMenu, -0.75f, 0.2f, 0));
        StartCoroutine(MoveMenuElementBy(pauseButton, -0.75f, 0.2f, 0));

        //Show the play button and hide the pause menu
        StartCoroutine(MoveMenuElementBy(playButton, -1.5f, 0.4f, 0.2f));
        StartCoroutine(MoveMenuElementBy(pauseMenu, -0.75f, 0.2f, 0.2f));

        StartCoroutine(ChangeStateAfter(LevelState.inPlanning, 0.8f));
    }

    //Called when the level is restarted from the finish screen
    private function RestartFromFinish()
    {
        //Disable and hide finish screen
        StopCoroutine("ShowFinishScreen");
        for (var star : Transform in finishStars)
            star.localScale = new Vector2(0, 0);

        //Reset the overlay
        overlay.color = new Color(0, 0, 0, 0);
        overlay.transform.localScale = new Vector2(4, 3);

        //Hide the finish menu and activate the play button
        finishMenu.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);

        //Put the level into planning mode
        StopLevel();
        LevelManagerJS.Instance().RestartFromFinish();
    }
    //Hides the object markers
    private function HideMarkers()
    {
        markersVisible = false;
        StopCoroutine("ShowMarkers");

        for (var item : Transform in markers)
            item.transform.localScale = new Vector3(0, 0, 1);
    }

    //Moves a menu element horizontally by a received ammount
    private function MoveMenuElementBy(element : Transform, moveAmmount : float, time : float, delay : float)
    {
        yield WaitForSeconds(delay);

        var i : float = 0.0f;
        var rate : float = 1.0f / time;

        var startPos : Vector3 = element.localPosition;
        var endPos : Vector3 = startPos;
        endPos.x += moveAmmount;

        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            element.localPosition = Vector3.Lerp(startPos, endPos, i);
            yield 0;
        }
    }
    //Changes the level state to the received state after time
    private function ChangeStateAfter(newState : LevelState, time : float)
    {
        yield WaitForSeconds(time);
        levelState = newState;
    }
    //Animates a button
    private function Animate(button : Transform, scaleFactor : float, time : float)
    {
        var originalScale : Vector3 = button.localScale;
        
        var rate : float = 1.0f / time;
		var t : float = 0.0f;

        var d : float = 0;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            button.localScale = originalScale + (originalScale * (scaleFactor * Mathf.Sin(d * Mathf.Deg2Rad)));

            d = 180 * t;
            yield WaitForEndOfFrame();
        }

        button.localScale = originalScale;
    }
    //Show the markers at the start of the level
    private function ShowMarkers(showTime : float)
    {
        markersVisible = true;
        yield WaitForSeconds(1);

        //Scale up the markers
        StartCoroutine(ScaleObject(markers[0], Vector2.one, 0.25f, 0));
        StartCoroutine(ScaleObject(markers[1], Vector2.one, 0.25f, 0));

        yield WaitForSeconds(showTime);

        //Scale down the markers to zero
        markers[0].localScale = Vector2.zero;
        markers[1].localScale = Vector2.zero;

        markersVisible = false;
    }
    //Shows the finish screen
    private function ShowFinishScreen()
    {
        levelState = LevelState.inGoal;

        //Disable the play button
        playButton.gameObject.SetActive(false);
        yield WaitForSeconds(1);

        //Scale up and show the check for 2 seconds
        StartCoroutine(ScaleObject(markers[2], new Vector2(0.5f, 0.5f), 0.2f, 0));
        yield WaitForSeconds(2);
        markers[2].transform.localScale = new Vector2(0, 0);

        //Show the overlay in the centre
        overlay.color = new Color(0, 0, 0, 0.6f);
        overlay.transform.localScale = new Vector2(0, 3);
        StartCoroutine(ScaleObject(overlay.transform, new Vector2(1.75f, 3), 0.4f, 0));

        //Activate the finish menu
        yield WaitForSeconds(0.3f);
        finishMenu.gameObject.SetActive(true);

        LevelManagerJS.Instance().SaveData();
        var stars : int = LevelManagerJS.Instance().GetStarsCollected();
        
        //Show the collected stars
        for (var i : int = 0; i < stars; i++)
        {
            StartCoroutine(ScaleObject(finishStars[i], new Vector2(1, 1), 0.4f, 0));
            yield WaitForSeconds(0.6f);
        }
    }
    //Fade overlay
    private function FadeScreen(time : float, to : float)
    {
        //Set the screen fade color to end in time
        var i : float = 0.0f;
        var rate : float= 1.0f / time;

        var start : Color = overlay.color;
        var end :Color = new Color(start.r, start.g, start.b, to);

        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            overlay.color = Color.Lerp(start, end, i);
            yield 0;
        }
    }
    //Scales an object
    private function ScaleObject(obj : Transform, end : Vector2, time : float, delay : float)
    {
        yield WaitForSeconds(delay);

        var originalScale : Vector2 = obj.localScale;

        var rate : float = 1.0f / time;
        var i : float = 0.0f;

        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            obj.localScale = Vector2.Lerp(originalScale, end, i);
            yield WaitForEndOfFrame();
        }
    }
}
