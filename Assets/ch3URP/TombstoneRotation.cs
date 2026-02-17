using UnityEngine;

public class TombstoneRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("Rotation speed in degrees per second")]
    public float rotationSpeed = 30f;
    
    [Header("Rotation Axis")]
    public bool rotateX = false;
    public bool rotateY = true;
    public bool rotateZ = false;

    void Update()
    {
        // Calculate rotation for this frame
        float rotationThisFrame = rotationSpeed * Time.deltaTime;
        
        // Build rotation vector
        Vector3 rotation = new Vector3(
            rotateX ? rotationThisFrame : 0,
            rotateY ? rotationThisFrame : 0,
            rotateZ ? rotationThisFrame : 0
        );
        
        // Apply rotation
        transform.Rotate(rotation, Space.World);
    }
}