using UnityEngine;

public class GuardAnimator : MonoBehaviour
{
    Animator animator;

    void Awake () {
        animator = GetComponent<Animator>();
    }

    public void SetState (bool isMoving) {
        animator.SetBool("isMoving", isMoving);
    }

    public void LookAround () {
        animator.SetTrigger("lookAround");
    }
}
