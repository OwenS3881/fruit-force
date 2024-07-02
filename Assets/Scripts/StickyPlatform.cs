using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyPlatform : MonoBehaviour
{

    private GameObject currentPlayerParent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            GameObject test = collision.transform.root.gameObject;
            if (test.tag == "Player")
            {
                currentPlayerParent = test;
                currentPlayerParent.transform.parent = transform;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            currentPlayerParent.transform.parent = null;
            //currentPlayerParent = null;
        }
    }
}
