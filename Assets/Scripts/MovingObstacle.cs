using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    public float speed = 2f;
    public Vector2 direction = Vector2.right;
    public float minX = -8f;
    public float maxX = 8f;
    public float minY = -3f;
    public float maxY = 3f;
    
    private Rigidbody rb;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configure rigidbody for kinematic movement
        rb.useGravity = false;
        rb.isKinematic = true;
        
        // Normalize direction
        direction.Normalize();
    }
    
    void Update()
    {
        // Move the obstacle
        transform.Translate(new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime);
        
        // Check if the obstacle is out of bounds
        if (transform.position.x < minX || transform.position.x > maxX)
        {
            direction.x = -direction.x;
        }
        
        if (transform.position.y < minY || transform.position.y > maxY)
        {
            direction.y = -direction.y;
        }
        
        // Keep the obstacle within bounds
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        float clampedY = Mathf.Clamp(transform.position.y, minY, maxY);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
