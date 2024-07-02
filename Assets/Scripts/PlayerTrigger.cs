using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{

    public bool triggered = false;
    public string _tag = "Player";

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(_tag))
        {
            triggered = true;
        }
    }
}
