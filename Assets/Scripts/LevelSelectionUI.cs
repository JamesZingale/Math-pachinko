using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class LevelSelectionUI : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public Button button;
        public TextMeshProUGUI levelText;
        public GameObject lockIcon;
        public GameObject[] stars;
    }
    
    [Header("Level Selection")]
    public LevelButton[] levelButtons;
    public GameObject levelButtonPrefab;
    public Transform levelButtonContainer;
    public int numberOfLevels = 10;
    public float buttonSpacing = 20f;
    
    [Header("Level Button Visuals")]
    public Sprite lockedLevelSprite;
    public Sprite unlockedLevelSprite;
    public Sprite completedLevelSprite;
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color unlockedColor = Color.white;
    public Color completedColor = new Color(1f, 1f, 0.8f, 1f);
    
    [Header("Animation")]
    public float animationDelay = 0.1f;
    public float animationDuration = 0.3f;
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    
    [Header("Navigation")]
    public Button nextPageButton;
    public Button prevPageButton;
    public int levelsPerPage = 9;
    
    private int currentPage = 0;
    private int totalPages;
    
    private void Start()
    {
        // Calculate total pages
        totalPages = Mathf.CeilToInt((float)numberOfLevels / levelsPerPage);
        
        // Set up navigation buttons
        if (nextPageButton != null)
            nextPageButton.onClick.AddListener(NextPage);
        
        if (prevPageButton != null)
            prevPageButton.onClick.AddListener(PreviousPage);
        
        // Generate level buttons if container and prefab are assigned
        if (levelButtonContainer != null && levelButtonPrefab != null)
        {
            GenerateLevelButtons();
        }
        // Otherwise use the manually assigned level buttons
        else if (levelButtons != null && levelButtons.Length > 0)
        {
            InitializeLevelButtons();
        }
        
        // Show the first page
        ShowPage(0);
    }
    
    private void GenerateLevelButtons()
    {
        // Clear existing buttons
        foreach (Transform child in levelButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        levelButtons = new LevelButton[numberOfLevels];
        
        // Create level buttons
        for (int i = 0; i < numberOfLevels; i++)
        {
            int levelIndex = i + 1;
            GameObject buttonObj = Instantiate(levelButtonPrefab, levelButtonContainer);
            
            LevelButton levelButton = new LevelButton();
            levelButton.button = buttonObj.GetComponent<Button>();
            
            // Find components
            levelButton.levelText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            
            // Find lock icon (assuming it's a direct child with "Lock" in the name)
            Transform lockTransform = buttonObj.transform.Find("LockIcon");
            if (lockTransform != null)
                levelButton.lockIcon = lockTransform.gameObject;
            
            // Find stars (assuming they're children with "Star" in the name)
            levelButton.stars = new GameObject[3];
            for (int s = 0; s < 3; s++)
            {
                Transform starTransform = buttonObj.transform.Find("Star" + (s + 1));
                if (starTransform != null)
                    levelButton.stars[s] = starTransform.gameObject;
            }
            
            // Set level number text
            if (levelButton.levelText != null)
            {
                levelButton.levelText.text = levelIndex.ToString();
            }
            
            // Set button action
            int level = levelIndex; // Create a local variable for the closure
            levelButton.button.onClick.AddListener(() => LoadLevel(level));
            
            // Add to array
            levelButtons[i] = levelButton;
        }
        
        // Initialize all buttons
        InitializeLevelButtons();
    }
    
    private void InitializeLevelButtons()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            LevelButton levelButton = levelButtons[i];
            
            if (levelButton == null || levelButton.button == null) continue;
            
            // Set button interactable based on level unlock status
            bool isUnlocked = IsLevelUnlocked(levelIndex);
            levelButton.button.interactable = isUnlocked;
            
            // Update button appearance
            UpdateLevelButtonAppearance(levelButton, levelIndex);
        }
    }
    
    private void UpdateLevelButtonAppearance(LevelButton levelButton, int levelIndex)
    {
        if (levelButton == null || levelButton.button == null) return;
        
        bool isUnlocked = IsLevelUnlocked(levelIndex);
        bool isCompleted = IsLevelCompleted(levelIndex);
        int stars = GetLevelStars(levelIndex);
        
        // Update button image
        Image buttonImage = levelButton.button.GetComponent<Image>();
        if (buttonImage != null)
        {
            if (isCompleted && completedLevelSprite != null)
                buttonImage.sprite = completedLevelSprite;
            else if (isUnlocked && unlockedLevelSprite != null)
                buttonImage.sprite = unlockedLevelSprite;
            else if (lockedLevelSprite != null)
                buttonImage.sprite = lockedLevelSprite;
            
            buttonImage.color = isCompleted ? completedColor : (isUnlocked ? unlockedColor : lockedColor);
        }
        
        // Show/hide lock icon
        if (levelButton.lockIcon != null)
            levelButton.lockIcon.SetActive(!isUnlocked);
        
        // Show stars based on level completion
        if (levelButton.stars != null)
        {
            for (int i = 0; i < levelButton.stars.Length; i++)
            {
                if (levelButton.stars[i] != null)
                    levelButton.stars[i].SetActive(i < stars);
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
    
    private bool IsLevelCompleted(int levelIndex)
    {
        return PlayerPrefs.GetInt("Level" + levelIndex + "Completed", 0) == 1;
    }
    
    private int GetLevelStars(int levelIndex)
    {
        return PlayerPrefs.GetInt("Level" + levelIndex + "Stars", 0);
    }
    
    public void LoadLevel(int levelIndex)
    {
        if (IsLevelUnlocked(levelIndex))
        {
            // Play button click sound
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("ButtonClick");
                
            // Load the level
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level" + levelIndex);
        }
        else
        {
            // Play locked sound
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlaySound("LevelLocked");
                
            // Shake the button to indicate it's locked
            if (levelIndex - 1 < levelButtons.Length)
                StartCoroutine(ShakeButton(levelButtons[levelIndex - 1].button.transform as RectTransform));
        }
    }
    
    private IEnumerator ShakeButton(RectTransform buttonRect)
    {
        if (buttonRect == null) yield break;
        
        Vector3 originalPosition = buttonRect.anchoredPosition;
        float shakeMagnitude = 10f;
        float shakeDuration = 0.5f;
        float elapsedTime = 0f;
        
        while (elapsedTime < shakeDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / shakeDuration;
            
            float xOffset = Mathf.Sin(normalizedTime * 40) * shakeMagnitude * (1 - normalizedTime);
            buttonRect.anchoredPosition = originalPosition + new Vector3(xOffset, 0, 0);
            
            yield return null;
        }
        
        buttonRect.anchoredPosition = originalPosition;
    }
    
    public void NextPage()
    {
        if (currentPage < totalPages - 1)
        {
            ShowPage(currentPage + 1);
        }
    }
    
    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            ShowPage(currentPage - 1);
        }
    }
    
    private void ShowPage(int pageIndex)
    {
        currentPage = pageIndex;
        
        // Update navigation buttons
        if (prevPageButton != null)
            prevPageButton.gameObject.SetActive(currentPage > 0);
        
        if (nextPageButton != null)
            nextPageButton.gameObject.SetActive(currentPage < totalPages - 1);
        
        // Show/hide level buttons based on current page
        int startIndex = currentPage * levelsPerPage;
        int endIndex = Mathf.Min(startIndex + levelsPerPage, numberOfLevels);
        
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i] == null || levelButtons[i].button == null) continue;
            
            bool isVisible = (i >= startIndex && i < endIndex);
            levelButtons[i].button.gameObject.SetActive(isVisible);
            
            // Animate buttons that are becoming visible
            if (isVisible)
            {
                StartCoroutine(AnimateButtonIn(levelButtons[i].button.transform as RectTransform, i - startIndex));
            }
        }
    }
    
    private IEnumerator AnimateButtonIn(RectTransform buttonRect, int index)
    {
        if (buttonRect == null) yield break;
        
        // Wait for staggered delay
        yield return new WaitForSeconds(index * animationDelay);
        
        // Store original values
        Vector3 targetScale = buttonRect.localScale;
        Vector3 startScale = Vector3.zero;
        
        // Set starting values
        buttonRect.localScale = startScale;
        
        // Animate
        float elapsedTime = 0f;
        
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / animationDuration;
            
            // Apply animation curve
            float curveValue = animationCurve.Evaluate(normalizedTime);
            
            // Scale animation
            buttonRect.localScale = Vector3.Lerp(startScale, targetScale, curveValue);
            
            yield return null;
        }
        
        // Ensure final values are set exactly
        buttonRect.localScale = targetScale;
    }
}
