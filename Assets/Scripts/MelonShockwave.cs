using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MelonShockwave : BaseBehaviour
{

    public float speed = 20f;
    public float destroyTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Shootable")
        {
            FindObjectOfType<PostProcessingManager>().startHitEffect = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time.rigidbody2D.velocity = new Vector2(speed * transform.right.x, 0);
    }
}
