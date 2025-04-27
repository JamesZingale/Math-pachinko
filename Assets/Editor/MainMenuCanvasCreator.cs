using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public static class MainMenuCanvasCreator
{
    public static GameObject CreateMainMenuCanvas()
    {
        // Create Canvas
        GameObject canvas = UICreationHelpers.CreateBaseCanvas("MainMenuCanvas");
        
        // Create Main Menu Panel
        GameObject mainMenuPanel = UICreationHelpers.CreatePanel(canvas.transform, "MainMenuPanel", UICreationHelpers.panelColor);
        UICreationHelpers.AddCanvasGroup(mainMenuPanel);
        
        // Create Title
        GameObject titleObj = UICreationHelpers.CreateTextMeshPro(mainMenuPanel.transform, "TitleText", "MATH PINBALL", 72);
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 200);
        TextMeshProUGUI titleText = titleObj.GetComponent<TextMeshProUGUI>();
        titleText.color = UICreationHelpers.mainColor;
        titleText.fontStyle = FontStyles.Bold;
        
        // Create Buttons
        GameObject playButton = UICreationHelpers.CreateButton(mainMenuPanel.transform, "PlayButton", "PLAY", new Vector2(0, 50));
        GameObject optionsButton = UICreationHelpers.CreateButton(mainMenuPanel.transform, "OptionsButton", "OPTIONS", new Vector2(0, -30));
        GameObject creditsButton = UICreationHelpers.CreateButton(mainMenuPanel.transform, "CreditsButton", "CREDITS", new Vector2(0, -110));
        GameObject quitButton = UICreationHelpers.CreateButton(mainMenuPanel.transform, "QuitButton", "QUIT", new Vector2(0, -190));
        
        // Create Version Text
        GameObject versionObj = UICreationHelpers.CreateTextMeshPro(mainMenuPanel.transform, "VersionText", "v1.0", 16);
        RectTransform versionRect = versionObj.GetComponent<RectTransform>();
        versionRect.anchorMin = new Vector2(1, 0);
        versionRect.anchorMax = new Vector2(1, 0);
        versionRect.pivot = new Vector2(1, 0);
        versionRect.anchoredPosition = new Vector2(-20, 20);
        
        // Create Options Panel
        GameObject optionsPanel = CreateOptionsPanel(canvas.transform);
        
        // Create Credits Panel
        GameObject creditsPanel = CreateCreditsPanel(canvas.transform);
        
        // Add MainMenuManager component
        MainMenuManager menuManager = canvas.AddComponent<MainMenuManager>();
        
        // Set up references
        menuManager.mainMenuPanel = mainMenuPanel;
        menuManager.optionsPanel = optionsPanel;
        menuManager.creditsPanel = creditsPanel;
        menuManager.playButton = playButton.GetComponent<Button>();
        menuManager.optionsButton = optionsButton.GetComponent<Button>();
        menuManager.creditsButton = creditsButton.GetComponent<Button>();
        menuManager.quitButton = quitButton.GetComponent<Button>();
        menuManager.gameTitle = titleText;
        menuManager.versionText = versionObj.GetComponent<TextMeshProUGUI>();
        
        // Set up back buttons
        GameObject backFromOptionsButton = optionsPanel.transform.Find("BackFromOptionsButton").gameObject;
        GameObject backFromCreditsButton = creditsPanel.transform.Find("BackFromCreditsButton").gameObject;
        
        menuManager.backFromOptionsButton = backFromOptionsButton.GetComponent<Button>();
        menuManager.backFromCreditsButton = backFromCreditsButton.GetComponent<Button>();
        
        // Set up sliders
        GameObject musicSlider = optionsPanel.transform.Find("MusicVolumeSlider").gameObject;
        GameObject sfxSlider = optionsPanel.transform.Find("SFXVolumeSlider").gameObject;
        GameObject fullscreenToggle = optionsPanel.transform.Find("FullscreenToggle").gameObject;
        
        menuManager.musicVolumeSlider = musicSlider.GetComponent<Slider>();
        menuManager.sfxVolumeSlider = sfxSlider.GetComponent<Slider>();
        menuManager.fullscreenToggle = fullscreenToggle.GetComponent<Toggle>();
        
        Debug.Log("Main Menu Canvas created with all UI elements!");
        return canvas;
    }
    
    private static GameObject CreateOptionsPanel(Transform parent)
    {
        GameObject optionsPanel = UICreationHelpers.CreatePanel(parent, "OptionsPanel", UICreationHelpers.panelColor);
        UICreationHelpers.AddCanvasGroup(optionsPanel);
        optionsPanel.SetActive(false);
        
        // Options Panel Content
        GameObject optionsTitle = UICreationHelpers.CreateTextMeshPro(optionsPanel.transform, "OptionsTitle", "OPTIONS", 48);
        RectTransform optionsTitleRect = optionsTitle.GetComponent<RectTransform>();
        optionsTitleRect.anchoredPosition = new Vector2(0, 200);
        
        // Music Slider
        GameObject musicLabel = UICreationHelpers.CreateTextMeshPro(optionsPanel.transform, "MusicLabel", "Music Volume", 24);
        RectTransform musicLabelRect = musicLabel.GetComponent<RectTransform>();
        musicLabelRect.anchoredPosition = new Vector2(-150, 100);
        
        GameObject musicSlider = UICreationHelpers.CreateSlider(optionsPanel.transform, "MusicVolumeSlider", new Vector2(100, 100));
        
        // SFX Slider
        GameObject sfxLabel = UICreationHelpers.CreateTextMeshPro(optionsPanel.transform, "SFXLabel", "SFX Volume", 24);
        RectTransform sfxLabelRect = sfxLabel.GetComponent<RectTransform>();
        sfxLabelRect.anchoredPosition = new Vector2(-150, 40);
        
        GameObject sfxSlider = UICreationHelpers.CreateSlider(optionsPanel.transform, "SFXVolumeSlider", new Vector2(100, 40));
        
        // Fullscreen Toggle
        GameObject fullscreenToggle = UICreationHelpers.CreateToggle(optionsPanel.transform, "FullscreenToggle", "Fullscreen", new Vector2(0, -20));
        
        // Back Button
        GameObject backFromOptionsButton = UICreationHelpers.CreateButton(optionsPanel.transform, "BackFromOptionsButton", "BACK", new Vector2(0, -150));
        
        return optionsPanel;
    }
    
    private static GameObject CreateCreditsPanel(Transform parent)
    {
        GameObject creditsPanel = UICreationHelpers.CreatePanel(parent, "CreditsPanel", UICreationHelpers.panelColor);
        UICreationHelpers.AddCanvasGroup(creditsPanel);
        creditsPanel.SetActive(false);
        
        // Credits Panel Content
        GameObject creditsTitle = UICreationHelpers.CreateTextMeshPro(creditsPanel.transform, "CreditsTitle", "CREDITS", 48);
        RectTransform creditsTitleRect = creditsTitle.GetComponent<RectTransform>();
        creditsTitleRect.anchoredPosition = new Vector2(0, 200);
        
        GameObject creditsText = UICreationHelpers.CreateTextMeshPro(creditsPanel.transform, "CreditsText", "Math Pinball\nCreated by: Your Name\n\nSpecial Thanks:\nUnity Technologies\nYour Friends & Family", 24);
        RectTransform creditsTextRect = creditsText.GetComponent<RectTransform>();
        creditsTextRect.anchoredPosition = new Vector2(0, 0);
        creditsTextRect.sizeDelta = new Vector2(600, 300);
        TextMeshProUGUI creditsTextComp = creditsText.GetComponent<TextMeshProUGUI>();
        creditsTextComp.alignment = TextAlignmentOptions.Center;
        
        // Back Button
        GameObject backFromCreditsButton = UICreationHelpers.CreateButton(creditsPanel.transform, "BackFromCreditsButton", "BACK", new Vector2(0, -200));
        
        return creditsPanel;
    }
}
