using UnityEngine;
using System.Collections;

public class PlayTwoAtOnce : MonoBehaviour
{
    [Header("Animators")]
    public Animator animator1A;
    public Animator animator1B;
    public Animator animator2A;
    public Animator animator2B;

    [Header("GameObjects to Control")]
    public GameObject object1A;
    public GameObject object1B;

    public string animation1Name = "Anim1";
    public string animation2Name = "Anim2";

    [Header("Loop Counts")]
    public int loopCount1 = 1;
    public int loopCount2 = 1;

    [Header("Animation Lengths (in seconds)")]
    public float animation1Length = 1f;
    public float animation2Length = 1f;

    public void PlayAnimations()
    {
        // Activate objects for animation 1
        if (object1A != null) object1A.SetActive(true);
        if (object1B != null) object1B.SetActive(true);

        // Start both animation 1 objects
        StartCoroutine(PlayLoopAndDisable(animator1A, animation1Name, animation1Length, loopCount1, object1A));
        StartCoroutine(PlayLoopAndDisable(animator1B, animation1Name, animation1Length, loopCount1, object1B));

        // Start both animation 2 objects
        StartCoroutine(PlayLoop(animator2A, animation2Name, animation2Length, loopCount2));
        StartCoroutine(PlayLoop(animator2B, animation2Name, animation2Length, loopCount2));
    }

    private IEnumerator PlayLoop(Animator animator, string animationName, float animLength, int loopCount)
    {
        for (int i = 0; i < loopCount; i++)
        {
            animator.Play(animationName, 0, 0f);
            yield return new WaitForSeconds(animLength);
        }
    }

    private IEnumerator PlayLoopAndDisable(Animator animator, string animationName, float animLength, int loopCount, GameObject objToDisable)
    {
        for (int i = 0; i < loopCount; i++)
        {
            animator.Play(animationName, 0, 0f);
            yield return new WaitForSeconds(animLength);
        }

        if (objToDisable != null)
        {
            objToDisable.SetActive(false);
        }
    }
}
