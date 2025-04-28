using UnityEngine;

public class OperatorZone : MonoBehaviour
{
    public enum OperatorType { Add, Multiply }
    public OperatorType operatorType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BallShooter.Instance.SetOperator(operatorType);
        }
    }
}