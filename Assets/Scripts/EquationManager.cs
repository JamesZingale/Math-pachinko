using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EquationManager : MonoBehaviour
{
    [Header("Equation Settings")]
    public int maxEquationLength = 10;
    public float evaluationDelay = 0.5f; // Time to wait before evaluating equation
    public bool allowMultipleOperators = true; // Allow more complex equations like 2+3*4
    
    [Header("UI References")]
    public TextMeshProUGUI equationText;
    public TextMeshProUGUI resultText;
    public GameObject feedbackPanel;
    public TextMeshProUGUI feedbackText;
    public Image feedbackBackground;
    
    [Header("Feedback Settings")]
    public Color successColor = new Color(0.2f, 0.8f, 0.2f);
    public Color errorColor = new Color(0.8f, 0.2f, 0.2f);
    public float feedbackDuration = 2f;
    public AudioClip successSound;
    public AudioClip errorSound;
    public GameObject successParticles;
    public GameObject errorParticles;
    
    private List<MathBall> equationSequence = new List<MathBall>();
    private LevelManager levelManager;
    private AudioSource audioSource;
    private bool isEvaluating = false;
    
    // Singleton pattern
    private static EquationManager _instance;
    public static EquationManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<EquationManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("EquationManager");
                    _instance = obj.AddComponent<EquationManager>();
                }
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        // Singleton setup
        if (_instance == null)
        {
            _instance = this;
            
            // Make sure this is a root GameObject before calling DontDestroyOnLoad
            if (transform.parent != null)
            {
                transform.SetParent(null); // Make it a root GameObject
            }
            DontDestroyOnLoad(gameObject);
            
            // Initialize
            ClearEquation();
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        
        // Add audio source if needed
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Hide feedback panel initially
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }
    
    private void Start()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
        ClearEquation();
    }
    
    public void AddToEquation(MathBall mathBall)
    {
        // Don't add to equation if we're already evaluating
        if (isEvaluating)
            return;
            
        // Check if we've reached the maximum equation length
        if (equationSequence.Count >= maxEquationLength)
        {
            ShowFeedback("Equation too long! Evaluating...", errorColor);
            EvaluateEquation();
            return;
        }
        
        // Add the ball to the equation
        equationSequence.Add(mathBall);
        UpdateEquationUI();
        
        // Check if we have a valid equation
        if (IsCompleteEquation())
        {
            // Wait a moment before evaluating to let the player see the complete equation
            StartCoroutine(DelayedEvaluation());
        }
    }
    
    private IEnumerator DelayedEvaluation()
    {
        isEvaluating = true;
        yield return new WaitForSeconds(evaluationDelay);
        EvaluateEquation();
        isEvaluating = false;
    }
    
    private bool IsCompleteEquation()
    {
        int count = equationSequence.Count;
        
        // Simple check: we need at least 3 elements (number, operator, number)
        if (count < 3)
            return false;
            
        // For simple equations (number, operator, number)
        if (count == 3)
        {
            return
                equationSequence[0].ballType == MathBall.BallType.Number &&
                equationSequence[1].ballType == MathBall.BallType.Operator &&
                equationSequence[2].ballType == MathBall.BallType.Number;
        }
        
        // For more complex equations
        if (allowMultipleOperators)
        {
            // Must end with a number
            if (equationSequence[count - 1].ballType != MathBall.BallType.Number)
                return false;
                
            // Must have alternating number/operator pattern
            bool isValid = true;
            for (int i = 0; i < count; i++)
            {
                if (i % 2 == 0) // Even positions should be numbers
                {
                    if (equationSequence[i].ballType != MathBall.BallType.Number)
                    {
                        isValid = false;
                        break;
                    }
                }
                else // Odd positions should be operators
                {
                    if (equationSequence[i].ballType != MathBall.BallType.Operator)
                    {
                        isValid = false;
                        break;
                    }
                }
            }
            return isValid;
        }
        
        return false;
    }
    
    private void UpdateEquationUI()
    {
        StringBuilder equation = new StringBuilder();
        
        foreach (MathBall ball in equationSequence)
        {
            equation.Append(ball.value + " ");
        }
        
        // Update UI text if available
        if (equationText != null)
        {
            equationText.text = equation.ToString();
        }
        
        // Also update through level manager for backward compatibility
        if (levelManager != null)
        {
            levelManager.UpdateEquation(equation.ToString());
        }
    }
    
    private void EvaluateEquation()
    {
        if (equationSequence.Count < 3)
        {
            ShowFeedback("Incomplete equation!", errorColor);
            ClearEquation();
            return;
        }
        
        // Build the equation string
        StringBuilder equationStr = new StringBuilder();
        foreach (MathBall ball in equationSequence)
        {
            equationStr.Append(ball.value);
        }
        
        try
        {
            // For complex equations with multiple operators
            if (allowMultipleOperators && equationSequence.Count > 3)
            {
                float result = EvaluateComplexEquation();
                HandleSuccessfulEquation(result);
            }
            else // Simple equation (number, operator, number)
            {
                string num1 = equationSequence[0].value;
                string op = equationSequence[1].value;
                string num2 = equationSequence[2].value;
                
                float result = CalculateResult(float.Parse(num1), op, float.Parse(num2));
                HandleSuccessfulEquation(result);
            }
        }
        catch (Exception e)
        {
            // Show error feedback
            ShowFeedback("Invalid equation: " + e.Message, errorColor);
            Debug.LogError($"Error evaluating equation: {e.Message}");
            ClearEquation();
        }
    }
    
    private void HandleSuccessfulEquation(float result)
    {
        // Build equation string for display
        StringBuilder equationStr = new StringBuilder();
        foreach (MathBall ball in equationSequence)
        {
            equationStr.Append(ball.value + " ");
        }
        
        // Award points based on the complexity of the equation
        int points = CalculatePoints(result);
        
        // Update result text
        if (resultText != null)
        {
            resultText.text = result.ToString();
        }
        
        // Show success feedback
        ShowFeedback($"{equationStr}= {result}\nPoints: +{points}", successColor);
        
        // Add score through level manager
        if (levelManager != null)
        {
            levelManager.AddScore(points);
        }
        
        // Log result
        Debug.Log($"Equation: {equationStr}= {result}, Points: {points}");
        
        // Reset equation after evaluation
        ClearEquation();
    }
    
    private float EvaluateComplexEquation()
    {
        // This is a simplified version that follows order of operations
        // For a real math parser, consider using a library or implementing a proper parser
        
        // First pass: extract all numbers and operators
        List<float> numbers = new List<float>();
        List<string> operators = new List<string>();
        
        for (int i = 0; i < equationSequence.Count; i++)
        {
            MathBall ball = equationSequence[i];
            if (ball.ballType == MathBall.BallType.Number)
            {
                numbers.Add(float.Parse(ball.value));
            }
            else if (ball.ballType == MathBall.BallType.Operator)
            {
                operators.Add(ball.value);
            }
        }
        
        // Sanity check: we should have one more number than operators
        if (numbers.Count != operators.Count + 1)
        {
            throw new ArgumentException("Malformed equation");
        }
        
        // Second pass: handle multiplication and division first
        for (int i = 0; i < operators.Count; i++)
        {
            if (operators[i] == "*" || operators[i] == "/")
            {
                float result;
                if (operators[i] == "*")
                {
                    result = numbers[i] * numbers[i + 1];
                }
                else // Division
                {
                    if (numbers[i + 1] == 0)
                        throw new DivideByZeroException("Cannot divide by zero");
                    result = numbers[i] / numbers[i + 1];
                }
                
                // Replace the two numbers with the result
                numbers[i] = result;
                
                // Remove the second number and the operator
                numbers.RemoveAt(i + 1);
                operators.RemoveAt(i);
                
                // Adjust index to account for removal
                i--;
            }
        }
        
        // Third pass: handle addition and subtraction
        float finalResult = numbers[0];
        for (int i = 0; i < operators.Count; i++)
        {
            if (operators[i] == "+")
            {
                finalResult += numbers[i + 1];
            }
            else if (operators[i] == "-")
            {
                finalResult -= numbers[i + 1];
            }
        }
        
        return finalResult;
    }
    
    private float CalculateResult(float num1, string op, float num2)
    {
        switch (op)
        {
            case "+":
                return num1 + num2;
            case "-":
                return num1 - num2;
            case "*":
                return num1 * num2;
            case "/":
                if (num2 == 0)
                    throw new DivideByZeroException("Cannot divide by zero");
                return num1 / num2;
            default:
                throw new ArgumentException($"Unknown operator: {op}");
        }
    }
    
    private int CalculatePoints(float result)
    {
        // Base points from the result value
        int basePoints = Mathf.Max(1, Mathf.RoundToInt(Mathf.Abs(result)));
        
        // Bonus points for longer equations
        int complexityBonus = (equationSequence.Count - 3) * 5;
        
        // Bonus for using multiplication or division
        int operationBonus = 0;
        foreach (MathBall ball in equationSequence)
        {
            if (ball.ballType == MathBall.BallType.Operator)
            {
                if (ball.value == "*" || ball.value == "/")
                {
                    operationBonus += 10;
                }
            }
        }
        
        return basePoints + complexityBonus + operationBonus;
    }
    
    private void ShowFeedback(string message, Color color)
    {
        // Play sound
        if (audioSource != null)
        {
            AudioClip clip = (color == successColor) ? successSound : errorSound;
            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
        
        // Show particles
        GameObject particlePrefab = (color == successColor) ? successParticles : errorParticles;
        if (particlePrefab != null)
        {
            GameObject particles = Instantiate(particlePrefab, transform.position, Quaternion.identity);
            Destroy(particles, 2f);
        }
        
        // Update and show feedback panel
        if (feedbackPanel != null && feedbackText != null)
        {
            feedbackText.text = message;
            
            if (feedbackBackground != null)
            {
                feedbackBackground.color = new Color(color.r, color.g, color.b, 0.8f);
            }
            
            feedbackPanel.SetActive(true);
            
            // Hide after duration
            StartCoroutine(HideFeedbackAfterDelay());
        }
    }
    
    private IEnumerator HideFeedbackAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);
        if (feedbackPanel != null)
        {
            feedbackPanel.SetActive(false);
        }
    }
    
    public void ClearEquation()
    {
        equationSequence.Clear();
        UpdateEquationUI();
        
        // Clear result text
        if (resultText != null)
        {
            resultText.text = "";
        }
    }
}
