using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
// Editor classes are used for creating custom Editor windows and inspectors
public class MathPinballSetupWizard : EditorWindow
{
    private enum SetupStep
    {
        Welcome,
        CreateUICanvases,
        SetupPhysics,
        ConnectManagers,
        ConfigureUI,
        CreateLevels,
        Complete
    }
    
    private SetupStep currentStep = SetupStep.Welcome;
    private Vector2 scrollPosition;
    private bool[] stepCompleted = new bool[7]; // Updated to include level creation step
    private GUIStyle headerStyle;
    private GUIStyle subHeaderStyle;
    private GUIStyle bodyStyle;
    private Texture2D logo;
    
    // UI Canvas options
    private bool createMainMenu = true;
    private bool createLevelSelection = true;
    private bool createGameUI = true;
    private bool createPauseMenu = true;
    private Color mainColor = new Color(0.2f, 0.6f, 1f);
    private Color accentColor = new Color(1f, 0.5f, 0.2f);
    
    // Physics setup options
    private bool setupNumberBalls = true;
    private bool setupOperatorBalls = true;
    private bool setupBoundaries = true;
    private bool setupPlayerBall = true;
    
    // Level creation options
    private bool createLevel1 = true;
    private bool createLevel2 = true;
    private bool createLevel3 = true;
    private int level1NumberBalls = 8;
    private int level1OperatorBalls = 3;
    private int level2NumberBalls = 10;
    private int level2OperatorBalls = 5;
    private int level3NumberBalls = 12;
    private int level3OperatorBalls = 8;
    private bool level1AdditionOnly = true;
    private bool level2AddSubtract = true;
    private bool level3AllOperations = true;
    
    // Manager references
    private GameObject gameManagerPrefab;
    private GameObject equationManagerPrefab;
    private GameObject visualEffectsManagerPrefab;
    
    // UI references
    private GameObject mainMenuCanvas;
    private GameObject levelSelectionCanvas;
    private GameObject gameUICanvas;
    private GameObject pauseMenuCanvas;
    
    [MenuItem("Math Pinball/Setup Wizard")]
    public static void ShowWindow()
    {
        MathPinballSetupWizard window = GetWindow<MathPinballSetupWizard>("Math Pinball Setup");
        window.minSize = new Vector2(600, 500);
    }
    
    private void OnEnable()
    {
        // Initialize styles when window is opened
        InitializeStyles();
    }
    
    private void InitializeStyles()
    {
        headerStyle = new GUIStyle(EditorStyles.boldLabel);
        headerStyle.fontSize = 20;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        headerStyle.margin = new RectOffset(10, 10, 20, 20);
        
        subHeaderStyle = new GUIStyle(EditorStyles.boldLabel);
        subHeaderStyle.fontSize = 14;
        subHeaderStyle.margin = new RectOffset(5, 5, 10, 10);
        
        bodyStyle = new GUIStyle(EditorStyles.label);
        bodyStyle.wordWrap = true;
        bodyStyle.margin = new RectOffset(10, 10, 5, 5);
        
        // Try to load logo
        logo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/MathPinballLogo.png");
    }
    
