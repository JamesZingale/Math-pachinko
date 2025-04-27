using UnityEngine;
using UnityEngine.UI;

public class SimpleButton : MonoBehaviour
{
    public enum ButtonType
    {
        Play,
        Back,
        Level,
        Pause,
        Resume,
        Quit
    }
    
    public ButtonType buttonType;
    public GameObject targetCanvas; // Canvas to show when clicked
    public GameObject currentCanvas; // Canvas to hide when clicked
    public int levelNumber; // Only used for Level type
    
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
            Debug.Log(gameObject.name + " button initialized");
        }
        else
        {
            Debug.LogError("No Button component found on " + gameObject.name);
        }
    }
    
    public void OnButtonClick()
    {
        Debug.Log(gameObject.name + " clicked!");
        
        switch (buttonType)
        {
            case ButtonType.Play:
            case ButtonType.Back:
                SwitchCanvas();
                break;
                
            case ButtonType.Level:
                LoadLevel();
                break;
                
            case ButtonType.Pause:
                PauseGame();
                break;
                
            case ButtonType.Resume:
                ResumeGame();
                break;
                
            case ButtonType.Quit:
                QuitGame();
                break;
        }
    }
    
    private void SwitchCanvas()
    {
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(false);
        }
        
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(true);
        }
    }
    
    private void LoadLevel()
    {
        Debug.Log("Loading level " + levelNumber);
        // Implement level loading here
    }
    
    private void PauseGame()
    {
        Time.timeScale = 0f;
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(true);
        }
    }
    
    private void ResumeGame()
    {
        Time.timeScale = 1f;
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(false);
        }
    }
    
    private void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
