using UnityEngine;

// MAKE SURE THIS MATCHES YOUR FILENAME EXACTLY (Case Sensitive!)
public class BGMManager : MonoBehaviour 
{
    public static BGMManager Instance { get; private set; }

    [Header("Audio Components")]
    public AudioSource bgmSource;

    private bool isMuted = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); 
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleMute()
    {
        if (bgmSource == null) return;

        isMuted = !isMuted;
        bgmSource.mute = isMuted;
    }

    public bool IsMuted()
    {
        return isMuted;
    }
}