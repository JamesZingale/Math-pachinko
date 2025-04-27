using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI equationText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI targetText;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI timerText;
    
    [Header("Game Panels")]
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public GameObject pauseMenuPanel;
    
    [Header("Buttons")]
    public Button restartButton;
    public Button menuButton;
    public Button nextLevelButton;
    public Button pauseButton;
    public Button resumeButton;
    
    [Header("Level Settings")]
    public int targetScore = 10;
    public float timeLimit = 60f; // Time in seconds, 0 = no time limit
    public int starsRequired = 3; // Number of stars needed to unlock next level
    public GameObject[] starObjects; // References to star UI objects
    
    [Header("Effects")]
    public ParticleSystem levelCompleteParticles;
    public AudioClip successSound;
    public AudioClip failSound;
    
    private int currentScore = 0;
    private float remainingTime;
    private bool isGameOver = false;
    private bool isPaused = false;
    private int currentLevel;
    private int earnedStars = 0;
    
    private void Start()
    {
        // Get current level number from scene name
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Level"))
        {
            if (int.TryParse(sceneName.Substring(5), out currentLevel))
            {
                if (levelNameText != null)
                    levelNameText.text = "Level " + currentLevel;
            }
        }
        
        // Initialize timer
        remainingTime = timeLimit;
        
        // Initialize UI
        UpdateScoreUI();
        UpdateTargetUI();
        UpdateTimerUI();
        
        // Initialize panels
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
            
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
        
        // Set up button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(ReturnToMenu);
        
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(LoadNextLevel);
            
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);
            
        if (resumeButton != null)
            resumeButton.onClick.AddListener(TogglePause);
    }
    
    private void Update()
    {
        // Handle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Update timer if game is active
        if (!isGameOver && !isPaused && timeLimit > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerUI();
            
            // Check for time-out
            if (remainingTime <= 0)
            {
                remainingTime = 0;
                GameOver();
            }
        }
    }
    
    public void UpdateEquation(string equation)
    {
        if (equationText != null)
            equationText.text = equation;
    }
    
    public void AddScore(int points)
    {
        if (isGameOver) return;
        
        currentScore += points;
        UpdateScoreUI();
        
        // Check for level completion
        if (currentScore >= targetScore)
        {
            LevelComplete();
        }
    }
    
    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore;
    }
    
    private void UpdateTargetUI()
    {
        if (targetText != null)
            targetText.text = "Target: " + targetScore;
    }
    
    private void UpdateTimerUI()
    {
        if (timerText != null && timeLimit > 0)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            
            // Change color when time is running low
            if (remainingTime <= 10)
                timerText.color = Color.red;
            else
                timerText.color = Color.white;
        }
    }
    
    public void GameOver()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
            
        // Play fail sound
        if (failSound != null)
            AudioSource.PlayClipAtPoint(failSound, Camera.main.transform.position);
    }
    
    public void LevelComplete()
    {
        if (isGameOver) return;
        
        isGameOver = true;
        
        // Calculate stars earned based on score and time
        CalculateStars();
        
        // Save level completion status
        PlayerPrefs.SetInt("Level" + currentLevel + "Completed", 1);
        PlayerPrefs.SetInt("Level" + currentLevel + "Stars", earnedStars);
        PlayerPrefs.Save();
        
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);
            
        // Show earned stars
        UpdateStarsUI();
        
        // Play success effects
        if (levelCompleteParticles != null)
            levelCompleteParticles.Play();
            
        if (successSound != null)
            AudioSource.PlayClipAtPoint(successSound, Camera.main.transform.position);
    }
    
    private void CalculateStars()
    {
        // Simple star calculation based on percentage of target score achieved
        float scoreRatio = (float)currentScore / targetScore;
        
        if (scoreRatio >= 1.5f) earnedStars = 3;
        else if (scoreRatio >= 1.2f) earnedStars = 2;
        else earnedStars = 1;
        
        // Adjust stars based on remaining time if there's a time limit
        if (timeLimit > 0)
        {
            float timeRatio = remainingTime / timeLimit;
            if (timeRatio > 0.5f && earnedStars < 3)
                earnedStars++;
        }
    }
    
    private void UpdateStarsUI()
    {
        if (starObjects == null || starObjects.Length == 0) return;
        
        // Show earned stars
        for (int i = 0; i < starObjects.Length; i++)
        {
            if (starObjects[i] != null)
                starObjects[i].SetActive(i < earnedStars);
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(isPaused);
            
        Time.timeScale = isPaused ? 0f : 1f;
    }
    
    public void RestartLevel()
    {
        // Reset time scale in case we're paused
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void ReturnToMenu()
    {
        // Reset time scale in case we're paused
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void LoadNextLevel()
    {
        // Reset time scale in case we're paused
        Time.timeScale = 1f;
        
        // Try to load next level by name (Level1, Level2, etc.)
        int nextLevel = currentLevel + 1;
        string nextLevelName = "Level" + nextLevel;
        
        // Check if the next level exists in build settings
        bool levelExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == nextLevelName)
            {
                levelExists = true;
                break;
            }
        }
        
        if (levelExists)
        {
            SceneManager.LoadScene(nextLevelName);
        }
        else
        {
            // If no more levels, return to menu
            ReturnToMenu();
        }
    }
}
