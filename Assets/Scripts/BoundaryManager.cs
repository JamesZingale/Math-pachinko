using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    [Header("Boundary Settings")]
    public float bounceForce = 5f;
    public bool isDeadZone = false;
    
    [Header("Visual Feedback")]
    public Color normalColor = Color.white;
    public Color hitColor = Color.cyan;
    public float flashDuration = 0.2f;
    
    private Renderer objectRenderer;
    private Material originalMaterial;
    
    private void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            originalMaterial = objectRenderer.material;
            objectRenderer.material.color = normalColor;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the colliding object is the player ball
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isDeadZone)
            {
                // If this is a dead zone, destroy the ball or reset it
                Destroy(collision.gameObject);
                
                // Notify level manager of failure if needed
                LevelManager levelManager = FindFirstObjectByType<LevelManager>();
                if (levelManager != null)
                {
                    levelManager.GameOver();
                }
            }
            else
            {
                // Apply bounce force if this is a regular boundary
                Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Calculate reflection direction
                    Vector3 normal = collision.contacts[0].normal;
                    Vector3 incomingVelocity = rb.linearVelocity;
                    Vector3 reflectionDirection = Vector3.Reflect(incomingVelocity.normalized, normal);
                    
                    // Apply force in reflection direction
                    rb.linearVelocity = reflectionDirection * Mathf.Max(incomingVelocity.magnitude, bounceForce);
                }
                
                // Visual feedback
                FlashBoundary();
            }
        }
    }
    
    private void FlashBoundary()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = hitColor;
            Invoke("ResetColor", flashDuration);
        }
    }
    
    private void ResetColor()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = originalMaterial.color;
        }
    }
}
