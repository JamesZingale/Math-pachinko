using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.IO;

public static class LevelSelectionCanvasCreator
{
    public static GameObject CreateLevelSelectionCanvas()
    {
        // Create Canvas
        GameObject canvas = UICreationHelpers.CreateBaseCanvas("LevelSelectionCanvas");
        
        // Create Level Selection Panel
        GameObject levelSelectionPanel = UICreationHelpers.CreatePanel(canvas.transform, "LevelSelectionPanel", UICreationHelpers.panelColor);
        UICreationHelpers.AddCanvasGroup(levelSelectionPanel);
        
        // Create Title
        GameObject titleObj = UICreationHelpers.CreateTextMeshPro(levelSelectionPanel.transform, "TitleText", "SELECT LEVEL", 48);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 250);
        
        // Create Grid Layout for Level Buttons
        GameObject gridObj = new GameObject("LevelButtonContainer", typeof(RectTransform));
        gridObj.transform.SetParent(levelSelectionPanel.transform, false);
        RectTransform gridRect = gridObj.GetComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.5f, 0.5f);
        gridRect.anchorMax = new Vector2(0.5f, 0.5f);
        gridRect.pivot = new Vector2(0.5f, 0.5f);
        gridRect.sizeDelta = new Vector2(700, 400);
        gridRect.anchoredPosition = new Vector2(0, 0);
        
        GridLayoutGroup grid = gridObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(200, 200);
        grid.spacing = new Vector2(20, 20);
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 3;
        
        // Create Level Button Prefab
        GameObject levelButtonPrefab = CreateLevelButtonPrefab();
        
        // Create Navigation Buttons
        GameObject backButton = UICreationHelpers.CreateButton(levelSelectionPanel.transform, "BackFromLevelsButton", "BACK", new Vector2(0, -250));
        GameObject prevPageButton = UICreationHelpers.CreateButton(levelSelectionPanel.transform, "PrevPageButton", "<", new Vector2(-400, 0));
        GameObject nextPageButton = UICreationHelpers.CreateButton(levelSelectionPanel.transform, "NextPageButton", ">", new Vector2(400, 0));
        
        RectTransform prevRect = prevPageButton.GetComponent<RectTransform>();
        prevRect.sizeDelta = new Vector2(80, 80);
        
        RectTransform nextRect = nextPageButton.GetComponent<RectTransform>();
        nextRect.sizeDelta = new Vector2(80, 80);
        
        // Add LevelSelectionUI component
        LevelSelectionUI levelSelectionUI = canvas.AddComponent<LevelSelectionUI>();
        levelSelectionUI.levelButtonPrefab = levelButtonPrefab;
        levelSelectionUI.levelButtonContainer = gridObj.transform;
        levelSelectionUI.prevPageButton = prevPageButton.GetComponent<Button>();
        levelSelectionUI.nextPageButton = nextPageButton.GetComponent<Button>();
        
        // Note: The back button needs to be connected to the MainMenuManager
        // We'll need to set this up manually or through another script
        
        Debug.Log("Level Selection Canvas created with all UI elements!");
        return canvas;
    }
    
    private static GameObject CreateLevelButtonPrefab()
    {
        // Create button
        GameObject buttonObj = new GameObject("LevelButtonPrefab", typeof(RectTransform));
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(180, 180);
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = UICreationHelpers.mainColor;
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = UICreationHelpers.mainColor;
        colors.highlightedColor = new Color(UICreationHelpers.mainColor.r + 0.1f, UICreationHelpers.mainColor.g + 0.1f, UICreationHelpers.mainColor.b + 0.1f, 1f);
        colors.pressedColor = new Color(UICreationHelpers.mainColor.r - 0.1f, UICreationHelpers.mainColor.g - 0.1f, UICreationHelpers.mainColor.b - 0.1f, 1f);
        button.colors = colors;
        
        // Level number
        GameObject textObj = UICreationHelpers.CreateTextMeshPro(buttonObj.transform, "LevelText", "1", 48);
        TextMeshProUGUI textComp = textObj.GetComponent<TextMeshProUGUI>();
        textComp.color = Color.white;
        textComp.fontStyle = FontStyles.Bold;
        
        // Lock icon
        GameObject lockObj = new GameObject("LockIcon", typeof(RectTransform));
        lockObj.transform.SetParent(buttonObj.transform, false);
        RectTransform lockRect = lockObj.GetComponent<RectTransform>();
        lockRect.sizeDelta = new Vector2(80, 80);
        lockRect.anchoredPosition = Vector2.zero;
        
        Image lockImage = lockObj.AddComponent<Image>();
        lockImage.color = Color.white;
        // You'll need to assign a lock sprite in the editor
        
        // Stars
        GameObject[] stars = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            stars[i] = new GameObject("Star" + (i + 1), typeof(RectTransform));
            stars[i].transform.SetParent(buttonObj.transform, false);
            RectTransform starRect = stars[i].GetComponent<RectTransform>();
            starRect.sizeDelta = new Vector2(40, 40);
            starRect.anchorMin = new Vector2(0.5f, 0);
            starRect.anchorMax = new Vector2(0.5f, 0);
            starRect.pivot = new Vector2(0.5f, 0);
            starRect.anchoredPosition = new Vector2(-40 + i * 40, 10);
            
            Image starImage = stars[i].AddComponent<Image>();
            starImage.color = UICreationHelpers.accentColor;
            // You'll need to assign a star sprite in the editor
        }
        
        // Save as prefab
        string prefabPath = "Assets/Prefabs/UI";
        if (!Directory.Exists(prefabPath))
        {
            Directory.CreateDirectory(prefabPath);
        }
        
        string fullPrefabPath = prefabPath + "/LevelButtonPrefab.prefab";
        
#if UNITY_2018_3_OR_NEWER
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(buttonObj, fullPrefabPath);
#else
        GameObject prefab = PrefabUtility.CreatePrefab(fullPrefabPath, buttonObj);
#endif
        
        Object.DestroyImmediate(buttonObj);
        return prefab;
    }
}
