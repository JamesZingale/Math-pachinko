using UnityEngine;
using System.Collections.Generic;

public class PlayerBall : MonoBehaviour
{
    [Header("Ball Settings")]
    public float maxSpeed = 20f;
    public float minSpeed = 2f;
    public float lifeTime = 15f; // Destroy the ball after this many seconds
    
    [Header("Physics Settings")]
    public float dragIncrease = 0.1f; // Gradually slow down the ball
    public float spinFactor = 0.5f; // How much the ball spins when bouncing
    
    [Header("Visual Feedback")]
    public GameObject collisionParticles;
    public AudioClip bounceSound;
    public float bounceVolumeMin = 0.1f;
    public float bounceVolumeMax = 1.0f;
    
    private Rigidbody rb;
    private TrailRenderer trail;
    private AudioSource audioSource;
    private List<GameObject> collidedMathBalls = new List<GameObject>();
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trail = GetComponent<TrailRenderer>();
        
        // Add audio source if needed
        if (GetComponent<AudioSource>() == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1f; // 3D sound
        }
        else
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Ensure we have the correct tag
        gameObject.tag = "Player";
        
        // Set a timer to destroy the ball after its lifetime
        Destroy(gameObject, lifeTime);
    }
    
    private void FixedUpdate()
    {
        // Limit the ball's maximum speed
        if (rb != null)
        {
            float currentSpeed = rb.linearVelocity.magnitude;
            
            if (currentSpeed > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
            else if (currentSpeed < minSpeed && currentSpeed > 0.1f)
            {
                // Apply a minimum speed to keep the ball moving
                rb.linearVelocity = rb.linearVelocity.normalized * minSpeed;
            }
            
            // Gradually increase drag to simulate friction
            rb.linearDamping += dragIncrease * Time.fixedDeltaTime;
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        // Handle collision effects
        PlayCollisionFeedback(collision);
        
        // Add spin to the ball for more realistic physics
        if (rb != null && collision.contactCount > 0)
        {
            Vector3 normal = collision.contacts[0].normal;
            Vector3 tangent = Vector3.Cross(normal, Vector3.up).normalized;
            
            // Add torque based on the tangent direction and current velocity
            rb.AddTorque(tangent * rb.linearVelocity.magnitude * spinFactor, ForceMode.Impulse);
        }
        
        // Track collisions with math balls to avoid duplicate activations
        if (collision.gameObject.CompareTag("NumberBall") || collision.gameObject.CompareTag("OperatorBall"))
        {
            if (!collidedMathBalls.Contains(collision.gameObject))
            {
                collidedMathBalls.Add(collision.gameObject);
            }
        }
    }
    
    private void PlayCollisionFeedback(Collision collision)
    {
        // Play bounce sound with volume based on impact velocity
        if (audioSource != null && bounceSound != null)
        {
            float impactVelocity = collision.relativeVelocity.magnitude;
            float volume = Mathf.Lerp(bounceVolumeMin, bounceVolumeMax, impactVelocity / maxSpeed);
            audioSource.PlayOneShot(bounceSound, volume);
        }
        
        // Spawn particles at collision point
        if (collisionParticles != null && collision.contactCount > 0)
        {
            ContactPoint contact = collision.contacts[0];
            GameObject particles = Instantiate(collisionParticles, contact.point, Quaternion.LookRotation(contact.normal));
            Destroy(particles, 1f);
        }
    }
    
    // Update trail effect based on velocity
    private void Update()
    {
        if (rb != null && trail != null)
        {
            float speedRatio = rb.linearVelocity.magnitude / maxSpeed;
            
            // Adjust trail time and width based on speed
            trail.time = Mathf.Lerp(0.1f, 0.5f, speedRatio);
            trail.startWidth = Mathf.Lerp(0.05f, 0.2f, speedRatio);
            
            // Adjust trail color based on speed (optional)
            Color startColor = Color.Lerp(Color.white, Color.cyan, speedRatio);
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0);
            
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(startColor, 0.0f), new GradientColorKey(startColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
            );
            
            trail.colorGradient = gradient;
        }
    }
}
