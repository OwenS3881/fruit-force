using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using Pathfinding;

public class Drone : BaseBehaviour
{
    public float rotationFactor = 10f;
    public float moveSpeed = 400f;
    public GameObject barrel;
    public GameObject firePoint;
    private GameObject player;
    public GameObject fork;
    private bool throwing = false;
    public Vector2 agroRange;
    public float shotForce = 10f;
    public float aimVariation = 10f;
    public GameObject deathEffect;
    private bool active = false;
    public float startDelay = 3f;
    public float shotSpeed = 3f;
    public float nextWaypointDistance = 3f;

    Path path;
    int currentWaypoint;
    bool reachedEndOfPath = false;
    Seeker seeker;

    private void Start()
    {
        while (player == null)
        {
            player = GameObject.FindWithTag("PlayerCenter");
        }

        seeker = GetComponent<Seeker>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);

        Invoke("Activate", startDelay);
    }

    void UpdatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, player.transform.position, OnPathComplete);
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void Activate()
    {
        active = true;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Projectile")
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (other.gameObject.tag == "Player")
        {
            if (FindObjectOfType<PlayerMovement>().invincible)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                FindObjectOfType<PostProcessingManager>().startHitEffect = true;
                Destroy(gameObject);
            }
        }
        else if (other.gameObject.tag == "CherryExplosion")
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            FindObjectOfType<PostProcessingManager>().startHitEffect = true;
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Projectile")
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (other.tag == "Player")
        {
            if (FindObjectOfType<PlayerMovement>().invincible)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
                FindObjectOfType<PostProcessingManager>().startHitEffect = true;
                Destroy(gameObject);
            }
        }
        else if (other.tag == "CherryExplosion")
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            FindObjectOfType<PostProcessingManager>().startHitEffect = true;
            Destroy(gameObject);
        }
    }

    IEnumerator Throwing()
    {
        throwing = true;
        GameObject currentFork = Instantiate(fork, firePoint.transform.position, Quaternion.identity);
        currentFork.GetComponent<Collider2D>().isTrigger = false;
        currentFork.transform.up = player.transform.position - transform.position;
        Vector3 force = player.transform.position - transform.position;
        force = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-aimVariation, aimVariation)) * force;
        currentFork.GetComponent<Timeline>().rigidbody2D.AddForce(force.normalized * shotForce, ForceMode2D.Impulse);
        yield return time.WaitForSeconds(shotSpeed);
        throwing = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0f, 0f, -time.rigidbody2D.velocity.x * rotationFactor);

        barrel.transform.up = -(player.transform.position - transform.position);
        if (Vector3.Distance(player.transform.position, transform.position) < agroRange.y && !throwing && active)
        {
            StartCoroutine(Throwing());
        }

        //All new update code must go above this code below
        if (Vector3.Distance(player.transform.position, transform.position) > agroRange.x && Vector3.Distance(player.transform.position, transform.position) < agroRange.y && active)
        {
            if (path == null)
            {
                return;
            }

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - GetComponent<Rigidbody2D>().position).normalized;
            Vector2 force = direction * moveSpeed * Time.deltaTime;

            time.rigidbody2D.AddForce(force);

            float distance = Vector2.Distance(GetComponent<Rigidbody2D>().position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }
 
    }
}
