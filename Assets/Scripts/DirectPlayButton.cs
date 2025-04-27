using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class DirectPlayButton : MonoBehaviour, IPointerClickHandler
{
    // This script should be attached directly to the Play button
    
    private void Start()
    {
        Debug.Log("DirectPlayButton initialized on " + gameObject.name);
        
        // Also set up the Button component's onClick event as a backup
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
            Debug.Log("Button onClick listener added");
        }
    }
    
    // This gets called when the button is clicked via the UI system
    public void OnPointerClick(PointerEventData eventData)
    {
        OnButtonClicked();
    }
    
    // This gets called when the button is clicked via the Button component
    public void OnButtonClicked()
    {
        Debug.Log("Play button clicked directly!");
        
        // Find the canvases
        GameObject mainMenuCanvas = GameObject.Find("MainMenuCanvas");
        GameObject levelSelectionCanvas = GameObject.Find("LevelSelectionCanvas");
        
        // Log what we found
        if (mainMenuCanvas != null)
            Debug.Log("Found MainMenuCanvas");
        else
            Debug.LogError("MainMenuCanvas not found!");
            
        if (levelSelectionCanvas != null)
            Debug.Log("Found LevelSelectionCanvas");
        else
            Debug.LogError("LevelSelectionCanvas not found!");
        
        // Switch canvases
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
            
        if (levelSelectionCanvas != null)
            levelSelectionCanvas.SetActive(true);
    }
    
    // Add this method so it can be called directly from the Inspector
    public void SwitchToLevelSelection()
    {
        OnButtonClicked();
    }
}
