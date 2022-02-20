using UnityEngine;

[ExecuteAlways]
public class CameraController : MonoBehaviour
{
    [SerializeField] Transform targetCamera;

    [Header("Following")]
    [SerializeField] Transform followTarget;

    [SerializeField] Vector2 offset;
    [SerializeField, Range(0, 1)] float shoulderSide = 1;
    [SerializeField] float cameraDistance = 2;

    [Header("Looking")]
    [SerializeField] float sensitivity = 300;
    [SerializeField] float lookUpClamp = 80;

    float mouseX;
    float mouseY;

    void Start () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        FollowTarget();
    }

    void LateUpdate () {
        //FollowTarget();

        mouseX += Input.GetAxis("Mouse X") * sensitivity * Time.smoothDeltaTime;
        mouseY -= Input.GetAxis("Mouse Y") * sensitivity * Time.smoothDeltaTime;

        mouseY = Mathf.Clamp(mouseY, -lookUpClamp, lookUpClamp);

        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
    }

    void FollowTarget () {
        transform.position = followTarget.position;

        Vector3 targetOffset = new Vector3(offset.x * (shoulderSide - 0.5f), offset.y, -cameraDistance);

        targetCamera.transform.position = followTarget.position + targetOffset;
    }
}
