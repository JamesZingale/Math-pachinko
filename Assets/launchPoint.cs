using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BallShooter : MonoBehaviour
{
    public static BallShooter Instance;

    public GameObject ballPrefab;
    public GameObject levelCompletePanel;
    public Transform launchPoint;
    public float shootForce = 10f;

    public int firstNumber = 0;
    public int secondNumber = 0;
    public string currentOperator = "+";
    public int totalScore = 0;
    public int neededScore = 10;
    public string nextlevel;
    public TMP_Text scoreText;

    private bool waitingForSecondShot = false;
    private bool isBallShot = false;
    private GameObject currentBall;

    public TMP_Text firstNumberText;
    public TMP_Text operatorText;
    public TMP_Text secondNumberText;
    public TMP_Text resultText;
    public TMP_Text finalScoreText; // New!

    private void UpdateUI()
    {
        if (firstNumberText != null) firstNumberText.text = "First Number:" + firstNumber.ToString();
        if (operatorText != null) operatorText.text = "Operator:" + currentOperator;
        if (secondNumberText != null) secondNumberText.text = "Second Number:" + secondNumber.ToString();
        if (scoreText != null) scoreText.text = "Score: " + totalScore.ToString() + "/" + neededScore;

    }

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isBallShot)
        {
            ShootBall();
        }
    }

    private void ShootBall()
    {
        Vector3 dir = GetMouseDirection();
        if (dir == Vector3.zero) return;

        currentBall = Instantiate(ballPrefab, launchPoint.position, Quaternion.identity);
        Rigidbody rb = currentBall.GetComponent<Rigidbody>();

        dir.x = 0f; // Lock X axis
        rb.AddForce(dir.normalized * shootForce, ForceMode.Impulse);

        isBallShot = true;
    }

    public void AddHitNumber(int value)
    {
        if (!waitingForSecondShot)
        {
            firstNumber += value;
        }
        else
        {
            secondNumber += value;
        }
        UpdateUI();
    }

    public void SetOperator(OperatorZone.OperatorType opType)
    {
        if (!waitingForSecondShot)
        {
            switch (opType)
            {
                case OperatorZone.OperatorType.Add:
                    currentOperator = "+";
                    break;
                case OperatorZone.OperatorType.Multiply:
                    currentOperator = "*";
                    break;
            }
            UpdateUI();
        }
    }

    public void BallHitFloor()
    {
        isBallShot = false;

        if (!waitingForSecondShot)
        {
            waitingForSecondShot = true;
        }
        else
        {
            CalculateFinalResult();
        }
    }

    private void CalculateFinalResult()
    {
        int result = 0;

        if (currentOperator == "+")
        {
            result = firstNumber + secondNumber;
        }
        else if (currentOperator == "*")
        {
            result = firstNumber * secondNumber;
        }

        if (resultText != null) resultText.text = "Result:" + result.ToString();

        Debug.Log($"Final Result: {firstNumber} {currentOperator} {secondNumber} = {result}");

        totalScore += result;

        UpdateUI();

        firstNumber = 0;
        secondNumber = 0;
        currentOperator = "+";
        waitingForSecondShot = false;

        // 🛑 Check for level complete AFTER updating the score
        if (totalScore >= neededScore)
        {
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        if (levelCompletePanel != null)
        {
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + totalScore.ToString() + "/" + neededScore;
            }

            levelCompletePanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }

    private Vector3 GetMouseDirection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.right, launchPoint.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);
            return hitPoint - launchPoint.position;
        }

        return Vector3.zero;
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextlevel);
    }
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
