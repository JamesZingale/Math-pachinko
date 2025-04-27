using UnityEngine;

[System.Serializable]
public class LevelConfiguration
{
    public string levelName = "Level";
    public int targetScore = 100;
    public float timeLimit = 60f;
    
    [Header("Math Elements")]
    public int numberOfNumberBalls = 10;
    public int numberOfOperatorBalls = 5;
    public bool includeAddition = true;
    public bool includeSubtraction = true;
    public bool includeMultiplication = false;
    public bool includeDivision = false;
    public int minNumberValue = 1;
    public int maxNumberValue = 9;
    
    [Header("Physics")]
    public float ballSpeed = 10f;
    public float gravity = 9.8f;
    public float bounciness = 0.7f;
    
    [Header("Layout")]
    public bool useRandomLayout = true;
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -3f;
    public float maxY = 3f;
    public float minSpacing = 1.5f;
    
    [Header("Obstacles")]
    public int numberOfObstacles = 0;
    public bool includeMovingObstacles = false;
    public float obstacleSpeed = 2f;
}

public class LevelConfigurationManager : MonoBehaviour
{
    public static LevelConfigurationManager Instance { get; private set; }
    
    public LevelConfiguration[] levelConfigurations;
    public LevelConfiguration currentLevel;
    private int currentLevelIndex = 0;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // If no configurations are set, create defaults
            if (levelConfigurations == null || levelConfigurations.Length == 0)
            {
                CreateDefaultLevels();
            }
            
            // Set the current level to the first one
            if (levelConfigurations.Length > 0)
            {
                currentLevel = levelConfigurations[0];
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void CreateDefaultLevels()
    {
        levelConfigurations = new LevelConfiguration[3];
        
        // Level 1 - Easy
        levelConfigurations[0] = new LevelConfiguration
        {
            levelName = "Level 1 - Addition",
            targetScore = 50,
            timeLimit = 90f,
            numberOfNumberBalls = 8,
            numberOfOperatorBalls = 3,
            includeAddition = true,
            includeSubtraction = false,
            includeMultiplication = false,
            includeDivision = false,
            minNumberValue = 1,
            maxNumberValue = 5,
            ballSpeed = 8f,
            gravity = 9.8f,
            bounciness = 0.7f,
            useRandomLayout = true,
            numberOfObstacles = 0,
            includeMovingObstacles = false
        };
        
        // Level 2 - Medium
        levelConfigurations[1] = new LevelConfiguration
        {
            levelName = "Level 2 - Addition & Subtraction",
            targetScore = 100,
            timeLimit = 75f,
            numberOfNumberBalls = 10,
            numberOfOperatorBalls = 5,
            includeAddition = true,
            includeSubtraction = true,
            includeMultiplication = false,
            includeDivision = false,
            minNumberValue = 1,
            maxNumberValue = 9,
            ballSpeed = 10f,
            gravity = 9.8f,
            bounciness = 0.6f,
            useRandomLayout = true,
            numberOfObstacles = 3,
            includeMovingObstacles = false
        };
        
        // Level 3 - Hard
        levelConfigurations[2] = new LevelConfiguration
        {
            levelName = "Level 3 - All Operations",
            targetScore = 150,
            timeLimit = 60f,
            numberOfNumberBalls = 12,
            numberOfOperatorBalls = 8,
            includeAddition = true,
            includeSubtraction = true,
            includeMultiplication = true,
            includeDivision = true,
            minNumberValue = 1,
            maxNumberValue = 9,
            ballSpeed = 12f,
            gravity = 10.5f,
            bounciness = 0.5f,
            useRandomLayout = true,
            numberOfObstacles = 5,
            includeMovingObstacles = true,
            obstacleSpeed = 3f
        };
    }
    
    public void SetLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levelConfigurations.Length)
        {
            currentLevelIndex = levelIndex;
            currentLevel = levelConfigurations[levelIndex];
        }
        else
        {
            Debug.LogError("Invalid level index: " + levelIndex);
        }
    }
    
    public LevelConfiguration GetCurrentLevel()
    {
        return currentLevel;
    }
    
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }
    
    public void LoadNextLevel()
    {
        SetLevel(currentLevelIndex + 1);
    }
}
