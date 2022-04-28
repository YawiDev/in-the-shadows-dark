using System.Collections.Generic;
using UnityEngine;

public class GuardFOVVisualisation : MonoBehaviour
{
    [HideInInspector] public bool show;

    [SerializeField] Material visualisationMaterial;
    [SerializeField] Color playerSpottedColour;
    Color originalColour;

    [Space]

    [SerializeField] float meshResolution;
    [SerializeField] int edgeResolveIterations;
    [SerializeField] float edgeDistanceThreshold;

    [SerializeField] MeshFilter viewMeshFilter;
    Mesh viewMesh;

    GuardFOV guardFOV;

    void Awake () {
        guardFOV = GetComponent<GuardFOV>();

        // Initialise mesh
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;

        originalColour = visualisationMaterial.color;
    }

    void Update () {
        if (guardFOV.PlayerIsInView()) {
            visualisationMaterial.color = playerSpottedColour;
        }
        else {
            visualisationMaterial.color = originalColour;
        }
    }

    void LateUpdate () {
        // Enable and disable field of view visualisation
        if (show) 
            DrawFieldOfView(guardFOV.viewAngle);
        else
            viewMesh.Clear();
    }

    public void DrawFieldOfView (float viewAngle) {
        // Calculate the number of check-rays in the mesh and the angle between each one
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;

        // View points are raycasts from the guard projected outwards in the field of view, to check for obstacles
        List<Vector3> viewPoints = new List<Vector3>();
        // Keep track of the previous view cast for the upcoming loop
        ViewCastInfo previousViewCast = new ViewCastInfo();

        // For the number of rays
        for (int i = 0; i < stepCount; i++) {
            // Calculate the angle. First, start where the guard is facing, then go to the left boundary of the FOV, and add i many steps to the right
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;

            // Send a ray at the angle from the guard outwards as far as the viewing distance
            ViewCastInfo newViewCast = ViewCast(angle, guardFOV.viewDistance, guardFOV.obstacleMask);

            // On the first iteration, the previousViewCast variable wouldn't have been set
            if (i > 0) {
                bool edgeDistanceThresholdExceeded = Mathf.Abs(previousViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;

                if (previousViewCast.hitSomething != newViewCast.hitSomething || (previousViewCast.hitSomething && newViewCast.hitSomething && edgeDistanceThresholdExceeded)) {
                    EdgeInfo edge = FindEdge(previousViewCast, newViewCast);

                    if (edge.pointA != Vector3.zero) {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero) {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            // Take note of the view cast, whether or not it hit something, so we can use it for later
            viewPoints.Add(newViewCast.hitPoint);
            previousViewCast = newViewCast;
        }

        // Begin constructing the mesh to visualise what the guard can see

        // Calculate the amount of vertices and triangles
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        // The first vertex is at the guard position
        vertices[0] = Vector3.zero;

        // For each vertex
        for (int i = 0; i < vertexCount - 1; i++) {
            // Set the position of the vertex to the local position of the corresponding view point, relative to the guard
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            // Draw the triangles using the vertices
            if (i < vertexCount - 2) {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        // Create the mesh using the vertices and triangles
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast (float globalAngle, float distance, LayerMask layerMask) {
        // Calculate the direction of the raycast
        Vector3 direction = guardFOV.DirectionFromAngle(globalAngle, true);
        RaycastHit hit;

        // If a ray, emitted from the guard's eyes, hits something with the target layermask
        if (Physics.Raycast(guardFOV.transform.position + guardFOV.eyes.localPosition, direction, out hit, distance, layerMask)) {
            // Return some information about the raycast that hit an obstacle
            return new ViewCastInfo(true, hit.point - guardFOV.eyes.localPosition, hit.distance, globalAngle);
        } 
        else {
            // Otherwise, just return the end of the guard's view distance as the hit point
            return new ViewCastInfo(false, guardFOV.transform.position + direction * distance, distance, globalAngle);
        }
    }

    EdgeInfo FindEdge (ViewCastInfo minViewCast, ViewCastInfo maxViewCast) {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++) {
            // Get the angle in the middle of the min and max angle
            float angle = (minAngle + maxAngle) / 2;

            ViewCastInfo newViewCast = ViewCast(angle, guardFOV.viewDistance, guardFOV.obstacleMask);
            bool edgeDistanceThresholdExceeded = Mathf.Abs(minViewCast.distance - newViewCast.distance) > edgeDistanceThreshold;

            if (newViewCast.hitPoint == minViewCast.hitPoint && !edgeDistanceThresholdExceeded) {
                minAngle = angle;
                minPoint = newViewCast.hitPoint;
            }
            else {
                maxAngle = angle;
                maxPoint = newViewCast.hitPoint;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    public struct ViewCastInfo {
        public bool hitSomething;
        public Vector3 hitPoint;
        public float distance;
        public float angle;

        public ViewCastInfo (bool _hitSomething, Vector3 _hitPoint, float _distance, float _angle) {
            hitSomething = _hitSomething;
            hitPoint = _hitPoint;
            distance = _distance;
            angle = _angle;
        }
    }

    public struct EdgeInfo {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo (Vector3 _pointA, Vector3 _pointB) {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
