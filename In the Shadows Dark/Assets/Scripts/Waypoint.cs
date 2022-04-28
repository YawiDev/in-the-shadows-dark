using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public bool isWaitingPoint;
    public bool isLookingAroundPoint;
    public float waitTime;

    void OnDrawGizmosSelected() {
        transform.parent.SendMessage("OnDrawGizmosSelected");
    }
}
