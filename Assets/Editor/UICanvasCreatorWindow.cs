using UnityEngine;
using UnityEditor;
using System.IO;

public class UICanvasCreatorWindow : EditorWindow
{
    private Color mainColor = new Color(0.2f, 0.6f, 1f);
    private Color accentColor = new Color(1f, 0.5f, 0.2f);
    private bool createPrefabsFolder = true;
    private int numberOfLevels = 10;
    private bool showAdvancedSettings = false;
    
    [MenuItem("Math Pinball/Create UI Canvases")]
    public static void ShowWindow()
    {
        GetWindow<UICanvasCreatorWindow>("UI Canvas Creator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Math Pinball UI Canvas Creator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("UI Settings", EditorStyles.boldLabel);
        mainColor = EditorGUILayout.ColorField("Main Color", mainColor);
        accentColor = EditorGUILayout.ColorField("Accent Color", accentColor);
        numberOfLevels = EditorGUILayout.IntSlider("Number of Levels", numberOfLevels, 1, 20);
        createPrefabsFolder = EditorGUILayout.Toggle("Create Prefabs Folder", createPrefabsFolder);
        
        showAdvancedSettings = EditorGUILayout.Foldout(showAdvancedSettings, "Advanced Settings");
        if (showAdvancedSettings)
        {
            EditorGUI.indentLevel++;
            UICreationHelpers.panelColor = EditorGUILayout.ColorField("Panel Color", UICreationHelpers.panelColor);
            EditorGUI.indentLevel--;
        }
        
        EditorGUILayout.Space();
        
        // Apply settings to the helpers
        UICreationHelpers.mainColor = mainColor;
        UICreationHelpers.accentColor = accentColor;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Create Individual Canvases", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Main Menu Canvas", GUILayout.Height(30)))
        {
            CreatePrefabsFolderIfNeeded();
            MainMenuCanvasCreator.CreateMainMenuCanvas();
        }
        
        if (GUILayout.Button("Create Level Selection Canvas", GUILayout.Height(30)))
        {
            CreatePrefabsFolderIfNeeded();
            LevelSelectionCanvasCreator.CreateLevelSelectionCanvas();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Game UI Canvas", GUILayout.Height(30)))
        {
            GameUICanvasCreator.CreateGameUICanvas();
        }
        
        if (GUILayout.Button("Create Pause Menu Canvas", GUILayout.Height(30)))
        {
            PauseMenuCanvasCreator.CreatePauseMenuCanvas();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Create All Canvases", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Create All UI Canvases", GUILayout.Height(40)))
        {
            CreateAllCanvases();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("This tool creates UI canvases for your Math Pinball game. You can customize the colors and settings above before creating the canvases.", MessageType.Info);
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("After creating the canvases, you may need to:\n1. Assign sprites to buttons and icons\n2. Adjust positions of UI elements\n3. Connect any missing references", MessageType.Warning);
    }
    
    private void CreatePrefabsFolderIfNeeded()
    {
        if (createPrefabsFolder)
        {
            string prefabPath = "Assets/Prefabs/UI";
            if (!Directory.Exists(prefabPath))
            {
                Directory.CreateDirectory(prefabPath);
                AssetDatabase.Refresh();
            }
        }
    }
    
    private void CreateAllCanvases()
    {
        CreatePrefabsFolderIfNeeded();
        
        MainMenuCanvasCreator.CreateMainMenuCanvas();
        LevelSelectionCanvasCreator.CreateLevelSelectionCanvas();
        GameUICanvasCreator.CreateGameUICanvas();
        PauseMenuCanvasCreator.CreatePauseMenuCanvas();
        
        Debug.Log("All UI canvases created successfully!");
    }
}
