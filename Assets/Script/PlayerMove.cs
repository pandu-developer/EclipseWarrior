using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 7f;
    public float jumpHeight = 8f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    private const float GROUND_CHECK_RADIUS = 0.2f;

    [Header("Attack")]
    public float attackCooldown = 0.5f;
    public Transform attackPoint;
    public float attackRadius = 0.8f;
    public LayerMask enemyLayer;
    public int attackDamage = 1;

    private float movement;
    private bool isGrounded;
    private bool isAttacking;
    private float attackTimer;
    private Rigidbody2D rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position, GROUND_CHECK_RADIUS, groundLayer
        );

        movement = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isAttacking)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.L) && !isAttacking)
        {
            Attack();
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
                isAttacking = false;
        }

        Flip();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        if (!isAttacking)
            rb.linearVelocity = new Vector2(movement * speed, rb.linearVelocity.y);
        else
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        anim.SetTrigger("Jump");
    }

    void Attack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;
        anim.SetTrigger("Attack");

        if (attackPoint == null) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            attackPoint.position, attackRadius, enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            Enemy e = enemy.GetComponent<Enemy>();
            if (e != null)
                e.TakeDamage(attackDamage);
        }
    }

    void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(movement) > 0.1f && isGrounded;
        anim.SetBool("IsRunning", isRunning);
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("velocityY", rb.linearVelocity.y);

        if (isGrounded && !isAttacking && Mathf.Abs(movement) < 0.1f)
        {
            anim.ResetTrigger("Jump");
            anim.ResetTrigger("Attack");
        }
    }

    void Flip()
    {
        if (Mathf.Abs(movement) < 0.1f) return;

        Vector3 scale = transform.localScale;

        if (movement > 0f)
            scale.x = Mathf.Abs(scale.x);
        else if (movement < 0f)
            scale.x = -Mathf.Abs(scale.x);

        transform.localScale = scale;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, GROUND_CHECK_RADIUS);

        if (attackPoint == null) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}