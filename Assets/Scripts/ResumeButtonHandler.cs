using UnityEngine;

public class ResumeButtonHandler : MonoBehaviour
{
    public GameObject pauseMenuCanvas;
    
    public void OnResumeButtonClick()
    {
        Debug.Log("Resume button clicked!");
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
        
        // Resume game time
        Time.timeScale = 1f;
    }
}