    private void OnGUI()
    {
        if (headerStyle == null)
        {
            InitializeStyles();
        }
        
        // Draw header
        GUILayout.Space(10);
        if (logo != null)
        {
            GUILayout.Label(logo, GUILayout.Height(100), GUILayout.ExpandWidth(true));
        }
        GUILayout.Label("Math Pinball Setup Wizard", headerStyle);
        
        // Draw progress bar
        DrawProgressBar();
        
        // Scrollable content area
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        // Draw current step
        switch (currentStep)
        {
            case SetupStep.Welcome:
                DrawWelcomeStep();
                break;
            case SetupStep.CreateUICanvases:
                DrawCreateUICanvasesStep();
                break;
            case SetupStep.SetupPhysics:
                DrawSetupPhysicsStep();
                break;
            case SetupStep.ConnectManagers:
                DrawConnectManagersStep();
                break;
            case SetupStep.ConfigureUI:
                DrawConfigureUIStep();
                break;
            case SetupStep.CreateLevels:
                DrawCreateLevelsStep();
                break;
            case SetupStep.Complete:
                DrawCompleteStep();
                break;
        }
        
        EditorGUILayout.EndScrollView();
        
        // Navigation buttons
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginHorizontal();
        
        if (currentStep > SetupStep.Welcome)
        {
            if (GUILayout.Button("Previous", GUILayout.Width(100)))
            {
                currentStep--;
            }
        }
        else
        {
            GUILayout.Space(104); // Space for alignment
        }
        
        GUILayout.FlexibleSpace();
        
        if (currentStep < SetupStep.Complete)
        {
            string buttonText = "Next";
            if (currentStep == SetupStep.Welcome)
                buttonText = "Start Setup";
                
            if (GUILayout.Button(buttonText, GUILayout.Width(100)))
            {
                currentStep++;
            }
        }
        else
        {
            if (GUILayout.Button("Close", GUILayout.Width(100)))
            {
                Close();
            }
        }
        
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
    
    private void DrawProgressBar()
    {
        EditorGUILayout.BeginHorizontal();
        
        for (int i = 0; i < 6; i++)
        {
            SetupStep step = (SetupStep)i;
            
            // Determine color based on current step and completion
            Color color;
            if (i < (int)currentStep)
                color = stepCompleted[i] ? Color.green : Color.yellow;
            else if (i == (int)currentStep)
                color = new Color(0.3f, 0.6f, 1f);
            else
                color = Color.gray;
                
            // Draw step indicator
            GUILayout.BeginVertical(GUILayout.Width(position.width / 6));
            
            Rect rect = GUILayoutUtility.GetRect(position.width / 6 - 10, 5);
            EditorGUI.DrawRect(rect, color);
            
            GUIStyle stepStyle = new GUIStyle(EditorStyles.miniLabel);
            stepStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label(step.ToString(), stepStyle);
            
            GUILayout.EndVertical();
        }
        
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
    
    #region Step Drawing Methods
    
    private void DrawWelcomeStep()
    {
        GUILayout.Label("Welcome to the Math Pinball Setup Wizard!", subHeaderStyle);
        
        EditorGUILayout.HelpBox(
            "This wizard will guide you through setting up your Math Pinball game with all necessary components:\n\n" +
            "1. Create UI Canvases (Main Menu, Level Selection, Game UI, Pause Menu)\n" +
            "2. Set up Physics Components for all game objects\n" +
            "3. Connect Game Managers (Game, Equation, Visual Effects)\n" +
            "4. Configure UI connections\n" +
            "5. Create levels\n\n" +
            "Click 'Start Setup' to begin!",
            MessageType.Info);
            
        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox(
            "Make sure you have already created your basic scene with math balls, boundaries, and a player launcher before running this wizard.",
            MessageType.Warning);
            
        stepCompleted[0] = true;
    }
    
    private void DrawCreateUICanvasesStep()
    {
        GUILayout.Label("Step 1: Create UI Canvases", subHeaderStyle);
        
        EditorGUILayout.HelpBox(
            "This step will create all the UI canvases needed for your game.",
            MessageType.Info);
            
        GUILayout.Space(10);
        
        // UI Canvas options
        EditorGUILayout.LabelField("Select which canvases to create:", EditorStyles.boldLabel);
        
        createMainMenu = EditorGUILayout.Toggle("Main Menu Canvas", createMainMenu);
        createLevelSelection = EditorGUILayout.Toggle("Level Selection Canvas", createLevelSelection);
        createGameUI = EditorGUILayout.Toggle("Game UI Canvas", createGameUI);
        createPauseMenu = EditorGUILayout.Toggle("Pause Menu Canvas", createPauseMenu);
        
        GUILayout.Space(10);
        
        // Color options
        EditorGUILayout.LabelField("UI Color Scheme:", EditorStyles.boldLabel);
        mainColor = EditorGUILayout.ColorField("Main Color", mainColor);
        accentColor = EditorGUILayout.ColorField("Accent Color", accentColor);
        
        GUILayout.Space(20);
        
        // Create UI button
        if (GUILayout.Button("Create UI Canvases Now", GUILayout.Height(30)))
        {
            CreateUICanvases();
            stepCompleted[1] = true;
        }
    }
    
    private void DrawSetupPhysicsStep()
    {
        GUILayout.Label("Step 2: Set Up Physics Components", subHeaderStyle);
        
        EditorGUILayout.HelpBox(
            "This step will configure physics components for all objects in your scene.",
            MessageType.Info);
            
        GUILayout.Space(10);
        
        // Physics setup options
        EditorGUILayout.LabelField("Select which objects to set up:", EditorStyles.boldLabel);
        
        setupNumberBalls = EditorGUILayout.Toggle("Number Balls", setupNumberBalls);
        setupOperatorBalls = EditorGUILayout.Toggle("Operator Balls", setupOperatorBalls);
        setupBoundaries = EditorGUILayout.Toggle("Boundaries", setupBoundaries);
        setupPlayerBall = EditorGUILayout.Toggle("Player Ball", setupPlayerBall);
        
        GUILayout.Space(20);
        
        // Create physics materials first
        if (GUILayout.Button("Create Physics Materials", GUILayout.Height(25)))
        {
            CreatePhysicsMaterialsForGame();
        }
        
        GUILayout.Space(10);
        
        // Set up physics button
        if (GUILayout.Button("Set Up Physics Components Now", GUILayout.Height(30)))
        {
            SetupPhysicsComponents();
            stepCompleted[2] = true;
        }
    }
    
    private void DrawConnectManagersStep()
    {
        GUILayout.Label("Step 3: Connect Game Managers", subHeaderStyle);
        
        EditorGUILayout.HelpBox(
            "This step will add the required manager scripts to your scene and configure them.",
            MessageType.Info);
            
        GUILayout.Space(20);
        
        // Create and connect managers button
        if (GUILayout.Button("Create and Connect Managers Now", GUILayout.Height(30)))
        {
            ConnectManagers();
            stepCompleted[3] = true;
        }
    }
    
    private void DrawConfigureUIStep()
    {
        GUILayout.Label("Step 4: Configure UI Connections", subHeaderStyle);
        
        EditorGUILayout.HelpBox(
            "This step will connect UI elements to the appropriate manager scripts.",
            MessageType.Info);
            
        GUILayout.Space(20);
        
        // Configure UI button
        if (GUILayout.Button("Configure UI Connections Now", GUILayout.Height(30)))
        {
            ConfigureUIConnections();
            stepCompleted[4] = true;
        }
    }
    
    private void DrawCreateLevelsStep()
    {
        GUILayout.Label("Step 5: Create Levels", subHeaderStyle);
        
        EditorGUILayout.HelpBox(
            "This step will create levels for your game.",
            MessageType.Info);
            
        GUILayout.Space(10);
        
        // Level creation options
        EditorGUILayout.LabelField("Select which levels to create:", EditorStyles.boldLabel);
        
        createLevel1 = EditorGUILayout.Toggle("Level 1", createLevel1);
        createLevel2 = EditorGUILayout.Toggle("Level 2", createLevel2);
        createLevel3 = EditorGUILayout.Toggle("Level 3", createLevel3);
        
        // Level 1 options
        if (createLevel1)
        {
            level1NumberBalls = EditorGUILayout.IntField("Number Balls", level1NumberBalls);
            level1OperatorBalls = EditorGUILayout.IntField("Operator Balls", level1OperatorBalls);
            level1AdditionOnly = EditorGUILayout.Toggle("Addition Only", level1AdditionOnly);
        }
        
        // Level 2 options
        if (createLevel2)
        {
            level2NumberBalls = EditorGUILayout.IntField("Number Balls", level2NumberBalls);
            level2OperatorBalls = EditorGUILayout.IntField("Operator Balls", level2OperatorBalls);
            level2AddSubtract = EditorGUILayout.Toggle("Addition and Subtraction", level2AddSubtract);
        }
        
        // Level 3 options
        if (createLevel3)
        {
            level3NumberBalls = EditorGUILayout.IntField("Number Balls", level3NumberBalls);
            level3OperatorBalls = EditorGUILayout.IntField("Operator Balls", level3OperatorBalls);
            level3AllOperations = EditorGUILayout.Toggle("All Operations", level3AllOperations);
        }
        
        GUILayout.Space(20);
        
        // Create levels button
        if (GUILayout.Button("Create Levels Now", GUILayout.Height(30)))
        {
            CreateLevels();
            stepCompleted[5] = true;
        }
    }
    
    private void DrawCompleteStep()
    {
        GUILayout.Label("Setup Complete!", subHeaderStyle);
        
        EditorGUILayout.HelpBox(
            "Your Math Pinball game has been set up successfully! Here's what was done:\n\n" +
            (stepCompleted[1] ? "✓ " : "✗ ") + "Created UI Canvases\n" +
            (stepCompleted[2] ? "✓ " : "✗ ") + "Set up Physics Components\n" +
            (stepCompleted[3] ? "✓ " : "✗ ") + "Connected Game Managers\n" +
            (stepCompleted[4] ? "✓ " : "✗ ") + "Configured UI Connections\n" +
            (stepCompleted[5] ? "✓ " : "✗ ") + "Created Levels\n\n" +
            "You're now ready to play your Math Pinball game!",
            MessageType.Info);
            
        GUILayout.Space(20);
        
        EditorGUILayout.HelpBox(
            "Next Steps:\n\n" +
            "1. Create or import sound effects and music\n" +
            "2. Design additional levels\n" +
            "3. Test and balance gameplay\n" +
            "4. Add visual polish with more particle effects",
            MessageType.Info);
    }
    
    #endregion
    
    #region Implementation Methods
    
    private void CreateUICanvases()
    {
        try
        {
            // Create main menu canvas
            GameObject mainMenuCanvas = new GameObject("MainMenuCanvas");
            Canvas mainMenuCanvasComponent = mainMenuCanvas.AddComponent<Canvas>();
            mainMenuCanvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            mainMenuCanvasComponent.sortingOrder = 0;
            
            CanvasScaler mainMenuScaler = mainMenuCanvas.AddComponent<CanvasScaler>();
            mainMenuScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            mainMenuScaler.referenceResolution = new Vector2(1920, 1080);
            mainMenuScaler.matchWidthOrHeight = 0.5f;
            
            mainMenuCanvas.AddComponent<GraphicRaycaster>();
            
            // Create main menu panel
            GameObject mainMenuPanel = new GameObject("Panel");
            mainMenuPanel.transform.SetParent(mainMenuCanvas.transform, false);
            Image mainMenuPanelImage = mainMenuPanel.AddComponent<Image>();
            mainMenuPanelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            RectTransform mainMenuPanelRect = mainMenuPanel.GetComponent<RectTransform>();
            mainMenuPanelRect.anchorMin = Vector2.zero;
            mainMenuPanelRect.anchorMax = Vector2.one;
            mainMenuPanelRect.offsetMin = Vector2.zero;
            mainMenuPanelRect.offsetMax = Vector2.zero;
            
            // Create title text
            GameObject titleText = new GameObject("TitleText");
            titleText.transform.SetParent(mainMenuPanel.transform, false);
            TextMeshProUGUI titleTextComponent = titleText.AddComponent<TextMeshProUGUI>();
            titleTextComponent.text = "MATH PINBALL";
            titleTextComponent.fontSize = 72;
            titleTextComponent.alignment = TextAlignmentOptions.Center;
            titleTextComponent.color = Color.white;
            
            RectTransform titleTextRect = titleText.GetComponent<RectTransform>();
            titleTextRect.anchorMin = new Vector2(0.5f, 0.8f);
            titleTextRect.anchorMax = new Vector2(0.5f, 0.9f);
            titleTextRect.sizeDelta = new Vector2(600, 100);
            titleTextRect.anchoredPosition = Vector2.zero;
            
            // Create level selection canvas
            GameObject levelSelectionCanvas = new GameObject("LevelSelectionCanvas");
            Canvas levelSelectionCanvasComponent = levelSelectionCanvas.AddComponent<Canvas>();
            levelSelectionCanvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            levelSelectionCanvasComponent.sortingOrder = 1;
            
            CanvasScaler levelSelectionScaler = levelSelectionCanvas.AddComponent<CanvasScaler>();
            levelSelectionScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            levelSelectionScaler.referenceResolution = new Vector2(1920, 1080);
            levelSelectionScaler.matchWidthOrHeight = 0.5f;
            
            levelSelectionCanvas.AddComponent<GraphicRaycaster>();
            
            // Initially hide level selection canvas
            levelSelectionCanvas.SetActive(false);
            
            // Create level selection panel
            GameObject levelSelectionPanel = new GameObject("Panel");
            levelSelectionPanel.transform.SetParent(levelSelectionCanvas.transform, false);
            Image levelSelectionPanelImage = levelSelectionPanel.AddComponent<Image>();
            levelSelectionPanelImage.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);
            
            RectTransform levelSelectionPanelRect = levelSelectionPanel.GetComponent<RectTransform>();
            levelSelectionPanelRect.anchorMin = Vector2.zero;
            levelSelectionPanelRect.anchorMax = Vector2.one;
            levelSelectionPanelRect.offsetMin = Vector2.zero;
            levelSelectionPanelRect.offsetMax = Vector2.zero;
            
            // Create level selection title
            GameObject levelSelectionTitle = new GameObject("TitleText");
            levelSelectionTitle.transform.SetParent(levelSelectionPanel.transform, false);
            TextMeshProUGUI levelSelectionTitleComponent = levelSelectionTitle.AddComponent<TextMeshProUGUI>();
            levelSelectionTitleComponent.text = "SELECT LEVEL";
            levelSelectionTitleComponent.fontSize = 60;
            levelSelectionTitleComponent.alignment = TextAlignmentOptions.Center;
            levelSelectionTitleComponent.color = Color.white;
            
            RectTransform levelSelectionTitleRect = levelSelectionTitle.GetComponent<RectTransform>();
            levelSelectionTitleRect.anchorMin = new Vector2(0.5f, 0.85f);
            levelSelectionTitleRect.anchorMax = new Vector2(0.5f, 0.95f);
            levelSelectionTitleRect.sizeDelta = new Vector2(600, 100);
            levelSelectionTitleRect.anchoredPosition = Vector2.zero;
            
            // Create UI Navigation Controller
            GameObject uiController = new GameObject("UINavigationController");
            UINavigationController uiControllerComponent = uiController.AddComponent<UINavigationController>();
            
            // Set references in the controller
            uiControllerComponent.mainMenuCanvas = mainMenuCanvas;
            uiControllerComponent.levelSelectionCanvas = levelSelectionCanvas;
            
            // Create play button
            GameObject playButton = new GameObject("PlayButton");
            playButton.transform.SetParent(mainMenuPanel.transform, false);
            
            Image playButtonImage = playButton.AddComponent<Image>();
            playButtonImage.color = mainColor;
            
            Button playButtonComponent = playButton.AddComponent<Button>();
            ColorBlock playButtonColors = playButtonComponent.colors;
            playButtonColors.normalColor = mainColor;
            playButtonColors.highlightedColor = new Color(mainColor.r * 1.2f, mainColor.g * 1.2f, mainColor.b * 1.2f, 1f);
            playButtonColors.pressedColor = new Color(mainColor.r * 0.8f, mainColor.g * 0.8f, mainColor.b * 0.8f, 1f);
            playButtonComponent.colors = playButtonColors;
            
            RectTransform playButtonRect = playButton.GetComponent<RectTransform>();
            playButtonRect.anchorMin = new Vector2(0.5f, 0.5f);
            playButtonRect.anchorMax = new Vector2(0.5f, 0.5f);
            playButtonRect.sizeDelta = new Vector2(200, 60);
            playButtonRect.anchoredPosition = Vector2.zero;
            
            // Create play button text
            GameObject playButtonText = new GameObject("Text");
            playButtonText.transform.SetParent(playButton.transform, false);
            TextMeshProUGUI playButtonTextComponent = playButtonText.AddComponent<TextMeshProUGUI>();
            playButtonTextComponent.text = "PLAY";
            playButtonTextComponent.fontSize = 36;
            playButtonTextComponent.alignment = TextAlignmentOptions.Center;
            playButtonTextComponent.color = Color.white;
            
            RectTransform playButtonTextRect = playButtonText.GetComponent<RectTransform>();
            playButtonTextRect.anchorMin = Vector2.zero;
            playButtonTextRect.anchorMax = Vector2.one;
            playButtonTextRect.offsetMin = Vector2.zero;
            playButtonTextRect.offsetMax = Vector2.zero;
            
            // Add direct button handler to the play button
            DirectPlayButtonHandler playButtonHandler = playButton.AddComponent<DirectPlayButtonHandler>();
            playButtonHandler.mainMenuCanvas = mainMenuCanvas;
            playButtonHandler.levelSelectionCanvas = levelSelectionCanvas;
            
            // Create back button
            GameObject backButton = new GameObject("BackButton");
            backButton.transform.SetParent(levelSelectionPanel.transform, false);
            
            Image backButtonImage = backButton.AddComponent<Image>();
            backButtonImage.color = mainColor;
            
            Button backButtonComponent = backButton.AddComponent<Button>();
            ColorBlock backButtonColors = backButtonComponent.colors;
            backButtonColors.normalColor = mainColor;
            backButtonColors.highlightedColor = new Color(mainColor.r * 1.2f, mainColor.g * 1.2f, mainColor.b * 1.2f, 1f);
            backButtonColors.pressedColor = new Color(mainColor.r * 0.8f, mainColor.g * 0.8f, mainColor.b * 0.8f, 1f);
            backButtonComponent.colors = backButtonColors;
            
            RectTransform backButtonRect = backButton.GetComponent<RectTransform>();
            backButtonRect.anchorMin = new Vector2(0, 0);
            backButtonRect.anchorMax = new Vector2(0, 0);
            backButtonRect.sizeDelta = new Vector2(150, 50);
            backButtonRect.anchoredPosition = new Vector2(100, 40);
            
            // Create back button text
            GameObject backButtonText = new GameObject("Text");
            backButtonText.transform.SetParent(backButton.transform, false);
            TextMeshProUGUI backButtonTextComponent = backButtonText.AddComponent<TextMeshProUGUI>();
            backButtonTextComponent.text = "BACK";
            backButtonTextComponent.fontSize = 24;
            backButtonTextComponent.alignment = TextAlignmentOptions.Center;
            backButtonTextComponent.color = Color.white;
            
            RectTransform backButtonTextRect = backButtonText.GetComponent<RectTransform>();
            backButtonTextRect.anchorMin = Vector2.zero;
            backButtonTextRect.anchorMax = Vector2.one;
            backButtonTextRect.offsetMin = Vector2.zero;
            backButtonTextRect.offsetMax = Vector2.zero;
            
            // Add direct button handler to the back button
            DirectBackButtonHandler backButtonHandler = backButton.AddComponent<DirectBackButtonHandler>();
            backButtonHandler.mainMenuCanvas = mainMenuCanvas;
            backButtonHandler.levelSelectionCanvas = levelSelectionCanvas;
            
            // Create level buttons
            for (int i = 0; i < 3; i++)
            {
                int levelIndex = i + 1;
                GameObject levelButton = new GameObject("Level" + levelIndex + "Button");
                levelButton.transform.SetParent(levelSelectionPanel.transform, false);
                
                Image levelButtonImage = levelButton.AddComponent<Image>();
                levelButtonImage.color = mainColor;
                
                Button levelButtonComponent = levelButton.AddComponent<Button>();
                ColorBlock levelButtonColors = levelButtonComponent.colors;
                levelButtonColors.normalColor = mainColor;
                levelButtonColors.highlightedColor = new Color(mainColor.r * 1.2f, mainColor.g * 1.2f, mainColor.b * 1.2f, 1f);
                levelButtonColors.pressedColor = new Color(mainColor.r * 0.8f, mainColor.g * 0.8f, mainColor.b * 0.8f, 1f);
                levelButtonComponent.colors = levelButtonColors;
                
                RectTransform levelButtonRect = levelButton.GetComponent<RectTransform>();
                levelButtonRect.anchorMin = new Vector2(0.5f, 0.5f);
                levelButtonRect.anchorMax = new Vector2(0.5f, 0.5f);
                levelButtonRect.sizeDelta = new Vector2(150, 150);
                levelButtonRect.anchoredPosition = new Vector2((i-1) * 200, 0);
                
                // Create level button text
                GameObject levelButtonText = new GameObject("Text");
                levelButtonText.transform.SetParent(levelButton.transform, false);
                TextMeshProUGUI levelButtonTextComponent = levelButtonText.AddComponent<TextMeshProUGUI>();
                levelButtonTextComponent.text = levelIndex.ToString();
                levelButtonTextComponent.fontSize = 48;
                levelButtonTextComponent.alignment = TextAlignmentOptions.Center;
                levelButtonTextComponent.color = Color.white;
                
                RectTransform levelButtonTextRect = levelButtonText.GetComponent<RectTransform>();
                levelButtonTextRect.anchorMin = Vector2.zero;
                levelButtonTextRect.anchorMax = Vector2.one;
                levelButtonTextRect.offsetMin = Vector2.zero;
                levelButtonTextRect.offsetMax = Vector2.zero;
                
                // Add direct button handler to the level button
                DirectLevelButtonHandler levelButtonHandler = levelButton.AddComponent<DirectLevelButtonHandler>();
                levelButtonHandler.levelIndex = levelIndex;
                levelButtonHandler.mainMenuCanvas = mainMenuCanvas;
                levelButtonHandler.levelSelectionCanvas = levelSelectionCanvas;
                levelButtonHandler.gameUICanvas = gameUICanvas;
            }
            
            // Create keyboard input handler
            GameObject keyboardHandler = new GameObject("KeyboardInputHandler");
            KeyboardInputHandler keyboardInputComponent = keyboardHandler.AddComponent<KeyboardInputHandler>();
            keyboardInputComponent.mainMenuCanvas = mainMenuCanvas;
            keyboardInputComponent.levelSelectionCanvas = levelSelectionCanvas;
            
            // Store references to canvases
            this.mainMenuCanvas = mainMenuCanvas;
            this.levelSelectionCanvas = levelSelectionCanvas;
            
            // Create game UI canvas if needed
            if (createGameUI)
            {
                GameObject gameUICanvas = new GameObject("GameUICanvas");
                Canvas gameUICanvasComponent = gameUICanvas.AddComponent<Canvas>();
                gameUICanvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
                gameUICanvasComponent.sortingOrder = 2;
                
                CanvasScaler gameUIScaler = gameUICanvas.AddComponent<CanvasScaler>();
                gameUIScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                gameUIScaler.referenceResolution = new Vector2(1920, 1080);
                gameUIScaler.matchWidthOrHeight = 0.5f;
                
                gameUICanvas.AddComponent<GraphicRaycaster>();
                
                // Initially hide game UI canvas
                gameUICanvas.SetActive(false);
                
                // Create game UI panel
                GameObject gameUIPanel = new GameObject("Panel");
                gameUIPanel.transform.SetParent(gameUICanvas.transform, false);
                Image gameUIPanelImage = gameUIPanel.AddComponent<Image>();
                gameUIPanelImage.color = new Color(0, 0, 0, 0);
                
                RectTransform gameUIPanelRect = gameUIPanel.GetComponent<RectTransform>();
                gameUIPanelRect.anchorMin = Vector2.zero;
                gameUIPanelRect.anchorMax = Vector2.one;
                gameUIPanelRect.offsetMin = Vector2.zero;
                gameUIPanelRect.offsetMax = Vector2.zero;
                
                // Create score text
                GameObject scoreText = new GameObject("ScoreText");
                scoreText.transform.SetParent(gameUIPanel.transform, false);
                TextMeshProUGUI scoreTextComponent = scoreText.AddComponent<TextMeshProUGUI>();
                scoreTextComponent.text = "Score: 0";
                scoreTextComponent.fontSize = 36;
                scoreTextComponent.alignment = TextAlignmentOptions.Left;
                scoreTextComponent.color = Color.white;
                
                RectTransform scoreTextRect = scoreText.GetComponent<RectTransform>();
                scoreTextRect.anchorMin = new Vector2(0, 1);
                scoreTextRect.anchorMax = new Vector2(0, 1);
                scoreTextRect.sizeDelta = new Vector2(300, 50);
                scoreTextRect.anchoredPosition = new Vector2(20, -20);
                
                // Create equation text
                GameObject equationText = new GameObject("EquationText");
                equationText.transform.SetParent(gameUIPanel.transform, false);
                TextMeshProUGUI equationTextComponent = equationText.AddComponent<TextMeshProUGUI>();
                equationTextComponent.text = "";
                equationTextComponent.fontSize = 48;
                equationTextComponent.alignment = TextAlignmentOptions.Center;
                equationTextComponent.color = Color.white;
                
                RectTransform equationTextRect = equationText.GetComponent<RectTransform>();
                equationTextRect.anchorMin = new Vector2(0.2f, 0.8f);
                equationTextRect.anchorMax = new Vector2(0.8f, 0.9f);
                equationTextRect.sizeDelta = Vector2.zero;
                equationTextRect.anchoredPosition = Vector2.zero;
                
                // Create result text
                GameObject resultText = new GameObject("ResultText");
                resultText.transform.SetParent(gameUIPanel.transform, false);
                TextMeshProUGUI resultTextComponent = resultText.AddComponent<TextMeshProUGUI>();
                resultTextComponent.text = "";
                resultTextComponent.fontSize = 48;
                resultTextComponent.alignment = TextAlignmentOptions.Center;
                resultTextComponent.color = Color.white;
                
                RectTransform resultTextRect = resultText.GetComponent<RectTransform>();
                resultTextRect.anchorMin = new Vector2(0.2f, 0.7f);
                resultTextRect.anchorMax = new Vector2(0.8f, 0.8f);
                resultTextRect.sizeDelta = Vector2.zero;
                resultTextRect.anchoredPosition = Vector2.zero;
                
                // Store reference to game UI canvas
                this.gameUICanvas = gameUICanvas;
                uiControllerComponent.gameUICanvas = gameUICanvas;
                keyboardInputComponent.gameUICanvas = gameUICanvas;
            }
            
            // Create pause menu canvas if needed
            if (createPauseMenu)
            {
                GameObject pauseMenuCanvas = new GameObject("PauseMenuCanvas");
                Canvas pauseMenuCanvasComponent = pauseMenuCanvas.AddComponent<Canvas>();
                pauseMenuCanvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
                pauseMenuCanvasComponent.sortingOrder = 10;
                
                CanvasScaler pauseMenuScaler = pauseMenuCanvas.AddComponent<CanvasScaler>();
                pauseMenuScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                pauseMenuScaler.referenceResolution = new Vector2(1920, 1080);
                pauseMenuScaler.matchWidthOrHeight = 0.5f;
                
                pauseMenuCanvas.AddComponent<GraphicRaycaster>();
                
                // Initially hide pause menu canvas
                pauseMenuCanvas.SetActive(false);
                
                // Create pause menu panel
                GameObject pauseMenuPanel = new GameObject("Panel");
                pauseMenuPanel.transform.SetParent(pauseMenuCanvas.transform, false);
                Image pauseMenuPanelImage = pauseMenuPanel.AddComponent<Image>();
                pauseMenuPanelImage.color = new Color(0, 0, 0, 0.9f);
                
                RectTransform pauseMenuPanelRect = pauseMenuPanel.GetComponent<RectTransform>();
                pauseMenuPanelRect.anchorMin = Vector2.zero;
                pauseMenuPanelRect.anchorMax = Vector2.one;
                pauseMenuPanelRect.offsetMin = Vector2.zero;
                pauseMenuPanelRect.offsetMax = Vector2.zero;
                
                // Create pause menu title
                GameObject pauseMenuTitle = new GameObject("PausedText");
                pauseMenuTitle.transform.SetParent(pauseMenuPanel.transform, false);
                TextMeshProUGUI pauseMenuTitleComponent = pauseMenuTitle.AddComponent<TextMeshProUGUI>();
                pauseMenuTitleComponent.text = "PAUSED";
                pauseMenuTitleComponent.fontSize = 60;
                pauseMenuTitleComponent.alignment = TextAlignmentOptions.Center;
                pauseMenuTitleComponent.color = Color.white;
                
                RectTransform pauseMenuTitleRect = pauseMenuTitle.GetComponent<RectTransform>();
                pauseMenuTitleRect.anchorMin = new Vector2(0, 0.85f);
                pauseMenuTitleRect.anchorMax = new Vector2(1, 0.95f);
                pauseMenuTitleRect.sizeDelta = Vector2.zero;
                pauseMenuTitleRect.anchoredPosition = Vector2.zero;
                
                // Create resume button
                GameObject resumeButton = new GameObject("ResumeButton");
                resumeButton.transform.SetParent(pauseMenuPanel.transform, false);
                
                Image resumeButtonImage = resumeButton.AddComponent<Image>();
                resumeButtonImage.color = mainColor;
                
                Button resumeButtonComponent = resumeButton.AddComponent<Button>();
                ColorBlock resumeButtonColors = resumeButtonComponent.colors;
                resumeButtonColors.normalColor = mainColor;
                resumeButtonColors.highlightedColor = new Color(mainColor.r * 1.2f, mainColor.g * 1.2f, mainColor.b * 1.2f, 1f);
                resumeButtonColors.pressedColor = new Color(mainColor.r * 0.8f, mainColor.g * 0.8f, mainColor.b * 0.8f, 1f);
                resumeButtonComponent.colors = resumeButtonColors;
                
                RectTransform resumeButtonRect = resumeButton.GetComponent<RectTransform>();
                resumeButtonRect.anchorMin = new Vector2(0.5f, 0.5f);
                resumeButtonRect.anchorMax = new Vector2(0.5f, 0.5f);
                resumeButtonRect.sizeDelta = new Vector2(200, 60);
                resumeButtonRect.anchoredPosition = Vector2.zero;
                
                // Create resume button text
                GameObject resumeButtonText = new GameObject("Text");
                resumeButtonText.transform.SetParent(resumeButton.transform, false);
                TextMeshProUGUI resumeButtonTextComponent = resumeButtonText.AddComponent<TextMeshProUGUI>();
                resumeButtonTextComponent.text = "RESUME";
                resumeButtonTextComponent.fontSize = 36;
                resumeButtonTextComponent.alignment = TextAlignmentOptions.Center;
                resumeButtonTextComponent.color = Color.white;
                
                RectTransform resumeButtonTextRect = resumeButtonText.GetComponent<RectTransform>();
                resumeButtonTextRect.anchorMin = Vector2.zero;
                resumeButtonTextRect.anchorMax = Vector2.one;
                resumeButtonTextRect.offsetMin = Vector2.zero;
                resumeButtonTextRect.offsetMax = Vector2.zero;
                
                // Add direct button handler to the resume button
                DirectResumeButtonHandler resumeButtonHandler = resumeButton.AddComponent<DirectResumeButtonHandler>();
                resumeButtonHandler.pauseMenuCanvas = pauseMenuCanvas;
                
                // Store reference to pause menu canvas
                this.pauseMenuCanvas = pauseMenuCanvas;
                uiControllerComponent.pauseMenuCanvas = pauseMenuCanvas;
                keyboardInputComponent.pauseMenuCanvas = pauseMenuCanvas;
            }
            
            // Display success message
            EditorUtility.DisplayDialog("UI Canvases Created", 
                "UI canvases have been created successfully! The buttons have been set up with direct handlers and keyboard support.", "OK");
                
            stepCompleted[(int)SetupStep.CreateUICanvases] = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error creating UI canvases: " + ex.Message);
            EditorUtility.DisplayDialog("Error", 
                "An error occurred while creating UI canvases: " + ex.Message, "OK");
        }
    }
    
    private void CreatePhysicsMaterialsForGame()
    {
        // Call the static method directly
        CreatePhysicsMaterials.CreateAllPhysicsMaterials();
    }
    
    private void SetupPhysicsComponents()
    {
        // Call the public static method in PhysicsSetupTool
        PhysicsSetupTool.SetupPhysicsComponentsExternal(
            setupNumberBalls,
            setupOperatorBalls,
            setupBoundaries,
            setupPlayerBall
        );
        
        EditorUtility.DisplayDialog("Physics Setup Complete", 
            "Physics components have been set up successfully!", "OK");
    }
    
    private void ConnectManagers()
    {
        // Create GameManager
        GameObject gameManagerObj = new GameObject("GameManager");
        GameManager gameManager = gameManagerObj.AddComponent<GameManager>();
        gameManagerObj.AddComponent<AudioSource>(); // Add AudioSource for music
        
        // Create EquationManager
        GameObject equationManagerObj = new GameObject("EquationManager");
        EquationManager equationManager = equationManagerObj.AddComponent<EquationManager>();
        equationManagerObj.AddComponent<AudioSource>(); // Add AudioSource for equation sounds
        
        // Create VisualEffectsManager
        GameObject visualEffectsManagerObj = new GameObject("VisualEffectsManager");
        VisualEffectsManager visualEffectsManager = visualEffectsManagerObj.AddComponent<VisualEffectsManager>();
        
        // Create a parent object for all managers
        GameObject managersObj = new GameObject("Managers");
        gameManagerObj.transform.parent = managersObj.transform;
        equationManagerObj.transform.parent = managersObj.transform;
        visualEffectsManagerObj.transform.parent = managersObj.transform;
        
        // Configure GameManager
        gameManager.totalLevels = 10; // Default value, can be changed in inspector
        
        // Save references for the next step
        gameManagerPrefab = gameManagerObj;
        equationManagerPrefab = equationManagerObj;
        visualEffectsManagerPrefab = visualEffectsManagerObj;
        
        EditorUtility.DisplayDialog("Managers Connected", 
            "Game managers have been created and connected successfully!", "OK");
    }
    
    private void ConfigureUIConnections()
    {
        // Find GameUI component or add it
        GameUI gameUI = FindFirstObjectByType<GameUI>();
        if (gameUI == null && gameUICanvas != null)
        {
            gameUI = gameUICanvas.AddComponent<GameUI>();
        }
        
        if (gameUI == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "Game UI Canvas not found. Please create it first.", "OK");
            return;
        }
        
        // Find EquationManager
        EquationManager equationManager = FindFirstObjectByType<EquationManager>();
        if (equationManager == null)
        {
            EditorUtility.DisplayDialog("Error", 
                "EquationManager not found. Please create it first.", "OK");
            return;
        }
        
        // Connect UI elements to GameUI
        if (gameUICanvas != null)
        {
            // Find UI elements in the Game UI Canvas
            Transform scoreTextTrans = FindChildRecursive(gameUICanvas.transform, "ScoreText");
            Transform targetScoreTextTrans = FindChildRecursive(gameUICanvas.transform, "TargetScoreText");
            Transform equationTextTrans = FindChildRecursive(gameUICanvas.transform, "EquationText");
            Transform resultTextTrans = FindChildRecursive(gameUICanvas.transform, "ResultText");
            Transform progressBarTrans = FindChildRecursive(gameUICanvas.transform, "ProgressBar");
            
            // Connect found elements
            if (scoreTextTrans != null)
                gameUI.scoreText = scoreTextTrans.GetComponent<TextMeshProUGUI>();
                
            if (targetScoreTextTrans != null)
                gameUI.targetScoreText = targetScoreTextTrans.GetComponent<TextMeshProUGUI>();
                
            if (equationTextTrans != null)
            {
                TextMeshProUGUI equationTextComponent = equationTextTrans.GetComponent<TextMeshProUGUI>();
                gameUI.equationText = equationTextComponent;
                equationManager.equationText = equationTextComponent;
            }
                
            if (resultTextTrans != null)
            {
                TextMeshProUGUI resultTextComponent = resultTextTrans.GetComponent<TextMeshProUGUI>();
                gameUI.resultText = resultTextComponent;
                equationManager.resultText = resultTextComponent;
            }
                
            if (progressBarTrans != null)
                gameUI.progressBar = progressBarTrans.GetComponent<Image>();
        }
        
        // Connect pause panel
        if (pauseMenuCanvas != null)
        {
            gameUI.pausePanel = pauseMenuCanvas;
        }
        
        // Find or create game state panels
        Transform levelCompletePanelTrans = FindChildRecursive(gameUICanvas.transform, "LevelCompletePanel");
        Transform gameOverPanelTrans = FindChildRecursive(gameUICanvas.transform, "GameOverPanel");
        
        if (levelCompletePanelTrans != null)
            gameUI.levelCompletePanel = levelCompletePanelTrans.gameObject;
            
        if (gameOverPanelTrans != null)
            gameUI.gameOverPanel = gameOverPanelTrans.gameObject;
        
        // Connect equation manager feedback panel
        Transform feedbackPanelTrans = FindChildRecursive(gameUICanvas.transform, "FeedbackPanel");
        if (feedbackPanelTrans != null)
        {
            equationManager.feedbackPanel = feedbackPanelTrans.gameObject;
            
            // Find feedback text and background
            Transform feedbackTextTrans = FindChildRecursive(feedbackPanelTrans, "FeedbackText");
            if (feedbackTextTrans != null)
                equationManager.feedbackText = feedbackTextTrans.GetComponent<TextMeshProUGUI>();
                
            Image feedbackBg = feedbackPanelTrans.GetComponent<Image>();
            if (feedbackBg != null)
                equationManager.feedbackBackground = feedbackBg;
        }
        
        EditorUtility.DisplayDialog("UI Connections Configured", 
            "UI elements have been connected to the appropriate scripts successfully!", "OK");
    }
    
