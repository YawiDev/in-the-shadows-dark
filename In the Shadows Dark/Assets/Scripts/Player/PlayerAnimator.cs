using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator animator;
    PlayerMovement player;

    void Awake () {
        animator = GetComponent<Animator>();
        player = GetComponentInParent<PlayerMovement>();
    }

    public void SetState (string stateName, bool enable) {
        animator.SetBool(stateName, enable);
    }
}
