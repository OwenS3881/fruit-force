using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class ForkScript : BaseBehaviour
{

    public GameObject deathEffect;

    void OnCollisionEnter2D(Collision2D other)
    {
      if (other.gameObject.tag == "Player" || other.gameObject.tag == "Ground")
      {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
      }
    }
}
