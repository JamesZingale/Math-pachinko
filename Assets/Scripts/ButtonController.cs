using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    public enum ButtonAction
    {
        Play,
        Back,
        LoadLevel,
        Pause,
        Resume,
        Restart,
        Quit
    }
    
    public ButtonAction action;
    public int levelIndex; // Only used for LoadLevel action
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    public GameObject gameUICanvas;
    public GameObject pauseMenuCanvas;
    
    private void Start()
    {
        // Add click listener
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("ButtonController attached to a GameObject without a Button component!");
        }
    }
    
    public void OnButtonClick()
    {
        Debug.Log("Button clicked: " + action);
        
        switch (action)
        {
            case ButtonAction.Play:
                ShowCanvas(levelSelectionCanvas);
                HideCanvas(mainMenuCanvas);
                HideCanvas(gameUICanvas);
                HideCanvas(pauseMenuCanvas);
                break;
                
            case ButtonAction.Back:
                ShowCanvas(mainMenuCanvas);
                HideCanvas(levelSelectionCanvas);
                HideCanvas(gameUICanvas);
                HideCanvas(pauseMenuCanvas);
                break;
                
            case ButtonAction.LoadLevel:
                LoadLevel(levelIndex);
                break;
                
            case ButtonAction.Pause:
                ShowCanvas(pauseMenuCanvas);
                Time.timeScale = 0f;
                break;
                
            case ButtonAction.Resume:
                HideCanvas(pauseMenuCanvas);
                Time.timeScale = 1f;
                break;
                
            case ButtonAction.Restart:
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                break;
                
            case ButtonAction.Quit:
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #else
                Application.Quit();
                #endif
                break;
        }
    }
    
    private void ShowCanvas(GameObject canvas)
    {
        if (canvas != null)
        {
            canvas.SetActive(true);
        }
    }
    
    private void HideCanvas(GameObject canvas)
    {
        if (canvas != null)
        {
            canvas.SetActive(false);
        }
    }
    
    private void LoadLevel(int index)
    {
        Debug.Log("Loading level " + index);
        string levelSceneName = "Level" + index;
        
        // Check if the scene exists in the build settings
        bool sceneExists = false;
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == levelSceneName)
            {
                sceneExists = true;
                break;
            }
        }
        
        if (sceneExists)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelSceneName);
        }
        else
        {
            Debug.LogWarning("Scene " + levelSceneName + " does not exist in build settings. Make sure to add it.");
            
            // For testing, show the game UI
            ShowCanvas(gameUICanvas);
            HideCanvas(mainMenuCanvas);
            HideCanvas(levelSelectionCanvas);
            HideCanvas(pauseMenuCanvas);
        }
    }
}
