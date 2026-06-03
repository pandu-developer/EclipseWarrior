using UnityEngine;
using UnityEngine.SceneManagement;

public class Coin : MonoBehaviour
{
    [Header("Score")]
    public int scoreValue = 50; // skor yang didapat saat ambil koin

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger kena: " + other.name); // tambah ini

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player menyentuh koin!"); // tambah ini
            Score.Instance.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}