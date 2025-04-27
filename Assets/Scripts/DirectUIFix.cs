using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DirectUIFix : MonoBehaviour
{
    // This script should be attached to an empty GameObject in your scene
    
    // References that will be found automatically
    private GameObject mainMenuCanvas;
    private GameObject levelSelectionCanvas;
    private Button playButton;
    private Button backButton;
    
    void Start()
    {
        Debug.Log("DirectUIFix: Starting to fix UI...");
        
        // Find or create canvases
        SetupCanvases();
        
        // Set up buttons
        SetupButtons();
        
        Debug.Log("DirectUIFix: UI setup complete!");
    }
    
    private void SetupCanvases()
    {
        // Find existing canvases
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        
        foreach (Canvas canvas in canvases)
        {
            if (canvas.gameObject.name.Contains("MainMenu"))
            {
                mainMenuCanvas = canvas.gameObject;
                Debug.Log("DirectUIFix: Found main menu canvas: " + mainMenuCanvas.name);
            }
            else if (canvas.gameObject.name.Contains("LevelSelection"))
            {
                levelSelectionCanvas = canvas.gameObject;
                Debug.Log("DirectUIFix: Found level selection canvas: " + levelSelectionCanvas.name);
            }
        }
        
        // Create canvases if they don't exist
        if (mainMenuCanvas == null)
        {
            mainMenuCanvas = CreateMainMenuCanvas();
            Debug.Log("DirectUIFix: Created new main menu canvas");
        }
        
        if (levelSelectionCanvas == null)
        {
            levelSelectionCanvas = CreateLevelSelectionCanvas();
            Debug.Log("DirectUIFix: Created new level selection canvas");
        }
        
        // Make sure level selection is initially hidden
        if (levelSelectionCanvas != null)
        {
            levelSelectionCanvas.SetActive(false);
        }
    }
    
    private void SetupButtons()
    {
        // Find play button in main menu
        if (mainMenuCanvas != null)
        {
            // Look for existing play button
            Button[] buttons = mainMenuCanvas.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.gameObject.name.Contains("Play"))
                {
                    playButton = button;
                    Debug.Log("DirectUIFix: Found play button: " + playButton.name);
                    break;
                }
            }
            
            // Create play button if it doesn't exist
            if (playButton == null)
            {
                playButton = CreatePlayButton(mainMenuCanvas);
                Debug.Log("DirectUIFix: Created new play button");
            }
            
            // Set up play button click handler
            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(() => {
                    Debug.Log("DirectUIFix: Play button clicked!");
                    mainMenuCanvas.SetActive(false);
                    levelSelectionCanvas.SetActive(true);
                });
                Debug.Log("DirectUIFix: Play button click handler set up");
            }
        }
        
        // Find back button in level selection
        if (levelSelectionCanvas != null)
        {
            // Look for existing back button
            Button[] buttons = levelSelectionCanvas.GetComponentsInChildren<Button>(true);
            foreach (Button button in buttons)
            {
                if (button.gameObject.name.Contains("Back"))
                {
                    backButton = button;
                    Debug.Log("DirectUIFix: Found back button: " + backButton.name);
                    break;
                }
            }
            
            // Create back button if it doesn't exist
            if (backButton == null)
            {
                backButton = CreateBackButton(levelSelectionCanvas);
                Debug.Log("DirectUIFix: Created new back button");
            }
            
            // Set up back button click handler
            if (backButton != null)
            {
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(() => {
                    Debug.Log("DirectUIFix: Back button clicked!");
                    levelSelectionCanvas.SetActive(false);
                    mainMenuCanvas.SetActive(true);
                });
                Debug.Log("DirectUIFix: Back button click handler set up");
            }
        }
    }
    
    private GameObject CreateMainMenuCanvas()
    {
        // Create canvas
        GameObject canvasObj = new GameObject("MainMenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasObj.transform, false);
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
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "MATH PINBALL";
        titleText.fontSize = 72;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(600, 100);
        
        return canvasObj;
    }
    
    private GameObject CreateLevelSelectionCanvas()
    {
        // Create canvas
        GameObject canvasObj = new GameObject("LevelSelectionCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create panel
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(canvasObj.transform, false);
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
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "SELECT LEVEL";
        titleText.fontSize = 60;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.85f);
        titleRect.anchorMax = new Vector2(0.5f, 0.95f);
        titleRect.anchoredPosition = Vector2.zero;
        titleRect.sizeDelta = new Vector2(600, 100);
        
        // Create level buttons (1, 2, 3)
        for (int i = 0; i < 3; i++)
        {
            GameObject levelButton = new GameObject("Level" + (i+1) + "Button");
            levelButton.transform.SetParent(panel.transform, false);
            
            Image buttonImage = levelButton.AddComponent<Image>();
            buttonImage.color = new Color(0.2f, 0.6f, 1f);
            
            Button button = levelButton.AddComponent<Button>();
            
            RectTransform buttonRect = levelButton.GetComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.sizeDelta = new Vector2(150, 150);
            buttonRect.anchoredPosition = new Vector2((i-1) * 200, 0);
            
            // Create text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(levelButton.transform, false);
            TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = (i+1).ToString();
            buttonText.fontSize = 48;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.white;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            // Add level button click handler
            int levelIndex = i+1;
            button.onClick.AddListener(() => {
                Debug.Log("DirectUIFix: Level " + levelIndex + " button clicked!");
                // Load level or show game UI for testing
                SceneManager.LoadScene("Level" + levelIndex);
            });
        }
        
        return canvasObj;
    }
    
    private Button CreatePlayButton(GameObject parent)
    {
        // Find panel
        Transform panel = parent.transform.GetChild(0);
        
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
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "PLAY";
        buttonText.fontSize = 36;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    private Button CreateBackButton(GameObject parent)
    {
        // Find panel
        Transform panel = parent.transform.GetChild(0);
        
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
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "BACK";
        buttonText.fontSize = 24;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.color = Color.white;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return button;
    }
}
