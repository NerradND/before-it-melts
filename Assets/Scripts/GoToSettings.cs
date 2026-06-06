using UnityEngine;
using UnityEngine.SceneManagement; // WAJIB untuk urusan pindah scene

public class GoToSettings : MonoBehaviour
{
    // Fungsi ini harus 'public' agar bisa dideteksi oleh tombol UI
    public void BukaSettings()
    {
        Debug.Log("Tombol Settings Diklik! Pindah ke scene Settings...");
        
        // Memuat scene bernama "Settings"
        // PASTIKAN besar kecil huruf dan penggunaan spasi/titik dua (jika ada) sama persis dengan nama file aslinya!
        SceneManager.LoadScene("Settings");
    }
}