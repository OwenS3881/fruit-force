using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyFunctions;

public class Mushroom : MonoBehaviour
{

    public bool bouncy;
    public float bouncyForce = 10f;

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.tag == "Player" || other.tag == "Projectile")
      {
        GetComponent<Animator>().Play("Base Layer.Mushroom-rustle");
            if (bouncy && other.gameObject.GetComponent<Rigidbody2D>() != null)
            {
                Bounce(other.gameObject.GetComponent<Rigidbody2D>());
            }
      }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" || other.gameObject.tag == "Projectile")
        {
            GetComponent<Animator>().Play("Base Layer.Mushroom-rustle");
            if (bouncy && other.gameObject.GetComponent<Rigidbody2D>() != null)
            {
                Bounce(other.gameObject.GetComponent<Rigidbody2D>());
            }
        }
    }

    public void Bounce(Rigidbody2D otherRB)
    {
        /*
        Vector2 direction;
        float directionF = VectorToFloat((Vector2)transform.position - otherRB.position);
        directionF -= transform.rotation.eulerAngles.z;
        while (directionF < 0)
        {
            directionF += 360f;
        }
        if (directionF >= 315 || directionF <= 45)
        {
            direction = transform.up;
        }
        else if (directionF <= 135)
        {
            direction = -transform.right;
        }
        else if (directionF >= 225)
        {
            direction = transform.right;
        }
        else
        {
            direction = -transform.up;
        }
        otherRB.AddForce(direction.normalized * bouncyForce, ForceMode2D.Impulse);
        */

        Vector2 direction = otherRB.position - (Vector2)transform.position;
        otherRB.AddForce(direction.normalized * bouncyForce, ForceMode2D.Impulse);
    }
}
