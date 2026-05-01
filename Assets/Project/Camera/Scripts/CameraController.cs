using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform playerTarget;
    public Transform pivot;
    public Transform cam;

    private PlayerInputHandler _input;

    [Header("Rotation")]
    public float sensitivity = 0.5f;
    public float minPitch = -30f;
    public float maxPitch = 70f;

    private float _yaw;
    private float _pitch;

    [Header("Distance")]
    public float distance = 4f;

    [Header("Collision")]
    public LayerMask collisionMask;
    public float sphereRadius = 0.2f;
    public float collisionOffset = 0.1f;

    private void Awake()
    {
        _input = playerTarget.GetComponent<PlayerInputHandler>();

        _yaw = transform.eulerAngles.y;
        _pitch = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (playerTarget == null) return;

        HandleRotation();
        RotateAroundTarget();
        HandleCameraPosition();
    }

    private void HandleRotation()
    {
        Vector2 look = _input.LookInput;

        _yaw += look.x * sensitivity;
        _pitch -= look.y * sensitivity;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);
    }

    private void RotateAroundTarget()
    {
        transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        pivot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private void HandleCameraPosition()
    {
        Vector3 pivotPos = pivot.position;
        Vector3 dir = -pivot.forward;

        float finalDistance = distance;

        // SphereCast to avoid passing through walls or objects
        if (Physics.SphereCast(
            pivotPos,
            sphereRadius,
            dir,
            out RaycastHit hit,
            distance,
            collisionMask,
            QueryTriggerInteraction.Ignore))
        {
            finalDistance = hit.distance - collisionOffset;
            finalDistance = Mathf.Clamp(finalDistance, 0.2f, distance);
        }

        cam.position = pivotPos + dir * finalDistance;
        cam.LookAt(pivotPos);
    }
}