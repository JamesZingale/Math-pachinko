using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

#if UNITY_EDITOR
public class MainMenuSetup : MonoBehaviour
{
    [MenuItem("Math Pachinko/Setup Main Menu Scene")]
    public static void SetupMainMenuScene()
    {
        // Check if we're in the MainMenu scene
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "MainMenu")
        {
            if (!EditorUtility.DisplayDialog("Wrong Scene", 
                "This tool should be run in the MainMenu scene. Would you like to continue anyway?", 
                "Continue", "Cancel"))
            {
                return;
            }
        }

        // Create Canvas
        GameObject canvasObj = new GameObject("MainMenuCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Add EventSystem if it doesn't exist
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        // Create Main Menu Panel
        GameObject mainMenuPanel = CreatePanel("MainMenuPanel", canvasObj.transform);
        
        // Create Level Selection Panel
        GameObject levelSelectionPanel = CreatePanel("LevelSelectionPanel", canvasObj.transform);
        levelSelectionPanel.SetActive(false);
        
        // Create Options Panel
        GameObject optionsPanel = CreatePanel("OptionsPanel", canvasObj.transform);
        optionsPanel.SetActive(false);
        
        // Create Credits Panel
        GameObject creditsPanel = CreatePanel("CreditsPanel", canvasObj.transform);
        creditsPanel.SetActive(false);

        // Create Main Menu UI Elements
        GameObject titleObj = CreateTextElement("GameTitle", mainMenuPanel.transform, "Math Pachinko", 60);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 200);

        // Create Buttons for Main Menu
        GameObject playButton = CreateButton("PlayButton", mainMenuPanel.transform, "Play", new Vector2(0, 50));
        GameObject optionsButton = CreateButton("OptionsButton", mainMenuPanel.transform, "Options", new Vector2(0, -30));
        GameObject creditsButton = CreateButton("CreditsButton", mainMenuPanel.transform, "Credits", new Vector2(0, -110));
        GameObject quitButton = CreateButton("QuitButton", mainMenuPanel.transform, "Quit", new Vector2(0, -190));

        // Create Version Text
        GameObject versionObj = CreateTextElement("VersionText", mainMenuPanel.transform, "v1.0", 20);
        RectTransform versionRect = versionObj.GetComponent<RectTransform>();
        versionRect.anchorMin = new Vector2(1, 0);
        versionRect.anchorMax = new Vector2(1, 0);
        versionRect.pivot = new Vector2(1, 0);
        versionRect.anchoredPosition = new Vector2(-20, 20);

        // Create Level Selection Panel Content
        GameObject levelSelectionTitle = CreateTextElement("LevelSelectionTitle", levelSelectionPanel.transform, "Select Level", 50);
        RectTransform levelTitleRect = levelSelectionTitle.GetComponent<RectTransform>();
        levelTitleRect.anchoredPosition = new Vector2(0, 200);

        // Create Level Buttons Container
        GameObject levelButtonContainer = new GameObject("LevelButtonContainer");
        levelButtonContainer.transform.SetParent(levelSelectionPanel.transform, false);
        RectTransform containerRect = levelButtonContainer.AddComponent<RectTransform>();
        containerRect.anchoredPosition = new Vector2(0, 0);
        containerRect.sizeDelta = new Vector2(600, 300);
        
        // Create a Grid Layout Group for level buttons
        GridLayoutGroup gridLayout = levelButtonContainer.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(100, 100);
        gridLayout.spacing = new Vector2(20, 20);
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 5;

        // Create Back Button for Level Selection
        GameObject backFromLevelsButton = CreateButton("BackFromLevelsButton", levelSelectionPanel.transform, "Back", new Vector2(0, -200));

        // Create Options Panel Content
        GameObject optionsTitle = CreateTextElement("OptionsTitle", optionsPanel.transform, "Options", 50);
        RectTransform optionsTitleRect = optionsTitle.GetComponent<RectTransform>();
        optionsTitleRect.anchoredPosition = new Vector2(0, 200);

        // Create Music Volume Slider
        GameObject musicVolumeObj = CreateSlider("MusicVolumeSlider", optionsPanel.transform, "Music Volume", new Vector2(0, 100));
        
        // Create SFX Volume Slider
        GameObject sfxVolumeObj = CreateSlider("SFXVolumeSlider", optionsPanel.transform, "SFX Volume", new Vector2(0, 20));
        
        // Create Fullscreen Toggle
        GameObject fullscreenObj = CreateToggle("FullscreenToggle", optionsPanel.transform, "Fullscreen", new Vector2(0, -60));
        
