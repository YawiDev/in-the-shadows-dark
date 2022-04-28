using UnityEngine;
using UnityEngine.UI;

public class GuardFOV : MonoBehaviour
{
    [Header("Field of View")]
    [SerializeField] bool showVisualisation;
    GuardFOVVisualisation fieldOfViewVisualiser;
    public Transform eyes;
    public float viewDistance = 10;
    [Range(0, 360)] public float viewAngle = 80;
    public LayerMask obstacleMask;

    [Header("Player Detection")]
    [SerializeField] float timeToSpotPlayer = 2.5f;
    [HideInInspector] public Transform playerTarget;
    bool playerIsWithinFOVArea;
    public bool isAwareOfPlayer;
    float awarenessOfPlayerTimer = 0;
    float awarenessOfPlayer = 0;
    [SerializeField] Transform[] playerBones = new Transform[6];

    [Header("UI")]
    [SerializeField] Slider awarenessIndicator;
    GuardUI guardUI;

    [Header("Torch")]
    [SerializeField] Light torchSpotlight;
    [SerializeField] Color awarenessBuildingSpotlightColour;
    [SerializeField] Color awareSpotlightColour;
    Color originalSpotlightColour;

    void Awake () {
        playerTarget = GameObject.FindGameObjectWithTag("Player").transform;

        fieldOfViewVisualiser = GetComponent<GuardFOVVisualisation>();
        guardUI = GetComponentInChildren<GuardUI>();
    }

    void Start () {
        torchSpotlight.spotAngle = viewAngle;
        originalSpotlightColour = torchSpotlight.color;
    }

    void Update () {
        HandleDetection();

        // Handle enabling and disabling of FOV visualiser
        fieldOfViewVisualiser.show = showVisualisation;
    }

    void HandleDetection () {
        // If the player is within the viewing distance, angle and can be seen with a raycast
        if (PlayerIsInView()) {
            // And the player hasn't already been marked as visible
            if (!playerIsWithinFOVArea) {
                // Mark the player as visible. This will ensure that the code within this loop will only run once
                playerIsWithinFOVArea = true;
                // Reset the timer
                awarenessOfPlayerTimer = 0;
            }
            
            int amountOfPlayerBonesVisible = CastToPlayerBones();

            float awarenessSpeed = 0;
            switch (amountOfPlayerBonesVisible) {
                case 2:
                    awarenessSpeed = 0.35f;
                    break;
                case 3:
                    awarenessSpeed = 0.45f;
                    break;
                case 4:
                    awarenessSpeed = 0.85f;
                    break;
                case 5:
                    awarenessSpeed = 0.9f;
                    break;
                case 6:
                    awarenessSpeed = 1;
                    break;
            }

            if (amountOfPlayerBonesVisible >= 2) {
                print(amountOfPlayerBonesVisible + " bones visible, Speed: " + awarenessSpeed);

                // Count how long the player is visible for when in FOV
                awarenessOfPlayerTimer += Time.deltaTime * awarenessSpeed;
                // Map the awareness timer to a value between 0 and 1
                awarenessOfPlayer = awarenessOfPlayerTimer / timeToSpotPlayer;
                // Set the value of the awareness slider to the awareness
                awarenessIndicator.value = awarenessOfPlayer;

                if (awarenessOfPlayer > 0.08f) {
                    awarenessIndicator.gameObject.SetActive(true);
                    torchSpotlight.color = awarenessBuildingSpotlightColour;
                }

                // If the guard is alert of the player
                if (awarenessOfPlayerTimer >= timeToSpotPlayer) {
                    awarenessIndicator.gameObject.SetActive(false);
                    guardUI.OnGuardAlerted(ref isAwareOfPlayer);

                    torchSpotlight.color = awareSpotlightColour;

                    isAwareOfPlayer = true;
                }
            }
            else {
                // Reset the torchlight colour
                torchSpotlight.color = originalSpotlightColour;

                // Disable awareness indicator
                awarenessIndicator.gameObject.SetActive(false);

                awarenessOfPlayerTimer = 0;
                isAwareOfPlayer = false;
            }
        }
        else {
            // Reset the torchlight colour
            torchSpotlight.color = originalSpotlightColour;
            // The player isn't visible anymore
            playerIsWithinFOVArea = false;
            // Disable awareness indicator
            awarenessIndicator.gameObject.SetActive(false);

            isAwareOfPlayer = false;
        }
    }

    public bool PlayerIsInView () {
        // Don't do anything if we don't have any information about the player
        if (playerTarget == null) return false;

        // If the player is within the viewing distance of the guard
        if (Vector3.Distance(transform.position, playerTarget.position) < viewDistance) {
            Vector3 directionToPlayer = (playerTarget.position - transform.position).normalized;
            float angleBetweenGuardAndPlayer = Vector3.Angle(new Vector3(eyes.forward.x, 0, eyes.forward.z), directionToPlayer);

            // And the player is within the guard's line of sight
            if (angleBetweenGuardAndPlayer < viewAngle / 2) {
                // The player is within the field of view
                return true;
            }
        }

        // If any of the checks are false, the guard can't see the player
        return false;
    }

    int CastToPlayerBones () {
        int amountOfVisibleBones = 0;

        foreach (Transform bone in playerBones) {
            if (!Physics.Linecast(eyes.position, bone.position, obstacleMask)) {
                amountOfVisibleBones++;
            }
        }

        return amountOfVisibleBones;
    }

    public Vector3 DirectionFromAngle (float angleInDegrees, bool angleIsGlobal) {
        // Make sure the angle is global
        if (!angleIsGlobal) {
            angleInDegrees += eyes.eulerAngles.y;
        }

        // Return the direction from the guard to the player
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
