using UnityEngine;
using UnityEngine.UI;

public class KeyboardInputHandler : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    public GameObject gameUICanvas;
    public GameObject pauseMenuCanvas;
    
    void Update()
    {
        // Check for space bar or enter key press
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            HandleKeyPress();
        }
        
        // Check for number keys when in level selection screen
        if (levelSelectionCanvas != null && levelSelectionCanvas.activeSelf)
        {
            // Check for number keys 1-9
            for (int i = 1; i <= 9; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i - 1) || Input.GetKeyDown(KeyCode.Keypad1 + i - 1))
                {
                    HandleNumberKeyPress(i);
                    break;
                }
            }
        }
    }
    
    private void HandleNumberKeyPress(int number)
    {
        Debug.Log("Number key " + number + " pressed");
        
        // Find the corresponding level button
        string buttonName = "Level" + number + "Button";
        Button levelButton = FindButtonInCanvas(levelSelectionCanvas, buttonName);
        
        if (levelButton != null)
        {
            Debug.Log("Activating " + buttonName);
            levelButton.onClick.Invoke();
        }
        else
        {
            Debug.LogWarning("Level button " + buttonName + " not found");
            
            // Try to find a DirectLevelButtonHandler with the matching level index
            DirectLevelButtonHandler[] handlers = levelSelectionCanvas.GetComponentsInChildren<DirectLevelButtonHandler>(true);
            foreach (DirectLevelButtonHandler handler in handlers)
            {
                if (handler.levelIndex == number)
                {
                    Debug.Log("Found level handler for level " + number);
                    handler.OnButtonClick();
                    return;
                }
            }
            
            // If no specific button found, just try to load the level directly
            LoadLevel(number);
        }
    }
    
    private void LoadLevel(int levelNumber)
    {
        Debug.Log("Directly loading level " + levelNumber);
        
        string levelSceneName = "Level" + levelNumber;
        
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
            Debug.LogWarning("Scene " + levelSceneName + " does not exist in build settings. Showing game UI for testing.");
            
            // For testing, show the game UI
            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(false);
                
            if (levelSelectionCanvas != null)
                levelSelectionCanvas.SetActive(false);
                
            if (gameUICanvas != null)
                gameUICanvas.SetActive(true);
        }
    }
    
    private void HandleKeyPress()
    {
        Debug.Log("Space or Enter key pressed");
        
        // Determine which canvas is active and perform the appropriate action
        if (mainMenuCanvas != null && mainMenuCanvas.activeSelf)
        {
            // In main menu, press the Play button
            Debug.Log("In main menu - activating Play button");
            Button playButton = FindButtonInCanvas(mainMenuCanvas, "PlayButton");
            if (playButton != null)
            {
                playButton.onClick.Invoke();
            }
            else
            {
                // Direct navigation if button not found
                mainMenuCanvas.SetActive(false);
                if (levelSelectionCanvas != null)
                    levelSelectionCanvas.SetActive(true);
            }
        }
        else if (levelSelectionCanvas != null && levelSelectionCanvas.activeSelf)
        {
            // In level selection, press the first level button
            Debug.Log("In level selection - activating Level1 button");
            Button levelButton = FindButtonInCanvas(levelSelectionCanvas, "Level1Button");
            if (levelButton != null)
            {
                levelButton.onClick.Invoke();
            }
            else
            {
                // Direct navigation if button not found
                levelSelectionCanvas.SetActive(false);
                if (gameUICanvas != null)
                    gameUICanvas.SetActive(true);
            }
        }
        else if (pauseMenuCanvas != null && pauseMenuCanvas.activeSelf)
        {
            // In pause menu, press the Resume button
            Debug.Log("In pause menu - activating Resume button");
            Button resumeButton = FindButtonInCanvas(pauseMenuCanvas, "ResumeButton");
            if (resumeButton != null)
            {
                resumeButton.onClick.Invoke();
            }
            else
            {
                // Direct action if button not found
                pauseMenuCanvas.SetActive(false);
                Time.timeScale = 1f;
            }
        }
    }
    
    private Button FindButtonInCanvas(GameObject canvas, string buttonName)
    {
        // Try to find button by name
        Transform buttonTransform = FindChildRecursive(canvas.transform, buttonName);
        if (buttonTransform != null)
        {
            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                Debug.Log("Found button: " + buttonName);
                return button;
            }
        }
        
        // If not found by name, return the first button in the canvas
        Button[] buttons = canvas.GetComponentsInChildren<Button>(true);
        if (buttons.Length > 0)
        {
            Debug.Log("Found first button in canvas: " + buttons[0].gameObject.name);
            return buttons[0];
        }
        
        Debug.LogWarning("No buttons found in canvas: " + canvas.name);
        return null;
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