    private void CreateLevels()
    {
        // Create the Scenes directory if it doesn't exist
        string scenesDirectory = "Assets/Scenes";
        if (!Directory.Exists(scenesDirectory))
        {
            Directory.CreateDirectory(scenesDirectory);
            AssetDatabase.Refresh();
        }
        
        // Create levels based on options
        if (createLevel1)
        {
            CreateLevelScene(1, level1NumberBalls, level1OperatorBalls, level1AdditionOnly, false, false);
        }
        
        if (createLevel2)
        {
            CreateLevelScene(2, level2NumberBalls, level2OperatorBalls, false, level2AddSubtract, false);
        }
        
        if (createLevel3)
        {
            CreateLevelScene(3, level3NumberBalls, level3OperatorBalls, false, false, level3AllOperations);
        }
        
        // Add scenes to build settings
        AddScenesToBuildSettings();
        
        EditorUtility.DisplayDialog("Levels Created", 
            "Level scenes have been created successfully and added to build settings!", "OK");
    }
    
    private void CreateLevelScene(int levelNumber, int numberBalls, int operatorBalls, 
                                 bool additionOnly, bool addSubtract, bool allOperations)
    {
        string levelName = "Level" + levelNumber;
        string scenePath = "Assets/Scenes/" + levelName + ".unity";
        
        // Check if scene already exists
        if (File.Exists(scenePath))
        {
            bool overwrite = EditorUtility.DisplayDialog("Scene Already Exists", 
                "The scene " + levelName + " already exists. Do you want to overwrite it?", 
                "Overwrite", "Skip");
                
            if (!overwrite)
            {
                Debug.Log("Skipping creation of " + levelName + " as it already exists.");
                return;
            }
        }
        
        // Create a new scene
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        
        // Create a main camera
        GameObject cameraObj = new GameObject("Main Camera");
        Camera camera = cameraObj.AddComponent<Camera>();
        cameraObj.tag = "MainCamera";
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
        camera.transform.position = new Vector3(0, 0, -10);
        
        // Create a directional light
        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50, -30, 0);
        
