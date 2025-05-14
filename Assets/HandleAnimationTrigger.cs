using UnityEngine;

public class HandleAnimatorTrigger : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayHandleAnimation()
    {
        animator.Play("HandlePull", 0, 0f); // Plays from start
    }
}
