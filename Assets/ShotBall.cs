using UnityEngine;

public class ShotBall : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            BallShooter.Instance.BallHitFloor();
            Destroy(gameObject);
        }
    }
}