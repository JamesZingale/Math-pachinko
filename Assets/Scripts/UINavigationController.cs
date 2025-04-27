using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UINavigationController : MonoBehaviour
{
    // References to canvases
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    public GameObject gameUICanvas;
    public GameObject pauseMenuCanvas;
    
    // References to buttons
    private Button playButton;
    private Button backButton;
    private Button[] levelButtons;
    
    private void Start()
    {
        Debug.Log("UINavigationController: Starting...");
        
        // Find buttons if not assigned
        FindButtons();
        
        // Set up button listeners
        SetupButtonListeners();
        
        // Show main menu by default
        ShowMainMenu();
        
        Debug.Log("UINavigationController: Initialization complete");
    }
    
    private void FindButtons()
    {
        // Find play button in main menu
        if (mainMenuCanvas != null)
        {
            Transform playButtonTransform = FindChildRecursive(mainMenuCanvas.transform, "PlayButton");
            if (playButtonTransform != null)
            {
                playButton = playButtonTransform.GetComponent<Button>();
                Debug.Log("Found play button: " + playButton.name);
            }
        }
        
        // Find back button in level selection
        if (levelSelectionCanvas != null)
        {
            Transform backButtonTransform = FindChildRecursive(levelSelectionCanvas.transform, "BackButton");
            if (backButtonTransform != null)
            {
                backButton = backButtonTransform.GetComponent<Button>();
                Debug.Log("Found back button: " + backButton.name);
            }
            
            // Find level buttons
            levelButtons = new Button[3]; // Assuming 3 levels
            for (int i = 0; i < 3; i++)
            {
                Transform levelButtonTransform = FindChildRecursive(levelSelectionCanvas.transform, "Level" + (i+1) + "Button");
                if (levelButtonTransform != null)
                {
                    levelButtons[i] = levelButtonTransform.GetComponent<Button>();
                    Debug.Log("Found level button " + (i+1) + ": " + levelButtons[i].name);
                }
            }
        }
    }
    
    private void SetupButtonListeners()
    {
        // Main menu play button
        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() => {
                Debug.Log("Play button clicked!");
                ShowLevelSelection();
            });
            Debug.Log("Play button listener added");
        }
        else
        {
            Debug.LogWarning("Play button reference is missing!");
        }
        
        // Level selection back button
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(() => {
                Debug.Log("Back button clicked!");
                ShowMainMenu();
            });
            Debug.Log("Back button listener added");
        }
        else
        {
            Debug.LogWarning("Back button reference is missing!");
        }
        
        // Level buttons
        if (levelButtons != null)
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                if (levelButtons[i] != null)
                {
                    int levelIndex = i + 1; // Level 1, 2, 3, etc.
                    levelButtons[i].onClick.RemoveAllListeners();
                    levelButtons[i].onClick.AddListener(() => {
                        Debug.Log("Level " + levelIndex + " button clicked!");
                        LoadLevel(levelIndex);
                    });
                    Debug.Log("Level " + levelIndex + " button listener added");
                }
            }
        }
    }
    
    // Navigation methods
    public void ShowMainMenu()
    {
        Debug.Log("Showing main menu");
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(true);
        if (levelSelectionCanvas != null) levelSelectionCanvas.SetActive(false);
        if (gameUICanvas != null) gameUICanvas.SetActive(false);
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
    }
    
    public void ShowLevelSelection()
    {
        Debug.Log("Showing level selection");
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (levelSelectionCanvas != null) levelSelectionCanvas.SetActive(true);
        if (gameUICanvas != null) gameUICanvas.SetActive(false);
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
    }
    
    public void LoadLevel(int levelIndex)
    {
        Debug.Log("Loading level " + levelIndex);
        string levelSceneName = "Level" + levelIndex;
        
        // Check if the scene exists in the build settings
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == levelSceneName)
            {
                sceneExists = true;
                break;
            }
        }
        
        if (sceneExists)
        {
            SceneManager.LoadScene(levelSceneName);
        }
        else
        {
            Debug.LogWarning("Scene " + levelSceneName + " does not exist in build settings. Make sure to add it.");
            
            // For testing, show the game UI
            if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
            if (levelSelectionCanvas != null) levelSelectionCanvas.SetActive(false);
            if (gameUICanvas != null) gameUICanvas.SetActive(true);
            if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
        }
    }
    
    public void ShowPauseMenu(bool show)
    {
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(show);
            Time.timeScale = show ? 0f : 1f; // Pause/unpause the game
        }
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    // Helper method to find a child transform recursively
    private Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name)
            return parent;
            
        foreach (Transform child in parent)
        {
            Transform found = FindChildRecursive(child, name);
            if (found != null)
                return found;
        }
        
        return null;
    }
}
