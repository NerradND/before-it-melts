using UnityEngine;

public class SugarPickup : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Amount of health to restore (0.1 = 10%, 0.25 = 25%, 1.0 = 100%)")]
    [SerializeField] private float healthRestoreAmount = 0.25f; 

    private bool hasBeenPickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenPickedUp) return;

        // Checks if the object touching the sugar is the player
        if (other.CompareTag("Player"))
        {
            hasBeenPickedUp = true;

            // FIX: Instead of looking on the player, search the entire scene for the HealthDrain script
            HealthDrain playerHealth = Object.FindFirstObjectByType<HealthDrain>();

            if (playerHealth != null)
            {
                playerHealth.Heal(healthRestoreAmount); 
            }
            else
            {
                Debug.LogError("SUGAR ERROR: Could not find the HealthDrain script anywhere in the scene!");
            }

            // Destroys the sugar pickup block instantly
            Destroy(gameObject);
        }
    }
}