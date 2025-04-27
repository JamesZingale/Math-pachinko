using UnityEngine;

public class PlayButtonHandler : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    
    public void OnPlayButtonClick()
    {
        Debug.Log("Play button clicked!");
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
        }
        
        if (levelSelectionCanvas != null)
        {
            levelSelectionCanvas.SetActive(true);
        }
    }
}
