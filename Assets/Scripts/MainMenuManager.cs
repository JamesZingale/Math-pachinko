using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

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
    public int numberOfLevels = 10;
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

    private void Start()
    {
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
        if (levelButtonContainer != null && levelButtonPrefab != null)
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
                    
                    // Set button interactable based on level unlock status
                    levelButtons[i].interactable = IsLevelUnlocked(levelIndex);
                    
                    // Update button appearance based on locked/unlocked state
                    UpdateLevelButtonAppearance(levelButtons[i], levelIndex);
                }
            }
        }

        // Start with only main menu active
        ShowOnlyPanel(mainMenuPanel);
    }

    private void GenerateLevelButtons()
    {
        // Clear existing buttons
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Create level buttons
        for (int i = 0; i < numberOfLevels; i++)
        {
            int levelIndex = i + 1;
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            Button button = buttonObj.GetComponent<Button>();
            
            // Set level number text
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = levelIndex.ToString();
            }
            
            // Set button action
            button.onClick.AddListener(() => LoadLevel(levelIndex));
            
            // Set button interactable based on level unlock status
            button.interactable = IsLevelUnlocked(levelIndex);
            
            // Update button appearance
            UpdateLevelButtonAppearance(button, levelIndex);
        }
    }

    private void UpdateLevelButtonAppearance(Button button, int levelIndex)
    {
        if (button == null) return;
        
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (IsLevelUnlocked(levelIndex))
            {
                if (unlockedLevelSprite != null)
                    buttonImage.sprite = unlockedLevelSprite;
                buttonImage.color = Color.white;
            }
            else
            {
                if (lockedLevelSprite != null)
                    buttonImage.sprite = lockedLevelSprite;
                buttonImage.color = new Color(0.7f, 0.7f, 0.7f, 1f);
            }
        }
    }

    private bool IsLevelUnlocked(int levelIndex)
    {
        // Level 1 is always unlocked
        if (levelIndex == 1) return true;
        
        // Check if previous level is completed
        return PlayerPrefs.GetInt("Level" + (levelIndex - 1) + "Completed", 0) == 1;
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
        if (IsLevelUnlocked(levelIndex))
        {
            PlayButtonClickSound();
            // Show loading screen or transition effect here if desired
            SceneManager.LoadScene("Level" + levelIndex);
        }
        else
        {
            // Play locked sound or show message
            Debug.Log("Level " + levelIndex + " is locked!");
        }
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
