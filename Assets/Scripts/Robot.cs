using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Chronos;

public class Robot : BaseBehaviour
{

    //public GameObject LeftArmTarget;
    //public GameObject RightArmTarget;
    [SerializeField] private GameObject playerCenter;

    //[SerializeField] private float upperBound = 10f;
    //[SerializeField] private float lowerBound = -10f;

    public GameObject deathCenter;
    public GameObject deathEffect;

    private Animator anim;

    public GameObject fork;
    public GameObject forkParent;
    private GameObject currentFork;
    [SerializeField] private bool throwing = false;
    [SerializeField] private float forkParentThrowAngle = 90f;
    [SerializeField] private float forceFactor = 10f;
    [SerializeField] private float aimVariation = 0.05f;
    [SerializeField] private float agroRange = 10f;
    private bool active = false;
    public float startDelay = 3f;


    void Start()
    {
      playerCenter = GameObject.FindGameObjectWithTag("PlayerCenter");
      anim = GetComponent<Animator>();
      CreateNewFork();
      Invoke("Activate", startDelay);
    }

    void Activate()
    {
      active = true;
    }

    void CreateNewFork()
    {
      currentFork = Instantiate(fork, transform.position, Quaternion.identity);
      currentFork.transform.parent = forkParent.transform;
      currentFork.transform.localPosition = new Vector3 (2.86f, 0f, 0f);
      currentFork.transform.localEulerAngles = new Vector3 (0f, 0f, 270f);
      currentFork.transform.localScale = new Vector3 (0.75f, 0.75f, 1f);
      currentFork.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    void destroySelf()
    {
        Destroy(transform.parent.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
      if (other.tag == "Projectile")
      {
        Instantiate(deathEffect, deathCenter.transform.position, Quaternion.identity);
        destroySelf();
      }
      else if (other.tag == "Player")
      {
        if (FindObjectOfType<PlayerMovement>().invincible)
        {
          Instantiate(deathEffect, deathCenter.transform.position, Quaternion.identity);
          FindObjectOfType<PostProcessingManager>().startHitEffect = true;
          destroySelf();
        }
      }
      else if (other.tag == "CherryExplosion")
      {
        Instantiate(deathEffect, deathCenter.transform.position, Quaternion.identity);
        FindObjectOfType<PostProcessingManager>().startHitEffect = true;
        destroySelf();
      }
    }

    IEnumerator Throwing()
    {
      throwing = true;
      anim.Play("Base Layer.Robot-Throw");
      while (forkParent.transform.localEulerAngles.z < forkParentThrowAngle)
      {
        yield return time.WaitForSeconds(0.0000000000001f);
      }
      currentFork.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
      currentFork.GetComponent<Collider2D>().isTrigger = false;
      currentFork.transform.parent = null;
      Vector3 force = playerCenter.transform.position - currentFork.transform.position;
      //force = new Vector3 (force.normalized.x + UnityEngine.Random.Range(-aimVariation, aimVariation), force.normalized.y + UnityEngine.Random.Range(-aimVariation, aimVariation), force.z);
      force = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-aimVariation, aimVariation)) * force;
      currentFork.GetComponent<Timeline>().rigidbody2D.AddForce(force.normalized*forceFactor, ForceMode2D.Impulse);
      yield return time.WaitForSeconds(2f);
      CreateNewFork();
      throwing = false;
    }

    // Update is called once per frame
    void Update()
    {
        playerCenter = GameObject.FindWithTag("PlayerCenter");
        if (playerCenter.transform.position.x > deathCenter.transform.position.x)
        {
          transform.localScale = new Vector3 (-Math.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
        else
        {
          transform.localScale = new Vector3 (Math.Abs(transform.localScale.x), transform.localScale.y, 1);
        }
        /*
        LeftArmTarget.transform.position = playerCenter.transform.position;
        LeftArmTarget.transform.localPosition = new Vector3 (-5f, LeftArmTarget.transform.localPosition.y, 0);
        if (LeftArmTarget.transform.localPosition.y > upperBound)
        {
          LeftArmTarget.transform.localPosition = new Vector3 (LeftArmTarget.transform.localPosition.x, upperBound, 0);
        }

        if (LeftArmTarget.transform.localPosition.y < lowerBound)
        {
          LeftArmTarget.transform.localPosition = new Vector3 (LeftArmTarget.transform.localPosition.x, lowerBound, 0);
        }
        RightArmTarget.transform.position = LeftArmTarget.transform.position;
        */

        if (Vector3.Distance(playerCenter.transform.position, deathCenter.transform.position) < agroRange && !throwing && active)
        {
          StartCoroutine(Throwing());
        }
    }
}