        // Create level root
        GameObject levelRoot = new GameObject(levelName + "Root");
        
        // Create level manager
        GameObject levelManagerObj = new GameObject("LevelManager");
        levelManagerObj.transform.SetParent(levelRoot.transform);
        
        // Create level generator
        GameObject levelGeneratorObj = new GameObject("LevelGenerator");
        levelGeneratorObj.transform.SetParent(levelRoot.transform);
        LevelGenerator levelGenerator = levelGeneratorObj.AddComponent<LevelGenerator>();
        
        // Create level configuration manager
        GameObject configManagerObj = new GameObject("LevelConfigurationManager");
        configManagerObj.transform.SetParent(levelRoot.transform);
        LevelConfigurationManager configManager = configManagerObj.AddComponent<LevelConfigurationManager>();
        
        // Create level object with settings
        GameObject levelObj = new GameObject(levelName);
        levelObj.transform.SetParent(levelRoot.transform);
        Level level = levelObj.AddComponent<Level>();
        
        // Configure level settings
        level.levelName = "Level " + levelNumber;
        level.levelNumber = levelNumber;
        level.numberBalls = numberBalls;
        level.operatorBalls = operatorBalls;
        level.additionOnly = additionOnly;
        level.addSubtract = addSubtract;
        level.allOperations = allOperations;
        
