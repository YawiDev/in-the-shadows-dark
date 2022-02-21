using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float mouseSensitivity = 10;

    [Header("Orbit")]
    [SerializeField] Vector2 offset;
    [SerializeField] float distance = 3;
    [SerializeField, Range(0, 1)] float shoulderSize = 1;

    [Header("Looking")]
    [SerializeField, Range(-90, 0)] float minLookAngle = -80;
    [SerializeField, Range(0, 90)] float maxLookAngle = 80;

    float mouseX, mouseY;

    float xRotation, yRotation;

    void Update () {
        GetInput();

        OrbitTarget();
        OffsetCamera();
    }

    void GetInput () {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.smoothDeltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.smoothDeltaTime;
    }

    void OrbitTarget () {
        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);

        Vector3 targetRotation = new Vector3(xRotation, yRotation);

        transform.eulerAngles = targetRotation;
    }

    void OffsetCamera () {
        Vector3 targetOffset = (transform.forward * -distance) + (transform.right * offset.x * (shoulderSize - 0.5f)) + (transform.up * offset.y);

        transform.position = target.position + targetOffset;
    }
}
