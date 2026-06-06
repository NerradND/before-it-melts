using UnityEngine;
using UnityEngine.SceneManagement; // WAJIB dimasukkan untuk urusan pindah scene

public class MainMenuController : MonoBehaviour
{
    // Fungsi ini harus 'public' agar bisa dipanggil oleh UI Button
    public void PlayGame()
    {
        // Mengganti scene aktif menjadi scene yang bernama "Level"
        // PASTIKAN besar kecil hurufnya sama persis dengan nama scene kamu
        SceneManager.LoadScene("Level");
    }
}