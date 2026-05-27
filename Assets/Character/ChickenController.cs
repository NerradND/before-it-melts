using UnityEngine;
// 1. We have to tell the script to look at the new Input System package
using UnityEngine.InputSystem; 

public class ChickenController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // 2. This is the new system's way of checking a direct keyboard press
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            animator.SetTrigger("Jump");
        }
    }
}