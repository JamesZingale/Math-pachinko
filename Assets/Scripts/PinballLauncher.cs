using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PinballLauncher : MonoBehaviour
{
    [Header("Launcher Settings")]
    public float minPower = 5f;
    public float maxPower = 20f;
    public float powerChargeRate = 10f; // How fast power increases when holding
    public float aimSpeed = 60f; // Degrees per second
    public float minAngle = -45f; // Minimum launch angle
    public float maxAngle = 45f;  // Maximum launch angle
    
    [Header("References")]
    public GameObject ballPrefab;
    public Transform launchPoint;
    public LineRenderer trajectoryLine;
    public Transform launcherVisual; // The visual part of the launcher that rotates/animates
    public ParticleSystem launchParticles; // Optional particle effect when launching
    public AudioSource launchSound; // Optional sound effect when launching
    
    [Header("UI Elements")]
    public Slider powerSlider; // Optional UI slider to show power
    public Image aimIndicator; // Optional UI image for aiming direction
    
    [Header("Input Settings")]
    public bool enableMouseInput = true;
    public bool enableTouchInput = true;
    public bool enableKeyboardInput = true;
    public LayerMask aimPlane; // Layer to use for raycasting when aiming with mouse/touch
    
    [Header("Visual Feedback")]
    public int trajectoryPoints = 10; // Number of points in the trajectory prediction
    public float trajectoryTimeStep = 0.1f; // Time step for physics prediction
    public float maxTrajectoryTime = 1.0f; // Maximum time to predict
    public Gradient trajectoryGradient; // Color gradient for trajectory line
    public float launcherStretchFactor = 0.2f; // How much the launcher stretches when charging
    
    private float currentPower;
    private bool isCharging = false;
    private bool canLaunch = true;
    private Vector3 originalLauncherScale;
    private Vector3 aimTarget;
    private Camera mainCamera;
    private Vector2 touchStartPosition;
    private bool isTouching = false;
    
    private void Start()
    {
        // Initialize trajectory line if available
        if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = trajectoryPoints;
            trajectoryLine.enabled = false;
            
            // Set up gradient if provided
            if (trajectoryGradient != null)
                trajectoryLine.colorGradient = trajectoryGradient;
        }
        
        // Store original scale of launcher visual
        if (launcherVisual != null)
            originalLauncherScale = launcherVisual.localScale;
            
        // Initialize power slider
        if (powerSlider != null)
        {
            powerSlider.minValue = minPower;
            powerSlider.maxValue = maxPower;
            powerSlider.value = minPower;
            powerSlider.gameObject.SetActive(false);
        }
        
        // Get main camera reference
        mainCamera = Camera.main;
    }
    
    private void Update()
    {
        HandleInput();
        
        if (isCharging)
        {
            ChargePower();
            UpdateLauncherVisuals();
            PredictTrajectory();
        }
    }
    
    private void HandleInput()
    {
        // Keyboard input (arrow keys + spacebar)
        if (enableKeyboardInput)
        {
            HandleKeyboardInput();
        }
        
        // Mouse input
        if (enableMouseInput && Input.mousePresent)
        {
            HandleMouseInput();
        }
        
        // Touch input for mobile
        if (enableTouchInput && Input.touchCount > 0)
        {
            HandleTouchInput();
        }
    }
    
    private void HandleKeyboardInput()
    {
        // Handle aiming with left/right arrow keys
        float rotationInput = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rotationInput = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            rotationInput = 1;
        }
        
        // Rotate the launcher
        if (rotationInput != 0)
        {
            RotateLauncher(rotationInput * aimSpeed * Time.deltaTime);
        }
        
        // Handle power charging with spacebar
        if (Input.GetKeyDown(KeyCode.Space) && canLaunch)
        {
            StartCharging();
        }
        
        if (Input.GetKeyUp(KeyCode.Space) && isCharging)
        {
            LaunchBall();
        }
    }
    
    private void HandleMouseInput()
    {
        // Aim by pointing mouse at target position
        if (mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, 100f, aimPlane))
            {
                aimTarget = hit.point;
                AimAtPosition(aimTarget);
            }
        }
        
        // Start charging on mouse down
        if (Input.GetMouseButtonDown(0) && canLaunch)
        {
            StartCharging();
        }
        
        // Launch on mouse up
        if (Input.GetMouseButtonUp(0) && isCharging)
        {
            LaunchBall();
        }
    }
    
    private void HandleTouchInput()
    {
        Touch touch = Input.GetTouch(0);
        
        // Handle touch phases
        switch (touch.phase)
        {
            case TouchPhase.Began:
                if (canLaunch)
                {
                    touchStartPosition = touch.position;
                    isTouching = true;
                    
                    // Check if touch is on the launcher area (bottom of screen)
                    if (touch.position.y < Screen.height * 0.3f)
                    {
                        StartCharging();
                    }
                }
                break;
                
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                if (isTouching)
                {
                    // Calculate aim direction based on touch drag
                    Vector2 touchDelta = touch.position - (Vector2)touchStartPosition;
                    float dragDistance = touchDelta.magnitude;
                    
                    // Only adjust aim if drag is significant
                    if (dragDistance > 20f)
                    {
                        // Convert touch position to world position for aiming
                        Ray ray = mainCamera.ScreenPointToRay(touch.position);
                        RaycastHit hit;
                        
                        if (Physics.Raycast(ray, out hit, 100f, aimPlane))
                        {
                            aimTarget = hit.point;
                            AimAtPosition(aimTarget);
                        }
                    }
                }
                break;
                
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                if (isTouching && isCharging)
                {
                    LaunchBall();
                    isTouching = false;
                }
                break;
        }
    }
    
    private void AimAtPosition(Vector3 targetPosition)
    {
        // Calculate direction to target (ignoring Y to keep launcher horizontal)
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        
        if (direction.magnitude > 0.1f)
        {
            // Calculate target rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // Extract Y rotation angle
            float targetAngle = targetRotation.eulerAngles.y;
            
            // Convert to -180 to 180 range for easier comparison
            if (targetAngle > 180) targetAngle -= 360;
            
            // Clamp to min/max angles
            targetAngle = Mathf.Clamp(targetAngle, minAngle, maxAngle);
            
            // Apply rotation
            transform.rotation = Quaternion.Euler(0, targetAngle, 0);
            
            // Update aim indicator if available
            UpdateAimIndicator();
        }
    }
    
    private void RotateLauncher(float rotationAmount)
    {
        float newRotation = transform.rotation.eulerAngles.y + rotationAmount;
        
        // Convert to -180 to 180 range for easier comparison
        if (newRotation > 180) newRotation -= 360;
        
        // Clamp to min/max angles
        newRotation = Mathf.Clamp(newRotation, minAngle, maxAngle);
        
        transform.rotation = Quaternion.Euler(0, newRotation, 0);
        
        // Update aim indicator if available
        UpdateAimIndicator();
    }
    
    private void UpdateAimIndicator()
    {
        if (aimIndicator != null)
        {
            // Calculate normalized angle (0-1) for UI positioning
            float normalizedAngle = Mathf.InverseLerp(minAngle, maxAngle, transform.rotation.eulerAngles.y > 180 ? 
                transform.rotation.eulerAngles.y - 360 : transform.rotation.eulerAngles.y);
            
            // Update indicator position
            aimIndicator.rectTransform.anchorMin = new Vector2(normalizedAngle, 0);
            aimIndicator.rectTransform.anchorMax = new Vector2(normalizedAngle, 0);
        }
    }
    
    private void StartCharging()
    {
        isCharging = true;
        currentPower = minPower;
        
        // Show trajectory line
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = true;
        }
        
        // Show power slider
        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(true);
            powerSlider.value = currentPower;
        }
    }
    
    private void ChargePower()
    {
        currentPower += powerChargeRate * Time.deltaTime;
        currentPower = Mathf.Clamp(currentPower, minPower, maxPower);
        
        // Update power slider
        if (powerSlider != null)
        {
            powerSlider.value = currentPower;
        }
    }
    
    private void UpdateLauncherVisuals()
    {
        if (launcherVisual != null)
        {
            // Calculate power ratio (0-1)
            float powerRatio = Mathf.InverseLerp(minPower, maxPower, currentPower);
            
            // Scale the launcher based on power (compress it as power increases)
            Vector3 newScale = originalLauncherScale;
            newScale.z = originalLauncherScale.z * (1f - powerRatio * launcherStretchFactor);
            launcherVisual.localScale = newScale;
        }
    }
    
    private void PredictTrajectory()
    {
        if (trajectoryLine == null || launchPoint == null) return;
        
        // Create a temporary physics scene to simulate the trajectory
        Vector3[] points = new Vector3[trajectoryPoints];
        Vector3 startPos = launchPoint.position;
        Vector3 startVelocity = transform.forward * currentPower;
        
        points[0] = startPos;
        
        // Simple physics prediction
        Vector3 pos = startPos;
        Vector3 vel = startVelocity;
        float timeStep = trajectoryTimeStep;
        float maxTime = maxTrajectoryTime;
        
        for (int i = 1; i < trajectoryPoints; i++)
        {
            float timeOffset = timeStep * i;
            
            if (timeOffset > maxTime)
                break;
                
            // Apply gravity
            vel += Physics.gravity * timeStep;
            pos += vel * timeStep;
            
            points[i] = pos;
        }
        
        // Update line renderer
        trajectoryLine.positionCount = trajectoryPoints;
        trajectoryLine.SetPositions(points);
    }
    
    private void LaunchBall()
    {
        isCharging = false;
        
        // Hide trajectory line
        if (trajectoryLine != null)
        {
            trajectoryLine.enabled = false;
        }
        
        // Hide power slider
        if (powerSlider != null)
        {
            powerSlider.gameObject.SetActive(false);
        }
        
        // Reset launcher visual
        if (launcherVisual != null)
        {
            launcherVisual.localScale = originalLauncherScale;
        }
        
        // Play launch particles
        if (launchParticles != null)
        {
            launchParticles.Play();
        }
        
        // Play launch sound
        if (launchSound != null)
        {
            launchSound.Play();
        }
        
        // Instantiate ball at launch point
        if (ballPrefab != null && launchPoint != null)
        {
            GameObject ball = Instantiate(ballPrefab, launchPoint.position, Quaternion.identity);
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            
            if (rb != null)
            {
                // Apply force in the direction the launcher is facing with current power
                rb.AddForce(transform.forward * currentPower, ForceMode.Impulse);
                
                // Add a small amount of random rotation for more natural movement
                rb.AddTorque(Random.insideUnitSphere * currentPower * 0.1f, ForceMode.Impulse);
                
                // Prevent launching another ball for a short time
                canLaunch = false;
                Invoke("EnableLaunching", 1.5f);
            }
        }
    }
    
    private void EnableLaunching()
    {
        canLaunch = true;
    }
}
