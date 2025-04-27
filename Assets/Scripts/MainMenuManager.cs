using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject levelSelectionPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button quitButton;

    [Header("Level Selection")]
    public Button[] levelButtons;
    public Button backFromLevelsButton;
    public GameObject levelButtonPrefab;
    public Transform levelButtonContainer;
    public Sprite lockedLevelSprite;
    public Sprite unlockedLevelSprite;

    [Header("Options Menu")]
    public Button backFromOptionsButton;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle fullscreenToggle;

    [Header("Credits Menu")]
    public Button backFromCreditsButton;

    [Header("UI Animation")]
    public float transitionSpeed = 0.5f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("UI Elements")]
    public TextMeshProUGUI gameTitle;
    public TextMeshProUGUI versionText;

    private List<string> levelSceneNames = new List<string>();

    private void Start()
    {
        // Find all level scenes in the Scenes folder
        FindAllLevelScenes();

        // Set version text
        if (versionText != null)
            versionText.text = "v" + Application.version;

        // Initialize main menu buttons
        if (playButton != null)
            playButton.onClick.AddListener(ShowLevelSelection);
        
        if (optionsButton != null)
            optionsButton.onClick.AddListener(ShowOptions);
        
        if (creditsButton != null)
            creditsButton.onClick.AddListener(ShowCredits);
        
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
        
        // Initialize back buttons
        if (backFromLevelsButton != null)
            backFromLevelsButton.onClick.AddListener(BackToMainMenu);
        
        if (backFromOptionsButton != null)
            backFromOptionsButton.onClick.AddListener(BackToMainMenu);
        
        if (backFromCreditsButton != null)
            backFromCreditsButton.onClick.AddListener(BackToMainMenu);

        // Initialize options
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        }

        // Generate level buttons if container and prefab are assigned
        if (levelButtonContainer != null)
        {
            GenerateLevelButtons();
        }
        // Otherwise use the manually assigned level buttons
        else if (levelButtons != null)
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                int levelIndex = i + 1; // Level indices start from 1
                if (levelButtons[i] != null)
                {
                    levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
                    
                    // All levels are now always unlocked
                    levelButtons[i].interactable = true;
                    
                    // Update button appearance
                    UpdateLevelButtonAppearance(levelButtons[i], levelIndex);
                }
            }
        }

        // Start with only main menu active
        ShowOnlyPanel(mainMenuPanel);
    }

    private void FindAllLevelScenes()
    {
        levelSceneNames.Clear();
        
        // Get all scene paths in build settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = Path.GetFileNameWithoutExtension(scenePath);
            
            // Check if it's a level scene (starts with "Level")
            if (sceneName.StartsWith("Level"))
            {
                levelSceneNames.Add(sceneName);
            }
        }

        // If no level scenes found in build settings, search in the Scenes folder
        if (levelSceneNames.Count == 0)
        {
            #if UNITY_EDITOR
            // Get all scene files in the Scenes folder
            string[] sceneGuids = UnityEditor.AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
            foreach (string guid in sceneGuids)
            {
                string scenePath = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                
                if (sceneName.StartsWith("Level"))
                {
                    levelSceneNames.Add(sceneName);
                }
            }
            #endif
        }
        
        // Sort the level names numerically
        levelSceneNames = levelSceneNames.OrderBy(name => {
            string numberPart = name.Substring("Level".Length);
            if (int.TryParse(numberPart, out int number))
                return number;
            return 999; // Put non-numeric levels at the end
        }).ToList();
        
        Debug.Log($"Found {levelSceneNames.Count} level scenes: {string.Join(", ", levelSceneNames)}");
    }

    private void GenerateLevelButtons()
    {
        // Clear existing buttons
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Log what levels we found to help with debugging
        Debug.Log($"Creating buttons for {levelSceneNames.Count} levels: {string.Join(", ", levelSceneNames)}");

        // Create level buttons based on found level scenes
        for (int i = 0; i < levelSceneNames.Count; i++)
        {
            string levelName = levelSceneNames[i];
            int levelNumber = i + 1;
            
            // Create a completely new button instead of using the prefab
            // This avoids any styling issues that might be in the prefab
            GameObject buttonObj = new GameObject($"LevelButton_{levelNumber}");
            buttonObj.transform.SetParent(levelButtonContainer, false);
            
            // Add required components
            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(100, 100);
            
            // Add image component
            Image buttonImage = buttonObj.AddComponent<Image>();
            buttonImage.color = Color.white;
            
            // Add button component
            Button button = buttonObj.AddComponent<Button>();
            
            // Set button colors to ensure it's not greyed out
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
            colors.selectedColor = Color.white;
            colors.disabledColor = Color.white;
            colors.colorMultiplier = 1f;
            button.colors = colors;
            
            // Disable navigation to prevent orange selection bar
            Navigation nav = button.navigation;
            nav.mode = Navigation.Mode.None;
            button.navigation = nav;
            
            // Create text object for the button
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
            
            TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
            buttonText.text = levelNumber.ToString();
            buttonText.fontSize = 36;
            buttonText.alignment = TextAlignmentOptions.Center;
            buttonText.color = Color.black;
            
            // IMPORTANT: Create a local copy of the level name to use in the lambda
            // This prevents the closure from capturing the loop variable
            string levelNameCopy = levelName;
            
            // Log what level this button will load
            Debug.Log($"Button {levelNumber} will load scene: {levelNameCopy}");
            
            // Set button action with the copied variable
            button.onClick.AddListener(() => {
                Debug.Log($"Button clicked, loading scene: {levelNameCopy}");
                LoadLevelByName(levelNameCopy);
            });
        }
    }

    private void UpdateLevelButtonAppearance(Button button, int levelIndex)
    {
        if (button == null) return;
        
        // Ensure the button is fully enabled
        button.interactable = true;
        
        // Update the button's visual state
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            // Set the sprite if available
            if (unlockedLevelSprite != null)
                buttonImage.sprite = unlockedLevelSprite;
            
            // Ensure the color is white (fully visible)
            buttonImage.color = Color.white;
            
            // Remove any orange highlight or selection state
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f);
            colors.pressedColor = new Color(0.7f, 0.7f, 0.7f);
            colors.selectedColor = Color.white;
            colors.disabledColor = Color.white; // This ensures even if somehow disabled, it looks normal
            button.colors = colors;
        }
        
        // Make sure any child text is visible
        TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            buttonText.color = Color.black; // Make text clearly visible
        }
    }

    private bool IsLevelUnlocked(int levelIndex)
    {
        // All levels are now always unlocked
        return true;
    }

    public void ShowLevelSelection()
    {
        ShowOnlyPanel(levelSelectionPanel);
        PlayButtonClickSound();
    }

    public void ShowOptions()
    {
        ShowOnlyPanel(optionsPanel);
        PlayButtonClickSound();
        
        // Load saved options
        if (musicVolumeSlider != null)
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);
    }

    public void ShowCredits()
    {
        ShowOnlyPanel(creditsPanel);
        PlayButtonClickSound();
    }

    public void BackToMainMenu()
    {
        ShowOnlyPanel(mainMenuPanel);
        PlayButtonClickSound();
    }

    private void ShowOnlyPanel(GameObject panelToShow)
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(mainMenuPanel == panelToShow);
        if (levelSelectionPanel != null) levelSelectionPanel.SetActive(levelSelectionPanel == panelToShow);
        if (optionsPanel != null) optionsPanel.SetActive(optionsPanel == panelToShow);
        if (creditsPanel != null) creditsPanel.SetActive(creditsPanel == panelToShow);
        
        // Animate the panel that's being shown
        if (panelToShow != null)
        {
            CanvasGroup canvasGroup = panelToShow.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                StartCoroutine(FadeInPanel(canvasGroup));
            }
        }
    }

    private IEnumerator FadeInPanel(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        float time = 0;
        
        while (time < transitionSpeed)
        {
            time += Time.deltaTime;
            float normalizedTime = time / transitionSpeed;
            canvasGroup.alpha = transitionCurve.Evaluate(normalizedTime);
            yield return null;
        }
        
        canvasGroup.alpha = 1;
    }

    public void LoadLevel(int levelIndex)
    {
        PlayButtonClickSound();
        
        // Check if we have this level in our list
        if (levelIndex > 0 && levelIndex <= levelSceneNames.Count)
        {
            string levelName = levelSceneNames[levelIndex - 1];
            LoadLevelByName(levelName);
        }
        else
        {
            // Fallback to the old method
            string levelSceneName = "Level" + levelIndex;
            if (DoesSceneExist(levelSceneName))
            {
                SceneManager.LoadScene(levelSceneName);
            }
            else
            {
                Debug.LogWarning("Level " + levelIndex + " does not exist!");
            }
        }
    }
    
    public void LoadLevelByName(string levelName)
    {
        PlayButtonClickSound();
        
        Debug.Log($"Attempting to load level: {levelName}");
        
        if (DoesSceneExist(levelName))
        {
            Debug.Log($"Scene exists, loading: {levelName}");
            SceneManager.LoadScene(levelName);
        }
        else
        {
            Debug.LogWarning($"Level {levelName} does not exist in build settings!");
        }
    }
    
    private bool DoesSceneExist(string sceneName)
    {
        // Check if the scene is in the build settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameFromPath = Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneNameFromPath == sceneName)
            {
                return true;
            }
        }
        
        Debug.LogWarning($"Scene {sceneName} not found in build settings. Checking asset database...");
        
        // If we're in the editor, also check if the scene exists as an asset
        #if UNITY_EDITOR
        string[] sceneGuids = UnityEditor.AssetDatabase.FindAssets("t:Scene " + sceneName);
        return sceneGuids.Length > 0;
        #else
        return false;
        #endif
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        // Implement actual volume change through an audio manager
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat("SFXVolume", volume);
        // Implement actual volume change through an audio manager
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    private void PlayButtonClickSound()
    {
        // Play button click sound through audio manager
        // AudioManager.Instance.PlaySound("ButtonClick");
    }

    public void QuitGame()
    {
        PlayButtonClickSound();
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
