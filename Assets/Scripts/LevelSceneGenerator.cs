using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelSceneGenerator : MonoBehaviour
{
    [Header("Level Settings")]
    public int numberOfLevels = 3;
    public bool generateOnStart = true;
    
    [Header("Level 1 - Easy")]
    public int level1NumberBalls = 8;
    public int level1OperatorBalls = 3;
    public bool level1IncludeAddition = true;
    public bool level1IncludeSubtraction = false;
    public bool level1IncludeMultiplication = false;
    public bool level1IncludeDivision = false;
    public int level1Obstacles = 0;
    
    [Header("Level 2 - Medium")]
    public int level2NumberBalls = 10;
    public int level2OperatorBalls = 5;
    public bool level2IncludeAddition = true;
    public bool level2IncludeSubtraction = true;
    public bool level2IncludeMultiplication = false;
    public bool level2IncludeDivision = false;
    public int level2Obstacles = 3;
    
    [Header("Level 3 - Hard")]
    public int level3NumberBalls = 12;
    public int level3OperatorBalls = 8;
    public bool level3IncludeAddition = true;
    public bool level3IncludeSubtraction = true;
    public bool level3IncludeMultiplication = true;
    public bool level3IncludeDivision = true;
    public int level3Obstacles = 5;
    public bool level3MovingObstacles = true;
    
    [Header("References")]
    public GameObject numberBallPrefab;
    public GameObject operatorBallPrefab;
    public GameObject obstaclePrefab;
    public GameObject movingObstaclePrefab;
    public GameObject playerBallPrefab;
    public GameObject launcherPrefab;
    
    private Dictionary<int, GameObject> levelRoots = new Dictionary<int, GameObject>();
    
    void Start()
    {
        if (generateOnStart)
        {
            StartCoroutine(GenerateLevels());
        }
    }
    
    public IEnumerator GenerateLevels()
    {
        Debug.Log("Generating levels...");
        
        // Create level configuration manager if it doesn't exist
        LevelConfigurationManager configManager = FindFirstObjectByType<LevelConfigurationManager>();
        if (configManager == null)
        {
            GameObject configManagerObj = new GameObject("LevelConfigurationManager");
            configManager = configManagerObj.AddComponent<LevelConfigurationManager>();
            DontDestroyOnLoad(configManagerObj);
        }
        
        // Create level roots
        for (int i = 1; i <= numberOfLevels; i++)
        {
            yield return StartCoroutine(GenerateLevel(i));
        }
        
        Debug.Log("All levels generated successfully!");
    }
    
    private IEnumerator GenerateLevel(int levelIndex)
    {
        Debug.Log("Generating Level " + levelIndex);
        
        // Create root game object for this level
        GameObject levelRoot = new GameObject("Level" + levelIndex);
        levelRoots[levelIndex] = levelRoot;
        
        // Configure level based on index
        ConfigureLevel(levelRoot, levelIndex);
        
        // Wait for one frame to ensure everything is set up
        yield return null;
        
        Debug.Log("Level " + levelIndex + " generated successfully!");
    }
    
    private void ConfigureLevel(GameObject levelRoot, int levelIndex)
    {
        // Add level generator component
        LevelGenerator levelGenerator = levelRoot.AddComponent<LevelGenerator>();
        
        // Create level configuration
        LevelConfiguration levelConfig = new LevelConfiguration();
        levelConfig.levelName = "Level " + levelIndex;
        
        // Configure based on level index
        switch (levelIndex)
        {
            case 1:
                ConfigureLevel1(levelConfig);
                break;
            case 2:
                ConfigureLevel2(levelConfig);
                break;
            case 3:
                ConfigureLevel3(levelConfig);
                break;
            default:
                // For any additional levels, use level 3 config with increased difficulty
                ConfigureLevel3(levelConfig);
                levelConfig.numberOfNumberBalls += (levelIndex - 3) * 2;
                levelConfig.numberOfOperatorBalls += (levelIndex - 3);
                levelConfig.numberOfObstacles += (levelIndex - 3);
                levelConfig.timeLimit -= (levelIndex - 3) * 5;
                levelConfig.targetScore += (levelIndex - 3) * 25;
                break;
        }
        
        // Assign configuration to level generator
        levelGenerator.levelConfig = levelConfig;
        
        // Add references to prefabs
        if (FindFirstObjectByType<MathBallFactory>() == null)
        {
            GameObject factoryObj = new GameObject("MathBallFactory");
            MathBallFactory factory = factoryObj.AddComponent<MathBallFactory>();
            
            // Set up number balls (0-9)
            for (int i = 0; i <= 9; i++)
            {
                MathBallFactory.BallPrefab ballPrefab = new MathBallFactory.BallPrefab();
                ballPrefab.value = i.ToString();
                ballPrefab.prefab = numberBallPrefab;
                factory.numberBalls.Add(ballPrefab);
            }
            
            // Set up operator balls (+, -, *, /)
            string[] operators = new string[] { "+", "-", "*", "/" };
            foreach (string op in operators)
            {
                MathBallFactory.BallPrefab ballPrefab = new MathBallFactory.BallPrefab();
                ballPrefab.value = op;
                ballPrefab.prefab = operatorBallPrefab;
                factory.operatorBalls.Add(ballPrefab);
            }
        }
        
        // Set obstacle prefabs
        levelGenerator.obstaclePrefab = obstaclePrefab;
        levelGenerator.movingObstaclePrefab = movingObstaclePrefab;
        
        // Add player ball and launcher
        if (playerBallPrefab != null)
        {
            GameObject playerBall = Instantiate(playerBallPrefab, new Vector3(0, -4, 0), Quaternion.identity);
            playerBall.transform.SetParent(levelRoot.transform);
        }
        
        if (launcherPrefab != null)
        {
            GameObject launcher = Instantiate(launcherPrefab, new Vector3(0, -5, 0), Quaternion.identity);
            launcher.transform.SetParent(levelRoot.transform);
        }
        
        // Add boundaries
        CreateBoundaries(levelRoot);
        
        // Add UI elements
        CreateUI(levelRoot, levelIndex);
    }
    
    private void ConfigureLevel1(LevelConfiguration config)
    {
        config.targetScore = 50;
        config.timeLimit = 90f;
        config.numberOfNumberBalls = level1NumberBalls;
        config.numberOfOperatorBalls = level1OperatorBalls;
        config.includeAddition = level1IncludeAddition;
        config.includeSubtraction = level1IncludeSubtraction;
        config.includeMultiplication = level1IncludeMultiplication;
        config.includeDivision = level1IncludeDivision;
        config.minNumberValue = 1;
        config.maxNumberValue = 5;
        config.ballSpeed = 8f;
        config.gravity = 9.8f;
        config.bounciness = 0.7f;
        config.numberOfObstacles = level1Obstacles;
        config.includeMovingObstacles = false;
    }
    
    private void ConfigureLevel2(LevelConfiguration config)
    {
        config.targetScore = 100;
        config.timeLimit = 75f;
        config.numberOfNumberBalls = level2NumberBalls;
        config.numberOfOperatorBalls = level2OperatorBalls;
        config.includeAddition = level2IncludeAddition;
        config.includeSubtraction = level2IncludeSubtraction;
        config.includeMultiplication = level2IncludeMultiplication;
        config.includeDivision = level2IncludeDivision;
        config.minNumberValue = 1;
        config.maxNumberValue = 9;
        config.ballSpeed = 10f;
        config.gravity = 9.8f;
        config.bounciness = 0.6f;
        config.numberOfObstacles = level2Obstacles;
        config.includeMovingObstacles = false;
    }
    
    private void ConfigureLevel3(LevelConfiguration config)
    {
        config.targetScore = 150;
        config.timeLimit = 60f;
        config.numberOfNumberBalls = level3NumberBalls;
        config.numberOfOperatorBalls = level3OperatorBalls;
        config.includeAddition = level3IncludeAddition;
        config.includeSubtraction = level3IncludeSubtraction;
        config.includeMultiplication = level3IncludeMultiplication;
        config.includeDivision = level3IncludeDivision;
        config.minNumberValue = 1;
        config.maxNumberValue = 9;
        config.ballSpeed = 12f;
        config.gravity = 10.5f;
        config.bounciness = 0.5f;
        config.numberOfObstacles = level3Obstacles;
        config.includeMovingObstacles = level3MovingObstacles;
        config.obstacleSpeed = 3f;
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
        
        // Add dead zone script
        deadZone.AddComponent<DeadZone>();
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
    
    private void CreateUI(GameObject parent, int levelIndex)
    {
        // Create UI Canvas
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(parent.transform);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Create EventSystem if it doesn't exist
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.transform.SetParent(parent.transform);
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Create Game UI
        GameObject gameUIObj = new GameObject("GameUI");
        gameUIObj.transform.SetParent(canvasObj.transform, false);
        GameUI gameUI = gameUIObj.AddComponent<GameUI>();
        
        // Create level text
        GameObject levelTextObj = new GameObject("LevelNameText");
        levelTextObj.transform.SetParent(canvasObj.transform, false);
        TMPro.TextMeshProUGUI levelText = levelTextObj.AddComponent<TMPro.TextMeshProUGUI>();
        levelText.text = "Level " + levelIndex;
        levelText.fontSize = 24;
        levelText.color = Color.white;
        levelText.alignment = TMPro.TextAlignmentOptions.Center;
        RectTransform levelTextRect = levelTextObj.GetComponent<RectTransform>();
        levelTextRect.anchorMin = new Vector2(0.5f, 1);
        levelTextRect.anchorMax = new Vector2(0.5f, 1);
        levelTextRect.pivot = new Vector2(0.5f, 1);
        levelTextRect.anchoredPosition = new Vector2(0, -20);
        levelTextRect.sizeDelta = new Vector2(200, 30);
    }
    
    public void LoadLevel(int levelIndex)
    {
        if (levelRoots.ContainsKey(levelIndex))
        {
            // Disable all other levels
            foreach (var kvp in levelRoots)
            {
                kvp.Value.SetActive(kvp.Key == levelIndex);
            }
            
            Debug.Log("Loaded Level " + levelIndex);
        }
        else
        {
            Debug.LogWarning("Level " + levelIndex + " has not been generated yet!");
        }
    }
    
    // This class can be added to detect when a ball falls out of the play area
    public class DeadZone : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // Check if this is the player ball
            if (other.CompareTag("Player"))
            {
                Debug.Log("Player ball entered dead zone");
                
                // Notify the game UI
                GameUI gameUI = FindFirstObjectByType<GameUI>();
                if (gameUI != null)
                {
                    gameUI.OnBallLost();
                }
                
                // Reset the ball position
                other.transform.position = new Vector3(0, -4, 0);
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            else
            {
                // For other objects like math balls, just destroy them
                Destroy(other.gameObject);
            }
        }
    }
}
