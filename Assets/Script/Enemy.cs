using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;
    public int scoreValue = 100;
    private int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRange = 5f;

    [Header("Attack")]
    public int damage = 1;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    private float attackTimer = 0f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    private const float CHECK_RADIUS = 0.2f;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private float distanceToPlayer;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, CHECK_RADIUS, groundLayer);

        attackTimer -= Time.deltaTime;

        if (distanceToPlayer <= attackRange)
        {
            Attack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            if (anim != null) anim.SetBool("IsRunning", false);
        }
    }

    void ChasePlayer()
    {
        float direction = (player.position.x > transform.position.x) ? 1f : -1f;
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        // Kalau musuh masih jalan terbalik, ganti jadi: scale.x = -direction
        Vector3 scale = transform.localScale;
        scale.x = direction * Mathf.Abs(scale.x);
        transform.localScale = scale;

        if (anim != null) anim.SetBool("IsRunning", true);
    }

    void Attack()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        if (anim != null) anim.SetBool("IsRunning", false);

        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            if (anim != null) anim.SetTrigger("Attack");

            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null && distanceToPlayer <= attackRange)
                ph.TakeDamage(damage);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null)
                ph.TakeDamage(damage);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (anim != null) anim.SetTrigger("Hurt");
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        Score.Instance.AddScore(scoreValue); // ← fix dari Score ke ScoreManager
        GetComponent<Collider2D>().enabled = false;

        if (anim != null)
        {
            anim.SetTrigger("Die");
            Destroy(gameObject, 1f);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}