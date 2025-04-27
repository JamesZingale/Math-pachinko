using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DirectBackButtonHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    
    private Button button;
    
    void Start()
    {
        Debug.Log("DirectBackButtonHandler initialized on " + gameObject.name);
        
        // Get the Button component
        button = GetComponent<Button>();
        if (button != null)
        {
            // Set up onClick listener
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
            Debug.Log("Back button onClick listener added");
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
        Debug.Log("Back button clicked!");
        
        if (levelSelectionCanvas != null)
        {
            levelSelectionCanvas.SetActive(false);
            Debug.Log("Level selection hidden");
        }
        else
        {
            Debug.LogError("Level selection canvas reference is missing!");
        }
        
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
            Debug.Log("Main menu shown");
        }
        else
        {
            Debug.LogError("Main menu canvas reference is missing!");
        }
    }
}
