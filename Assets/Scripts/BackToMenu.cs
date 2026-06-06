using UnityEngine;
using UnityEngine.SceneManagement; // WAJIB untuk urusan pindah scene

public class BackToMenu : MonoBehaviour
{
    // Fungsi ini harus 'public' agar bisa dipanggil oleh UI Button
    public void KembaliKeMainMenu()
    {
        Debug.Log("Tombol Kembali Diklik! Pindah ke MainMenu...");
        
        // Memuat scene bernama "MainMenu"
        // PASTIKAN besar kecil hurufnya sama persis dengan nama file scene Main Menu kamu!
        // Contoh: "MainMenu", "Mainmenu", atau "Main Menu" (pakai spasi)
        SceneManager.LoadScene("Main Menu");
    }
}