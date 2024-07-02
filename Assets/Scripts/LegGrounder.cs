using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using static MyFunctions;

public class LegGrounder : MonoBehaviour
{

    public List<GameObject> currentCollisions = new List<GameObject>();
    public bool grounded;
    public bool doPlayerRotation = true;

    void OnTriggerEnter2D(Collider2D other)
    {
      currentCollisions.Add(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
      currentCollisions.Remove(other.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      if (doPlayerRotation)
      {
          SetRotation(transform, 2, 90);

      }
      grounded = false;
      for (var i = 0; i < currentCollisions.Count; i++)
      {
        if (currentCollisions[i] != null)
        {
          if (currentCollisions[i].tag == "Ground")
          {
            grounded = true;
            break;
          }
        }
        else
        {
          currentCollisions.Remove(currentCollisions[i]);
        }
      }
    }
}
