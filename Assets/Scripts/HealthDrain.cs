using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class HealthDrain : MonoBehaviour
{
    [Header("UI References")]
    public Image hpBarImage;
    public GameObject gameOverCanvas; 
    public GameObject victoryCanvas; 

    [Header("Player & Camera Freeze")]
    public MonoBehaviour playerScript;
    public MonoBehaviour cameraScript; 

    [Header("Settings")]
    public float totalSurvivalTime = 30f; 

    private float currentHealth = 1f;
    private bool isGameOver = false;

    void Start()
    {
        if (hpBarImage == null) hpBarImage = GetComponent<Image>();
        currentHealth = 1f;
        isGameOver = false;
        
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);
        if (victoryCanvas != null) victoryCanvas.SetActive(false);
    }

    void Update()
    {
        if (currentHealth > 0f && !isGameOver)
        {
            currentHealth -= Time.deltaTime / totalSurvivalTime;
            currentHealth = Mathf.Clamp01(currentHealth);

            if (hpBarImage != null) hpBarImage.fillAmount = currentHealth;

            if (currentHealth <= 0f) TriggerGameOver();
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isGameOver) return;
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp01(currentHealth);
        if (hpBarImage != null) hpBarImage.fillAmount = currentHealth;
        if (currentHealth <= 0f) TriggerGameOver();
    }

    public void Heal(float healAmount)
    {
        if (isGameOver) return;
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp01(currentHealth);
        if (hpBarImage != null) hpBarImage.fillAmount = currentHealth;
    }

    public void TriggerFinishLine()
    {
        if (isGameOver) return;
        isGameOver = true; 

        StartCoroutine(VictoryCutsceneRoutine());
    }

    private IEnumerator VictoryCutsceneRoutine()
    {
        Camera mainCam = Camera.main;

        // --- NEW BULLETPROOF UNPARENT LOGIC ---
        if (mainCam != null)
        {
            // Cut the umbilical cord! Detaches the camera from the chicken's hierarchy
            // so it stays perfectly frozen in world space.
            mainCam.transform.SetParent(null);
        }

        // Disable standard tracking scripts safely
        if (cameraScript != null) cameraScript.enabled = false;
        if (playerScript != null) playerScript.enabled = false;

        if (playerScript != null)
        {
            GameObject playerObj = playerScript.gameObject;
            Animator anim = playerObj.GetComponentInChildren<Animator>();
            
            // Immediately force the chicken into your custom Victory state box
            if (anim != null)
            {
                anim.Play("VictoryPose", 0, 0f);
                anim.Update(0f);
            }

            // Shut down character controller physical movement safely
            CharacterController controller = playerObj.GetComponent<CharacterController>();
            if (controller != null) controller.enabled = false;

            // Wait one engine frame to fully finish processing script states
            yield return null;

            // Smoothly spin the chicken around to look directly back at the detached camera
            if (mainCam != null)
            {
                Transform playerTransform = playerScript.transform;
                float elapsed = 0f;
                float duration = 1.0f; // Spin animation duration (1 second)
                Quaternion startRotation = playerTransform.rotation;
                
                // Calculate direction pointing straight back toward the camera position
                Vector3 targetDir = mainCam.transform.position - playerTransform.position;
                targetDir.y = 0; // Keeps chicken flat on the floor surface
                
                if (targetDir.sqrMagnitude > 0.01f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDir);

                    while (elapsed < duration)
                    {
                        elapsed += Time.deltaTime;
                        playerTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
                        
                        // Continuously reinforce the custom victory animation clip during the turn transition
                        if (anim != null) anim.Play("VictoryPose", 0);

                        yield return null;
                    }
                    playerTransform.rotation = targetRotation; // Hard secure lock face-to-face
                }
            }
        }

        yield return new WaitForSeconds(0.1f);

        // Reveal the Level Completed Victory screen overlay panel
        if (victoryCanvas != null) 
            victoryCanvas.SetActive(true);
        else if (gameOverCanvas != null) 
            gameOverCanvas.SetActive(true); 

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void TriggerGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f; 

        if (gameOverCanvas != null) gameOverCanvas.SetActive(true);
        if (playerScript != null) playerScript.enabled = false;
        if (cameraScript != null) cameraScript.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartLevel() { Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void BackToMainMenu() { Time.timeScale = 1f; SceneManager.LoadScene("Main Menu"); }
}