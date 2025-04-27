using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public static class GameUICanvasCreator
{
    public static GameObject CreateGameUICanvas()
    {
        // Create Canvas
        GameObject canvas = UICreationHelpers.CreateBaseCanvas("GameUICanvas");
        
        // Create Top Panel for game info
        GameObject topPanel = CreateTopPanel(canvas.transform);
        
        // Create Level Complete Panel
        GameObject levelCompletePanel = CreateLevelCompletePanel(canvas.transform);
        
        // Create Game Over Panel
        GameObject gameOverPanel = CreateGameOverPanel(canvas.transform);
        
        // Create Pause Button
        GameObject pauseButton = CreatePauseButton(canvas.transform);
        
        // Create Pause Menu Panel (referenced in LevelManager but created separately)
        GameObject pauseMenuPanel = UICreationHelpers.CreatePanel(canvas.transform, "PauseMenuPanel", UICreationHelpers.panelColor);
        UICreationHelpers.AddCanvasGroup(pauseMenuPanel);
        pauseMenuPanel.SetActive(false);
        
        // Add pause menu content
        GameObject pauseTitle = UICreationHelpers.CreateTextMeshPro(pauseMenuPanel.transform, "PauseTitle", "PAUSED", 48);
        RectTransform pauseTitleRect = pauseTitle.GetComponent<RectTransform>();
        pauseTitleRect.anchoredPosition = new Vector2(0, 150);
        
        GameObject resumeButton = UICreationHelpers.CreateButton(pauseMenuPanel.transform, "ResumeButton", "RESUME", new Vector2(0, 50));
        GameObject restartFromPauseButton = UICreationHelpers.CreateButton(pauseMenuPanel.transform, "RestartButton", "RESTART", new Vector2(0, -30));
        GameObject menuFromPauseButton = UICreationHelpers.CreateButton(pauseMenuPanel.transform, "MenuButton", "QUIT TO MENU", new Vector2(0, -110));
        
        // Add LevelManager component
        LevelManager levelManager = canvas.AddComponent<LevelManager>();
        
        // Set up references
        GameObject equationText = topPanel.transform.Find("EquationText").gameObject;
        GameObject scoreText = topPanel.transform.Find("ScoreText").gameObject;
        GameObject targetText = topPanel.transform.Find("TargetText").gameObject;
        GameObject timerText = topPanel.transform.Find("TimerText").gameObject;
        
        levelManager.equationText = equationText.GetComponent<TextMeshProUGUI>();
        levelManager.scoreText = scoreText.GetComponent<TextMeshProUGUI>();
        levelManager.targetText = targetText.GetComponent<TextMeshProUGUI>();
        levelManager.timerText = timerText.GetComponent<TextMeshProUGUI>();
        levelManager.gameOverPanel = gameOverPanel;
        levelManager.levelCompletePanel = levelCompletePanel;
        levelManager.pauseMenuPanel = pauseMenuPanel;
        levelManager.pauseButton = pauseButton.GetComponent<Button>();
        levelManager.resumeButton = resumeButton.GetComponent<Button>();
        
        // Connect restart and menu buttons from pause menu to the same actions
        restartFromPauseButton.GetComponent<Button>().onClick.AddListener(() => levelManager.RestartLevel());
        menuFromPauseButton.GetComponent<Button>().onClick.AddListener(() => levelManager.ReturnToMenu());
        
        // Set up buttons
        GameObject retryButton = levelCompletePanel.transform.Find("RetryButton").gameObject;
        GameObject menuButton = levelCompletePanel.transform.Find("MenuButton").gameObject;
        GameObject nextLevelButton = levelCompletePanel.transform.Find("NextLevelButton").gameObject;
        
        levelManager.restartButton = retryButton.GetComponent<Button>();
        levelManager.menuButton = menuButton.GetComponent<Button>();
        levelManager.nextLevelButton = nextLevelButton.GetComponent<Button>();
        
        // Set up star objects
        GameObject starsContainer = levelCompletePanel.transform.Find("StarsContainer").gameObject;
        GameObject[] stars = new GameObject[3];
        for (int i = 0; i < 3; i++)
        {
            stars[i] = starsContainer.transform.Find("Star" + (i + 1)).gameObject;
        }
        levelManager.starObjects = stars;
        
        Debug.Log("Game UI Canvas created with all UI elements!");
        return canvas;
    }
    
    private static GameObject CreateTopPanel(Transform parent)
    {
        GameObject topPanel = UICreationHelpers.CreatePanel(parent, "TopPanel", new Color(0.1f, 0.1f, 0.2f, 0.8f));
        RectTransform topPanelRect = topPanel.GetComponent<RectTransform>();
        topPanelRect.anchorMin = new Vector2(0, 1);
        topPanelRect.anchorMax = new Vector2(1, 1);
        topPanelRect.pivot = new Vector2(0.5f, 1);
        topPanelRect.sizeDelta = new Vector2(0, 100);
        
        // Create Game Info Elements
        GameObject equationText = UICreationHelpers.CreateTextMeshPro(topPanel.transform, "EquationText", "Equation: 2 + 3 = 5", 24);
        RectTransform equationRect = equationText.GetComponent<RectTransform>();
        equationRect.anchorMin = new Vector2(0, 0.5f);
        equationRect.anchorMax = new Vector2(0, 0.5f);
        equationRect.pivot = new Vector2(0, 0.5f);
        equationRect.anchoredPosition = new Vector2(20, 0);
        
        GameObject scoreText = UICreationHelpers.CreateTextMeshPro(topPanel.transform, "ScoreText", "Score: 0", 24);
        RectTransform scoreRect = scoreText.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(1, 0.5f);
        scoreRect.anchorMax = new Vector2(1, 0.5f);
        scoreRect.pivot = new Vector2(1, 0.5f);
        scoreRect.anchoredPosition = new Vector2(-200, 15);
        
        GameObject targetText = UICreationHelpers.CreateTextMeshPro(topPanel.transform, "TargetText", "Target: 10", 24);
        RectTransform targetRect = targetText.GetComponent<RectTransform>();
        targetRect.anchorMin = new Vector2(1, 0.5f);
        targetRect.anchorMax = new Vector2(1, 0.5f);
        targetRect.pivot = new Vector2(1, 0.5f);
        targetRect.anchoredPosition = new Vector2(-200, -15);
        
        GameObject timerText = UICreationHelpers.CreateTextMeshPro(topPanel.transform, "TimerText", "01:00", 24);
        RectTransform timerRect = timerText.GetComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(1, 0.5f);
        timerRect.anchorMax = new Vector2(1, 0.5f);
        timerRect.pivot = new Vector2(1, 0.5f);
        timerRect.anchoredPosition = new Vector2(-20, 0);
        
        return topPanel;
    }
    
    private static GameObject CreatePauseButton(Transform parent)
    {
        GameObject pauseButton = UICreationHelpers.CreateButton(parent, "PauseButton", "II", new Vector2(0, 0));
        RectTransform pauseRect = pauseButton.GetComponent<RectTransform>();
        pauseRect.anchorMin = new Vector2(1, 1);
        pauseRect.anchorMax = new Vector2(1, 1);
        pauseRect.pivot = new Vector2(1, 1);
        pauseRect.anchoredPosition = new Vector2(-10, -10);
        pauseRect.sizeDelta = new Vector2(50, 50);
        
        return pauseButton;
    }
    
    private static GameObject CreateLevelCompletePanel(Transform parent)
    {
        GameObject levelCompletePanel = UICreationHelpers.CreatePanel(parent, "LevelCompletePanel", UICreationHelpers.panelColor);
        UICreationHelpers.AddCanvasGroup(levelCompletePanel);
        levelCompletePanel.SetActive(false);
        
        // Level Complete Panel Content
        GameObject completeTitle = UICreationHelpers.CreateTextMeshPro(levelCompletePanel.transform, "CompleteTitle", "LEVEL COMPLETE!", 48);
        RectTransform completeTitleRect = completeTitle.GetComponent<RectTransform>();
        completeTitleRect.anchoredPosition = new Vector2(0, 200);
        TextMeshProUGUI completeTitleText = completeTitle.GetComponent<TextMeshProUGUI>();
        completeTitleText.color = UICreationHelpers.accentColor;
        
        // Stars container
        GameObject starsContainer = new GameObject("StarsContainer", typeof(RectTransform));
        starsContainer.transform.SetParent(levelCompletePanel.transform, false);
        RectTransform starsContainerRect = starsContainer.GetComponent<RectTransform>();
        starsContainerRect.sizeDelta = new Vector2(400, 100);
        starsContainerRect.anchoredPosition = new Vector2(0, 100);
        
        // Create stars
        for (int i = 0; i < 3; i++)
        {
            GameObject starObj = new GameObject("Star" + (i + 1), typeof(RectTransform));
            starObj.transform.SetParent(starsContainer.transform, false);
            RectTransform starRect = starObj.GetComponent<RectTransform>();
            starRect.sizeDelta = new Vector2(80, 80);
            starRect.anchoredPosition = new Vector2(-120 + i * 120, 0);
            
            Image starImage = starObj.AddComponent<Image>();
            starImage.color = UICreationHelpers.accentColor;
            // You'll need to assign a star sprite in the editor
        }
        
        // Score summary
        GameObject scoreSummary = UICreationHelpers.CreateTextMeshPro(levelCompletePanel.transform, "ScoreSummary", "Final Score: 0", 36);
        RectTransform scoreSummaryRect = scoreSummary.GetComponent<RectTransform>();
        scoreSummaryRect.anchoredPosition = new Vector2(0, 20);
        
        // Buttons
        GameObject nextLevelButton = UICreationHelpers.CreateButton(levelCompletePanel.transform, "NextLevelButton", "NEXT LEVEL", new Vector2(0, -60));
        GameObject retryButton = UICreationHelpers.CreateButton(levelCompletePanel.transform, "RetryButton", "RETRY", new Vector2(0, -140));
        GameObject menuButton = UICreationHelpers.CreateButton(levelCompletePanel.transform, "MenuButton", "MENU", new Vector2(0, -220));
        
        return levelCompletePanel;
    }
    
    private static GameObject CreateGameOverPanel(Transform parent)
    {
        GameObject gameOverPanel = UICreationHelpers.CreatePanel(parent, "GameOverPanel", UICreationHelpers.panelColor);
        UICreationHelpers.AddCanvasGroup(gameOverPanel);
        gameOverPanel.SetActive(false);
        
        // Game Over Panel Content
        GameObject gameOverTitle = UICreationHelpers.CreateTextMeshPro(gameOverPanel.transform, "GameOverTitle", "GAME OVER", 48);
        RectTransform gameOverTitleRect = gameOverTitle.GetComponent<RectTransform>();
        gameOverTitleRect.anchoredPosition = new Vector2(0, 100);
        TextMeshProUGUI gameOverTitleText = gameOverTitle.GetComponent<TextMeshProUGUI>();
        gameOverTitleText.color = Color.red;
        
        GameObject finalScore = UICreationHelpers.CreateTextMeshPro(gameOverPanel.transform, "FinalScore", "Score: 0", 36);
        RectTransform finalScoreRect = finalScore.GetComponent<RectTransform>();
        finalScoreRect.anchoredPosition = new Vector2(0, 20);
        
        // Buttons
        GameObject retryButton = UICreationHelpers.CreateButton(gameOverPanel.transform, "RetryButton", "RETRY", new Vector2(0, -60));
        GameObject menuButton = UICreationHelpers.CreateButton(gameOverPanel.transform, "MenuButton", "MENU", new Vector2(0, -140));
        
        return gameOverPanel;
    }
}
