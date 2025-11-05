using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    private Vector2 moveDirection;
    public GameObject impactEffect;
    private AudioSource audioSource;
     public AudioClip shootSound;
    private Rigidbody2D rb;
    public GameObject playerBlood;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        rb.velocity = moveDirection * speed;
         if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        moveDirection = dir.normalized;
    }



    void OnCollisionEnter2D(Collision2D hitInfo)
    {
        if (hitInfo.gameObject.CompareTag("Player"))
        {
            hitInfo.gameObject.GetComponent<PlayerMovement>().TakeDamage(10f);
            if (playerBlood != null)
            {
                Instantiate(playerBlood, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
         else if (hitInfo.gameObject.CompareTag("Wall")|| hitInfo.gameObject.CompareTag("UpGround"))
        {
            if (impactEffect != null)
            {
                Instantiate(impactEffect, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
