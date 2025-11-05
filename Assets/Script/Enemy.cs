using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Effects")]
    public GameObject explosionEffect;
    public AudioClip hurtClip; 

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public float shootInterval = 2f;
    private float shootTimer;
    public Transform firePoint;

    [Header("Movement")]
    private Transform player;
    public float moveSpeed = 2f;
    public float shootRange = 20f;

    public System.Action onDeath;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        // Flip enemy depending on player position
        if (player.position.x < transform.position.x)
            transform.localScale = new Vector3(-2, 2, 2);
        else
            transform.localScale = new Vector3(2, 2, 2);

        float distance = Vector2.Distance(transform.position, player.position);

        // Move toward player if out of range
        if (distance > shootRange)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(player.position.x, transform.position.y),
                moveSpeed * Time.deltaTime
            );
        }
        else // Shoot at intervals
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                Shoot();
                shootTimer = shootInterval;
            }
        }
    }

    void Shoot()
    {
        if (player == null) return;

        Vector2 direction = (player.position - firePoint.position).normalized;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            // 🎧 Play hurt sound at default volume
            if (hurtClip != null)
            {
                AudioSource.PlayClipAtPoint(hurtClip, transform.position);
            }

            Instantiate(explosionEffect, transform.position, Quaternion.identity);

            if (onDeath != null)
                onDeath.Invoke();

            Destroy(gameObject);
        }
    }
}
