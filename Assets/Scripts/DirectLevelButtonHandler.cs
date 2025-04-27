using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class DirectLevelButtonHandler : MonoBehaviour, IPointerClickHandler
{
    public int levelIndex;
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    public GameObject gameUICanvas;
    
    private Button button;
    
    void Start()
    {
        Debug.Log("DirectLevelButtonHandler initialized on " + gameObject.name + " for level " + levelIndex);
        
        // Get the Button component
        button = GetComponent<Button>();
        if (button != null)
        {
            // Set up onClick listener
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
            Debug.Log("Level button onClick listener added");
        }
    }
    
    // This gets called when the button is clicked via the UI system
    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClick();
    }
    
    // This gets called when the button is clicked via the Button component
    public void OnButtonClick()
    {
        Debug.Log("Level " + levelIndex + " button clicked!");
        
        string levelSceneName = "Level" + levelIndex;
        
        // Check if the scene exists in the build settings
        bool sceneExists = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (sceneName == levelSceneName)
            {
                sceneExists = true;
                break;
            }
        }
        
        if (sceneExists)
        {
            Debug.Log("Loading scene: " + levelSceneName);
            SceneManager.LoadScene(levelSceneName);
        }
        else
        {
            Debug.LogWarning("Scene " + levelSceneName + " does not exist in build settings. Showing game UI for testing.");
            
            // For testing, show the game UI
            if (mainMenuCanvas != null)
                mainMenuCanvas.SetActive(false);
                
            if (levelSelectionCanvas != null)
                levelSelectionCanvas.SetActive(false);
                
            if (gameUICanvas != null)
                gameUICanvas.SetActive(true);
        }
    }
}
