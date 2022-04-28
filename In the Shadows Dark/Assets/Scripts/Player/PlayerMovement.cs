using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Transform cameraHolder;
    [SerializeField] float walkSpeed = 4;
    [SerializeField] float runSpeed = 5.5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float turnSmoothing = 0.1f;
    float turnSmoothVelocity;

    float horizontalInput;
    float verticalInput;

    CharacterController playerController;
    PlayerAnimator playerAnimator;
    Vector3 velocity;

    public bool isMoving { get; private set; }
    bool isRunning;

    void Awake () {
        playerController = GetComponent<CharacterController>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
    }

    void Update () {
        GetInput();

        Vector3 forwardOrientation = cameraHolder.forward;
        forwardOrientation.y = 0;

        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (direction.magnitude >= 0.1f) {
            isMoving = true;

            float turnAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraHolder.eulerAngles.y;
            float yRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, turnAngle, ref turnSmoothVelocity, turnSmoothing);
            transform.rotation = Quaternion.Euler(0, yRotation, 0);

            Vector3 moveDirection = Quaternion.Euler(0, yRotation, 0) * Vector3.forward;

            playerController.Move(moveDirection.normalized * (isRunning ? runSpeed : walkSpeed) * Time.deltaTime);
        }
        else {
            isMoving = false;
        }

        if (playerController.isGrounded && velocity.y < 0) {
            velocity.y = -2;
        }

        velocity.y += gravity * Time.deltaTime;

        playerController.Move(velocity * Time.deltaTime);

        playerAnimator.SetState("isMoving", isMoving);
        playerAnimator.SetState("isRunning", isRunning);
    }

    void GetInput () {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving;
    }
}
