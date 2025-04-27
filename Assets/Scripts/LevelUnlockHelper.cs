using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class LevelUnlockHelper : MonoBehaviour
{
    [MenuItem("Math Pachinko/Unlock All Levels")]
    public static void UnlockAllLevels()
    {
        // Find how many levels exist in the build settings
        int levelCount = 0;
        
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneName.StartsWith("Level"))
            {
                levelCount++;
            }
        }
        
        // If no levels found in build settings, default to 3
        if (levelCount == 0)
        {
            levelCount = 3;
        }
        
        Debug.Log($"Found {levelCount} levels to unlock");
        
        // Mark all levels as completed in PlayerPrefs
        for (int i = 1; i <= levelCount; i++)
        {
            PlayerPrefs.SetInt($"Level{i}Completed", 1);
            Debug.Log($"Unlocked Level{i}");
        }
        
        PlayerPrefs.Save();
        Debug.Log("All levels have been unlocked!");
    }
    
    [MenuItem("Math Pachinko/Reset Level Unlocks")]
    public static void ResetLevelUnlocks()
    {
        // Find how many levels exist
        int levelCount = 0;
        
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneName.StartsWith("Level"))
            {
                levelCount++;
            }
        }
        
        // If no levels found in build settings, default to 3
        if (levelCount == 0)
        {
            levelCount = 3;
        }
        
        // Reset all level completion status
        for (int i = 1; i <= levelCount; i++)
        {
            if (i == 1)
            {
                // Level 1 is always unlocked
                PlayerPrefs.SetInt($"Level{i}Completed", 0);
            }
            else
            {
                PlayerPrefs.SetInt($"Level{i}Completed", 0);
            }
        }
        
        PlayerPrefs.Save();
        Debug.Log("All level unlocks have been reset. Only Level 1 is now accessible.");
    }
}
#endif
