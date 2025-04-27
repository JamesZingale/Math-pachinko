using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class LevelSceneCreator : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Math Pinball/Create Level Scenes")]
    public static void CreateLevelScenes()
    {
        // Create 3 level scenes
        for (int i = 1; i <= 3; i++)
        {
            string sceneName = "Level" + i;
            string scenePath = "Assets/Scenes/" + sceneName + ".unity";
            
            // Check if scene already exists
            if (System.IO.File.Exists(scenePath))
            {
                Debug.Log("Scene " + sceneName + " already exists at " + scenePath);
                continue;
            }
            
            // Create a new scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // Add essential objects
            CreateEssentialObjects(scene, i);
            
            // Save the scene
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("Created scene: " + sceneName + " at " + scenePath);
            
            // Add to build settings if not already there
            AddSceneToBuildSettings(scenePath);
        }
        
        Debug.Log("Level scenes created successfully!");
    }
    
    private static void CreateEssentialObjects(UnityEngine.SceneManagement.Scene scene, int levelIndex)
    {
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
        
        // Create level manager
        GameObject levelManagerObj = new GameObject("LevelManager");
        
        // Create level generator
        GameObject levelGeneratorObj = new GameObject("LevelGenerator");
        LevelGenerator levelGenerator = levelGeneratorObj.AddComponent<LevelGenerator>();
        
        // Create level configuration manager if it doesn't exist
        LevelConfigurationManager configManager = FindObjectOfType<LevelConfigurationManager>();
        if (configManager == null)
        {
            GameObject configManagerObj = new GameObject("LevelConfigurationManager");
            configManager = configManagerObj.AddComponent<LevelConfigurationManager>();
        }
        
        // Set the current level
        configManager.SetLevel(levelIndex - 1);
        
        // Create UI Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Create EventSystem if it doesn't exist
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
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
        UnityEngine.UI.Text levelText = levelTextObj.AddComponent<UnityEngine.UI.Text>();
        levelText.text = "Level " + levelIndex;
        levelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        levelText.fontSize = 24;
        levelText.color = Color.white;
        levelText.alignment = TextAnchor.UpperCenter;
        RectTransform levelTextRect = levelTextObj.GetComponent<RectTransform>();
        levelTextRect.anchorMin = new Vector2(0.5f, 1);
        levelTextRect.anchorMax = new Vector2(0.5f, 1);
        levelTextRect.pivot = new Vector2(0.5f, 1);
        levelTextRect.anchoredPosition = new Vector2(0, -20);
        levelTextRect.sizeDelta = new Vector2(200, 30);
    }
    
    private static void AddSceneToBuildSettings(string scenePath)
    {
        // Get current scenes in build settings
        var scenes = EditorBuildSettings.scenes;
        bool sceneExists = false;
        
        // Check if scene is already in build settings
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
            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            System.Array.Copy(scenes, newScenes, scenes.Length);
            newScenes[scenes.Length] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = newScenes;
            Debug.Log("Added scene to build settings: " + scenePath);
        }
    }
#endif
}
