using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PineappleSpike : BaseBehaviour
{
    
    private bool collided = false;
    public float forceFactor = 10f;
    private Color tmp;
    private bool dying = false;
    public PhotonView view;
    private bool isPhoton;

    private void Start()
    {
        Invoke("InstantDie", 10f);
        isPhoton = view != null;
    }

    void InstantDie()
    {
        if (!isPhoton)
        {
            Destroy(gameObject);
        }
        else if (IsMine())
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    bool IsMine()
    {
        return !isPhoton || view.IsMine;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collide();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Shootable")
        {
            if (!isPhoton)
            {
                FindObjectOfType<PostProcessingManager>().startHitEffect = true;
            }
            Collide();
        }
    }

    void Collide()
    {
        collided = true;
        time.rigidbody2D.velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    IEnumerator Die(float reduce, float delay)
    {
        dying = true;
        tmp = GetComponent<SpriteRenderer>().color;
        while (tmp.a > 0f)
        {
            tmp.a -= reduce;
            GetComponent<SpriteRenderer>().color = tmp;
            yield return time.WaitForSeconds(delay);
        }
        InstantDie();
        yield return null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!collided)
        {
            time.rigidbody2D.velocity = transform.right * forceFactor;
        }
        else
        {
            time.rigidbody2D.velocity = Vector2.zero;
        }

        if (collided && !dying)
        {
            StartCoroutine(Die(0.05f, 0.01f));
        }
    }
}
