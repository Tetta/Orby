#pragma strict

public class MainMenuManagerJS extends MonoBehaviour 
{
    enum MenuState { WaitingForInput, InTransit };
    private var menuState		: MenuState = MenuState.WaitingForInput;

    public var levelSelect		: GameObject;                      	//The level select parent object
    public var mask				: LayerMask = -1;					//Input layer mask

    private var hit				: RaycastHit2D;						//The raycast to detect the target item
    private var inputPos		: Vector3;

    //Called at every frame
    function Update()
    {
        inputPos = Camera.main.ScreenToWorldPoint(GetInputPosition());
        inputPos.z = 0;
        hit = Physics2D.Raycast(inputPos, new Vector2(0, 0), 0.1f, mask);

        if (menuState == MenuState.WaitingForInput)
            ScanForInput();
    }
    //Scans for inputs
    private function ScanForInput()
    {
        if (HasInput() && hit.collider != null)
        {
            if (hit.transform.name == "Play")
                StartCoroutine(PlayPressed(hit.transform));
            else if (hit.transform.name == "backButton")
            	StartCoroutine(BackPressed(hit.transform));
            else
                hit.transform.GetComponent(LevelSelectButtonJS).ClickEvent();
        }
    }
    //Called when the play button is pressed
    private function PlayPressed(button : Transform)
    {
        StartCoroutine(Animate(button, 0.1f, 0.2f));
        yield WaitForSeconds(0.3f);
        levelSelect.SetActive(true);
    }
	
	//Called when the back button is pressed
	private function BackPressed(button : Transform)
	{
		StartCoroutine(Animate(button, 0.1f, 0.2f));
        yield WaitForSeconds(0.3f);
        levelSelect.SetActive(false);
	}
	
    //Returns true if there is an active input
    private function HasInput()
    {
        return Input.GetMouseButtonDown(0);
    }
    //Returns true, if the input was released
    public function InputReleased()
    {
        return Input.GetMouseButtonUp(0);
    }
    //Returns the input position
    private function GetInputPosition()
    {
        return Input.mousePosition;
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
}
