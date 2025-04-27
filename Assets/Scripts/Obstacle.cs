using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float bounciness = 0.7f;
    
    void Start()
    {
        // Make sure we have a collider
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider>();
        }
        
        // Set up physics material
        PhysicsMaterial physicsMaterial = collider.material;
        if (physicsMaterial == null)
        {
            physicsMaterial = new PhysicsMaterial("ObstacleMaterial");
            collider.material = physicsMaterial;
        }
        
        // Configure physics properties
        physicsMaterial.bounciness = bounciness;
        physicsMaterial.frictionCombine = PhysicsMaterialCombine.Minimum;
        physicsMaterial.bounceCombine = PhysicsMaterialCombine.Maximum;
    }
}
