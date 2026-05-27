using UnityEngine;
using UnityEngine.InputSystem; // Tetap pakai New Input System dengan aman

public class AyamController : MonoBehaviour
{
    private CharacterController controller;

    [SerializeField]
    private Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 720f; 
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    [Header("Camera Settings")]
    public Transform cameraTransform; // Drag Main Camera lu ke slot ini di Inspector!
    public float mouseSensitivity = 0.2f; // Sensivitas putaran mouse
    public float cameraDistance = 5f; // Jarak kamera dari ayam
    public float cameraHeight = 2f; // Tinggi kamera dari tanah
    public float minVerticalAngle = -20f; // Batas kamera nunduk
    public float maxVerticalAngle = 60f; // Batas kamera dongak

    private Vector3 velocity;
    private bool isGrounded;
    
    // Variabel untuk menyimpan akumulasi putaran mouse
    private float mouseX;
    private float mouseY;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // Mengunci kursor mouse di tengah layar agar tidak lepas saat digerakkan
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Jika lupa pasang kamera di inspector, otomatis cari Main Camera
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // ==========================================
        // KOLEKSI INPUT MOUSE & ROTASI KAMERA
        // ==========================================
        if (Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            mouseX += mouseDelta.x * mouseSensitivity;
            mouseY -= mouseDelta.y * mouseSensitivity; // Dikurang agar gerak mouse ke atas bikin kamera dongak

            // Batasi sudut vertikal biar kamera gak muter kebalik
            mouseY = Mathf.Clamp(mouseY, minVerticalAngle, maxVerticalAngle);
        }

        // Hitung rotasi kamera berdasarkan akumulasi input mouse
        Quaternion cameraRotation = Quaternion.Euler(mouseY, mouseX, 0f);
        
        // Tentukan posisi ideal kamera di belakang ayam (Orbit)
        Vector3 cameraTargetPosition = transform.position - (cameraRotation * Vector3.forward * cameraDistance) + (Vector3.up * cameraHeight);
        
        // Aplikasikan posisi dan rotasi ke kamera secara smooth
        if (cameraTransform != null)
        {
            cameraTransform.position = cameraTargetPosition;
            cameraTransform.LookAt(transform.position + Vector3.up * (cameraHeight * 0.5f));
        }

        // ==========================================
        // PERGERAKAN KARAKTER (IKUT ARAH KAMERA)
        // ==========================================
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }

        float moveX = 0f;
        float moveZ = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX = -1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX = 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ = -1f;
        }

        // MODIFIKASI: Arah gerak disesuaikan dengan arah hadap Kamera depan dan kanan
        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;
        camForward.y = 0f; // Kunci sumbu Y biar ayam gak terbang/ambles saat kamera dongak
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 inputDirection = (camForward * moveZ + camRight * moveX).normalized;

        // Handle Gerak & Rotasi Ayam
        if (inputDirection.magnitude >= 0.1f)
        {
            controller.Move(inputDirection * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        // Handle Jumping
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("Jump");
        }

        // Apply Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}