        // Configure level difficulty based on level number
        ConfigureLevelDifficulty(level, levelNumber);
        
        // Create UI Canvas
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(levelRoot.transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Create EventSystem
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.transform.SetParent(levelRoot.transform);
        eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        
        // Create Game UI
        GameObject gameUIObj = new GameObject("GameUI");
        gameUIObj.transform.SetParent(canvasObj.transform, false);
        GameUI gameUI = gameUIObj.AddComponent<GameUI>();
        
        // Create level text
        GameObject levelTextObj = new GameObject("LevelNameText");
        levelTextObj.transform.SetParent(canvasObj.transform, false);
        TMPro.TextMeshProUGUI levelText = levelTextObj.AddComponent<TMPro.TextMeshProUGUI>();
        levelText.text = "Level " + levelNumber;
        levelText.fontSize = 24;
        levelText.color = Color.white;
        levelText.alignment = TMPro.TextAlignmentOptions.Center;
        RectTransform levelTextRect = levelTextObj.GetComponent<RectTransform>();
        levelTextRect.anchorMin = new Vector2(0.5f, 1);
        levelTextRect.anchorMax = new Vector2(0.5f, 1);
        levelTextRect.pivot = new Vector2(0.5f, 1);
        levelTextRect.anchoredPosition = new Vector2(0, -20);
        levelTextRect.sizeDelta = new Vector2(200, 30);
        
        // Create boundaries
        CreateBoundaries(levelRoot);
        
        // Create player launcher
        CreatePlayerLauncher(levelRoot);
        
        // Save the scene
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("Created scene: " + levelName + " at " + scenePath);
    }
    
