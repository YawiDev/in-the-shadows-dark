using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GuardFOV))]
public class GuardFOVEditor : Editor
{
    void OnSceneGUI() {
        GuardFOV fov = (GuardFOV)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(fov.eyes.position, Vector3.up, Vector3.forward, 360, fov.viewDistance);

        Vector3 viewAngleA = fov.DirectionFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirectionFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(fov.eyes.position, fov.eyes.position + viewAngleA * fov.viewDistance);
        Handles.DrawLine(fov.eyes.position, fov.eyes.position + viewAngleB * fov.viewDistance);

        if (fov.isAwareOfPlayer) {
            Handles.color = Color.red;
            Handles.DrawLine(fov.eyes.position, fov.playerTarget.position);
        }
    }
}
