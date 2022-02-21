using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform cameraHolder;
    [SerializeField] float movementSpeed = 4;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float turnSmoothing = 0.1f;
    float turnSmoothVelocity;

    float horizontalInput;
    float verticalInput;

    CharacterController playerController;
    Vector3 velocity;

    void Awake () {
        playerController = GetComponent<CharacterController>();
    }

    void Update () {
        GetInput();

        Vector3 forwardOrientation = cameraHolder.forward;
        forwardOrientation.y = 0;

        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (direction.magnitude >= 0.1f) {
            float turnAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraHolder.eulerAngles.y;
            float yRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, turnAngle, ref turnSmoothVelocity, turnSmoothing);
            transform.rotation = Quaternion.Euler(0, yRotation, 0);

            Vector3 moveDirection = Quaternion.Euler(0, yRotation, 0) * Vector3.forward;

            playerController.Move(moveDirection.normalized * movementSpeed * Time.deltaTime);
        }

        velocity.y += gravity * Time.deltaTime;

        playerController.Move(velocity * Time.deltaTime);
    }

    void GetInput () {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
}
