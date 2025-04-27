using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public static class PauseMenuCanvasCreator
{
    public static GameObject CreatePauseMenuCanvas()
    {
        // Create Canvas
        GameObject canvas = UICreationHelpers.CreateBaseCanvas("PauseMenuCanvas");
        Canvas canvasComponent = canvas.GetComponent<Canvas>();
        canvasComponent.sortingOrder = 10; // Make sure it renders on top
        
        // Create Pause Menu Panel
        GameObject pauseMenuPanel = UICreationHelpers.CreatePanel(canvas.transform, "PauseMenuPanel", UICreationHelpers.panelColor);
        UICreationHelpers.AddCanvasGroup(pauseMenuPanel);
        
        // Pause Menu Content
        GameObject pauseTitle = UICreationHelpers.CreateTextMeshPro(pauseMenuPanel.transform, "PauseTitle", "PAUSED", 48);
        RectTransform pauseTitleRect = pauseTitle.GetComponent<RectTransform>();
        pauseTitleRect.anchoredPosition = new Vector2(0, 150);
        
        // Buttons
        GameObject resumeButton = UICreationHelpers.CreateButton(pauseMenuPanel.transform, "ResumeButton", "RESUME", new Vector2(0, 50));
        GameObject restartButton = UICreationHelpers.CreateButton(pauseMenuPanel.transform, "RestartButton", "RESTART", new Vector2(0, -30));
        GameObject optionsButton = UICreationHelpers.CreateButton(pauseMenuPanel.transform, "OptionsButton", "OPTIONS", new Vector2(0, -110));
        GameObject menuButton = UICreationHelpers.CreateButton(pauseMenuPanel.transform, "MenuButton", "QUIT TO MENU", new Vector2(0, -190));
        
        // Set the panel to be inactive by default
        pauseMenuPanel.SetActive(false);
        
        Debug.Log("Pause Menu Canvas created with all UI elements!");
        return canvas;
    }
}
