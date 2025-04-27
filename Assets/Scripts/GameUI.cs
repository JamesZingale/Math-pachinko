using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [Header("HUD Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI targetScoreText;
    public TextMeshProUGUI equationText;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI timerText;
    public Image progressBar;
    public GameObject ballCountDisplay;
    public TextMeshProUGUI ballsRemainingText;
    
    [Header("Game State Panels")]
    public GameObject pausePanel;
    public GameObject levelCompletePanel;
    public GameObject gameOverPanel;
    public GameObject tutorialPanel;
    
    [Header("Level Complete Panel")]
    public TextMeshProUGUI levelCompleteScoreText;
    public TextMeshProUGUI levelCompleteHighScoreText;
    public GameObject[] starObjects;
    public Button nextLevelButton;
    public Button retryButton;
    public Button menuButton;
    
    [Header("Game Over Panel")]
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverHighScoreText;
    public Button restartButton;
    public Button quitButton;
    
    [Header("Pause Panel")]
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle vibrationToggle;
    
    [Header("Animations & Effects")]
    public Animator hudAnimator;
    public Animator scoreAnimator;
    public float scorePopDuration = 0.5f;
    public GameObject scorePopPrefab;
    
    [Header("Audio")]
    public AudioClip buttonClickSound;
    public AudioClip levelCompleteSound;
    public AudioClip gameOverSound;
    
    private LevelManager levelManager;
    private EquationManager equationManager;
    private AudioSource audioSource;
    private bool isPaused = false;
    private int currentScore = 0;
    private int targetScore = 100;
    private float gameTimer = 0f;
    private int ballsRemaining = 3;
    
    // Game state tracking properties
    private bool isGameOver = false;
    private bool isLevelComplete = false;
    
    private void Awake()
    {
        // Add audio source if needed
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Hide all panels initially
        if (pausePanel != null) pausePanel.SetActive(false);
        if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        
        // Get references to managers
        levelManager = FindFirstObjectByType<LevelManager>();
        equationManager = FindFirstObjectByType<EquationManager>();
    }
    
    private void Start()
    {
        // Initialize UI elements
        InitializeUI();
        
        // Set up button listeners
        SetupButtonListeners();
        
        // Show tutorial if it's the first level
        if (PlayerPrefs.GetInt("FirstTime", 1) == 1)
        {
            ShowTutorial();
            PlayerPrefs.SetInt("FirstTime", 0);
        }
    }
    
    private void Update()
    {
        // Handle pause button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Update timer if game is active
        if (!isPaused && !isGameOver && !isLevelComplete)
        {
            gameTimer += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }
    
    private void InitializeUI()
    {
        // Get initial values from level manager
        if (levelManager != null)
        {
            // Get the current score via reflection (since it's private in LevelManager)
            System.Reflection.FieldInfo scoreField = typeof(LevelManager).GetField("currentScore", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (scoreField != null)
            {
                currentScore = (int)scoreField.GetValue(levelManager);
            }
            
            // Get target score (this is public)
            targetScore = levelManager.targetScore;
            
            // Try to get balls remaining via reflection
            System.Reflection.FieldInfo ballsField = typeof(LevelManager).GetField("ballsRemaining", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (ballsField != null)
            {
                object value = ballsField.GetValue(levelManager);
                if (value != null)
                {
                    ballsRemaining = (int)value;
                }
            }
        }
        
        // Update all UI elements
        UpdateScoreDisplay();
        UpdateTargetScoreDisplay();
        UpdateBallsRemainingDisplay();
        UpdateProgressBar();
        UpdateTimerDisplay();
        
        // Initialize sliders with saved values
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        
        if (vibrationToggle != null)
        {
            vibrationToggle.isOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
            vibrationToggle.onValueChanged.AddListener(SetVibration);
        }
    }
    
    private void SetupButtonListeners()
    {
        // Level Complete Panel
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(GoToNextLevel);
            
        if (retryButton != null)
            retryButton.onClick.AddListener(RestartLevel);
            
        if (menuButton != null)
            menuButton.onClick.AddListener(GoToMainMenu);
            
        // Game Over Panel
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(GoToMainMenu);
    }
    
    #region UI Updates
    
    public void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + currentScore;
        }
    }
    
    public void UpdateTargetScoreDisplay()
    {
        if (targetScoreText != null)
        {
            targetScoreText.text = "Target: " + targetScore;
        }
    }
    
    public void UpdateBallsRemainingDisplay()
    {
        if (ballsRemainingText != null)
        {
            ballsRemainingText.text = "Balls: " + ballsRemaining;
        }
    }
    
    public void UpdateProgressBar()
    {
        if (progressBar != null && targetScore > 0)
        {
            progressBar.fillAmount = (float)currentScore / targetScore;
        }
    }
    
    public void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTimer / 60f);
            int seconds = Mathf.FloorToInt(gameTimer % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    
    public void UpdateEquationDisplay(string equation)
    {
        if (equationText != null)
        {
            equationText.text = equation;
        }
    }
    
    #endregion
    
    #region Score Handling
    
    public void AddScore(int points)
    {
        currentScore += points;
        
        // Update UI
        UpdateScoreDisplay();
        UpdateProgressBar();
        
        // Animate score pop
        if (scoreAnimator != null)
        {
            scoreAnimator.SetTrigger("ScorePop");
        }
        
        // Show floating score text
        ShowScorePop(points);
        
        // Check for level complete
        if (levelManager != null && currentScore >= targetScore)
        {
            levelManager.LevelComplete();
            isLevelComplete = true;
        }
    }
    
    private void ShowScorePop(int points)
    {
        if (scorePopPrefab != null && scoreText != null)
        {
            GameObject popObj = Instantiate(scorePopPrefab, scoreText.transform.position, Quaternion.identity, transform);
            TextMeshProUGUI popText = popObj.GetComponent<TextMeshProUGUI>();
            
            if (popText != null)
            {
                popText.text = "+" + points;
                
                // Destroy after animation
                Destroy(popObj, scorePopDuration);
            }
        }
    }
    
    #endregion
    
    #region Game State Management
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        // Update time scale
        Time.timeScale = isPaused ? 0f : 1f;
        
        // Show/hide pause panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
        }
        
        // Notify level manager
        if (levelManager != null)
        {
            levelManager.TogglePause();
        }
        
        // Play sound
        PlayButtonSound();
    }
    
    public void ShowLevelComplete()
    {
        // Update panel text
        if (levelCompleteScoreText != null)
        {
            levelCompleteScoreText.text = "Score: " + currentScore;
        }
        
        // Get high score
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int highScore = PlayerPrefs.GetInt("HighScore_Level" + currentLevel, 0);
        
        // Update high score if needed
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore_Level" + currentLevel, highScore);
        }
        
        // Show high score
        if (levelCompleteHighScoreText != null)
        {
            levelCompleteHighScoreText.text = "High Score: " + highScore;
        }
        
        // Set stars based on score
        SetStarsDisplay();
        
        // Show panel
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
        
        // Play sound
        if (audioSource != null && levelCompleteSound != null)
        {
            audioSource.PlayOneShot(levelCompleteSound);
        }
        
        // Update game state
        isLevelComplete = true;
    }
    
    public void ShowGameOver()
    {
        // Update panel text
        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = "Score: " + currentScore;
        }
        
        // Get high score
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int highScore = PlayerPrefs.GetInt("HighScore_Level" + currentLevel, 0);
        
        // Show high score
        if (gameOverHighScoreText != null)
        {
            gameOverHighScoreText.text = "High Score: " + highScore;
        }
        
        // Show panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        // Play sound
        if (audioSource != null && gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
        
        // Update game state
        isGameOver = true;
    }
    
    public void ShowTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }
    
    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
            Time.timeScale = 1f; // Resume the game
        }
        
        // Play sound
        PlayButtonSound();
    }
    
    private void SetStarsDisplay()
    {
        if (starObjects == null || starObjects.Length == 0)
            return;
            
        // Calculate star rating (0-3 stars)
        float scoreRatio = (float)currentScore / targetScore;
        int stars = 0;
        
        if (scoreRatio >= 1.5f) stars = 3;
        else if (scoreRatio >= 1.25f) stars = 2;
        else if (scoreRatio >= 1.0f) stars = 1;
        
        // Save star rating
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int previousStars = PlayerPrefs.GetInt("Stars_Level" + currentLevel, 0);
        if (stars > previousStars)
        {
            PlayerPrefs.SetInt("Stars_Level" + currentLevel, stars);
        }
        
        // Update star display
        for (int i = 0; i < starObjects.Length; i++)
        {
            if (i < stars)
            {
                starObjects[i].SetActive(true);
            }
            else
            {
                starObjects[i].SetActive(false);
            }
        }
    }
    
    #endregion
    
    #region Button Actions
    
    public void GoToNextLevel()
    {
        PlayButtonSound();
        
        // Get current level index
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        
        // Load next level or go to level selection if this was the last level
        if (currentLevel + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentLevel + 1);
        }
        else
        {
            // Go to level selection
            SceneManager.LoadScene("LevelSelection");
        }
        
        // Ensure time scale is reset
        Time.timeScale = 1f;
    }
    
    public void RestartLevel()
    {
        PlayButtonSound();
        
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        // Ensure time scale is reset
        Time.timeScale = 1f;
    }
    
    public void GoToMainMenu()
    {
        PlayButtonSound();
        
        // Load main menu scene
        SceneManager.LoadScene("MainMenu");
        
        // Ensure time scale is reset
        Time.timeScale = 1f;
    }
    
    #endregion
    
    #region Settings
    
    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        
        // Find and update music audio source
        AudioSource musicSource = GameObject.FindGameObjectWithTag("MusicPlayer")?.GetComponent<AudioSource>();
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }
    
    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        
        // Update all SFX audio sources
        AudioSource[] sfxSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (AudioSource source in sfxSources)
        {
            if (source.gameObject.CompareTag("MusicPlayer"))
                continue;
                
            source.volume = volume;
        }
    }
    
    public void SetVibration(bool enabled)
    {
        PlayerPrefs.SetInt("Vibration", enabled ? 1 : 0);
    }
    
    #endregion
    
    private void PlayButtonSound()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
    
    // Called by LevelManager
    public void OnBallLost()
    {
        ballsRemaining--;
        UpdateBallsRemainingDisplay();
        
        // Check for game over
        if (ballsRemaining <= 0 && levelManager != null)
        {
            levelManager.GameOver();
            isGameOver = true;
        }
    }
    
    // Called by LevelGenerator to update level information
    public void UpdateLevelInfo(string levelName, int newTargetScore, float newTimeLimit)
    {
        Debug.Log("Updating level info: " + levelName + ", Target Score: " + newTargetScore);
        
        // Update target score
        targetScore = newTargetScore;
        UpdateTargetScoreDisplay();
        
        // Update level name if there's a UI element for it
        TextMeshProUGUI levelNameText = GameObject.Find("LevelNameText")?.GetComponent<TextMeshProUGUI>();
        if (levelNameText != null)
        {
            levelNameText.text = levelName;
        }
        
        // Reset timer if time limit is set
        gameTimer = 0f;
        
        // Update any other level-specific UI elements
        UpdateScoreDisplay();
        UpdateProgressBar();
        UpdateEquationDisplay("");
    }
}