    private void ConfigureLevelDifficulty(Level level, int levelNumber)
    {
        // Configure level difficulty based on level number
        switch (levelNumber)
        {
            case 1:
                level.targetScore = 50;
                level.timeLimit = 90f;
                level.minNumberValue = 1;
                level.maxNumberValue = 5;
                level.ballSpeed = 8f;
                level.bounciness = 0.7f;
                level.obstacles = 0;
                level.movingObstacles = false;
                break;
                
            case 2:
                level.targetScore = 100;
                level.timeLimit = 75f;
                level.minNumberValue = 1;
                level.maxNumberValue = 9;
                level.ballSpeed = 10f;
                level.bounciness = 0.6f;
                level.obstacles = 3;
                level.movingObstacles = false;
                break;
                
            case 3:
                level.targetScore = 150;
                level.timeLimit = 60f;
                level.minNumberValue = 1;
                level.maxNumberValue = 9;
                level.ballSpeed = 12f;
                level.bounciness = 0.5f;
                level.obstacles = 5;
                level.movingObstacles = true;
                level.obstacleSpeed = 3f;
                break;
                
            default:
                // For any additional levels, increase difficulty
                level.targetScore = 150 + (levelNumber - 3) * 50;
                level.timeLimit = 60f - (levelNumber - 3) * 5;
                level.ballSpeed = 12f + (levelNumber - 3);
                level.bounciness = 0.5f - (levelNumber - 3) * 0.05f;
                level.obstacles = 5 + (levelNumber - 3) * 2;
                level.movingObstacles = true;
                level.obstacleSpeed = 3f + (levelNumber - 3) * 0.5f;
                break;
        }
    }
    
