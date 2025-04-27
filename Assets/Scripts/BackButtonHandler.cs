using UnityEngine;

public class BackButtonHandler : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    
    public void OnBackButtonClick()
    {
        Debug.Log("Back button clicked!");
        if (levelSelectionCanvas != null)
        {
            levelSelectionCanvas.SetActive(false);
        }
        
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
        }
    }
}
