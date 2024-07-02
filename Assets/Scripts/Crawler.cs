using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : BaseBehaviour
{

    public float speed = 1f;
    public LayerMask groundLayer;
    public GameObject deathEffect;
    private bool active = false;
    public float activateDelay = 0.25f;
    public float gravity = 0.25f;
    public LegGrounder legGrounder;
    [Tooltip("WARNING: Only enable this if you are sure that the platform that the crawler is on can be completely walked around")]
    public bool do360 = false;

    private void Start()
    {
        Invoke("Activate", activateDelay);
    }

    public void Flip()
    {
        if (!do360)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    void Activate()
    {
        active = true;
    }

    public void Death(string type)
    {
        if(type == "Projectile")
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (type == "Player")
        {
            if (FindObjectOfType<PlayerMovement>().invincible)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                FindObjectOfType<PostProcessingManager>().startHitEffect = true;
                Destroy(gameObject);
            }
        }
        else if (type == "CherryExplosion")
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            FindObjectOfType<PostProcessingManager>().startHitEffect = true;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (FindObjectOfType<PlayerMovement>().invincible)
            {
                Death("Player");
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 3f, groundLayer);
        if (hit.collider != null)
        {
            transform.up = hit.normal;
        }

        //print(gameObject.name + " right: " + transform.right);

        Vector2 direction = transform.right;
        if (transform.localScale.x > 0)
        {
            direction = -direction;
        }

        if (transform.eulerAngles.z == 180)
        {
            direction = -direction;
        }

        //print(gameObject.name + " direction to be applied: " + direction);

        if (active)
        {
            if (legGrounder.grounded)
            {
                time.rigidbody2D.velocity = new Vector2(direction.x * speed, direction.y * speed/*time.rigidbody2D.velocity.y*/);
            }
            else
            {
                transform.Translate(Vector2.down * gravity);
            }
        }
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0f, transform.eulerAngles.z);
    }
}
