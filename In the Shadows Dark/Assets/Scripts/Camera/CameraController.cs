using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float mouseSensitivity = 10;

    [Header("Orbit")]
    [SerializeField] Vector2 offset;
    [SerializeField] float distance = 3;
    [SerializeField, Range(0, 1)] float shoulderSide = 1;
    [SerializeField] float shoulderSwitchSpeed = 0.4f;

    [Header("Looking")]
    [SerializeField, Range(-90, 0)] float minLookAngle = -80;
    [SerializeField, Range(0, 90)] float maxLookAngle = 80;

    float mouseX, mouseY;
    float xRotation, yRotation;

    bool isSwitchingShoulder;

    void Awake () {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update () {
        GetInput();

        OrbitTarget();
        OffsetCamera();
    }

    void GetInput () {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.smoothDeltaTime;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.smoothDeltaTime;

        if (Input.GetKeyDown(KeyCode.Mouse2)) {
            StartCoroutine(SwitchShoulder());
        }
    }

    void OrbitTarget () {
        xRotation -= mouseY;
        yRotation += mouseX;

        xRotation = Mathf.Clamp(xRotation, minLookAngle, maxLookAngle);

        Vector3 targetRotation = new Vector3(xRotation, yRotation);

        transform.eulerAngles = targetRotation;
    }

    void OffsetCamera () {
        float targetShoulderSide = shoulderSide * 2 - 1;
        print(targetShoulderSide);
        Vector3 targetOffset = (transform.forward * -distance) + (transform.right * offset.x * targetShoulderSide) + (transform.up * offset.y);

        transform.position = target.position + targetOffset;
    }

    IEnumerator SwitchShoulder () {
        isSwitchingShoulder = true;

        float timeElapsed = 0;
        float targetShoulder = shoulderSide > 0.5f ? 0 : 1;

        while (timeElapsed < shoulderSwitchSpeed) {
            shoulderSide = Mathf.Lerp(shoulderSide, targetShoulder, timeElapsed / shoulderSwitchSpeed);

            timeElapsed += Time.deltaTime;

            yield return null;
        }

        isSwitchingShoulder = false;
    }
}
