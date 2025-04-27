using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DirectPlayButtonHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject mainMenuCanvas;
    public GameObject levelSelectionCanvas;
    
    private Button button;
    
    void Start()
    {
        Debug.Log("DirectPlayButtonHandler initialized on " + gameObject.name);
        
        // Get the Button component
        button = GetComponent<Button>();
        if (button != null)
        {
            // Set up onClick listener
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClick);
            Debug.Log("Play button onClick listener added");
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
        Debug.Log("Play button clicked!");
        
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(false);
            Debug.Log("Main menu hidden");
        }
        else
        {
            Debug.LogError("Main menu canvas reference is missing!");
        }
        
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
