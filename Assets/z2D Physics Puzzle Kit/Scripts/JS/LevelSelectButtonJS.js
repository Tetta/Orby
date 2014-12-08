#pragma strict

public class LevelSelectButtonJS extends MonoBehaviour 
{
    public var levelNumber			: String;				//The number of the level
    public var alwaysUnlocked		: boolean;				//The level is always/not always unlocked
    public var greenSprite			: Sprite;				//The level unlocked texture
    public var redSprite			: Sprite;				//The level locked texture
    public var stars				: GameObject[];			//The achieved level stars

    private var islocked			: boolean;				//The level is locked/unlocked
    private var inAnimation			: boolean;				//The animation is playing/not playing

    //Called at the beginning of the game
    function Start()
    {
        //If save data does not exist for this level, create it
        if (!PlayerPrefs.HasKey(levelNumber))
            CreateData();

        //If the level is always unlocked, unlock it
        if (alwaysUnlocked)
            Unlocked();
        //If the level is not always unlocked, it means the level id is between 5 and 8
        else
        {
            //Loop trought the save data for level 1-4
            for (var i : int = 1; i < 5; i++)
            {
                //If a level is not completed, lock this level
                if (PlayerPrefs.GetInt(i.ToString()) == -1 || !PlayerPrefs.HasKey(i.ToString()))
                {
                    Locked();
                    return;
                }
            }
            //If every level between 1 and 4 is completed, unlock this
            Unlocked();
        }
    }

    //Creates a save data for this level
    private function CreateData()
    {
        PlayerPrefs.SetInt(levelNumber, -1);
        PlayerPrefs.Save();
    }
    //Unlocks this button
    private function Unlocked()
    {
        islocked = false;
        this.GetComponent(SpriteRenderer).sprite = greenSprite;

        var numberOfStars : int = PlayerPrefs.GetInt(levelNumber);
        for (var i : int = 0; i < numberOfStars; i++)
            stars[i].SetActive(true);
    }
    //Locks this button
    private function Locked()
    {
        islocked = true;
        this.GetComponent(SpriteRenderer).sprite = redSprite;
        this.transform.FindChild("LevelNumber").gameObject.SetActive(false);
    }

    //Called when an input is registered in this button
    public function ClickEvent()
    {
        if (!inAnimation)
            StartCoroutine(Animate());
    }
    //Play the level animation
    private function Animate()
    {
        inAnimation = true;
        var originalScale : Vector3 = this.transform.localScale;

        var time : float = 0.2f;
        var rate : float = 1.0f / time;
        var t : float = 0.0f;

        var d : float = 0;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            this.transform.localScale = originalScale + (originalScale * (0.1f * Mathf.Sin(d * Mathf.Deg2Rad)));

            d = 180 * t;
            yield 0;
        }

        this.transform.localScale = originalScale;
        inAnimation = false;

        if (!islocked)
            Application.LoadLevel("Level" + levelNumber);
    }
}
