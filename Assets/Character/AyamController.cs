using UnityEngine;
using UnityEngine.InputSystem;

public class AyamController : MonoBehaviour
{
    private CharacterController controller;

    [Header("Animation Setup")]
    [SerializeField] private Animator animator; 

    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 720f; 
    public float gravity = -20f; 
    public float jumpHeight = 1.2f;

    [Header("Camera Settings")]
    public Transform cameraTransform; 
    public float mouseSensitivity = 0.2f; 
    public float cameraDistance = 5f; 
    public float cameraHeight = 2f; 

    [Header("Terrain Alignment")]
    public Transform characterMesh; // Should be your TiltPivot
    public float alignmentSpeed = 15f; 

    private Vector3 velocity;
    private bool isGrounded;
    private float mouseX;
    private float mouseY;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0) velocity.y = -2f;

        Vector3 moveInput = GetMovementInput();
        
        if (moveInput.magnitude > 0.1f)
        {
            controller.Move(moveInput * moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                Quaternion.LookRotation(moveInput), rotationSpeed * Time.deltaTime);
            
            if (animator != null) animator.SetBool("isRunning", true);
        }
        else
        {
            if (animator != null) animator.SetBool("isRunning", false);
        }

        ApplyTerrainTilt();

        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator != null) animator.SetTrigger("Jump");
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void LateUpdate()
    {
        HandleCamera();
        
        // ====================================================================
        // NETRALISASI SKALA RAHAZIA (ANTI-INFLASI)
        // ====================================================================
        // Jika objek model ayam mendadak membengkak (skala > 1) akibat keyframe animasi,
        // kita paksa objek pembungkusnya (TiltPivot) untuk mengecil dengan rasio yang sama.
        if (characterMesh != null && characterMesh.childCount > 0)
        {
            Transform actualModel = characterMesh.GetChild(0);
            Vector3 currentModelScale = actualModel.localScale;

            // Pastikan skala tidak nol agar tidak crash saat pembagian
            if (currentModelScale.x > 0.001f && currentModelScale.y > 0.001f && currentModelScale.z > 0.001f)
            {
                characterMesh.localScale = new Vector3(
                    1f / currentModelScale.x,
                    1f / currentModelScale.y,
                    1f / currentModelScale.z
                );
            }
        }
        // ====================================================================
    }

    void OnAnimatorMove()
    {
        // Intercepts Animator to prevent Root Motion floating
    }

    void ApplyTerrainTilt()
    {
        if (characterMesh == null) return;
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 3f))
        {
            Vector3 localNormal = transform.InverseTransformDirection(hit.normal);
            float pitch = Mathf.Atan2(localNormal.z, localNormal.y) * Mathf.Rad2Deg;
            float roll = Mathf.Atan2(-localNormal.x, localNormal.y) * Mathf.Rad2Deg;
            Quaternion targetLocalRotation = Quaternion.Euler(pitch, 0f, roll);
            characterMesh.localRotation = Quaternion.Slerp(characterMesh.localRotation, 
                targetLocalRotation, Time.deltaTime * alignmentSpeed);
        }
    }

    private void HandleCamera()
    {
        if (cameraTransform == null) return;
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        mouseX += mouseDelta.x * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY - mouseDelta.y * mouseSensitivity, -20f, 60f);
        Quaternion camRot = Quaternion.Euler(mouseY, mouseX, 0f);
        cameraTransform.position = transform.position - (camRot * Vector3.forward * cameraDistance) + (Vector3.up * cameraHeight);
        cameraTransform.LookAt(transform.position + Vector3.up);
    }

    private Vector3 GetMovementInput()
    {
        if (cameraTransform == null) return Vector3.zero;
        float x = (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0);
        float z = (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0);
        Vector3 forward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 right = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1)).normalized;
        return (forward * z + right * x).normalized;
    }
}