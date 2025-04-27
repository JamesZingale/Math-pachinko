using UnityEngine;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class LevelSceneSetup : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Math Pinball/Setup Level Scenes")]
    public static void SetupLevelScenes()
    {
        // Create the Scenes directory if it doesn't exist
        string scenesDirectory = "Assets/Scenes";
        if (!Directory.Exists(scenesDirectory))
        {
            Directory.CreateDirectory(scenesDirectory);
            AssetDatabase.Refresh();
        }
        
        // Create level scenes
        CreateLevelScene(1);
        CreateLevelScene(2);
        CreateLevelScene(3);
        
        // Add scenes to build settings without removing existing ones
        AddScenesToBuildSettings();
        
        EditorUtility.DisplayDialog("Level Scenes Created", 
            "Level scenes have been created successfully and added to build settings!", "OK");
    }
    
    private static void CreateLevelScene(int levelNumber)
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
    
    private static void ConfigureLevelDifficulty(Level level, int levelNumber)
    {
        // Configure level difficulty based on level number
        switch (levelNumber)
        {
            case 1:
                level.targetScore = 50;
                level.timeLimit = 90f;
                level.numberBalls = 8;
                level.operatorBalls = 3;
                level.additionOnly = true;
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
                level.numberBalls = 10;
                level.operatorBalls = 5;
                level.addSubtract = true;
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
                level.numberBalls = 12;
                level.operatorBalls = 8;
                level.allOperations = true;
                level.minNumberValue = 1;
                level.maxNumberValue = 9;
                level.ballSpeed = 12f;
                level.bounciness = 0.5f;
                level.obstacles = 5;
                level.movingObstacles = true;
                level.obstacleSpeed = 3f;
                break;
        }
    }
    
    private static void CreateBoundaries(GameObject parent)
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
    }
    
    private static void CreateWall(GameObject parent, Vector3 position, Vector3 size, string name)
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
    
    private static void CreatePlayerLauncher(GameObject parent)
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
        launcher.AddComponent<PinballLauncher>();
    }
    
    private static void AddScenesToBuildSettings()
    {
        // Get current scenes in build settings
        var existingScenes = EditorBuildSettings.scenes;
        List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>(existingScenes);
        
        // Add level scenes if they don't already exist in build settings
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
    
    private static void AddSceneToBuildSettings(List<EditorBuildSettingsScene> scenes, string scenePath)
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
#endif
}
