using UnityEngine;

[System.Serializable]
public class Level : MonoBehaviour
{
    [Header("Level Settings")]
    public string levelName = "Level";
    public int levelNumber = 1;
    public int targetScore = 100;
    public float timeLimit = 60f;
    
    [Header("Math Elements")]
    public int numberBalls = 10;
    public int operatorBalls = 5;
    public bool additionOnly = false;
    public bool addSubtract = false;
    public bool allOperations = false;
    public int minNumberValue = 1;
    public int maxNumberValue = 9;
    
    [Header("Physics")]
    public float ballSpeed = 10f;
    public float gravity = 9.8f;
    public float bounciness = 0.7f;
    
    [Header("Layout")]
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -3f;
    public float maxY = 3f;
    public float minSpacing = 1.5f;
    
    [Header("Obstacles")]
    public int obstacles = 0;
    public bool movingObstacles = false;
    public float obstacleSpeed = 2f;
    
    void Start()
    {
        // Configure level settings when the scene starts
        ConfigureLevel();
    }
    
    public void ConfigureLevel()
    {
        // Set up level configuration
        LevelConfiguration config = new LevelConfiguration();
        config.levelName = levelName;
        config.targetScore = targetScore;
        config.timeLimit = timeLimit;
        config.numberOfNumberBalls = numberBalls;
        config.numberOfOperatorBalls = operatorBalls;
        
        // Configure operators based on level type
        if (additionOnly)
        {
            config.includeAddition = true;
            config.includeSubtraction = false;
            config.includeMultiplication = false;
            config.includeDivision = false;
        }
        else if (addSubtract)
        {
            config.includeAddition = true;
            config.includeSubtraction = true;
            config.includeMultiplication = false;
            config.includeDivision = false;
        }
        else if (allOperations)
        {
            config.includeAddition = true;
            config.includeSubtraction = true;
            config.includeMultiplication = true;
            config.includeDivision = true;
        }
        
        config.minNumberValue = minNumberValue;
        config.maxNumberValue = maxNumberValue;
        config.ballSpeed = ballSpeed;
        config.gravity = gravity;
        config.bounciness = bounciness;
        config.minX = minX;
        config.maxX = maxX;
        config.minY = minY;
        config.maxY = maxY;
        config.minSpacing = minSpacing;
        config.numberOfObstacles = obstacles;
        config.includeMovingObstacles = movingObstacles;
        config.obstacleSpeed = obstacleSpeed;
        
        // Find level generator and apply configuration
        LevelGenerator levelGenerator = FindObjectOfType<LevelGenerator>();
        if (levelGenerator != null)
        {
            levelGenerator.levelConfig = config;
            levelGenerator.GenerateLevel();
        }
        
        // Apply physics settings
        Physics.gravity = new Vector3(0, -config.gravity, 0);
    }
}