        // Create Back Button for Options
        GameObject backFromOptionsButton = CreateButton("BackFromOptionsButton", optionsPanel.transform, "Back", new Vector2(0, -200));

        // Create Credits Panel Content
        GameObject creditsTitle = CreateTextElement("CreditsTitle", creditsPanel.transform, "Credits", 50);
        RectTransform creditsTitleRect = creditsTitle.GetComponent<RectTransform>();
        creditsTitleRect.anchoredPosition = new Vector2(0, 200);

        GameObject creditsText = CreateTextElement("CreditsText", creditsPanel.transform, "Game created by: [Your Name]\n\nMath Pachinko - A fun way to learn math!", 30);
        RectTransform creditsTextRect = creditsText.GetComponent<RectTransform>();
        creditsTextRect.anchoredPosition = new Vector2(0, 0);
        creditsTextRect.sizeDelta = new Vector2(600, 300);

        // Create Back Button for Credits
        GameObject backFromCreditsButton = CreateButton("BackFromCreditsButton", creditsPanel.transform, "Back", new Vector2(0, -200));

        // Add MainMenuManager component
        MainMenuManager menuManager = canvasObj.AddComponent<MainMenuManager>();
        
        // Set references
        menuManager.mainMenuPanel = mainMenuPanel;
        menuManager.levelSelectionPanel = levelSelectionPanel;
        menuManager.optionsPanel = optionsPanel;
        menuManager.creditsPanel = creditsPanel;
        
        menuManager.playButton = playButton.GetComponent<Button>();
        menuManager.optionsButton = optionsButton.GetComponent<Button>();
        menuManager.creditsButton = creditsButton.GetComponent<Button>();
        menuManager.quitButton = quitButton.GetComponent<Button>();
        
        menuManager.backFromLevelsButton = backFromLevelsButton.GetComponent<Button>();
        menuManager.backFromOptionsButton = backFromOptionsButton.GetComponent<Button>();
        menuManager.backFromCreditsButton = backFromCreditsButton.GetComponent<Button>();
        
        menuManager.musicVolumeSlider = musicVolumeObj.GetComponent<Slider>();
        menuManager.sfxVolumeSlider = sfxVolumeObj.GetComponent<Slider>();
        menuManager.fullscreenToggle = fullscreenObj.GetComponent<Toggle>();
        
        menuManager.gameTitle = titleObj.GetComponent<TextMeshProUGUI>();
        menuManager.versionText = versionObj.GetComponent<TextMeshProUGUI>();
        
        menuManager.levelButtonContainer = levelButtonContainer.transform;
        
        // Try to find the level button prefab
        GameObject levelButtonPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/UI/LevelButtonPrefab.prefab");
        if (levelButtonPrefab != null)
        {
            menuManager.levelButtonPrefab = levelButtonPrefab;
        }
        else
        {
            Debug.LogWarning("Level button prefab not found at Assets/Prefabs/UI/LevelButtonPrefab.prefab. You'll need to assign it manually.");
        }

        // Add a camera if one doesn't exist
        if (Camera.main == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
            camera.orthographic = true;
            camera.tag = "MainCamera";
        }

