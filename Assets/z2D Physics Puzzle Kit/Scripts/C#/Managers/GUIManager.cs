using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour 
{
    enum LevelState { inPlanning, inPlaying, inPause, inFinish, inGoal, inChange };
    LevelState levelState = LevelState.inPlanning;

    public SpriteRenderer overlay;

    public Transform playButton;                        //The play button
    public Transform pauseButton;                       //The pause button
    public Transform toolbox;                           //The toolbox parent object
    public Transform pauseMenu;                         //The pause menu parent object
    public Transform finishMenu;                        //The finish menu parent object
    
    public Transform[] finishStars;                     //The stars in the finish screen
    public Transform[] markers;                         //Goal marker, Target marker, Goal check  
    public Sprite[] playButtonTextures;                 //Play and Stop texture

    bool markersVisible = false;
    static GUIManager myInstance;                       //Hold a reference to this script
    public static GUIManager Instance { get { return myInstance; } }

    //Called at the begining of the level
    void Start()
    {
        myInstance = this;
        StartCoroutine(ShowMarkers(2));
    }

    //Called from the input manager, when a button is pressed
    public void ReceiveInput(Transform button)
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
                ToolboxManager.Instance.ButtonPressed();
                break;

            case "Restart":
                LevelManager.Instance.RestartScene();
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
    public void GoalReached()
    {
        StartCoroutine(ShowFinishScreen());
    }
    //Called from the level manager, when the level is restarted
    public void Restart()
    {
        UnpauseLevel();
    }

    //Puts the level into play mode
    void PlayLevel()
    {
        levelState = LevelState.inChange;
        if (markersVisible) HideMarkers();

        //Change play button texture, enable the level objects, and hide the toolbox
        playButton.GetComponent<SpriteRenderer>().sprite = playButtonTextures[1];
        LevelManager.Instance.EnableScene();
        ToolboxManager.Instance.HideToolbox();

        //Hide the pause menu and change the level state after it
        StartCoroutine(MoveMenuElementBy(pauseButton, -1.5f, 0.4f, 0));
        StartCoroutine(ChangeStateAfter(LevelState.inPlaying, 0.8f));
    }
    //Puts the level into planning mode
    void StopLevel()
    {
        levelState = LevelState.inChange;

        //Reset the level and show the toolbox
        playButton.GetComponent<SpriteRenderer>().sprite = playButtonTextures[0];
        LevelManager.Instance.ResetScene();
        ToolboxManager.Instance.ShowToolbox();

        //Show the pause menu and change the level state after it
        StartCoroutine(MoveMenuElementBy(pauseButton, 1.5f, 0.4f, 0));
        StartCoroutine(ChangeStateAfter(LevelState.inPlanning, 0.8f));
    }
    //Puts the level into pause mode
    void PauseLevel()
    {
        levelState = LevelState.inChange;
        if (markersVisible) HideMarkers();

        //Hide the toolbox and fade the screen
        ToolboxManager.Instance.HideToolbox();
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
    void UnpauseLevel()
    {
        levelState = LevelState.inChange;

        //Show the toolbox and unfade the screen
        ToolboxManager.Instance.ShowToolbox();
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
    void RestartFromFinish()
    {
        //Disable and hide finish screen
        StopCoroutine("ShowFinishScreen");
        foreach (Transform star in finishStars)
            star.localScale = new Vector2(0, 0);

        //Reset the overlay
        overlay.color = new Color(0, 0, 0, 0);
        overlay.transform.localScale = new Vector2(4, 3);

        //Hide the finish menu and activate the play button
        finishMenu.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);

        //Put the level into planning mode
        StopLevel();
        LevelManager.Instance.RestartFromFinish();
    }
    //Hides the object markers
    void HideMarkers()
    {
        markersVisible = false;
        StopCoroutine("ShowMarkers");

        foreach (Transform item in markers)
            item.transform.localScale = new Vector3(0, 0, 1);
    }

    //Moves a menu element horizontally by a received ammount
    IEnumerator MoveMenuElementBy(Transform element, float moveAmmount, float time, float delay)
    {
        yield return new WaitForSeconds(delay);

        float i = 0.0f;
        float rate = 1.0f / time;

        Vector3 startPos = element.localPosition;
        Vector3 endPos = startPos;
        endPos.x += moveAmmount;

        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            element.localPosition = Vector3.Lerp(startPos, endPos, i);
            yield return 0;
        }
    }
    //Changes the level state to the received state after time
    IEnumerator ChangeStateAfter(LevelState newState, float time)
    {
        yield return new WaitForSeconds(time);
        levelState = newState;
    }
    //Animates a button
    IEnumerator Animate(Transform button, float scaleFactor, float time)
    {
        Vector3 originalScale = button.localScale;

        float rate = 1.0f / time;
        float t = 0.0f;

        float d = 0;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            button.localScale = originalScale + (originalScale * (scaleFactor * Mathf.Sin(d * Mathf.Deg2Rad)));

            d = 180 * t;
            yield return new WaitForEndOfFrame();
        }

        button.localScale = originalScale;
    }
    //Show the markers at the start of the level
    IEnumerator ShowMarkers(float showTime)
    {
        markersVisible = true;
        yield return new WaitForSeconds(1);

        //Scale up the markers
        StartCoroutine(ScaleObject(markers[0], Vector2.one, 0.25f, 0));
        StartCoroutine(ScaleObject(markers[1], Vector2.one, 0.25f, 0));

        yield return new WaitForSeconds(showTime);

        //Scale down the markers to zero
        markers[0].localScale = Vector2.zero;
        markers[1].localScale = Vector2.zero;

        markersVisible = false;
    }
    //Shows the finish screen
    IEnumerator ShowFinishScreen()
    {
        levelState = LevelState.inGoal;

        //Disable the play button
        playButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1);

        //Scale up and show the check for 2 seconds
        StartCoroutine(ScaleObject(markers[2], new Vector2(0.5f, 0.5f), 0.2f, 0));
        yield return new WaitForSeconds(2);
        markers[2].transform.localScale = new Vector2(0, 0);

        //Show the overlay in the centre
        overlay.color = new Color(0, 0, 0, 0.6f);
        overlay.transform.localScale = new Vector2(0, 3);
        StartCoroutine(ScaleObject(overlay.transform, new Vector2(1.75f, 3), 0.4f, 0));

        //Activate the finish menu
        yield return new WaitForSeconds(0.3f);
        finishMenu.gameObject.SetActive(true);

        LevelManager.Instance.SaveData();
        int stars = LevelManager.Instance.GetStarsCollected();
        
        //Show the collected stars
        for (int i = 0; i < stars; i++)
        {
            StartCoroutine(ScaleObject(finishStars[i], new Vector2(1, 1), 0.4f, 0));
            yield return new WaitForSeconds(0.6f);
        }
    }
    //Fade overlay
    IEnumerator FadeScreen(float time, float to)
    {
        //Set the screen fade color to end in time
        float i = 0.0f;
        float rate = 1.0f / time;

        Color start = overlay.color;
        Color end = new Color(start.r, start.g, start.b, to);

        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            overlay.color = Color.Lerp(start, end, i);
            yield return 0;
        }
    }
    //Scales an object
    IEnumerator ScaleObject(Transform obj, Vector2 end, float time, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector2 originalScale = obj.localScale;

        float rate = 1.0f / time;
        float i = 0.0f;

        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;
            obj.localScale = Vector2.Lerp(originalScale, end, i);
            yield return new WaitForEndOfFrame();
        }
    }
}
