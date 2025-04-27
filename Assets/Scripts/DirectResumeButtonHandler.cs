using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DirectResumeButtonHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject pauseMenuCanvas;
    
    private Button button;
    
    void Start()
    {
        Debug.Log("DirectResumeButtonHandler initialized on " + gameObject.name);
        
        // Get the Button component
        button = GetComponent<Button>();
        if (button != null)
        {
            // Set up onClick listener
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
            Debug.Log("Resume button onClick listener added");
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
        Debug.Log("Resume button clicked!");
        
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
            Debug.Log("Pause menu hidden");
        }
        else
        {
            Debug.LogError("Pause menu canvas reference is missing!");
        }
        
        // Resume game time
        Time.timeScale = 1f;
    }
}