        Debug.Log("Main Menu setup completed! Don't forget to save the scene.");
    }

    private static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        CanvasGroup canvasGroup = panel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        
        return panel;
    }

    private static GameObject CreateTextElement(string name, Transform parent, string text, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500, 100);
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        return textObj;
    }

    private static GameObject CreateButton(string name, Transform parent, string text, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 60);
        rect.anchoredPosition = position;
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f);
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = new Color(0.2f, 0.2f, 0.2f);
        colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f);
        colors.pressedColor = new Color(0.1f, 0.1f, 0.1f);
        colors.selectedColor = new Color(0.3f, 0.3f, 0.3f);
        button.colors = colors;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        
        return buttonObj;
    }

    private static GameObject CreateSlider(string name, Transform parent, string labelText, Vector2 position)
    {
        // Create container
        GameObject container = new GameObject(name + "Container");
        container.transform.SetParent(parent, false);
        
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(400, 60);
        containerRect.anchoredPosition = position;
        
        // Create label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0, 0.5f);
        labelRect.pivot = new Vector2(0, 0.5f);
        labelRect.sizeDelta = new Vector2(150, 40);
        labelRect.anchoredPosition = new Vector2(0, 0);
        
        TextMeshProUGUI labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
        labelTmp.text = labelText;
        labelTmp.fontSize = 20;
        labelTmp.alignment = TextAlignmentOptions.Left;
        labelTmp.color = Color.white;
        
        // Create slider
        GameObject sliderObj = new GameObject(name);
        sliderObj.transform.SetParent(container.transform, false);
        
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(1, 0.5f);
        sliderRect.anchorMax = new Vector2(1, 0.5f);
        sliderRect.pivot = new Vector2(1, 0.5f);
        sliderRect.sizeDelta = new Vector2(200, 20);
        sliderRect.anchoredPosition = new Vector2(0, 0);
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0.75f;
        
        // Create Background
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObj.transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f);
        
        // Create Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0, 0.5f);
        fillAreaRect.anchorMax = new Vector2(1, 0.5f);
        fillAreaRect.offsetMin = new Vector2(5, -5);
        fillAreaRect.offsetMax = new Vector2(-5, 5);
        
        // Create Fill
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0.75f, 1); // Set to match slider.value
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.6f, 1f);
        
        // Create Handle Slide Area
        GameObject handleSlideArea = new GameObject("Handle Slide Area");
        handleSlideArea.transform.SetParent(sliderObj.transform, false);
        
        RectTransform handleSlideAreaRect = handleSlideArea.AddComponent<RectTransform>();
        handleSlideAreaRect.anchorMin = Vector2.zero;
        handleSlideAreaRect.anchorMax = Vector2.one;
        handleSlideAreaRect.offsetMin = Vector2.zero;
        handleSlideAreaRect.offsetMax = Vector2.zero;
        
        // Create Handle
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleSlideArea.transform, false);
        
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0.75f, 0.5f); // Set to match slider.value
        handleRect.anchorMax = new Vector2(0.75f, 0.5f); // Set to match slider.value
        handleRect.sizeDelta = new Vector2(20, 30);
        handleRect.anchoredPosition = Vector2.zero;
        
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(1f, 1f, 1f);
        
        // Set up slider references
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        
        return sliderObj;
    }

    private static GameObject CreateToggle(string name, Transform parent, string labelText, Vector2 position)
    {
        // Create container
        GameObject container = new GameObject(name + "Container");
        container.transform.SetParent(parent, false);
        
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.sizeDelta = new Vector2(400, 40);
        containerRect.anchoredPosition = position;
        
        // Create toggle
        GameObject toggleObj = new GameObject(name);
        toggleObj.transform.SetParent(container.transform, false);
        
        RectTransform toggleRect = toggleObj.AddComponent<RectTransform>();
        toggleRect.anchorMin = new Vector2(0, 0.5f);
        toggleRect.anchorMax = new Vector2(0, 0.5f);
        toggleRect.pivot = new Vector2(0, 0.5f);
        toggleRect.sizeDelta = new Vector2(40, 40);
        toggleRect.anchoredPosition = new Vector2(0, 0);
        
        Image toggleImage = toggleObj.AddComponent<Image>();
        toggleImage.color = new Color(0.2f, 0.2f, 0.2f);
        
        Toggle toggle = toggleObj.AddComponent<Toggle>();
        toggle.isOn = true;
        
        // Create checkmark
        GameObject checkmark = new GameObject("Checkmark");
        checkmark.transform.SetParent(toggleObj.transform, false);
        
        RectTransform checkmarkRect = checkmark.AddComponent<RectTransform>();
        checkmarkRect.anchorMin = new Vector2(0.1f, 0.1f);
        checkmarkRect.anchorMax = new Vector2(0.9f, 0.9f);
        checkmarkRect.offsetMin = Vector2.zero;
        checkmarkRect.offsetMax = Vector2.zero;
        
        Image checkmarkImage = checkmark.AddComponent<Image>();
        checkmarkImage.color = new Color(0.2f, 0.6f, 1f);
        
        // Create label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0, 0.5f);
        labelRect.pivot = new Vector2(0, 0.5f);
        labelRect.sizeDelta = new Vector2(200, 40);
        labelRect.anchoredPosition = new Vector2(60, 0);
        
        TextMeshProUGUI labelTmp = labelObj.AddComponent<TextMeshProUGUI>();
        labelTmp.text = labelText;
        labelTmp.fontSize = 20;
        labelTmp.alignment = TextAlignmentOptions.Left;
        labelTmp.color = Color.white;
        
        // Set up toggle references
        toggle.graphic = checkmarkImage;
        toggle.targetGraphic = toggleImage;
        
        return toggleObj;
    }
}
#endif
