using UnityEngine;

public class LoopCounter : StateMachineBehaviour
{
    private int currentLoops = 0;

    // This runs automatically every time the animation loop resets
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Calculate current loop cycle based on normalized time
        int loops = Mathf.FloorToInt(stateInfo.normalizedTime);

        if (loops > currentLoops)
        {
            currentLoops = loops;
            animator.SetInteger("CycleCount", currentLoops);
        }
    }

    // Reset the counter if the animation resets
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currentLoops = 0;
        animator.SetInteger("CycleCount", 0);
    }
}