using UnityEngine;
using UnityEngine.UI; // Required for the Toggle component

public class MuteToggleController : MonoBehaviour
{
    private Toggle toggleComponent;
    private bool isSyncing = false;

    void Start()
    {
        toggleComponent = GetComponent<Toggle>();
        SynchronizeToggleState();

        // Listen for when the player clicks the toggle
        toggleComponent.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnEnable()
    {
        // Whenever the pause menu opens or a scene loads, sync it up!
        SynchronizeToggleState();
    }

    public void SynchronizeToggleState()
    {
        if (BGMManager.Instance == null || toggleComponent == null) return;

        // Prevent the listener from firing a loop while syncing
        isSyncing = true; 

        // Set the toggle visual state to match our persistent BGM audio state
        toggleComponent.isOn = BGMManager.Instance.IsMuted();

        isSyncing = false;
    }

    private void OnToggleValueChanged(bool isMuted)
    {
        if (isSyncing) return; // Skip if we are just updating visuals via code

        if (BGMManager.Instance != null)
        {
            BGMManager.Instance.ToggleMute();
        }
    }

    void OnDestroy()
    {
        if (toggleComponent != null)
        {
            toggleComponent.onValueChanged.RemoveListener(OnToggleValueChanged);
        }
    }
}