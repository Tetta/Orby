using UnityEngine;
using System.Collections;

public class LevelSelectButton : MonoBehaviour 
{
    public string levelNumber;              //The number of the level
    public bool alwaysUnlocked;             //The level is always/not always unlocked
    public Sprite greenSprite;              //The level unlocked texture
    public Sprite redSprite;                //The level locked texture
    public GameObject[] stars;              //The achieved level stars

    bool islocked;                          //The level is locked/unlocked
    bool inAnimation;                       //The animation is playing/not playing

    //Called at the beginning of the game
    void Start()
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
            for (int i = 1; i < 5; i++)
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
    void CreateData()
    {
        PlayerPrefs.SetInt(levelNumber, -1);
        PlayerPrefs.Save();
    }
    //Unlocks this button
    void Unlocked()
    {
        islocked = false;
        this.GetComponent<SpriteRenderer>().sprite = greenSprite;

        int numberOfStars = PlayerPrefs.GetInt(levelNumber);
        for (int i = 0; i < numberOfStars; i++)
            stars[i].SetActive(true);
    }
    //Locks this button
    void Locked()
    {
        islocked = true;
        this.GetComponent<SpriteRenderer>().sprite = redSprite;
        this.transform.FindChild("LevelNumber").gameObject.SetActive(false);
    }

    //Called when an input is registered in this button
    public void ClickEvent()
    {
        if (!inAnimation)
            StartCoroutine(Animate());
    }
    //Play the level animation
    IEnumerator Animate()
    {
        inAnimation = true;
        Vector3 originalScale = this.transform.localScale;

        float time = 0.2f;
        float rate = 1.0f / time;
        float t = 0.0f;

        float d = 0;

        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            this.transform.localScale = originalScale + (originalScale * (0.1f * Mathf.Sin(d * Mathf.Deg2Rad)));

            d = 180 * t;
            yield return new WaitForEndOfFrame();
        }

        this.transform.localScale = originalScale;
        inAnimation = false;

        if (!islocked)
            Application.LoadLevel("Level" + levelNumber);
    }
}
