using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EmergencyUIFix : MonoBehaviour
{
    // This script should be attached to an empty GameObject in your scene
    
    private GameObject mainMenuCanvas;
    private GameObject levelSelectionCanvas;
    private Button playButton;
    
    void Start()
    {
        Debug.Log("EmergencyUIFix: Starting...");
        StartCoroutine(SetupUIWithDelay());
    }
    
    IEnumerator SetupUIWithDelay()
    {
        // Wait for a frame to make sure everything is initialized
        yield return null;
        
        // Find canvases
        mainMenuCanvas = GameObject.Find("MainMenuCanvas");
        levelSelectionCanvas = GameObject.Find("LevelSelectionCanvas");
        
        if (mainMenuCanvas == null)
        {
            Debug.LogError("MainMenuCanvas not found!");
            yield break;
        }
        
        if (levelSelectionCanvas == null)
        {
            Debug.LogError("LevelSelectionCanvas not found!");
            yield break;
        }
        
        // Make sure level selection is initially hidden
        levelSelectionCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        
        // Find play button
        Transform playButtonTransform = FindChildRecursive(mainMenuCanvas.transform, "PlayButton");
        if (playButtonTransform != null)
        {
            playButton = playButtonTransform.GetComponent<Button>();
            if (playButton != null)
            {
                // Clear existing listeners and add our own
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(OnPlayButtonClick);
                
                // Also add a direct click handler component
                DirectClickHandler clickHandler = playButton.gameObject.AddComponent<DirectClickHandler>();
                clickHandler.mainMenuCanvas = mainMenuCanvas;
                clickHandler.levelSelectionCanvas = levelSelectionCanvas;
                
                Debug.Log("EmergencyUIFix: Play button set up successfully!");
            }
            else
            {
                Debug.LogError("Play button doesn't have a Button component!");
            }
        }
        else
        {
            Debug.LogError("Play button not found in MainMenuCanvas!");
        }
        
        // Find back button
        Transform backButtonTransform = FindChildRecursive(levelSelectionCanvas.transform, "BackButton");
        if (backButtonTransform != null)
        {
            Button backButton = backButtonTransform.GetComponent<Button>();
            if (backButton != null)
            {
                // Clear existing listeners and add our own
                backButton.onClick.RemoveAllListeners();
                backButton.onClick.AddListener(OnBackButtonClick);
                
                // Also add a direct click handler component
                DirectClickHandler clickHandler = backButton.gameObject.AddComponent<DirectClickHandler>();
                clickHandler.mainMenuCanvas = mainMenuCanvas;
                clickHandler.levelSelectionCanvas = levelSelectionCanvas;
                clickHandler.isBackButton = true;
                
                Debug.Log("EmergencyUIFix: Back button set up successfully!");
            }
        }
    }
    
    public void OnPlayButtonClick()
    {
        Debug.Log("EmergencyUIFix: Play button clicked!");
        mainMenuCanvas.SetActive(false);
        levelSelectionCanvas.SetActive(true);
    }
    
    public void OnBackButtonClick()
    {
        Debug.Log("EmergencyUIFix: Back button clicked!");
        levelSelectionCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
    
    // Helper method to find a child transform recursively
    private Transform FindChildRecursive(Transform parent, string name)
    {
        if (parent.name == name)
            return parent;
            
        foreach (Transform child in parent)
        {
            Transform found = FindChildRecursive(child, name);
            if (found != null)
                return found;
        }
        
        return null;
    }
}

// This component is added directly to buttons
public class DirectClickHandler : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    public bool isBackButton = false;
    
    void Start()
    {
        // Get the button component
        Button button = GetComponent<Button>();
        if (button != null)
        {
            // Add our listener
            button.onClick.AddListener(OnButtonClick);
            Debug.Log("DirectClickHandler: Added click listener to " + gameObject.name);
        }
    }
    
    public void OnButtonClick()
    {
        Debug.Log("DirectClickHandler: Button " + gameObject.name + " clicked!");
        
        if (isBackButton)
        {
            // Back button functionality
            if (levelSelectionCanvas != null)
                levelSelectionCanvas.SetActive(false);
                
            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(true);
        }
        else
        {
            // Play button functionality
            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(false);
                
            if (levelSelectionCanvas != null)
                levelSelectionCanvas.SetActive(true);
        }
    }
}
