using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelButtonHandler : MonoBehaviour
{
    public int levelIndex;
    
    public void OnLevelButtonClick()
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
            SceneManager.LoadScene(levelSceneName);
        }
        else
        {
            Debug.LogWarning("Scene " + levelSceneName + " does not exist in build settings. Make sure to add it.");
            
            // For testing, show the game UI
            GameObject mainMenuCanvas = GameObject.Find("MainMenuCanvas");
            GameObject levelSelectionCanvas = GameObject.Find("LevelSelectionCanvas");
            GameObject gameUICanvas = GameObject.Find("GameUICanvas");
            
            if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
            if (levelSelectionCanvas != null) levelSelectionCanvas.SetActive(false);
            if (gameUICanvas != null) gameUICanvas.SetActive(true);
        }
    }
}