    private void CreateBoundaries(GameObject parent)
    {
        // Create walls
        CreateWall(parent, new Vector3(-9, 0, 0), new Vector3(1, 10, 1), "LeftWall");
        CreateWall(parent, new Vector3(9, 0, 0), new Vector3(1, 10, 1), "RightWall");
        CreateWall(parent, new Vector3(0, 5, 0), new Vector3(20, 1, 1), "TopWall");
        
        // Create dead zone
        GameObject deadZone = new GameObject("DeadZone");
        deadZone.transform.SetParent(parent.transform);
        deadZone.transform.position = new Vector3(0, -6, 0);
        BoxCollider deadZoneCollider = deadZone.AddComponent<BoxCollider>();
        deadZoneCollider.size = new Vector3(20, 1, 1);
        deadZoneCollider.isTrigger = true;
        
        // Add dead zone script if it exists
        System.Type deadZoneType = System.Type.GetType("DeadZone");
        if (deadZoneType != null)
        {
            deadZone.AddComponent(deadZoneType);
        }
    }
    
    private void CreateWall(GameObject parent, Vector3 position, Vector3 size, string name)
    {
        GameObject wall = new GameObject(name);
        wall.transform.SetParent(parent.transform);
        wall.transform.position = position;
        
        // Add collider
        BoxCollider wallCollider = wall.AddComponent<BoxCollider>();
        wallCollider.size = size;
        
        // Add renderer for visibility
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Cube);
        visual.transform.SetParent(wall.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = size;
        
        // Remove the primitive's collider as we already added one
        DestroyImmediate(visual.GetComponent<BoxCollider>());
    }
    
