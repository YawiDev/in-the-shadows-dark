using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [HideInInspector] public Waypoint[] waypoints;

    float guardYPosition;

    public void Initialise (float yPosition) {
        guardYPosition = yPosition;

        // Initialise the waypoints position array
        waypoints = new Waypoint[transform.childCount];

        // For the number of waypoints
        for (int i = 0; i < waypoints.Length; i++) {
            // Set each waypoint in its corresponding slot
            waypoints[i] = transform.GetChild(i).GetComponent<Waypoint>();
            // Modify the position of the waypoint so that the y position is at the same height as the guard; this will ensure the guard doesn't move up or down
            waypoints[i].transform.position = new Vector3(waypoints[i].transform.position.x, guardYPosition, waypoints[i].transform.position.z);
        }
    }

    void Update () {
        for (int i = 0; i < waypoints.Length; i++) {
            waypoints[i].transform.position = new Vector3(waypoints[i].transform.position.x, guardYPosition, waypoints[i].transform.position.z);
        }
    }

    void OnDrawGizmosSelected() {
        // Set both the start and previous position for the line
        Vector3 startPosition = transform.GetChild(0).position;
        Vector3 previousPosition = startPosition;

        // For each waypoint
        foreach (Transform waypoint in transform) {
            // Draw a red sphere at its position
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(waypoint.position, 0.2f);
            // Draw a white line from the previous waypoint to this waypoint
            Gizmos.color = Color.white;
            Gizmos.DrawLine(previousPosition, waypoint.position);
            // Set the next previous position for the next loop
            previousPosition = waypoint.position;
        }

        // Draw the line between waypoints
        Gizmos.DrawLine(previousPosition, startPosition);
    }
}
