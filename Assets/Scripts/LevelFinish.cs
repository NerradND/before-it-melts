using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player crossed the finish line beacon
        if (other.CompareTag("Player"))
        {
            Debug.Log("Level Complete! Starting victory sequence.");

            // FIX: Search the entire scene for the HealthDrain script since it's on the HP Bar UI
            HealthDrain healthScript = Object.FindFirstObjectByType<HealthDrain>();
            
            if (healthScript != null)
            {
                // This activates the turning routine, locks inputs, and opens the canvas!
                healthScript.TriggerFinishLine();
            }
            else
            {
                Debug.LogError("LEVEL FINISH ERROR: Could not find the HealthDrain script anywhere in the scene!");
            }

            // Disable this trigger box so it doesn't accidentally run twice
            Collider myCollider = GetComponent<Collider>();
            if (myCollider != null) myCollider.enabled = false;
        }
    }
}