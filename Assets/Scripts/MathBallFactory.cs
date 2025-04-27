using UnityEngine;
using System.Collections.Generic;

public class MathBallFactory : MonoBehaviour
{
    [System.Serializable]
    public class BallPrefab
    {
        public string value;
        public GameObject prefab;
    }

    public List<BallPrefab> numberBalls = new List<BallPrefab>();
    public List<BallPrefab> operatorBalls = new List<BallPrefab>();

    // Singleton pattern
    private static MathBallFactory _instance;
    public static MathBallFactory Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<MathBallFactory>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("MathBallFactory");
                    _instance = obj.AddComponent<MathBallFactory>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Get a random number ball
    public GameObject GetRandomNumberBall()
    {
        if (numberBalls.Count == 0)
            return null;

        int randomIndex = Random.Range(0, numberBalls.Count);
        return numberBalls[randomIndex].prefab;
    }
    
    // Get a specific number ball
    public GameObject GetNumberBall(int number)
    {
        string numStr = number.ToString();
        foreach (BallPrefab ball in numberBalls)
        {
            if (ball.value == numStr)
                return ball.prefab;
        }
        
        Debug.LogWarning("Number ball not found: " + number);
        return GetRandomNumberBall();
    }

    // Get a random operator ball
    public GameObject GetRandomOperatorBall()
    {
        if (operatorBalls.Count == 0)
            return null;

        int randomIndex = Random.Range(0, operatorBalls.Count);
        return operatorBalls[randomIndex].prefab;
    }
    
    // Get a specific operator ball
    public GameObject GetOperatorBall(string op)
    {
        foreach (BallPrefab ball in operatorBalls)
        {
            if (ball.value == op)
                return ball.prefab;
        }
        
        Debug.LogWarning("Operator ball not found: " + op);
        return GetRandomOperatorBall();
    }

    // Get a specific ball by value
    public GameObject GetBallByValue(string value)
    {
        // Check number balls
        foreach (BallPrefab ball in numberBalls)
        {
            if (ball.value == value)
                return ball.prefab;
        }

        // Check operator balls
        foreach (BallPrefab ball in operatorBalls)
        {
            if (ball.value == value)
                return ball.prefab;
        }

        return null;
    }
}
