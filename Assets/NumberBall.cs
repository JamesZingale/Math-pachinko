using UnityEngine;

public class NumberBall : MonoBehaviour
{
    public int numberValue;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BallShooter.Instance.AddHitNumber(numberValue);
            Destroy(gameObject);
        }
    }
}