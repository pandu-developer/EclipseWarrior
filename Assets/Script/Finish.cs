using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [Header("Next Level")]
    public string Level2; // isi nama scene berikutnya

    [Header("Delay")]
    public float delay = 1f; // jeda sebelum pindah scene

    private bool isTriggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            Invoke("LoadNextScene", delay);
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(Level2);
    }
}