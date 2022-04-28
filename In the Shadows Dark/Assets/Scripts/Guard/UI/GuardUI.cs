using UnityEngine;

public class GuardUI : MonoBehaviour
{
    Animator animator;

    void Awake () {
        animator = GetComponent<Animator>();
    }

    public void OnGuardAlerted (ref bool guardIsAlreadyAlerted) {
        if (!guardIsAlreadyAlerted) {
            animator.SetTrigger("guardAlerted");

            guardIsAlreadyAlerted = true;
        }
    }
}
