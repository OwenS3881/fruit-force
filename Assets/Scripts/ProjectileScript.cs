using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Chronos;
using Photon.Pun;

public class ProjectileScript : BaseBehaviour
{

    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private float particleVelocityThreshold = 0.5f;
    private float targetRotation;
    public GameObject particles;
    [Header("Effects")]
    public GameObject deathEffect;
    public Material goodMat;
    public Material badMat;
    public GameObject goodEffects;
    public GameObject badEffects;
    public Gradient goodGradient;
    public Gradient badGradient;
    [Header("Photon")]
    public PhotonView view;
    public bool isPhoton;

    private void Start()
    {
        UpdateEffects();
    }

    bool IsMine()
    {
        return !isPhoton || view.IsMine;
    }

    void UpdateEffects()
    {
        if (!isPhoton)
        {
            return;
        }
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (IsMine())
        {
            sr.material = goodMat;
            goodEffects.SetActive(true);
            badEffects.SetActive(false);
            trail.colorGradient = goodGradient;
        }
        else
        {
            sr.material = badMat;
            goodEffects.SetActive(false);
            badEffects.SetActive(true);
            trail.colorGradient = badGradient;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Shootable")
        {
            FindObjectOfType<PostProcessingManager>().startHitEffect = true;
            StartCoroutine(DestroySelf(0f));
        }
        else if (other.tag == "Deadly")
        {
            StartCoroutine(DestroySelf(0));
        }
        else if (isPhoton && other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DestroySelf(0.75f));
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Shootable")
        {
            FindObjectOfType<PostProcessingManager>().startHitEffect = true;
            StartCoroutine(DestroySelf(0f));
        }
        else if (other.gameObject.tag == "Deadly")
        {
            StartCoroutine(DestroySelf(0));
        }
        else if (isPhoton && other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DestroySelf(0.25f));
        }
    }

    float PointSelf(Vector3 Start, Vector3 Target)
    {
      Vector3 direction = Target - Start;
      double dX = direction.x;
      double dY = direction.y;
      double newDirection = Math.Atan2(dY, dX);
      newDirection = newDirection*(180/Math.PI);
      float floatNewDirection = (float) newDirection;
      return floatNewDirection;
    }

    public void Despawn()
    {
      StartCoroutine(DestroySelf(0f));
    }

    IEnumerator DestroySelf(float TimeS)
    {
        yield return time.WaitForSeconds(TimeS);
        if (!isPhoton)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (IsMine())
        {
            PhotonNetwork.Instantiate(deathEffect.name, transform.position, Quaternion.identity);
            PhotonNetwork.Destroy(gameObject);         
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEffects();
      targetRotation = PointSelf(Vector3.zero, time.rigidbody2D.velocity);
      if (transform.rotation.eulerAngles.z < targetRotation+rotateSpeed || transform.rotation.eulerAngles.z > targetRotation-rotateSpeed)
      {
        transform.eulerAngles = new Vector3 (0,0,targetRotation);
      }
      else if (transform.rotation.eulerAngles.z > targetRotation && transform.rotation.eulerAngles.z <= targetRotation+180)
      {
        transform.Rotate(0,0,-rotateSpeed);
      }
      else if (transform.rotation.eulerAngles.z > targetRotation+180)
      {
        transform.Rotate(0,0,rotateSpeed);
      }

      particles.SetActive(time.rigidbody2D.velocity.magnitude > particleVelocityThreshold);
      if (time.rigidbody2D.velocity.magnitude < particleVelocityThreshold && time.timeScale > 0)
      {
        StartCoroutine(DestroySelf(3f));
      }
    }
}
