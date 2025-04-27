using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class VisualEffectsManager : MonoBehaviour
{
    [Header("Particle Effects")]
    public GameObject ballCollisionParticles;
    public GameObject equationSuccessParticles;
    public GameObject equationErrorParticles;
    public GameObject scorePopParticles;
    public GameObject levelCompleteParticles;
    
    [Header("Screen Effects")]
    public Material screenFlashMaterial;
    public float flashDuration = 0.2f;
    public Color successFlashColor = new Color(0.2f, 1f, 0.2f, 0.3f);
    public Color errorFlashColor = new Color(1f, 0.2f, 0.2f, 0.3f);
    
    [Header("Camera Shake")]
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.2f;
    
    // Singleton pattern
    private static VisualEffectsManager _instance;
    public static VisualEffectsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<VisualEffectsManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("VisualEffectsManager");
                    _instance = obj.AddComponent<VisualEffectsManager>();
                }
            }
            return _instance;
        }
    }
    
    private Camera mainCamera;
    private Transform originalCameraTransform;
    private Vector3 originalCameraPosition;
    private bool isShaking = false;
    
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
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // Find main camera
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraTransform = mainCamera.transform;
            originalCameraPosition = originalCameraTransform.position;
        }
    }
    
    #region Particle Effects
    
    public void PlayBallCollisionEffect(Vector3 position, Vector3 normal)
    {
        if (ballCollisionParticles != null)
        {
            GameObject effect = Instantiate(ballCollisionParticles, position, Quaternion.LookRotation(normal));
            Destroy(effect, 2f);
        }
    }
    
    public void PlayEquationSuccessEffect(Vector3 position)
    {
        if (equationSuccessParticles != null)
        {
            GameObject effect = Instantiate(equationSuccessParticles, position, Quaternion.identity);
            Destroy(effect, 3f);
        }
        
        // Also flash the screen
        FlashScreen(successFlashColor);
        
        // Small camera shake
        ShakeCamera(shakeIntensity * 0.5f, shakeDuration * 0.5f);
    }
    
    public void PlayEquationErrorEffect(Vector3 position)
    {
        if (equationErrorParticles != null)
        {
            GameObject effect = Instantiate(equationErrorParticles, position, Quaternion.identity);
            Destroy(effect, 2f);
        }
        
        // Also flash the screen
        FlashScreen(errorFlashColor);
        
        // Small camera shake
        ShakeCamera(shakeIntensity * 0.7f, shakeDuration * 0.5f);
    }
    
    public void PlayScorePopEffect(Vector3 position, int score)
    {
        if (scorePopParticles != null)
        {
            GameObject effect = Instantiate(scorePopParticles, position, Quaternion.identity);
            
            // You could modify the particle system based on score
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                
                // Adjust size based on score
                float sizeMultiplier = Mathf.Clamp(score / 50f, 0.5f, 3f);
                main.startSize = main.startSize.constant * sizeMultiplier;
                
                // Adjust color based on score
                if (score > 100)
                {
                    main.startColor = Color.yellow;
                }
                else if (score > 50)
                {
                    main.startColor = Color.cyan;
                }
            }
            
            Destroy(effect, 2f);
        }
    }
    
    public void PlayLevelCompleteEffect()
    {
        if (levelCompleteParticles != null && mainCamera != null)
        {
            // Position in front of camera
            Vector3 position = mainCamera.transform.position + mainCamera.transform.forward * 2f;
            GameObject effect = Instantiate(levelCompleteParticles, position, Quaternion.identity);
            Destroy(effect, 5f);
        }
        
        // Flash screen
        FlashScreen(successFlashColor);
        
        // Camera shake
        ShakeCamera(shakeIntensity, shakeDuration);
    }
    
    #endregion
    
    #region Screen Effects
    
    public void FlashScreen(Color color)
    {
        StartCoroutine(FlashScreenRoutine(color));
    }
    
    private IEnumerator FlashScreenRoutine(Color color)
    {
        if (screenFlashMaterial == null || mainCamera == null)
            yield break;
            
        // Create a temporary camera overlay
        GameObject overlay = new GameObject("ScreenFlash");
        overlay.transform.parent = mainCamera.transform;
        overlay.transform.localPosition = new Vector3(0, 0, 0.1f);
        overlay.transform.localRotation = Quaternion.identity;
        
        // Add quad renderer
        MeshRenderer renderer = overlay.AddComponent<MeshRenderer>();
        MeshFilter filter = overlay.AddComponent<MeshFilter>();
        
        // Create quad mesh
        Mesh mesh = new Mesh();
        mesh.vertices = new Vector3[] {
            new Vector3(-1, -1, 0),
            new Vector3(1, -1, 0),
            new Vector3(-1, 1, 0),
            new Vector3(1, 1, 0)
        };
        mesh.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };
        mesh.triangles = new int[] { 0, 2, 1, 2, 3, 1 };
        filter.mesh = mesh;
        
        // Set material and color
        Material flashMat = new Material(screenFlashMaterial);
        flashMat.color = color;
        renderer.material = flashMat;
        
        // Fade out
        float startTime = Time.time;
        while (Time.time < startTime + flashDuration)
        {
            float t = (Time.time - startTime) / flashDuration;
            Color fadeColor = color;
            fadeColor.a = Mathf.Lerp(color.a, 0, t);
            flashMat.color = fadeColor;
            yield return null;
        }
        
        // Clean up
        Destroy(overlay);
    }
    
    #endregion
    
    #region Camera Effects
    
    public void ShakeCamera(float intensity, float duration)
    {
        if (mainCamera != null && !isShaking)
        {
            StartCoroutine(ShakeCameraRoutine(intensity, duration));
        }
    }
    
    private IEnumerator ShakeCameraRoutine(float intensity, float duration)
    {
        isShaking = true;
        
        // Store original position
        originalCameraPosition = mainCamera.transform.position;
        
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            // Calculate shake amount based on remaining time
            float remainingTime = duration - elapsed;
            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);
            
            // Generate random shake offset
            float x = Random.Range(-1f, 1f) * intensity * damper;
            float y = Random.Range(-1f, 1f) * intensity * damper;
            
            // Apply shake
            mainCamera.transform.position = originalCameraPosition + new Vector3(x, y, 0);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Reset camera position
        mainCamera.transform.position = originalCameraPosition;
        isShaking = false;
    }
    
    #endregion
}
