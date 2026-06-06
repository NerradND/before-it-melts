using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // Fungsi ini harus bersifat 'public' agar bisa dipanggil oleh UI Button
    public void KeluarDariGame()
    {
        Debug.Log("Tombol Quit Diklik! Game akan menutup...");

        // 1. Jika dijalankan di dalam Editor Unity (saat testing)
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // 2. Jika game sudah di-build menjadi aplikasi (.exe / .apk / .app)
            Application.Quit();
        #endif
    }
}