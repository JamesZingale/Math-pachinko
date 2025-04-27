using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public static class UICreationHelpers
{
    public static Color mainColor = new Color(0.2f, 0.6f, 1f);
    public static Color accentColor = new Color(1f, 0.5f, 0.2f);
    public static Color panelColor = new Color(0.1f, 0.1f, 0.2f, 0.9f);
    
    // Create a basic canvas setup
    public static GameObject CreateBaseCanvas(string name)
    {
        // Create the canvas object
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        // Add canvas scaler
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        // Add graphic raycaster
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create event system if needed
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        return canvasObj;
    }
    
    // Create a panel
    public static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        GameObject panelObj = new GameObject(name, typeof(RectTransform));
        panelObj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = panelObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.sizeDelta = Vector2.zero;
        
        Image image = panelObj.AddComponent<Image>();
        image.color = color;
        
        return panelObj;
    }
    
    // Create TextMeshPro text
    public static GameObject CreateTextMeshPro(Transform parent, string name, string text, int fontSize)
    {
        GameObject textObj = new GameObject(name, typeof(RectTransform));
        textObj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 50);
        rectTransform.anchoredPosition = Vector2.zero;
        
        TextMeshProUGUI textComponent = textObj.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.color = Color.white;
        
        return textObj;
    }
    
    // Create a button
    public static GameObject CreateButton(Transform parent, string name, string text, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name, typeof(RectTransform));
        buttonObj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(250, 60);
        rectTransform.anchoredPosition = position;
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = mainColor;
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.normalColor = mainColor;
        colors.highlightedColor = new Color(mainColor.r + 0.1f, mainColor.g + 0.1f, mainColor.b + 0.1f, 1f);
        colors.pressedColor = new Color(mainColor.r - 0.1f, mainColor.g - 0.1f, mainColor.b - 0.1f, 1f);
        button.colors = colors;
        
        GameObject textObj = CreateTextMeshPro(buttonObj.transform, "Text", text, 24);
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        return buttonObj;
    }
    
    // Create a slider
    public static GameObject CreateSlider(Transform parent, string name, Vector2 position)
    {
        GameObject sliderObj = new GameObject(name, typeof(RectTransform));
        sliderObj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = sliderObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 20);
        rectTransform.anchoredPosition = position;
        
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0.75f;
        
        // Background
        GameObject background = new GameObject("Background", typeof(RectTransform));
        background.transform.SetParent(sliderObj.transform, false);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // Fill Area
        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform fillRect = fillArea.GetComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0.5f);
        fillRect.anchorMax = new Vector2(1, 0.5f);
        fillRect.sizeDelta = new Vector2(-20, 10);
        
        // Fill
        GameObject fill = new GameObject("Fill", typeof(RectTransform));
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillImageRect = fill.GetComponent<RectTransform>();
        fillImageRect.anchorMin = Vector2.zero;
        fillImageRect.anchorMax = new Vector2(1, 1);
        fillImageRect.sizeDelta = Vector2.zero;
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = accentColor;
        
        // Handle
        GameObject handle = new GameObject("Handle", typeof(RectTransform));
        handle.transform.SetParent(sliderObj.transform, false);
        RectTransform handleRect = handle.GetComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(20, 30);
        
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = Color.white;
        
        // Setup references
        slider.fillRect = fillImageRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        
        return sliderObj;
    }
    
    // Create a toggle
    public static GameObject CreateToggle(Transform parent, string name, string text, Vector2 position)
    {
        GameObject toggleObj = new GameObject(name, typeof(RectTransform));
        toggleObj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = toggleObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 30);
        rectTransform.anchoredPosition = position;
        
        Toggle toggle = toggleObj.AddComponent<Toggle>();
        
        // Background
        GameObject background = new GameObject("Background", typeof(RectTransform));
        background.transform.SetParent(toggleObj.transform, false);
        RectTransform bgRect = background.GetComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(30, 30);
        bgRect.anchorMin = new Vector2(0, 0.5f);
        bgRect.anchorMax = new Vector2(0, 0.5f);
        bgRect.pivot = new Vector2(0, 0.5f);
        bgRect.anchoredPosition = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // Checkmark
        GameObject checkmark = new GameObject("Checkmark", typeof(RectTransform));
        checkmark.transform.SetParent(background.transform, false);
        RectTransform checkRect = checkmark.GetComponent<RectTransform>();
        checkRect.anchorMin = new Vector2(0.1f, 0.1f);
        checkRect.anchorMax = new Vector2(0.9f, 0.9f);
        checkRect.sizeDelta = Vector2.zero;
        
        Image checkImage = checkmark.AddComponent<Image>();
        checkImage.color = accentColor;
        
        // Label
        GameObject label = CreateTextMeshPro(toggleObj.transform, "Label", text, 18);
        RectTransform labelRect = label.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.5f);
        labelRect.anchorMax = new Vector2(0, 0.5f);
        labelRect.pivot = new Vector2(0, 0.5f);
        labelRect.anchoredPosition = new Vector2(40, 0);
        labelRect.sizeDelta = new Vector2(160, 30);
        TextMeshProUGUI labelText = label.GetComponent<TextMeshProUGUI>();
        labelText.alignment = TextAlignmentOptions.Left;
        
        // Setup references
        toggle.graphic = checkImage;
        toggle.targetGraphic = bgImage;
        
        return toggleObj;
    }
    
    // Add a CanvasGroup component
    public static void AddCanvasGroup(GameObject obj)
    {
        CanvasGroup group = obj.AddComponent<CanvasGroup>();
        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }
}
