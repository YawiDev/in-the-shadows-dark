using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    Animator animator;

    void Awake () {
        animator = GetComponent<Animator>();
    }

    public void SetState (string stateName, bool enable) {
        animator.SetBool(stateName, enable);
    }
}
