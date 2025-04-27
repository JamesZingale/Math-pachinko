using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

#if UNITY_EDITOR
public class SimpleUISetup : EditorWindow
{
    private Color mainColor = new Color(0.2f, 0.6f, 1f);
    private Color accentColor = new Color(1f, 0.5f, 0.2f);
    
    [MenuItem("Math Pinball/Simple UI Setup")]
    public static void ShowWindow()
    {
        GetWindow<SimpleUISetup>("Simple UI Setup");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Math Pinball Simple UI Setup", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        mainColor = EditorGUILayout.ColorField("Main Color", mainColor);
        accentColor = EditorGUILayout.ColorField("Accent Color", accentColor);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create UI", GUILayout.Height(30)))
        {
            CreateUI();
        }
    }
    
    private void CreateUI()
    {
        // Create UI canvases
        GameObject mainMenuCanvas = CreateCanvas("MainMenuCanvas", 0);
        GameObject levelSelectionCanvas = CreateCanvas("LevelSelectionCanvas", 1);
        GameObject gameUICanvas = CreateCanvas("GameUICanvas", 2);
        GameObject pauseMenuCanvas = CreateCanvas("PauseMenuCanvas", 10);
        
        // Initially disable all but main menu
        levelSelectionCanvas.SetActive(false);
        gameUICanvas.SetActive(false);
        pauseMenuCanvas.SetActive(false);
        
        // Create main menu UI
        GameObject mainPanel = CreatePanel(mainMenuCanvas, "Panel", new Color(0.1f, 0.1f, 0.1f, 0.9f));
        CreateText(mainPanel, "TitleText", "MATH PINBALL", 72, new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.9f));
        
        // Create play button
        GameObject playButton = CreateButton(mainPanel, "PlayButton", "PLAY", 36, 
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(200, 60), Vector2.zero);
        
        // Add ButtonController to play button
        ButtonController playButtonController = playButton.AddComponent<ButtonController>();
        playButtonController.action = ButtonController.ButtonAction.Play;
        playButtonController.mainMenuCanvas = mainMenuCanvas;
        playButtonController.levelSelectionCanvas = levelSelectionCanvas;
        playButtonController.gameUICanvas = gameUICanvas;
        playButtonController.pauseMenuCanvas = pauseMenuCanvas;
        
        // Create level selection UI
        GameObject levelPanel = CreatePanel(levelSelectionCanvas, "Panel", new Color(0.1f, 0.1f, 0.1f, 0.9f));
        CreateText(levelPanel, "TitleText", "SELECT LEVEL", 60, new Vector2(0.5f, 0.85f), new Vector2(0.5f, 0.95f));
        
        // Create back button
        GameObject backButton = CreateButton(levelPanel, "BackButton", "BACK", 24, 
            new Vector2(0, 0), new Vector2(0, 0), new Vector2(150, 50), new Vector2(100, 40));
        
        // Add ButtonController to back button
        ButtonController backButtonController = backButton.AddComponent<ButtonController>();
        backButtonController.action = ButtonController.ButtonAction.Back;
        backButtonController.mainMenuCanvas = mainMenuCanvas;
        backButtonController.levelSelectionCanvas = levelSelectionCanvas;
        backButtonController.gameUICanvas = gameUICanvas;
        backButtonController.pauseMenuCanvas = pauseMenuCanvas;
        
        // Create level buttons
        for (int i = 0; i < 3; i++)
        {
            GameObject levelButton = CreateButton(levelPanel, "Level" + (i + 1) + "Button", (i + 1).ToString(), 48, 
                new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(150, 150), new Vector2((i - 1) * 200, 0));
            
            // Add ButtonController to level button
            ButtonController levelButtonController = levelButton.AddComponent<ButtonController>();
            levelButtonController.action = ButtonController.ButtonAction.LoadLevel;
            levelButtonController.levelIndex = i + 1;
            levelButtonController.mainMenuCanvas = mainMenuCanvas;
            levelButtonController.levelSelectionCanvas = levelSelectionCanvas;
            levelButtonController.gameUICanvas = gameUICanvas;
            levelButtonController.pauseMenuCanvas = pauseMenuCanvas;
        }
        
        // Create game UI
        GameObject gamePanel = CreatePanel(gameUICanvas, "Panel", new Color(0, 0, 0, 0));
        CreateText(gamePanel, "ScoreText", "Score: 0", 36, new Vector2(0, 1), new Vector2(0, 1), new Vector2(300, 50), new Vector2(20, -20));
        CreateText(gamePanel, "EquationText", "", 48, new Vector2(0.2f, 0.8f), new Vector2(0.8f, 0.9f));
        
        // Create pause menu UI
        GameObject pausePanel = CreatePanel(pauseMenuCanvas, "Panel", new Color(0, 0, 0, 0.9f));
        CreateText(pausePanel, "PausedText", "PAUSED", 60, new Vector2(0, 0.85f), new Vector2(1, 0.95f));
        
        // Create resume button
        GameObject resumeButton = CreateButton(pausePanel, "ResumeButton", "RESUME", 36, 
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(200, 60), Vector2.zero);
        
        // Add ButtonController to resume button
        ButtonController resumeButtonController = resumeButton.AddComponent<ButtonController>();
        resumeButtonController.action = ButtonController.ButtonAction.Resume;
        resumeButtonController.mainMenuCanvas = mainMenuCanvas;
        resumeButtonController.levelSelectionCanvas = levelSelectionCanvas;
        resumeButtonController.gameUICanvas = gameUICanvas;
        resumeButtonController.pauseMenuCanvas = pauseMenuCanvas;
        
        // Display success message
        EditorUtility.DisplayDialog("UI Created", 
            "UI has been created successfully! Make sure to save your scene.", "OK");
    }
    
    private GameObject CreateCanvas(string name, int sortingOrder)
    {
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        return canvasObj;
    }
    
    private GameObject CreatePanel(GameObject parent, string name, Color color)
    {
        GameObject panelObj = new GameObject(name);
        panelObj.transform.SetParent(parent.transform, false);
        
        Image image = panelObj.AddComponent<Image>();
        image.color = color;
        
        RectTransform rectTransform = panelObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        
        return panelObj;
    }
    
    private GameObject CreateText(GameObject parent, string name, string text, float fontSize, 
        Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta = default, Vector2 anchoredPosition = default)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent.transform, false);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;
        
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        
        if (sizeDelta != default)
        {
            rectTransform.sizeDelta = sizeDelta;
        }
        
        if (anchoredPosition != default)
        {
            rectTransform.anchoredPosition = anchoredPosition;
        }
        
        return textObj;
    }
    
    private GameObject CreateButton(GameObject parent, string name, string text, float fontSize,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 anchoredPosition)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent.transform, false);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = mainColor;
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = mainColor;
        colors.highlightedColor = new Color(mainColor.r * 1.2f, mainColor.g * 1.2f, mainColor.b * 1.2f, 1f);
        colors.pressedColor = new Color(mainColor.r * 0.8f, mainColor.g * 0.8f, mainColor.b * 0.8f, 1f);
        button.colors = colors;
        
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.sizeDelta = sizeDelta;
        rectTransform.anchoredPosition = anchoredPosition;
        
        // Create text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;
        
        RectTransform textRectTransform = textObj.GetComponent<RectTransform>();
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.offsetMin = Vector2.zero;
        textRectTransform.offsetMax = Vector2.zero;
        
        return buttonObj;
    }
}
#endif
