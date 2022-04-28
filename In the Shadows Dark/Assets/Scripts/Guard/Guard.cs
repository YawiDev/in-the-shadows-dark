using UnityEngine;

public class Guard : MonoBehaviour
{
    [Header("Gravity")]
    [SerializeField] float gravity = -5;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask ground;
    [SerializeField] float groundCheckRadius = 0.1f;

    bool isGrounded;

    CharacterController guardController;
    GuardPatroller patroller;
    Vector3 gravityVelocity;

    void Awake () {
        guardController = GetComponent<CharacterController>();
        patroller = GetComponent<GuardPatroller>();
    }

    void Update () {
        // Check if the guard is on the ground, using an invisible sphere to check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, ground);

        // If the guard is not touching the ground
        if (!isGrounded) {
            // Set the velocity based on the gravity
            gravityVelocity.y += gravity * Time.deltaTime;
        }

        // Apply the gravity
        guardController.Move(gravityVelocity * Time.deltaTime);
    }
}
