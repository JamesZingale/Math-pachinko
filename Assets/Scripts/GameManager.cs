using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int totalLevels = 10;
    public bool unlockAllLevels = false; // For testing
    
    [Header("Audio")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public float musicFadeDuration = 1.0f;
    
    // Singleton pattern
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("GameManager");
                    _instance = obj.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }
    
    private AudioSource musicSource;
    private Dictionary<string, bool> loadedScenes = new Dictionary<string, bool>();
    private bool isTransitioning = false;
    
    private void Awake()
    {
        // Singleton setup
        if (_instance == null)
        {
            _instance = this;
            
            // Make sure this is a root GameObject before calling DontDestroyOnLoad
            if (transform.parent != null)
            {
                transform.SetParent(null); // Make it a root GameObject
            }
            DontDestroyOnLoad(gameObject);
            
            // Initialize audio
            SetupAudio();
            
            // Initialize player progress
            InitializePlayerProgress();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    private void SetupAudio()
    {
        // Add audio source for music if needed
        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        
        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        
        // Try to set the MusicPlayer tag, but don't crash if it doesn't exist
        try
        {
            gameObject.tag = "MusicPlayer";
        }
        catch (UnityException)
        {
            Debug.LogWarning("MusicPlayer tag is not defined. Please use the 'Math Pinball/Add Required Tags' menu item to add it.");
            
            // Create the tag automatically if in the editor
            #if UNITY_EDITOR
            Debug.Log("Attempting to add MusicPlayer tag automatically...");
            TagManager.AddRequiredTags();
            
            // Try again after adding the tag
            try
            {
                gameObject.tag = "MusicPlayer";
                Debug.Log("MusicPlayer tag successfully added and applied.");
            }
            catch
            {
                Debug.LogError("Failed to apply MusicPlayer tag even after attempting to add it.");
            }
            #endif
        }
    }
    
    private void InitializePlayerProgress()
    {
        // Initialize level unlocks if not already done
        if (!PlayerPrefs.HasKey("LevelsUnlocked"))
        {
            // Unlock first level by default
            PlayerPrefs.SetInt("LevelsUnlocked", 1);
        }
        
        // Unlock all levels if specified (for testing)
        if (unlockAllLevels)
        {
            PlayerPrefs.SetInt("LevelsUnlocked", totalLevels);
        }
    }
    
    private void Start()
    {
        // Start playing appropriate music based on current scene
        PlayMusicForCurrentScene();
        
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from scene loaded event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Track loaded scenes
        loadedScenes[scene.name] = true;
        
        // Play appropriate music for the new scene
        PlayMusicForCurrentScene();
    }
    
    private void PlayMusicForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        
        // Determine which music to play
        AudioClip targetMusic = null;
        
        if (currentSceneName == "MainMenu" || currentSceneName == "LevelSelection")
        {
            targetMusic = menuMusic;
        }
        else
        {
            targetMusic = gameMusic;
        }
        
        // Change music if needed
        if (targetMusic != null && (musicSource.clip != targetMusic || !musicSource.isPlaying))
        {
            StartCoroutine(CrossFadeMusic(targetMusic));
        }
    }
    
    private IEnumerator CrossFadeMusic(AudioClip newMusic)
    {
        // If music is already playing, fade it out
        if (musicSource.isPlaying)
        {
            float startVolume = musicSource.volume;
            
            // Fade out
            for (float t = 0; t < musicFadeDuration; t += Time.deltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / musicFadeDuration);
                yield return null;
            }
        }
        
        // Change clip and start playing
        musicSource.clip = newMusic;
        musicSource.volume = 0;
        musicSource.Play();
        
        // Fade in
        float targetVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        for (float t = 0; t < musicFadeDuration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, targetVolume, t / musicFadeDuration);
            yield return null;
        }
        
        // Ensure we end at the exact target volume
        musicSource.volume = targetVolume;
    }
    
    #region Level Management
    
    public void LoadLevel(int levelIndex)
    {
        if (isTransitioning)
            return;
            
        isTransitioning = true;
        
        // Check if level is unlocked
        int unlockedLevels = PlayerPrefs.GetInt("LevelsUnlocked", 1);
        if (levelIndex > unlockedLevels && !unlockAllLevels)
        {
            Debug.LogWarning("Attempted to load locked level: " + levelIndex);
            isTransitioning = false;
            return;
        }
        
        // Determine scene name
        string sceneName = "Level" + levelIndex;
        
        // Load the level
        StartCoroutine(LoadLevelRoutine(sceneName));
    }
    
    private IEnumerator LoadLevelRoutine(string sceneName)
    {
        // Show loading screen if needed
        // TODO: Implement loading screen
        
        // Load the scene
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Wait until the scene is fully loaded
        while (!asyncLoad.isDone)
        {
            // Update loading progress if needed
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            // TODO: Update loading screen progress
            
            yield return null;
        }
        
        // Reset transition flag
        isTransitioning = false;
    }
    
    public void LoadMainMenu()
    {
        if (isTransitioning)
            return;
            
        isTransitioning = true;
        StartCoroutine(LoadLevelRoutine("MainMenu"));
    }
    
    public void LoadLevelSelection()
    {
        if (isTransitioning)
            return;
            
        isTransitioning = true;
        StartCoroutine(LoadLevelRoutine("LevelSelection"));
    }
    
    public void UnlockNextLevel(int currentLevel)
    {
        int unlockedLevels = PlayerPrefs.GetInt("LevelsUnlocked", 1);
        
        // If we completed the last unlocked level, unlock the next one
        if (currentLevel >= unlockedLevels && currentLevel < totalLevels)
        {
            PlayerPrefs.SetInt("LevelsUnlocked", currentLevel + 1);
        }
    }
    
    #endregion
    
    #region Player Progress
    
    public int GetLevelStars(int levelIndex)
    {
        return PlayerPrefs.GetInt("Stars_Level" + levelIndex, 0);
    }
    
    public int GetLevelHighScore(int levelIndex)
    {
        return PlayerPrefs.GetInt("HighScore_Level" + levelIndex, 0);
    }
    
    public int GetTotalStars()
    {
        int totalStars = 0;
        for (int i = 1; i <= totalLevels; i++)
        {
            totalStars += GetLevelStars(i);
        }
        return totalStars;
    }
    
    public bool IsLevelUnlocked(int levelIndex)
    {
        if (unlockAllLevels)
            return true;
            
        int unlockedLevels = PlayerPrefs.GetInt("LevelsUnlocked", 1);
        return levelIndex <= unlockedLevels;
    }
    
    public void ResetAllProgress()
    {
        // Clear all player prefs except settings
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        int vibration = PlayerPrefs.GetInt("Vibration", 1);
        
        PlayerPrefs.DeleteAll();
        
        // Restore settings
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetInt("Vibration", vibration);
        
        // Reset level unlocks
        PlayerPrefs.SetInt("LevelsUnlocked", 1);
        
        // Set first time flag for tutorial
        PlayerPrefs.SetInt("FirstTime", 1);
    }
    
    #endregion
    
    #region Game State
    
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
    
    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    #endregion
}
