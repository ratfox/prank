using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;  // Reference to the main character's transform
    public float smoothness = 2f;  // Adjust this value to control the smoothness of the camera follow

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the desired position of the camera
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // Smoothly interpolate the current camera position towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);
        }
    }
}