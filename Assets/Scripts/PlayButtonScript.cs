using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButtonScript : MonoBehaviour
{
    // Assign these in the Inspector
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    
    void Start()
    {
        // Get the Button component and add a click listener
        Button button = GetComponent<Button>();
        if (button != null)
        {
            // Clear any existing listeners
            button.onClick.RemoveAllListeners();
            
            // Add our listener
            button.onClick.AddListener(OnPlayButtonClick);
            
            Debug.Log("Play button listener added successfully");
        }
        else
        {
            Debug.LogError("No Button component found on " + gameObject.name);
        }
    }
    
    public void OnPlayButtonClick()
    {
        Debug.Log("Play button clicked!");
        
        // Hide main menu
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
            Debug.Log("Main menu hidden");
        }
        else
        {
            Debug.LogError("Main menu canvas reference is missing!");
        }
        
        // Show level selection
        if (levelSelectionCanvas != null)
        {
            levelSelectionCanvas.SetActive(true);
            Debug.Log("Level selection shown");
        }
        else
        {
            Debug.LogError("Level selection canvas reference is missing!");
        }
    }
}
