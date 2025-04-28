using UnityEngine;

public class AimingPointer : MonoBehaviour
{
    public Transform launchPoint; 

    void Update()
    {
        AimAtMouse();
    }

    private void AimAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.right, launchPoint.position);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 hitPoint = ray.GetPoint(distance);

            Vector3 direction = hitPoint - launchPoint.position;

            direction.x = 0f;

            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.z) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(-angle-90f, 0f, 0f);
            }
        }
    }
}