using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Respawn")]
    public float fallLimit = -10f;
    public Transform respawnPoint;

    [Header("UI Hearts")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Invincibility")]
    public float invincibleDuration = 1.5f;
    private bool isInvincible = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    void Update()
    {
        // Cek jatuh ke bawah
        if (transform.position.y < fallLimit)
        {
            TakeDamage(1);
            Respawn(); // langsung respawn, jangan Destroy
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHeartsUI();

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibleCoroutine());
    }

    System.Collections.IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    void Die()
    {
        // Reset HP lalu respawn, JANGAN Destroy player
        currentHealth = maxHealth;
        UpdateHeartsUI();
        Respawn();
    }

    void Respawn()
    {
        if (respawnPoint != null)
            transform.position = respawnPoint.position;
        else
            Debug.LogWarning("Respawn Point belum di-assign!");

        rb.linearVelocity = Vector2.zero;
    }
}