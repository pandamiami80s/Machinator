using UnityEngine;

public class GunTurret : MonoBehaviour
{
    public Transform verticalTransform;
    Transform cameraTransform;
    public float rotationSpeed = 8.0f;
    public float minPitch = -5.0f;
    public float maxPitch = 5.0f;

    public Transform nozzle;
    public float range = 10.0f;

    [Header("Aim Settings")]
    public float maxAimDistance = 500f;
    public LayerMask aimLayers; // Set this to "Everything" except the Player/Vehicle layer


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!cameraTransform || !verticalTransform) return;

        // 1. FIND THE TARGET POINT (Where the camera is looking)
        Vector3 targetPoint;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxAimDistance, aimLayers))
        {
            targetPoint = hit.point; // Hit an object
        }
        else
        {
            targetPoint = ray.GetPoint(maxAimDistance); // Hit nothing, aim at horizon
        }

        // 2. HORIZONTAL ROTATION (Y-Axis)
        // Find direction from turret base to the target point
        Vector3 dirToTarget = targetPoint - transform.position;
        // Convert that direction into the vehicle's local space
        Vector3 localDir = transform.parent.InverseTransformDirection(dirToTarget);
        localDir.y = 0; // Flatten it to the horizontal plane

        if (localDir != Vector3.zero)
        {
            Quaternion targetRotationY = Quaternion.LookRotation(localDir);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotationY, Time.deltaTime * rotationSpeed);
        }

        // 3. VERTICAL ROTATION (X-Axis)
        // Find direction from the PIVOT to the target point
        Vector3 dirToTargetVertical = targetPoint - verticalTransform.position;
        // Convert to local space of the horizontal base
        Vector3 localDirVert = transform.InverseTransformDirection(dirToTargetVertical);

        // Calculate the pitch angle
        float targetPitch = -Mathf.Atan2(localDirVert.y, localDirVert.z) * Mathf.Rad2Deg;
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        Quaternion targetRotationX = Quaternion.Euler(targetPitch, 0, 0);
        verticalTransform.localRotation = Quaternion.Slerp(verticalTransform.localRotation, targetRotationX, Time.deltaTime * rotationSpeed);

        // 4. SHOOT DEBUG LINE (On Click)
        if (Input.GetMouseButtonDown(0))
        {
            Debug.DrawLine(verticalTransform.position, targetPoint, Color.yellow, 1.0f);
        }
    
    }
}
