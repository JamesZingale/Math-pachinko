using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("Level Settings")]
    public LevelConfiguration levelConfig;
    
    [Header("References")]
    public MathBallFactory mathBallFactory;
    public GameObject obstaclePrefab;
    public GameObject movingObstaclePrefab;
    
    private List<Vector2> occupiedPositions = new List<Vector2>();
    private List<GameObject> levelObjects = new List<GameObject>();
    
    private void Start()
    {
        if (mathBallFactory == null)
        {
            mathBallFactory = FindFirstObjectByType<MathBallFactory>();
        }
        
        // Get level configuration from manager if not set
        if (levelConfig == null)
        {
            LevelConfigurationManager configManager = FindFirstObjectByType<LevelConfigurationManager>();
            if (configManager != null)
            {
                levelConfig = configManager.GetCurrentLevel();
                Debug.Log("Using level configuration: " + levelConfig.levelName);
            }
            else
            {
                Debug.LogWarning("No LevelConfigurationManager found. Using default settings.");
                levelConfig = new LevelConfiguration();
            }
        }
        
        GenerateLevel();
    }
    
    public void GenerateLevel()
    {
        // Clear any existing level elements
        ClearLevel();
        
        // Apply physics settings
        Physics.gravity = new Vector3(0, -levelConfig.gravity, 0);
        
        // Generate number balls
        for (int i = 0; i < levelConfig.numberOfNumberBalls; i++)
        {
            PlaceRandomBall(true); // true for number balls
        }
        
        // Generate operator balls
        for (int i = 0; i < levelConfig.numberOfOperatorBalls; i++)
        {
            PlaceRandomBall(false); // false for operator balls
        }
        
        // Generate obstacles
        for (int i = 0; i < levelConfig.numberOfObstacles; i++)
        {
            PlaceObstacle(levelConfig.includeMovingObstacles && Random.value > 0.5f);
        }
        
        // Update UI elements with level info
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        // Find GameUI script and update it with level info
        GameUI gameUI = FindFirstObjectByType<GameUI>();
        if (gameUI != null)
        {
            gameUI.UpdateLevelInfo(levelConfig.levelName, levelConfig.targetScore, levelConfig.timeLimit);
        }
    }
    
    private void PlaceRandomBall(bool isNumber)
    {
        Vector2 position = GetRandomAvailablePosition();
        GameObject ballPrefab = null;
        
        if (isNumber)
        {
            // Get a number ball within the level's range
            int number = Random.Range(levelConfig.minNumberValue, levelConfig.maxNumberValue + 1);
            ballPrefab = mathBallFactory.GetNumberBall(number);
        }
        else
        {
            // Get an operator ball based on level configuration
            List<string> allowedOperators = new List<string>();
            if (levelConfig.includeAddition) allowedOperators.Add("+");
            if (levelConfig.includeSubtraction) allowedOperators.Add("-");
            if (levelConfig.includeMultiplication) allowedOperators.Add("*");
            if (levelConfig.includeDivision) allowedOperators.Add("/");
            
            if (allowedOperators.Count > 0)
            {
                string op = allowedOperators[Random.Range(0, allowedOperators.Count)];
                ballPrefab = mathBallFactory.GetOperatorBall(op);
            }
        }
        
        if (ballPrefab != null)
        {
            GameObject ball = Instantiate(ballPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            ball.transform.SetParent(transform);
            
            // Set physics properties
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                PhysicsMaterial physicsMaterial = rb.GetComponent<Collider>()?.material as PhysicsMaterial;
                if (physicsMaterial != null)
                {
                    physicsMaterial.bounciness = levelConfig.bounciness;
                }
            }
            
            levelObjects.Add(ball);
            occupiedPositions.Add(position);
        }
    }
    
    private void PlaceObstacle(bool isMoving)
    {
        Vector2 position = GetRandomAvailablePosition();
        GameObject obstaclePrefabToUse = isMoving ? movingObstaclePrefab : obstaclePrefab;
        
        if (obstaclePrefabToUse != null)
        {
            GameObject obstacle = Instantiate(obstaclePrefabToUse, new Vector3(position.x, position.y, 0), Quaternion.identity);
            obstacle.transform.SetParent(transform);
            
            // Configure moving obstacle
            if (isMoving)
            {
                MovingObstacle movingObstacleScript = obstacle.GetComponent<MovingObstacle>();
                if (movingObstacleScript != null)
                {
                    movingObstacleScript.speed = levelConfig.obstacleSpeed;
                    
                    // Set random direction
                    float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
                    movingObstacleScript.direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                }
            }
            
            levelObjects.Add(obstacle);
            occupiedPositions.Add(position);
        }
        else
        {
            Debug.LogWarning("Obstacle prefab is missing!");
        }
    }
    
    private Vector2 GetRandomAvailablePosition()
    {
        Vector2 position;
        int maxAttempts = 100; // Prevent infinite loops
        int attempts = 0;
        
        do
        {
            position = new Vector2(
                Random.Range(levelConfig.minX, levelConfig.maxX),
                Random.Range(levelConfig.minY, levelConfig.maxY)
            );
            attempts++;
            
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Could not find available position after " + maxAttempts + " attempts");
                break;
            }
        }
        while (IsPositionOccupied(position));
        
        return position;
    }
    
    private bool IsPositionOccupied(Vector2 position)
    {
        foreach (Vector2 occupiedPos in occupiedPositions)
        {
            if (Vector2.Distance(occupiedPos, position) < levelConfig.minSpacing)
            {
                return true;
            }
        }
        return false;
    }
    
    private void ClearLevel()
    {
        // Remove all level objects
        foreach (GameObject obj in levelObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        
        levelObjects.Clear();
        occupiedPositions.Clear();
    }
    
    // This can be called from a UI button to regenerate the level
    public void RegenerateLevel()
    {
        GenerateLevel();
    }
}
