using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ForceUINavigation : MonoBehaviour
{
    // This script should be attached to any GameObject in your scene
    
    private GameObject mainMenuCanvas;
    private GameObject levelSelectionCanvas;
    
    void Start()
    {
        Debug.Log("ForceUINavigation: Starting...");
        
        // Add key listener to the scene
        GameObject keyListener = new GameObject("KeyListener");
        keyListener.AddComponent<KeyboardListener>();
        
        StartCoroutine(SetupUI());
    }
    
    IEnumerator SetupUI()
    {
        // Wait for a frame to ensure everything is initialized
        yield return null;
        
        // Find canvases
        mainMenuCanvas = GameObject.Find("MainMenuCanvas");
        levelSelectionCanvas = GameObject.Find("LevelSelectionCanvas");
        
        if (mainMenuCanvas == null)
        {
            Debug.LogError("MainMenuCanvas not found! Creating it...");
            CreateMainMenuCanvas();
        }
        
        if (levelSelectionCanvas == null)
        {
            Debug.LogError("LevelSelectionCanvas not found! Creating it...");
            CreateLevelSelectionCanvas();
        }
        
        // Make sure main menu is visible and level selection is hidden
        mainMenuCanvas.SetActive(true);
        levelSelectionCanvas.SetActive(false);
        
        // Find play button
        Button playButton = FindPlayButton();
        if (playButton == null)
        {
            Debug.LogError("Play button not found! Creating it...");
            playButton = CreatePlayButton();
        }
        
        // Add a direct click handler to the play button
        DirectButtonHandler playButtonHandler = playButton.gameObject.AddComponent<DirectButtonHandler>();
        playButtonHandler.mainMenuCanvas = mainMenuCanvas;
        playButtonHandler.levelSelectionCanvas = levelSelectionCanvas;
        playButtonHandler.isPlayButton = true;
        
        // Clear and set up onClick listener
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(playButtonHandler.OnButtonClick);
        
        // Find back button
        Button backButton = FindBackButton();
        if (backButton == null)
        {
            Debug.LogError("Back button not found! Creating it...");
            backButton = CreateBackButton();
        }
        
        // Add a direct click handler to the back button
        DirectButtonHandler backButtonHandler = backButton.gameObject.AddComponent<DirectButtonHandler>();
        backButtonHandler.mainMenuCanvas = mainMenuCanvas;
        backButtonHandler.levelSelectionCanvas = levelSelectionCanvas;
        backButtonHandler.isPlayButton = false;
        
        // Clear and set up onClick listener
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(backButtonHandler.OnButtonClick);
        
        Debug.Log("ForceUINavigation: Setup complete");
    }
    
    private Button FindPlayButton()
    {
        if (mainMenuCanvas == null) return null;
        
        // Try to find by name first
        Transform playButtonTransform = FindChildRecursive(mainMenuCanvas.transform, "PlayButton");
        if (playButtonTransform != null)
        {
            Button button = playButtonTransform.GetComponent<Button>();
            if (button != null)
            {
                Debug.Log("Found play button by name");
                return button;
            }
        }
        
        // If not found by name, look for any button in the main menu
        Button[] buttons = mainMenuCanvas.GetComponentsInChildren<Button>(true);
        if (buttons.Length > 0)
        {
            Debug.Log("Found play button as first button in main menu");
            return buttons[0];
        }
        
        return null;
    }
    
    private Button FindBackButton()
    {
        if (levelSelectionCanvas == null) return null;
        
        // Try to find by name first
        Transform backButtonTransform = FindChildRecursive(levelSelectionCanvas.transform, "BackButton");
        if (backButtonTransform != null)
        {
            Button button = backButtonTransform.GetComponent<Button>();
            if (button != null)
            {
                Debug.Log("Found back button by name");
                return button;
            }
        }
        
        // If not found by name, look for any button in the level selection
        Button[] buttons = levelSelectionCanvas.GetComponentsInChildren<Button>(true);
        if (buttons.Length > 0)
        {
            Debug.Log("Found back button as first button in level selection");
            return buttons[0];
        }
        
        return null;
    }
    
    private void CreateMainMenuCanvas()
    {
        // Create main menu canvas
        mainMenuCanvas = new GameObject("MainMenuCanvas");
        Canvas canvas = mainMenuCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        mainMenuCanvas.AddComponent<CanvasScaler>();
        mainMenuCanvas.AddComponent<GraphicRaycaster>();
        
        // Create panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(mainMenuCanvas.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Create title
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panel.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "MATH PINBALL";
        titleText.fontSize = 72;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.sizeDelta = new Vector2(600, 100);
        titleRect.anchoredPosition = Vector2.zero;
    }
    
    private Button CreatePlayButton()
    {
        if (mainMenuCanvas == null) return null;
        
        // Find panel
        Transform panel = mainMenuCanvas.transform.GetChild(0);
        
        // Create button
        GameObject buttonObj = new GameObject("PlayButton");
        buttonObj.transform.SetParent(panel, false);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.sizeDelta = new Vector2(200, 60);
        buttonRect.anchoredPosition = Vector2.zero;
        
        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = "PLAY";
        buttonText.fontSize = 36;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    private void CreateLevelSelectionCanvas()
    {
        // Create level selection canvas
        levelSelectionCanvas = new GameObject("LevelSelectionCanvas");
        Canvas canvas = levelSelectionCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1;
        levelSelectionCanvas.AddComponent<CanvasScaler>();
        levelSelectionCanvas.AddComponent<GraphicRaycaster>();
        
        // Create panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(levelSelectionCanvas.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
        
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Create title
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(panel.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "SELECT LEVEL";
        titleText.fontSize = 60;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.85f);
        titleRect.anchorMax = new Vector2(0.5f, 0.95f);
        titleRect.sizeDelta = new Vector2(600, 100);
        titleRect.anchoredPosition = Vector2.zero;
    }
    
    private Button CreateBackButton()
    {
        if (levelSelectionCanvas == null) return null;
        
        // Find panel
        Transform panel = levelSelectionCanvas.transform.GetChild(0);
        
        // Create button
        GameObject buttonObj = new GameObject("BackButton");
        buttonObj.transform.SetParent(panel, false);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.6f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0, 0);
        buttonRect.anchorMax = new Vector2(0, 0);
        buttonRect.sizeDelta = new Vector2(150, 50);
        buttonRect.anchoredPosition = new Vector2(100, 40);
        
        // Create text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        Text buttonText = textObj.AddComponent<Text>();
        buttonText.text = "BACK";
        buttonText.fontSize = 24;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return button;
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

// Component to handle button clicks
public class DirectButtonHandler : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    public bool isPlayButton = true;
    
    public void OnButtonClick()
    {
        Debug.Log("DirectButtonHandler: Button " + gameObject.name + " clicked!");
        
        if (isPlayButton)
        {
            // Play button functionality
            if (mainMenuCanvas != null)
            {
                mainMenuCanvas.SetActive(false);
                Debug.Log("Hiding main menu");
            }
            
            if (levelSelectionCanvas != null)
            {
                levelSelectionCanvas.SetActive(true);
                Debug.Log("Showing level selection");
            }
        }
        else
        {
            // Back button functionality
            if (levelSelectionCanvas != null)
            {
                levelSelectionCanvas.SetActive(false);
                Debug.Log("Hiding level selection");
            }
            
            if (mainMenuCanvas != null)
            {
                mainMenuCanvas.SetActive(true);
                Debug.Log("Showing main menu");
            }
        }
    }
}

// Component to listen for keyboard input
public class KeyboardListener : MonoBehaviour
{
    void Update()
    {
        // Space bar or Enter key
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Space/Enter pressed - finding active button");
            
            // Find the active canvas
            GameObject mainMenuCanvas = GameObject.Find("MainMenuCanvas");
            GameObject levelSelectionCanvas = GameObject.Find("LevelSelectionCanvas");
            
            if (mainMenuCanvas != null && mainMenuCanvas.activeSelf)
            {
                // Find play button in main menu
                Button[] buttons = mainMenuCanvas.GetComponentsInChildren<Button>(true);
                if (buttons.Length > 0)
                {
                    Debug.Log("Pressing play button via keyboard");
                    buttons[0].onClick.Invoke();
                }
            }
            else if (levelSelectionCanvas != null && levelSelectionCanvas.activeSelf)
            {
                // Find back button in level selection
                Button[] buttons = levelSelectionCanvas.GetComponentsInChildren<Button>(true);
                foreach (Button button in buttons)
                {
                    if (button.gameObject.name.Contains("Back"))
                    {
                        Debug.Log("Pressing back button via keyboard");
                        button.onClick.Invoke();
                        return;
                    }
                }
                
                // If back button not found, press first button
                if (buttons.Length > 0)
                {
                    Debug.Log("Pressing first level selection button via keyboard");
                    buttons[0].onClick.Invoke();
                }
            }
        }
    }
}