    private void CreatePlayerLauncher(GameObject parent)
    {
        // Create player ball
        GameObject playerBall = new GameObject("PlayerBall");
        playerBall.transform.SetParent(parent.transform);
        playerBall.transform.position = new Vector3(0, -4, 0);
        playerBall.tag = "Player";
        
        // Add sphere mesh
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.transform.SetParent(playerBall.transform);
        visual.transform.localPosition = Vector3.zero;
        visual.transform.localScale = Vector3.one;
        
        // Remove the primitive's collider as we'll add our own
        DestroyImmediate(visual.GetComponent<SphereCollider>());
        
        // Add rigidbody and collider
        Rigidbody rb = playerBall.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.mass = 1f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        SphereCollider collider = playerBall.AddComponent<SphereCollider>();
        collider.radius = 0.5f;
        
        // Create launcher
        GameObject launcher = new GameObject("PinballLauncher");
        launcher.transform.SetParent(parent.transform);
        launcher.transform.position = new Vector3(0, -5, 0);
        
        // Try to add launcher script if it exists
        System.Type launcherType = System.Type.GetType("PinballLauncher");
        if (launcherType != null)
        {
            launcher.AddComponent(launcherType);
        }
    }
    
    private void AddScenesToBuildSettings()
    {
        // Get current scenes in build settings
        var scenes = EditorBuildSettings.scenes;
        List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>(scenes);
        
        // Add main menu scene if it exists
        string mainMenuPath = "Assets/Scenes/MainMenu.unity";
        if (File.Exists(mainMenuPath))
        {
            AddSceneToBuildSettings(newScenes, mainMenuPath);
        }
        
        // Add level selection scene if it exists
        string levelSelectionPath = "Assets/Scenes/LevelSelection.unity";
        if (File.Exists(levelSelectionPath))
        {
            AddSceneToBuildSettings(newScenes, levelSelectionPath);
        }
        
        // Add level scenes
        for (int i = 1; i <= 3; i++)
        {
            string levelPath = "Assets/Scenes/Level" + i + ".unity";
            if (File.Exists(levelPath))
            {
                AddSceneToBuildSettings(newScenes, levelPath);
            }
        }
        
        // Update build settings
        EditorBuildSettings.scenes = newScenes.ToArray();
    }
    
    private void AddSceneToBuildSettings(List<EditorBuildSettingsScene> scenes, string scenePath)
    {
        // Check if scene is already in build settings
        bool sceneExists = false;
        foreach (var scene in scenes)
        {
            if (scene.path == scenePath)
            {
                sceneExists = true;
                break;
            }
        }
        
        if (!sceneExists)
        {
            // Add the new scene to build settings
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            Debug.Log("Added scene to build settings: " + scenePath);
        }
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
    
    #endregion
}
#endif
