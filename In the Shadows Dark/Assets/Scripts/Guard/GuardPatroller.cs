using UnityEngine;
using System.Collections;

public class GuardPatroller : MonoBehaviour
{
    // Variables

    [Header("Patrolling")]

    [SerializeField] PatrolPath patrollingPath;

    [SerializeField] float movementSpeed = 1.5f;
    [SerializeField] float turnSpeed = 2;
    Waypoint targetWaypoint;
    int targetWaypointIndex = 0;

    CharacterController controller;
    GuardAnimator animator;

    void Awake () {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<GuardAnimator>();
    }

    void Start () {
        patrollingPath.Initialise(transform.position.y);

        // Start following the patrol path
        StartCoroutine(FollowPatrolPath(patrollingPath.waypoints));
    }

    IEnumerator FollowPatrolPath (Waypoint[] waypoints) {
        // Begin at the first waypoint
        transform.position = waypoints[0].transform.position;
        // Set the target waypoint that the guard needs to move to currently
        targetWaypoint = waypoints[targetWaypointIndex];    

        // Check if the starting waypoint is a wait waypoint
        yield return StartCoroutine(CheckWaypoint(targetWaypoint));

        while (true) {
            MoveTo(targetWaypoint.transform.position);
            Vector3 movementDirection = (targetWaypoint.transform.position - transform.position).normalized;

            // Set the walking animation
            animator.SetState(true);

            // If the guard is within a certain threshold to the target waypoint
            if (Vector3.Distance(transform.position, targetWaypoint.transform.position) < 0.5f) {
                // Lerp the guard to the waypoint position
                transform.position = Vector3.Lerp(transform.position, targetWaypoint.transform.position, turnSpeed * Time.deltaTime);

                // If the waypoint isn't the starting waypoint, check it. This is to ensure that the guard doesn't wait twice at the first waypoint
                if (targetWaypointIndex != 0) {
                    // Check if the guard is supposed to wait at this waypoint or not
                    yield return StartCoroutine(CheckWaypoint(targetWaypoint));
                }
                
                // Set the next waypoint, if the guard is at the last waypoint, loop back to the first one
                SetNextWaypoint((targetWaypointIndex + 1) % waypoints.Length);

                movementDirection = (targetWaypoint.transform.position - transform.position).normalized;
            }

            // Rotate the guard in the direction of travel
            TurnToFaceTarget(movementDirection);

            yield return null;
        }
    }

    IEnumerator CheckWaypoint (Waypoint waypoint) {
        // If the waypoint is a waiting waypoint
        if (waypoint.isWaitingPoint) {
            // Stop and Wait
            yield return StartCoroutine(StopAndWait());
        }
        else {
            // Otherwise don't do anything
            yield break;
        }
    }

    IEnumerator StopAndWait () {
        // Set the idle animation
        animator.SetState(false);

        // If the waypoint is a point that the guard shoould look around, look around
        if (targetWaypoint.isLookingAroundPoint) {
            animator.LookAround();
        }

        // Wait for a bit
        yield return new WaitForSeconds(targetWaypoint.waitTime);
    }

    void SetNextWaypoint (int waypointIndex) {
        // Set both the waypoint index and target waypoint position
        targetWaypointIndex = waypointIndex;
        targetWaypoint = patrollingPath.waypoints[targetWaypointIndex];
    }

    void MoveTo (Vector3 destination) {
        // Calculate the direction from the guard to the waypoint
        destination.y = 0;
        Vector3 direction = (destination - transform.position).normalized;

        // Move the guard in that direction
        controller.Move(direction * movementSpeed * Time.deltaTime);
    }

    void TurnToFaceTarget (Vector3 direction) {
        // Set the rotation based on the direction
        direction.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        // Lerp the guard's rotation in the direction of travel
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
    }
}
