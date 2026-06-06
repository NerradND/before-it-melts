using UnityEngine;
using UnityEngine.SceneManagement; // WAJIB untuk urusan pindah scene

public class StartGameController : MonoBehaviour
{
    // Fungsi ini harus 'public' agar bisa dideteksi oleh tombol UI
    public void MulaiLevel()
    {
        Debug.Log("Tombol StartLevel Diklik! Pindah ke SampleScene...");
        
        // Memuat scene bernama "SampleScene"
        // Pastikan penulisan huruf besar dan kecilnya sama persis!
        SceneManager.LoadScene("SampleScene");
    }
}