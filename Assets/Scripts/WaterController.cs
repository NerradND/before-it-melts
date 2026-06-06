using UnityEngine;

public class WaterController : MonoBehaviour
{
    [Header("Wave Motion Settings")]
    public float waveSpeed = 2f;       // How fast it bobs up and down
    public float waveHeight = 0.5f;    // How high/low it moves from start position

    [Header("Teleport Settings")]
    [Tooltip("Create an empty GameObject at your spawn point and drag it here")]
    public Transform respawnPoint;
    
    [Tooltip("Percentage of total health lost on touch (0.25 = 25% damage)")]
    [Range(0f, 1f)]
    public float waterDamageAmount = 0.25f;

    [Header("References")]
    public HealthDrain healthDrainScript;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Smoothly bob up and down over time using a basic sine wave formula
        float newY = startPosition.y + Mathf.Sin(Time.time * waveSpeed) * waveHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the water is your player character
        if (other.CompareTag("Player") || other.gameObject.name.Contains("Ayam"))
        {
            // 1. Inflict the health penalty
            if (healthDrainScript != null)
            {
                healthDrainScript.TakeDamage(waterDamageAmount);
            }

            // 2. Handle Teleportation safely
            if (respawnPoint != null)
            {
                // If using a Character Controller component, it must be temporarily disabled to override position changes
                CharacterController cc = other.GetComponent<CharacterController>();
                if (cc != null)
                {
                    cc.enabled = false;
                    other.transform.position = respawnPoint.position;
                    cc.enabled = true;
                }
                else
                {
                    other.transform.position = respawnPoint.position;
                }
            }
        }
    }
}