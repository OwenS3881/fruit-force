using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Chronos;
using Cinemachine;
using Photon.Pun;

public class BananaProjectile : BaseBehaviour
{

    [SerializeField] private GameObject joystickCanvas;
    [SerializeField] private GameObject bananaCanvas;
    [SerializeField] private GameObject mainCameraPositioner;
    [SerializeField] private Joystick pointJoystick;
    [HideInInspector] public Vector3 LastDirection;
    public float speed = 1f;
    [SerializeField] private GameObject pointJoystickHandle;
    public float camMoveDelay = 0.25f;
    public Color defaultColor;
    public Color flashColor;
    public float lifetime = 15f;
    public SpriteRenderer sr;
    public GameObject deathEffect;
    private bool hasDied = false;
    [Header("Photon Stuff")]
    public PhotonView view;
    public GameObject powerShield;
    public PlayerMovement bananaPlayer;
    [SerializeField] private bool isPhoton;
    public BananaPowerupController photonController;

    // Start is called before the first frame update
    void Awake()
    {
        isPhoton = view != null;
        if (!isPhoton)
        {
            Timekeeper.instance.Clock("Root").localTimeScale = 0f;
            FindObjectOfType<CinemachineConfiner>().m_ConfineScreenEdges = false;
            joystickCanvas = GameObject.FindWithTag("JoystickCanvas");
            joystickCanvas.SetActive(false);
            bananaCanvas = GameObject.FindWithTag("BananaCanvas");
            mainCameraPositioner = GameObject.FindWithTag("MainCameraPositionerBanana");
            pointJoystick = GameObject.FindWithTag("PointJoystick").GetComponent<FixedJoystick>();
            pointJoystickHandle = GameObject.FindWithTag("PointJoystickHandle");
            StartCoroutine(FlashSequence());
            StartCoroutine(Die());
        }
    }

    public void PhotonAwake()
    {
        FindObjectOfType<CinemachineConfiner>().m_ConfineScreenEdges = false;
        if (IsMine())
        {
            photonController = bananaPlayer.bananaPowerUpObject.GetComponent<BananaPowerupController>();
            joystickCanvas = GameObject.FindWithTag("JoystickCanvas");
            joystickCanvas.SetActive(false);
            bananaCanvas = GameObject.FindWithTag("BananaCanvas");
            mainCameraPositioner = GameObject.FindWithTag("MainCameraPositionerBanana");
            pointJoystick = GameObject.FindWithTag("PointJoystick").GetComponent<FixedJoystick>();
            pointJoystickHandle = GameObject.FindWithTag("PointJoystickHandle");
        }
        StartCoroutine(FlashSequence());
        StartCoroutine(Die());
    }

    bool IsMine()
    {
        return !isPhoton || view.IsMine;
    }

    IEnumerator FlashSequence()
    {
      float gap = 2f;
      float max = (lifetime*5f);
      while (true)
      {
        yield return time.WaitForSeconds(lifetime/gap);
        StartCoroutine(Flash(0.1f));
        gap += 6;
        gap = Mathf.Clamp(gap, 0f, max);
      }
    }

    IEnumerator Flash(float delay)
    {
        AudioManager.instance.PlaySoundOneShot("BananaFlash");
      sr.color = flashColor;
      yield return time.WaitForSeconds(delay);
      sr.color = defaultColor;
    }

    IEnumerator Die()
    {
      if (!hasDied)
      {
            yield return time.WaitForSeconds(lifetime);
            AudioManager.instance.PlaySound("BananaBoom");
            if (!isPhoton)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }
            else if (IsMine())
            {
                PhotonNetwork.Instantiate(deathEffect.name, transform.position, Quaternion.identity);
            }
            Collide();
      }
    }

    float VectorToFloat(Vector2 direction)
    {
      double dX = direction.x;
      double dY = direction.y;
      double newDirection = Math.Atan2(dY, dX);
      newDirection = newDirection*(180/Math.PI);
      float floatNewDirection = (float) newDirection;
      return floatNewDirection;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
      if (!hasDied)
      {
        if (other.gameObject.tag == "Shootable")
        {
          FindObjectOfType<PostProcessingManager>().startSlowHitEffect = true;
        }
        else
        {
                AudioManager.instance.PlaySound("BananaBoom");
                Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Collide();
      }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<StickyPlatform>() != null && other.isTrigger)
        {
            return;
        }
      if (!hasDied)
      {
        if (other.gameObject.tag == "Shootable")
        {
          FindObjectOfType<PostProcessingManager>().startSlowHitEffect = true;
        }
        else
        {
                AudioManager.instance.PlaySound("BananaBoom");
                Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        Collide();
      }
    }

    void Collide()
    {
        if (!hasDied)
        {
            hasDied = true;
            if (IsMine())
            {
                joystickCanvas.SetActive(true);
                bananaCanvas.SetActive(false);
                pointJoystickHandle.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                pointJoystick.input = Vector2.zero;
                if (isPhoton)
                {
                    photonController.Invoke("MoveCam", camMoveDelay);
                    photonController.currentOres = 0;
                }
            }
                
            if (!isPhoton)
            {
                    FindObjectOfType<BananaPowerupController>().Invoke("MoveCam", camMoveDelay);
                    FindObjectOfType<BananaPowerupController>().currentOres = 0;
                    FindObjectOfType<PostProcessingManager>().stopSlowEffect = true;
                    Destroy(gameObject);
            }
            else if (IsMine())
            {
                powerShield.transform.localScale = new Vector3(powerShield.transform.localScale.x, powerShield.transform.localScale.y, 1);
                bananaPlayer.bananaFrozen = false;
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
      if (IsMine())
      {
            mainCameraPositioner.transform.position = transform.position;
            if (pointJoystick.Direction.magnitude > 0)
            {
                LastDirection = pointJoystick.Direction;
            }
            transform.eulerAngles = new Vector3(0, 0, VectorToFloat(LastDirection) + 80f);
            time.rigidbody2D.velocity = -transform.up * speed;
            //time.rigidbody2D.velocity = Vector2.zero;
        }
    }
}
