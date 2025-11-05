using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Needed for Image

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody2D rb;

    [Header("Ground Check")]
    private bool isGrounded = true;

    [Header("Prefab")]
    public GameObject Bullet;
    public Transform firePoint;
    private float Timer;

    [Header("Health System")]
    public Image healthBar;
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Explosion Effect")]
    public GameObject explosionEffect;
    public GameObject impactEffect;

    [Header("Health Regen")]
    public float regenAmount = 2f;   // HP healed per second
    private Coroutine healingCoroutine;

    [Header("Audio")]
    public AudioClip hurtClip;
    private AudioSource audioSource;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        UpdateHealthBar();

        // Start passive regen
        healingCoroutine = StartCoroutine(HealthRegen());

        // Setup audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        Timer += Time.deltaTime;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (Timer > 0.5f)
            {
                Timer = 0;
                Attack();
            }
        }
    }

    void FixedUpdate()
    {
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * maxSpeed, rb.velocity.y);

        if (moveInput > 0)
        {
            transform.localScale = new Vector3(2, 2, 2);
        }
        else if (moveInput < 0)
        {
            transform.localScale = new Vector3(-2, 2, 2);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("UpGround"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("UpGround"))
        {
            isGrounded = false;
        }
    }

    void Attack()
    {
        GameObject bullet = Instantiate(Bullet, firePoint.position, firePoint.rotation);

        // check player face direction
        float direction = transform.localScale.x;

        // apply the direction to the bullet
        bullet.GetComponent<Bullet>().SetDirection(direction);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        // play hurt sound
        if (hurtClip != null && audioSource != null)
            audioSource.PlayOneShot(hurtClip);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
            healthBar.fillAmount = currentHealth / maxHealth;
    }

    void Die()
    {
        Debug.Log("Player died!");
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Call GameManager
        WaveSpawner gm = FindObjectOfType<WaveSpawner>();
        if (gm != null)
        {
            gm.PlayerDied();
        }

        gameObject.SetActive(false);
    }

    IEnumerator HealthRegen()
    {
        while (true)
        {
            if (currentHealth < maxHealth)
            {
                Heal(regenAmount);
            }
            yield return new WaitForSeconds(1f); // heal every second
        }
    }
}
