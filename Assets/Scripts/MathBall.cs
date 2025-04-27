using UnityEngine;
using System.Collections;

public class MathBall : MonoBehaviour
{
    public enum BallType
    {
        Number,
        Operator
    }

    [Header("Ball Properties")]
    public BallType ballType;
    public string value; // "1", "2", "3", "+", "-", "*", "/"
    
    [Header("Physics Settings")]
    public float bounceForce = 5f;
    public float activationCooldown = 1f; // Prevent multiple activations in quick succession
    
    [Header("Visual Feedback")]
    public Color hitColor = Color.yellow;
    public float flashDuration = 0.5f;
    public GameObject activationParticles;
    public AudioClip activationSound;
    
    private Renderer ballRenderer;
    private Material originalMaterial;
    private Color originalColor;
    private bool canBeActivated = true;
    private AudioSource audioSource;
    
    private void Awake()
    {
        // Ensure this object has the correct tag based on its type
        gameObject.tag = (ballType == BallType.Number) ? "NumberBall" : "OperatorBall";
        
        // Get renderer reference
        ballRenderer = GetComponent<Renderer>();
        if (ballRenderer != null)
        {
            originalMaterial = ballRenderer.material;
            originalColor = originalMaterial.color;
        }
        
        // Add audio source if needed
        if (activationSound != null && GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = activationSound;
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Ensure we have a rigidbody
        if (GetComponent<Rigidbody>() == null)
        {
            Rigidbody rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 0.5f;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        
        // Ensure we have a collider
        if (GetComponent<SphereCollider>() == null)
        {
            SphereCollider collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = 0.5f;
            collider.material = Resources.Load<PhysicsMaterial>("BouncyBall");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Only process player ball collisions when not on cooldown
        if (collision.gameObject.CompareTag("Player") && canBeActivated)
        {
            // Add this ball to the equation
            EquationManager.Instance.AddToEquation(this);
            
            // Apply bounce force to player ball
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Calculate reflection direction
                Vector3 normal = collision.contacts[0].normal;
                Vector3 incomingVelocity = playerRb.linearVelocity;
                Vector3 reflectionDirection = Vector3.Reflect(incomingVelocity.normalized, normal);
                
                // Apply additional force in reflection direction
                playerRb.AddForce(reflectionDirection * bounceForce, ForceMode.Impulse);
            }
            
            // Visual feedback
            StartCoroutine(ActivationFeedback());
            
            // Start cooldown
            StartCoroutine(ActivationCooldown());
        }
    }
    
    private IEnumerator ActivationFeedback()
    {
        // Flash the ball
        if (ballRenderer != null)
        {
            ballRenderer.material.color = hitColor;
        }
        
        // Play sound
        if (audioSource != null && activationSound != null)
        {
            audioSource.Play();
        }
        
        // Show particles
        if (activationParticles != null)
        {
            GameObject particles = Instantiate(activationParticles, transform.position, Quaternion.identity);
            Destroy(particles, 2f); // Clean up particles after 2 seconds
        }
        
        // Wait for flash duration
        yield return new WaitForSeconds(flashDuration);
        
        // Reset color
        ResetColor();
    }
    
    private IEnumerator ActivationCooldown()
    {
        canBeActivated = false;
        yield return new WaitForSeconds(activationCooldown);
        canBeActivated = true;
    }

    private void ResetColor()
    {
        if (ballRenderer != null)
        {
            ballRenderer.material.color = originalColor;
        }
    }
}
