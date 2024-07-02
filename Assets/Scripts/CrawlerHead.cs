using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlerHead : MonoBehaviour
{

    public Crawler parent;
    public GameObject edgeDetector;

    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Deadly")
        {
            parent.Flip();
        }

        if (collision.gameObject.tag == "Projectile")
        {
            parent.Death("Projectile");
        }
        else if (collision.gameObject.tag == "Player")
        {
            if (FindObjectOfType<PlayerMovement>().invincible)
            {
                parent.Death("Player");
            }
        }
        else if (collision.gameObject.tag == "CherryExplosion")
        {
            parent.Death("CherryExplosion");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile")
        {
            parent.Death("Projectile");
        }
        else if (other.tag == "Player")
        {
            if (FindObjectOfType<PlayerMovement>().invincible)
            {
                parent.Death("Player");
            }
        }
        else if (other.tag == "CherryExplosion")
        {
            parent.Death("CherryExplosion");
        }
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit2D hit = Physics2D.Raycast(edgeDetector.transform.position, -transform.up, 1f, parent.groundLayer);
        //Debug.DrawRay(edgeDetector.transform.position, -transform.up, Color.red, 0.01f);
        if (hit.collider == null)
        {
            parent.Flip();
        }
    }
}
