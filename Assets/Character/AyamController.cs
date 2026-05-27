using UnityEngine;
using UnityEngine.InputSystem; // Menggunakan New Input System secara clean

public class AyamController : MonoBehaviour
{
    private CharacterController controller;

    [Header("Animation Setup")]
    [SerializeField]
    private Animator animator; 

    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 720f; 
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Camera Settings")]
    public Transform cameraTransform; 
    public float mouseSensitivity = 0.2f; 
    public float cameraDistance = 5f; 
    public float cameraHeight = 2f; 
    public float minVerticalAngle = -20f; 
    public float maxVerticalAngle = 60f; 

    private Vector3 velocity;
    private bool isGrounded;
    
    private float mouseX;
    private float mouseY;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Jaminan manual: Kalau slot kosong, paksa cari di anak
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        // Kunci kursor mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // ==========================================
        // 1. ROTASI KAMERA (MOUSE)
        // ==========================================
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            mouseX += mouseDelta.x * mouseSensitivity;
            mouseY -= mouseDelta.y * mouseSensitivity; 
            mouseY = Mathf.Clamp(mouseY, minVerticalAngle, maxVerticalAngle);
        }

        Quaternion cameraRotation = Quaternion.Euler(mouseY, mouseX, 0f);
        Vector3 cameraTargetPosition = transform.position - (cameraRotation * Vector3.forward * cameraDistance) + (Vector3.up * cameraHeight);
        
        if (cameraTransform != null)
        {
            cameraTransform.position = cameraTargetPosition;
            cameraTransform.LookAt(transform.position + Vector3.up * (cameraHeight * 0.5f));
        }

        // ==========================================
        // 2. SISTEM INPUT KEYBOARD BARU (LEBIH SENSITIF)
        // ==========================================
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        float moveX = 0f;
        float moveZ = 0f;

        // Menggunakan Keyboard.current secara presisi dengan fallback check
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            // Cek tombol W / S / Up / Down
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) moveZ = 1f;
            else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) moveZ = -1f;

            // Cek tombol A / D / Left / Right
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) moveX = 1f;
            else if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) moveX = -1f;
        }

        // Hitung arah gerak berdasarkan sudut pandang kamera
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f; 
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 inputDirection = (camForward * moveZ + camRight * moveX).normalized;

        // ==========================================
        // 3. EKSEKUSI JALAN & TRANSISE ANIMASI LARI
        // ==========================================
        // Kita pakai toleransi angka kecil (0.01f) biar deteksinya super sensitif
        if (inputDirection.magnitude > 0.01f)
        {
            // Gerakkan fisik karakter
            controller.Move(inputDirection * moveSpeed * Time.deltaTime);

            // Putar badan karakter
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Paksa nyalakan checkbox parameter di Animator
            if (animator != null)
            {
                animator.SetBool("isRunning", true);
            }
        }
        else
        {
            // Paksa matikan checkbox parameter di Animator saat tombol dilepas
            if (animator != null)
            {
                animator.SetBool("isRunning", false);
            }
        }

        // ==========================================
        // 4. LOGIKA LONCAT (JUMP)
        // ==========================================
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }

        // Efek Gravitasi
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}