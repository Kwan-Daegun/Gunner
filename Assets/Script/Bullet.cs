using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 20f;
    public float lifeTime = 2f;
    private float direction = 1f;
    public GameObject impactEffect;
    public AudioClip shootSound;
    private AudioSource audioSource;
    private Rigidbody2D rb;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        rb.velocity = new Vector2(direction * speed, 0f);
        if (shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }

        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(float dir)
    {
        direction = dir;
    }
    void OnCollisionEnter2D(Collision2D hitInfo)
    {

        if (hitInfo.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
            Destroy(hitInfo.gameObject);
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
