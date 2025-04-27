using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class KeyboardMouseUIController : MonoBehaviour
{
    // Canvases
    private GameObject mainMenuCanvas;
    private GameObject levelSelectionCanvas;
    private GameObject gameUICanvas;
    private GameObject pauseMenuCanvas;
    
    // Current selected button
    private GameObject currentSelectedButton;
    
    // Lists of buttons in each screen
    private Button[] mainMenuButtons;
    private Button[] levelSelectionButtons;
    
    // State tracking
    private enum UIState { MainMenu, LevelSelection, Game, Pause }
    private UIState currentState = UIState.MainMenu;
    
    void Start()
    {
        Debug.Log("KeyboardMouseUIController: Starting...");
        StartCoroutine(SetupWithDelay());
    }
    
    IEnumerator SetupWithDelay()
    {
        // Wait for a frame to make sure everything is initialized
        yield return null;
        
        // Find canvases
        mainMenuCanvas = GameObject.Find("MainMenuCanvas");
        levelSelectionCanvas = GameObject.Find("LevelSelectionCanvas");
        gameUICanvas = GameObject.Find("GameUICanvas");
        pauseMenuCanvas = GameObject.Find("PauseMenuCanvas");
        
        if (mainMenuCanvas == null)
        {
            Debug.LogError("MainMenuCanvas not found!");
            yield break;
        }
        
        // Find all buttons in main menu
        mainMenuButtons = mainMenuCanvas.GetComponentsInChildren<Button>(true);
        Debug.Log("Found " + mainMenuButtons.Length + " buttons in main menu");
        
        // Find all buttons in level selection
        if (levelSelectionCanvas != null)
        {
            levelSelectionButtons = levelSelectionCanvas.GetComponentsInChildren<Button>(true);
            Debug.Log("Found " + levelSelectionButtons.Length + " buttons in level selection");
        }
        
        // Set initial state
        SetState(UIState.MainMenu);
        
        // Select first button in main menu
        if (mainMenuButtons.Length > 0)
        {
            currentSelectedButton = mainMenuButtons[0].gameObject;
            HighlightCurrentButton();
        }
        
        Debug.Log("KeyboardMouseUIController: Setup complete");
    }
    
    void Update()
    {
        // Handle keyboard input
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            PressCurrentButton();
        }
        
        // Handle mouse input
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            // Check if mouse is over a button
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // The mouse is over a UI element, let the normal click handling work
                return;
            }
            
            // If not over a UI element, press the currently selected button
            PressCurrentButton();
        }
        
        // Navigation with arrow keys
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || 
            Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            NavigateButtons(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.DownArrow));
        }
    }
    
    private void NavigateButtons(bool forward)
    {
        Button[] currentButtons = null;
        int currentIndex = 0;
        
        // Determine which set of buttons to use
        switch (currentState)
        {
            case UIState.MainMenu:
                currentButtons = mainMenuButtons;
                // Find current index
                for (int i = 0; i < currentButtons.Length; i++)
                {
                    if (currentButtons[i].gameObject == currentSelectedButton)
                    {
                        currentIndex = i;
                        break;
                    }
                }
                break;
                
            case UIState.LevelSelection:
                currentButtons = levelSelectionButtons;
                // Find current index
                for (int i = 0; i < currentButtons.Length; i++)
                {
                    if (currentButtons[i].gameObject == currentSelectedButton)
                    {
                        currentIndex = i;
                        break;
                    }
                }
                break;
                
            default:
                return;
        }
        
        if (currentButtons == null || currentButtons.Length == 0)
            return;
        
        // Move to next/previous button
        if (forward)
        {
            currentIndex = (currentIndex + 1) % currentButtons.Length;
        }
        else
        {
            currentIndex = (currentIndex - 1 + currentButtons.Length) % currentButtons.Length;
        }
        
        // Update current button
        currentSelectedButton = currentButtons[currentIndex].gameObject;
        HighlightCurrentButton();
    }
    
    private void HighlightCurrentButton()
    {
        if (currentSelectedButton == null)
            return;
            
        // Visually highlight the button
        Button button = currentSelectedButton.GetComponent<Button>();
        if (button != null)
        {
            // Set it as the selected object in the event system
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(currentSelectedButton);
            }
            
            // Also change the color to make it more obvious
            ColorBlock colors = button.colors;
            colors.normalColor = new Color(0.5f, 0.8f, 1f);
            button.colors = colors;
            
            Debug.Log("Highlighted button: " + currentSelectedButton.name);
        }
    }
    
    private void PressCurrentButton()
    {
        if (currentSelectedButton == null)
            return;
            
        Debug.Log("Pressing button: " + currentSelectedButton.name);
        
        // Handle button press based on the button's name
        switch (currentSelectedButton.name)
        {
            case "PlayButton":
                SetState(UIState.LevelSelection);
                break;
                
            case "BackButton":
                SetState(UIState.MainMenu);
                break;
                
            case "Level1Button":
            case "Level2Button":
            case "Level3Button":
                // Extract level number from button name
                string levelNumberStr = currentSelectedButton.name.Substring(5, 1);
                int levelNumber;
                if (int.TryParse(levelNumberStr, out levelNumber))
                {
                    LoadLevel(levelNumber);
                }
                break;
                
            case "ResumeButton":
                SetState(UIState.Game);
                break;
                
            default:
                // Try to invoke the button's onClick event
                Button button = currentSelectedButton.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.Invoke();
                }
                break;
        }
    }
    
    private void SetState(UIState newState)
    {
        currentState = newState;
        
        // Update canvas visibility
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(newState == UIState.MainMenu);
        if (levelSelectionCanvas != null) levelSelectionCanvas.SetActive(newState == UIState.LevelSelection);
        if (gameUICanvas != null) gameUICanvas.SetActive(newState == UIState.Game);
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(newState == UIState.Pause);
        
        // Update current button selection
        switch (newState)
        {
            case UIState.MainMenu:
                if (mainMenuButtons != null && mainMenuButtons.Length > 0)
                {
                    currentSelectedButton = mainMenuButtons[0].gameObject;
                }
                break;
                
            case UIState.LevelSelection:
                if (levelSelectionButtons != null && levelSelectionButtons.Length > 0)
                {
                    // Find the back button first
                    for (int i = 0; i < levelSelectionButtons.Length; i++)
                    {
                        if (levelSelectionButtons[i].gameObject.name == "BackButton")
                        {
                            currentSelectedButton = levelSelectionButtons[i].gameObject;
                            break;
                        }
                    }
                    
                    // If back button not found, use the first button
                    if (currentSelectedButton == null || currentSelectedButton.name != "BackButton")
                    {
                        currentSelectedButton = levelSelectionButtons[0].gameObject;
                    }
                }
                break;
        }
        
        // Highlight the current button
        HighlightCurrentButton();
        
        Debug.Log("Set state to: " + newState);
    }
    
    private void LoadLevel(int levelNumber)
    {
        Debug.Log("Loading level: " + levelNumber);
        
        // Try to load the level scene
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
            Debug.LogWarning("Scene " + levelSceneName + " does not exist in build settings.");
            
            // For testing, just show the game UI
            SetState(UIState.Game);
        }
    }
}
