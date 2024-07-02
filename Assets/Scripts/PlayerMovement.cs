using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Chronos;
using UnityEngine.UI;
using static MyFunctions;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Linq;

public class PlayerMovement : BaseBehaviour
{

    public Joystick MovementJoystick;
    [SerializeField] private float movespeed = 0f;
    [SerializeField] private float sensitivity = 0f;
    private float horizontalmovement = 0;
    private Animator animator;
    public BlasterPoint BlasterScript;
    public GameObject joystickCanvas;
    public GameObject pauseButtonCanvas;
    public GameObject leftFoot;
    public GameObject rightFoot;
    public GameObject landEffect;
    private bool flying = false;
    public string type;
    public GameObject melonPowerupObject;
    public GameObject bananaPowerUpObject;
    public GameObject cherryPowerupObject;
    public GameObject pineapplePowerupObject;
    public GameObject dragonfruitPowerupObject;
    public GameObject coconutPowerupObject;
    public bool invincible = false;
    public float groundPoundSpeed = -3f;
    public float groundPoundMax = -25f;
    public GameObject groundPoundFX;
    public GameObject groundPoundDust;
    public bool dead = false;
    public float deathY = -100f;
    private bool snapWalk = false;
    private bool spawnImmunity = false;
    public float spawnImmunityLength = 3f;
    public bool safe = true;
    [SerializeField]private List<GameObject> collisions = new List<GameObject>();
    public GameObject dragonfruitFireball;
    public GameObject dragonfruitMuzzleFlash;
    private bool frozen = false;
    public Transform[] ikTargets;
    public bool coconutActive = false;
    private int coconutDirection = 1;
    //public float coconutRollSpeed = 5f;
    public float coconutDuration = 5f;
    [Range(0,90)]
    public float coconutAngle = 45f;
    public float coconutBounceForce = 10f;
    public float coconutTorque = 45f;
    //[Range(0,1)]
    //public float coconutForceScale = 0.1f;
    //private List<GameObject> coconutGrounds = new List<GameObject>();
    private Vector3 defaultCameraPos;
    public float coconutMaxVelocity = 20f;
    private Vector2 playerOffset;
    public float lookSensitvity = 0.5f;
    public float lookDistance = 5f;
    public float lookSpeed = 5f;
    public float lookStartDelay = 1f;
    private float lookStartDelayCounter;
    public GameObject melonShockwave;
    public GameObject blasterUI;
    public Checkpoint secondCheckpoint;
    public GameObject secondCheckpointBlocker;
    public float airMoveSpeed = 0.1f;
    public bool notMoving;
    private Vector2 lastFramePos;
    private int notMovingCount;
    private bool touchingLaser;
    private bool touchingMushroom;
    private bool inGravityZone;
    [Header("Photon Stuff")]
    public bool isPhoton;
    public PhotonView view;
    public bool bananaFrozen;
    private Vector2 bananaFrozenVelocity;
    public TMP_Text nameText;
    private string[] validTags = { "Projectile", "BananaProjectileMulti", "MelonShockwave", "CherryExplosion", "PineappleSpike", "DragonfruitFireball", "CoconutMulti" };
    private Transform photonCenter;
    public GameObject elimMessage;

    private void Awake()
    {
        if (isPhoton && IsMine())
        {
            PhotonSetup();
        }
        if (isPhoton)
        {
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("PlayerCenter"))
                {
                    photonCenter = t;
                    break;
                }
            }
        }

        //GetPlayerByUserId(gameObject.GetPhotonView().Owner.UserId).DisplayElimMessage(gameObject.GetPhotonView().Owner.NickName);
    }

    void Start()
    {
        defaultCameraPos = BlasterScript.MainCameraPositioner.transform.localPosition;
        playerOffset = new Vector2(GameObject.FindWithTag("PlayerCenter").transform.position.x - transform.position.x, GameObject.FindWithTag("PlayerCenter").transform.position.y - transform.position.y);
        animator = GetComponent<Animator>();
    }

    void PhotonSetup()
    {
        MovementJoystick = GameObject.Find("MovementJoystick").GetComponent<Joystick>();
        joystickCanvas = GameObject.FindGameObjectWithTag("JoystickCanvas");
        bananaPowerUpObject = FindObjectOfType<BananaPowerupController>().gameObject;
        melonPowerupObject = FindObjectOfType<MelonPowerupController>().gameObject;
        cherryPowerupObject = FindObjectOfType<CherryPowerupController>().gameObject;
        pineapplePowerupObject = FindObjectOfType<PineapplePowerupController>().gameObject;
        dragonfruitPowerupObject = FindObjectOfType<DragonfruitPowerupController>().gameObject;
        coconutPowerupObject = FindObjectOfType<CoconutPowerupController>().gameObject;
        blasterUI = GameObject.FindGameObjectWithTag("BlasterUI");
        BlasterScript.PhotonSetup();
        Physics2D.IgnoreLayerCollision(21, 11);
        Physics2D.IgnoreLayerCollision(21, 12);
        Physics2D.IgnoreLayerCollision(21, 27);
        Physics2D.IgnoreLayerCollision(21, 8, false);
        SetScale(transform, 2, 3f);
        if (type.Equals("Coconut"))
        {
            groundPoundFX.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            groundPoundFX.transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
        }
    }

    void OnEnable()
    {
        lastFramePos = transform.position;
      flying = false;
        StartCoroutine(SpawnImmunity(spawnImmunityLength));
        if (IsMine())
        {
            bananaPowerUpObject.SetActive(type == "Banana");
            melonPowerupObject.SetActive(type == "Melon");
            cherryPowerupObject.SetActive(type == "Cherry");
            pineapplePowerupObject.SetActive(type == "Pineapple");
            dragonfruitPowerupObject.SetActive(type == "Dragonfruit");
            coconutPowerupObject.SetActive(type == "Coconut");
        }
        if (type == "Banana")
        {
            /*
            if (FindObjectOfType<BananaPowerupController>().currentCooldown > 0)
            {
                FindObjectOfType<BananaPowerupController>().StartCountdown();
            }
            */
        }
        else if (type == "Melon")
        {
            invincible = false;
            groundPoundFX.SetActive(false);
            /*
            if (FindObjectOfType<MelonPowerupController>().currentCooldown > 0)
            {
                FindObjectOfType<MelonPowerupController>().StartCountdown();
            }
            */
        }
        else if (type == "Cherry")
        {
            /*
            if (FindObjectOfType<CherryPowerupController>().currentCooldown > 0)
            {
                FindObjectOfType<CherryPowerupController>().StartCountdown();
            }
            */
        }
        else if (type == "Pineapple")
        {
            /*
            if (FindObjectOfType<PineapplePowerupController>().currentCooldown > 0)
            {
                FindObjectOfType<PineapplePowerupController>().StartCountdown();
            }
            */
        }
        else if (type == "Dragonfruit")
        {

        }
        else if (type == "Coconut")
        {

        }
    }

    IEnumerator SpawnImmunity(float t)
    {
        spawnImmunity = true;
        yield return new WaitForSeconds(t);
        spawnImmunity = false;
    }

    public void PhotonCollect()
    {
        if (!IsMine())
        {
            return;
        }

        Debug.Log(view.Owner.NickName + " has recieved the PhotonCollect() call");
        if (bananaPowerUpObject.activeSelf)
        {
            bananaPowerUpObject.GetComponent<BananaPowerupController>().Increase();
        }
        else if (melonPowerupObject.activeSelf)
        {
            melonPowerupObject.GetComponent<MelonPowerupController>().Increase();
        }
        else if (cherryPowerupObject.activeSelf)
        {
            cherryPowerupObject.GetComponent<CherryPowerupController>().Increase();
        }
        else if (pineapplePowerupObject.activeSelf)
        {
            pineapplePowerupObject.GetComponent<PineapplePowerupController>().Increase();
        }
        else if (dragonfruitPowerupObject.activeSelf)
        {
            dragonfruitPowerupObject.GetComponent<DragonfruitPowerupController>().Increase();
        }
        else if (coconutPowerupObject.activeSelf)
        {
            coconutPowerupObject.GetComponent<CoconutPowerupController>().Increase();
        }
    }

    public void DisplayElimMessage(string playerKilled, string creatorId)
    {
        GameObject em = PhotonNetwork.Instantiate(elimMessage.name, Vector3.zero, Quaternion.identity);
        //GameObject em = Instantiate(elimMessage, Vector3.zero, Quaternion.identity);
        em.transform.SetParent(GameObject.FindGameObjectWithTag("ButtonCanvas").transform);
        em.transform.localScale = new Vector3(1, 1, 1);
        em.GetComponent<RectTransform>().offsetMax = new Vector2(-300, em.GetComponent<RectTransform>().offsetMax.y);
        em.GetComponent<RectTransform>().anchoredPosition = new Vector3(em.transform.localPosition.x, 35, 0);
        em.GetComponent<EliminationMessage>().message = "Eliminated " + playerKilled;
        em.GetComponent<EliminationMessage>().creatorUserId = creatorId;
    }

    public PlayerMovement GetPlayerByUserId(string id)
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement p in players)
        {
            if (p.gameObject.GetPhotonView().Owner.UserId.Equals(id))
            {
                return p;
            }
        }
        Debug.LogError("No Player Found with user id: " + id);
        return null;
    }

    void OnCollisionEnter2D(Collision2D other)
    { 
        //Debug.Log("Collision with: " + other.gameObject.tag);
        if (other.gameObject.tag == "EnemyProjectile")
        {
            Die();
        }
        else if (other.gameObject.tag == "Deadly")
        {
            Die();
        }
        else if (other.gameObject.tag == "Ground" && coconutActive)
        {
            CoconutEffectorEnter(other);
        }
        else if (isPhoton && other.gameObject.GetPhotonView() != null && !other.gameObject.GetPhotonView().Owner.Equals(view.Owner))
        {
            //string[] validTags = { "Projectile", "BananaProjectileMulti", "MelonShockwave", "CherryExplosion", "DragonfruitFireball" }; ;
            if (validTags.Contains(other.gameObject.tag))
            {
                //Debug.Log("other.gameObject.GetPhotonView().ViewID: " + other.gameObject.GetPhotonView().ViewID);
                //Debug.Log("gameObject.GetPhotonView().Owner.NickName: " + gameObject.GetPhotonView().Owner.NickName);
                PhotonDie(other.gameObject);
            }
        }
        /*
        else if (isPhoton && other.gameObject.CompareTag("Projectile") && !other.gameObject.GetComponent<ProjectileScript>().view.IsMine)
        {
            PhotonDie();
        }
        else if (isPhoton && other.gameObject.CompareTag("BananaProjectileMulti") && !other.gameObject.GetComponent<BananaProjectile>().view.IsMine)
        {
            PhotonDie();
        }
        */
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Trigger with: " + other.gameObject.tag);
        if (other.tag == "EnemyProjectile")
        {
            Die();
        }
        else if (other.tag == "Deadly")
        {
            Die();
        }
        else if (isPhoton && other.gameObject.GetPhotonView() != null && !other.gameObject.GetPhotonView().Owner.Equals(view.Owner))
        {
            //string[] validTags = { "Projectile", "BananaProjectileMulti", "MelonShockwave", "CherryExplosion", "DragonfruitFireball" }; ;
            if (validTags.Contains(other.gameObject.tag))
            {
                //Debug.Log("other.gameObject.GetPhotonView().ViewID: " + other.gameObject.GetPhotonView().ViewID);
                //Debug.Log("gameObject.GetPhotonView().Owner.NickName: " + gameObject.GetPhotonView().Owner.NickName);
                PhotonDie(other.gameObject);
            }
        }
        /*
        else if (isPhoton && other.gameObject.CompareTag("Projectile") && !other.gameObject.GetComponent<ProjectileScript>().view.Owner.Equals(view.Owner))
        {
            PhotonDie();
        }
        else if (isPhoton && other.gameObject.CompareTag("BananaProjectileMulti") && !other.gameObject.GetComponent<BananaProjectile>().view.Owner.Equals(view.Owner))
        {
            PhotonDie();
        }
        else if (isPhoton && other.gameObject.CompareTag("MelonShockwave") && !other.gameObject.GetPhotonView().Owner.Equals(view.Owner))
        {
            PhotonDie();
        }
        else if (isPhoton && other.gameObject.CompareTag("CherryExplosion") && !other.gameObject.GetPhotonView().Owner.Equals(view.Owner))
        {
            PhotonDie();
        }
        else if (isPhoton && other.gameObject.CompareTag("PineappleSpike") && !other.gameObject.GetPhotonView().Owner.Equals(view.Owner))
        {
            PhotonDie();
        }
        */

        collisions.Add(other.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collisions.Remove(collision.gameObject);
    }

    void Die()
    {
        if (!invincible && !spawnImmunity && !isPhoton)
        {
            joystickCanvas.SetActive(false);
            if (!isPhoton)
            {
                pauseButtonCanvas.SetActive(false);
            }
            dead = true;
            AudioManager.instance.PlaySound("PlayerDeath");
            FindObjectOfType<PostProcessingManager>().startDieEffect = true;
        }
    }

    [ContextMenu("Photon Die")]
    void PhotonDie(GameObject other)
    {
        Debug.Log("Started Photon Die");
        if (!invincible && !spawnImmunity && !dead && (!type.Equals("Banana") || BlasterScript.powerShield.transform.localScale.z == 1))
        {
            if (joystickCanvas != null)
            {
                joystickCanvas.SetActive(false);
            }
            dead = true;
            if (IsMine())
            {
                SetScale(transform, 2, transform.localScale.z - 1f);
                if (transform.localScale.z == 0)
                {
                    EndPlayer();
                }
            }
            if (transform.localScale.z > 0)
            {
                GetPlayerByUserId(other.GetPhotonView().Owner.UserId).DisplayElimMessage(gameObject.GetPhotonView().Owner.NickName, other.GetPhotonView().Owner.UserId);
                StartCoroutine(PhotonDieCo(5f));
            }
        }
        else
        {
            if (BlasterScript.powerShield != null)
            {
                Debug.Log("Photon Die Failed, Invincible == " + invincible + ", spawnImmunity == " + spawnImmunity + ", dead == " + dead + ", powerShield z scale == " + BlasterScript.powerShield.transform.localScale.z);
            }
            else
            {
                Debug.Log("Photon Die Failed, Invincible == " + invincible + ", spawnImmunity == " + spawnImmunity + ", dead == " + dead + ", powerShield z scale == " + null);
            }
        }
    }

    IEnumerator PhotonDieCo(float respawnTime)
    {
        Vector3 lastPos = BlasterScript.MainCameraPositioner.transform.position;
        transform.position = FindObjectOfType<PlayerSpawner>().spawnPoints[view.Owner.ActorNumber - 1].position;
        //GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

        BlasterScript.MainCameraPositioner.transform.position = lastPos;

        float t = 0;
        while (Vector2.Distance(BlasterScript.MainCameraPositioner.transform.localPosition, defaultCameraPos) > 0.001f)
        {
            BlasterScript.MainCameraPositioner.transform.localPosition = Vector2.Lerp(BlasterScript.MainCameraPositioner.transform.localPosition, defaultCameraPos, t);
            t += Time.deltaTime * (1/respawnTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        //GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        BlasterScript.MainCameraPositioner.transform.localPosition = defaultCameraPos;

        dead = false;
        if (joystickCanvas != null)
        {
            joystickCanvas.SetActive(true);
        }
        //StartCoroutine(SpawnImmunity(spawnImmunityLength));
    }

    void EndPlayer()
    {
        List<GameObject> camPoses = new List<GameObject>();
        foreach (Transform t in BlasterScript.MainCameraPositioner.transform.parent.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.name.Contains("MainCameraPos"))
            {
                camPoses.Add(t.gameObject);
            }
        }
        int index;
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        while (true)
        {
            index = UnityEngine.Random.Range(0, players.Length);
            if (players[index].transform.localScale.z > 0)
            {
                break;
            }
        }
        foreach (GameObject camPos in camPoses)
        {
            camPos.transform.SetParent(players[index].BlasterScript.MainCameraPositioner.transform);
            camPos.transform.localPosition = Vector3.zero;
            camPos.transform.SetParent(camPos.transform.parent.parent);
        }
        SetPosition(transform, 1, 1000);
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        SetScale(transform, 2, -1);
    }

    public void DoMelonPower()
    {
        StartCoroutine(MelonPower(BlasterScript.playerGrounded));
    }

    IEnumerator MelonPower(bool grounded)
    {
        
      FindObjectOfType<MelonPowerupController>().currentOres = 0;
        if (grounded)
        {
            time.rigidbody2D.AddForce(Vector2.up * BlasterScript.forceFactor, ForceMode2D.Impulse);
            yield return time.WaitForSeconds(0.25f);
            time.rigidbody2D.velocity = Vector2.zero;
        }
        groundPoundFX.SetActive(true);
      //Physics2D.IgnoreLayerCollision(8, 20, true);
      while (!BlasterScript.playerGrounded && !touchingLaser && !touchingMushroom)
      {
        invincible = true;
        time.rigidbody2D.velocity = new Vector2 (0, time.rigidbody2D.velocity.y);
        Vector2 force = new Vector2 (0, groundPoundSpeed);
        GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
        time.rigidbody2D.velocity = new Vector2 (0, Mathf.Clamp(time.rigidbody2D.velocity.y, groundPoundMax, 0));

            //Touching Laser
            touchingLaser = false;
            for (int i = 0; i < collisions.Count; i++)
            {
                if (collisions[i] == null) continue;

                if (collisions[i].GetComponent<Laser>() != null)
                {
                    touchingLaser = true;
                    break;
                }
            }

            //Touching Mushrrom
            touchingMushroom = false;
            for (int i = 0; i < collisions.Count; i++)
            {
                if (collisions[i] == null) continue;

                if (collisions[i].transform.root.GetComponent<Mushroom>() != null)
                {
                    touchingMushroom = true;
                    break;
                }
            }

            yield return time.WaitForSeconds(0.00000000000000000000000001f);
      }
        //Physics2D.IgnoreLayerCollision(8, 20, false);
        invincible = false;
      flying = false;
      groundPoundFX.SetActive(false);
        AudioManager.instance.PlaySound("MelonSmash");
        GameObject shockwave1;
        GameObject shockwave2;
        if (isPhoton)
        {
            Transform center = null;
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("PlayerCenter"))
                {
                    center = t;
                    break;
                }
            }
            if (center == null)
            {
                Debug.LogError("That aint good chief, your center is null in the dragonfruit method :(");
                yield break;
            }
            PhotonNetwork.Instantiate(groundPoundDust.name, center.position, Quaternion.identity);
            shockwave1 = PhotonNetwork.Instantiate(melonShockwave.name, GameObject.FindWithTag("PlayerCenter").transform.position, Quaternion.identity);
            shockwave2 = PhotonNetwork.Instantiate(melonShockwave.name, GameObject.FindWithTag("PlayerCenter").transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(groundPoundDust, GameObject.FindWithTag("PlayerCenter").transform.position, Quaternion.identity);
            shockwave1 = Instantiate(melonShockwave, GameObject.FindWithTag("PlayerCenter").transform.position, Quaternion.identity);
            shockwave2 = Instantiate(melonShockwave, GameObject.FindWithTag("PlayerCenter").transform.position, Quaternion.identity);
        }
        shockwave1.transform.right = new Vector3(1, 0, 0);
        shockwave2.transform.right = new Vector3(-1, 0, 0);
    }

    public void DoDragonfruitPower()
    {
        FindObjectOfType<DragonfruitPowerupController>().currentOres = 0;
        AudioManager.instance.PlaySound("DragonfruitFireball");
        StartCoroutine(freezePlayer(0.25f));
        int qdir;
        if (transform.localScale.x > 0)
        {
            qdir = 0;
        }
        else
        {
            qdir = 180;
        }
        Quaternion rotation = Quaternion.Euler(0, 0, qdir);
        if (isPhoton)
        {
            Transform center = null;
            foreach(Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("PlayerCenter"))
                {
                    center = t;
                    break;
                }
            }
            if (center == null)
            {
                Debug.LogError("That aint good chief, your center is null in the dragonfruit method :(");
                return;
            }
            PhotonNetwork.Instantiate(dragonfruitFireball.name, center.position, rotation);
            PhotonNetwork.Instantiate(dragonfruitMuzzleFlash.name, center.position, Quaternion.identity);
        }
        else
        {
            Instantiate(dragonfruitFireball, GameObject.FindWithTag("PlayerCenter").transform.position, rotation);
            Instantiate(dragonfruitMuzzleFlash, GameObject.FindWithTag("PlayerCenter").transform.position, Quaternion.identity);
        }
    }

    public void DoCoconutPower()
    {
        StartCoroutine(CoconutPower());
    }

    IEnumerator CoconutPower()
    {
        Vector3 defaultCameraPosWorld = BlasterScript.MainCameraPositioner.transform.position;
        coconutActive = true;
        if (isPhoton)
        {
            //SetScale(groundPoundFX.transform.GetChild(0).transform, 2, 2);
            groundPoundFX.transform.GetChild(0).transform.localScale = new Vector3(groundPoundFX.transform.GetChild(0).transform.localScale.x, groundPoundFX.transform.GetChild(0).transform.localScale.y, 2);
        }
        if (transform.localScale.x > 0)
        {
            coconutDirection = 1;
        }
        else
        {
            coconutDirection = -1;
        }
        SetRotation(BlasterScript.gameObject.transform, 2, 90f);
        FindObjectOfType<CoconutPowerupController>().currentOres = 0;
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        animator.enabled = false;
        invincible = true;
        float t = 0;
        Transform center = null;
        if (!isPhoton)
        {
            center = GameObject.FindWithTag("PlayerCenter").transform;
        }
        else
        {
            foreach (Transform tr in GetComponentsInChildren<Transform>())
            {
                if (tr.CompareTag("PlayerCenter"))
                {
                    center = tr;
                    break;
                }
            }
            if (center == null)
            {
                Debug.LogError("That aint good chief, your center is null in the dragonfruit method :(");
                yield break;
            }
        }
        while (t < 1)
        {
            time.rigidbody2D.velocity = new Vector2(0, 0);
            foreach (Transform ikt in ikTargets)
            {
                ikt.position = Vector3.Lerp(ikt.position, center.position, t);
            }
            BlasterScript.MainCameraPositioner.transform.position = Vector3.Lerp(defaultCameraPosWorld, center.position, t);
            t += 0.1f;
            yield return time.WaitForSeconds(0.005f);
        }
        if (isPhoton)
        {
            groundPoundFX.GetComponent<ParticleSystem>().Play();
            groundPoundFX.transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
        }
        else
        {
            groundPoundFX.SetActive(true);
        }
        yield return time.WaitForSeconds(coconutDuration);
        EndCoconutPower();
    }

    public void EndCoconutPower()
    {
        StartCoroutine(CoconutEnd());
    }

    IEnumerator CoconutEnd()
    {
        yield return null;
        
        //Vector2 newPlayerOffset = new Vector2(GameObject.FindWithTag("PlayerCenter").transform.position.x - transform.position.x, GameObject.FindWithTag("PlayerCenter").transform.position.y - transform.position.y);
        if (time.rigidbody2D.velocity.sqrMagnitude > time.rigidbody2D.velocity.normalized.sqrMagnitude)
        {
            time.rigidbody2D.velocity = time.rigidbody2D.velocity.normalized;
        }
        if (!isPhoton)
        {
            transform.RotateAround(GameObject.FindWithTag("PlayerCenter").transform.position, transform.forward, -transform.eulerAngles.z);
        }
        else
        {
            Transform center = null;
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.CompareTag("PlayerCenter"))
                {
                    center = t;
                    break;
                }
            }
            if (center == null)
            {
                Debug.LogError("That aint good chief, your center is null in the dragonfruit method :(");
                yield break;
            }
            transform.RotateAround(center.position, transform.forward, -transform.eulerAngles.z);
        }
        SetRotation(transform, 2, 0);
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;  
        animator.enabled = true;
        invincible = false;
        BlasterScript.MainCameraPositioner.transform.localPosition = defaultCameraPos;
        coconutActive = false;
        
        
        if (isPhoton)
        {
            groundPoundFX.transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
            groundPoundFX.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            //SetScale(groundPoundFX.transform.GetChild(0).transform, 2, 1);
            groundPoundFX.transform.GetChild(0).transform.localScale = new Vector3(groundPoundFX.transform.GetChild(0).transform.localScale.x, groundPoundFX.transform.GetChild(0).transform.localScale.y, 1);
        }
        else
        {
            groundPoundFX.SetActive(false);
        }
        //transform.position = transform.position - (Vector3)playerOffset - (Vector3)newPlayerOffset;
        /*
        foreach (GameObject g in coconutGrounds)
        {
            g.GetComponent<Collider2D>().usedByEffector = false;
            Destroy(g.GetComponent<SurfaceEffector2D>());
        }
        coconutGrounds.Clear();
        */
    }

    void CoconutEffectorEnter(Collision2D collider)
    {
        //print("Collided");
        Vector2 norm = collider.contacts[0].normal;
        //Debug.DrawRay(collider.contacts[0].point, collider.contacts[0].normal,Color.green, 5f);
        norm = Quaternion.Euler(0, 0, -coconutAngle*coconutDirection) * norm;
        //Debug.DrawRay(collider.contacts[0].point, norm, Color.red, 5f);
        time.rigidbody2D.AddForce(norm.normalized * coconutBounceForce, ForceMode2D.Impulse);
        AudioManager.instance.PlaySoundOneShot("CoconutCollision");
        //time.rigidbody2D.AddTorque(coconutTorque * coconutDirection);
        /*
        if (ground.GetComponent<Collider2D>() == null)
        {
            return;
        }

        if (ground.GetComponent<SurfaceEffector2D>() != null)
        {
            ground.GetComponent<SurfaceEffector2D>().speed = coconutRollSpeed * coconutDirection;
            return;
        }
        ground.GetComponent<Collider2D>().usedByEffector = true;

        SurfaceEffector2D surface = ground.AddComponent<SurfaceEffector2D>();
        surface.speed = coconutRollSpeed * coconutDirection;
        surface.forceScale = coconutForceScale;
        surface.useContactForce = true;

        coconutGrounds.Add(ground);
        */
    }

    IEnumerator freezePlayer(float t)
    {
        frozen = true;
        StartCoroutine(unfreezePlayer(t));
        while (frozen)
        {
            time.rigidbody2D.velocity = new Vector2(0, 0);
            yield return new WaitForSeconds(0.00000000001f);
        }
    }

    IEnumerator unfreezePlayer(float t)
    {
        yield return time.WaitForSeconds(t);
        frozen = false;
    }

    private void Move()
    {
          if (MovementJoystick.Horizontal >= sensitivity)
          {
            if (BlasterScript.playerGrounded && !BlasterScript.firing && !inGravityZone)
            {
              horizontalmovement = movespeed;
              if (!snapWalk)
              {
                        StartCoroutine(SnapWalk());
              }
            }
            else
            {
              horizontalmovement = time.rigidbody2D.velocity.x;
            }
            transform.localScale = new Vector3 (Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            animator.SetBool("Walking", BlasterScript.playerGrounded);
          }
          else if (MovementJoystick.Horizontal <= -sensitivity && !inGravityZone)
          {
            if (BlasterScript.playerGrounded && !BlasterScript.firing)
            {
              horizontalmovement = -movespeed;
                    if (!snapWalk)
                    {
                        StartCoroutine(SnapWalk());
                    }
            }
            else
            {
              horizontalmovement = time.rigidbody2D.velocity.x;
            }
            transform.localScale = new Vector3 (-Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            animator.SetBool("Walking", BlasterScript.playerGrounded);
          }
          else
          {
            horizontalmovement = time.rigidbody2D.velocity.x;
            animator.SetBool("Walking", false);
          }
          time.rigidbody2D.velocity = new Vector2 (horizontalmovement, time.rigidbody2D.velocity.y);

        //Air walk
        if (!BlasterScript.playerGrounded || inGravityZone)
        {
            if (MovementJoystick.Horizontal >= sensitivity)
            {
                animator.SetBool("Walking", true);
                time.rigidbody2D.AddForce(Vector2.right * airMoveSpeed);
                transform.localScale = new Vector3(Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (MovementJoystick.Horizontal <= -sensitivity)
            {
                animator.SetBool("Walking", true);
                time.rigidbody2D.AddForce(Vector2.left * airMoveSpeed);
                transform.localScale = new Vector3(-Math.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                animator.SetBool("Walking", false);
            }
        }
    }

    IEnumerator Flying()
    {
        flying = true;
        while (true)
        {
            if (BlasterScript.playerGrounded)
            {
                if (type == "Banana")
                {
                    animator.Play("Base Layer.Banana-Land");
                }
                else if (type == "Melon")
                {
                    animator.Play("Base Layer.Melon-land");
                }
                else if (type == "Cherry")
                {
                    animator.Play("Base Layer.Cherry-land");
                }
                else if (type == "Pineapple")
                {
                    animator.Play("Base Layer.Pineapple-land");
                }
                else if (type == "Dragonfruit")
                {
                    animator.Play("Base Layer.Dragonfruit-land");
                }
                else if (type == "Coconut")
                {
                    animator.Play("Base Layer.Coconut-land");
                }
                AudioManager.instance.PlaySound("Land");
                GameObject left;
                GameObject right;
                if (isPhoton)
                {
                    left = PhotonNetwork.Instantiate(landEffect.name, leftFoot.transform.position, Quaternion.identity);
                    right = PhotonNetwork.Instantiate(landEffect.name, rightFoot.transform.position, Quaternion.identity);
                }
                else
                {
                    left = Instantiate(landEffect, leftFoot.transform.position, Quaternion.identity);
                    right = Instantiate(landEffect, rightFoot.transform.position, Quaternion.identity);
                }
                left.transform.parent = leftFoot.transform;
                right.transform.parent = rightFoot.transform;
                flying = false;
                yield break;
            }
            if (invincible)
            {
              yield break;
            }
            yield return time.WaitForSeconds(0.00000000000000000000000001f);
        }
    }

    IEnumerator SnapWalk()
    {
        snapWalk = true;
        while (BlasterScript.playerGrounded && !coconutActive)
        {
            if (!animator.GetBool("Walking"))
            {
                time.rigidbody2D.velocity = new Vector2(0, time.rigidbody2D.velocity.y);
                snapWalk = false;
                yield break;
            }
            yield return time.WaitForSeconds(0.000000000000000000001f);
        }
        snapWalk = false;
    }

    private void Look()
    {
        Vector2 up = new Vector2(defaultCameraPos.x, defaultCameraPos.y + lookDistance);
        Vector2 down = new Vector2(defaultCameraPos.x, defaultCameraPos.y - lookDistance);

        if (!coconutActive && !BlasterScript.endingCutscene && (BlasterScript.playerGrounded || BlasterScript.headGrounded) && (MovementJoystick.Vertical >= lookSensitvity || MovementJoystick.Vertical <= -lookSensitvity))
        {
            lookStartDelayCounter += Time.fixedDeltaTime;
        }


        if (!coconutActive && !BlasterScript.endingCutscene && (BlasterScript.playerGrounded || BlasterScript.headGrounded) && !isPhoton)
        {
            if (MovementJoystick.Vertical >= lookSensitvity && lookStartDelayCounter > lookStartDelay)
            {
                BlasterScript.MainCameraPositioner.transform.localPosition = Vector2.MoveTowards(BlasterScript.MainCameraPositioner.transform.localPosition, up, lookSpeed * Time.deltaTime);
            }
            else if (MovementJoystick.Vertical <= -lookSensitvity && lookStartDelayCounter > lookStartDelay)
            {
                BlasterScript.MainCameraPositioner.transform.localPosition = Vector2.MoveTowards(BlasterScript.MainCameraPositioner.transform.localPosition, down, lookSpeed * Time.deltaTime);
            }
            else
            {
                BlasterScript.MainCameraPositioner.transform.localPosition = Vector2.MoveTowards(BlasterScript.MainCameraPositioner.transform.localPosition, defaultCameraPos, lookSpeed * Time.deltaTime);
            }
        }
        else if (!isPhoton)
        {
            BlasterScript.MainCameraPositioner.transform.localPosition = Vector2.MoveTowards(BlasterScript.MainCameraPositioner.transform.localPosition, defaultCameraPos, lookSpeed * Time.deltaTime);
        }

        if (coconutActive || BlasterScript.endingCutscene || (!BlasterScript.playerGrounded && !BlasterScript.headGrounded) || (MovementJoystick.Vertical <= lookSensitvity && MovementJoystick.Vertical >= -lookSensitvity))
        {
            lookStartDelayCounter = 0;
        }
    }

    private void IsMoving()
    {
        Vector2 dif = (Vector2)transform.position - lastFramePos;
        if (dif.magnitude < 0.01f)
        {
            notMovingCount++;
        }
        else
        {
            notMovingCount = 0;
        }
        notMoving = notMovingCount > 30;
        //Must be last line of function
        lastFramePos = transform.position;
    }

    bool IsMine()
    {
        return !isPhoton || view.IsMine;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!coconutActive && !BlasterScript.endingCutscene)
        {
            if (IsMine())
            {
                Move();
            }
        }

        if (IsMine())
        {
            Look();
            IsMoving();
        }

        if (!BlasterScript.playerGrounded && !flying)
      {
        StartCoroutine(Flying());
      }

      if (transform.position.y < deathY && !dead)
      {
        invincible = false;
        Die();
      }

      if (dead && !coconutActive)
        {
            animator.SetBool("Walking", false);
        }

        if (!coconutActive)
        {
            if (animator.GetBool("Walking"))
            {
                AudioManager.instance.PlaySound("Walk");
            }
            else
            {
                AudioManager.instance.StopSound("Walk");
            }
        }

        safe = true;
        for (var i = 0; i < collisions.Count; i++)
        {
            if (collisions[i] != null)
            {
                if (collisions[i].tag == "Unsafe")
                {
                    safe = false;
                    break;
                }
            }
            else
            {
                collisions.Remove(collisions[i]);
            }
        }

        inGravityZone = false;
        for (var i = 0; i < collisions.Count; i++)
        {
            if (collisions[i] != null)
            {
                if (collisions[i].tag == "GravityZone")
                {
                    inGravityZone = true;
                    break;
                }
            }
            else
            {
                collisions.Remove(collisions[i]);
            }
        }
        BlasterScript.headGrounder.gameObject.SetActive(inGravityZone);
        for (int i = 0; i < BlasterScript.LegGrounders.Length; i++)
        {
            if (invincible)
            {
                BlasterScript.LegGrounders[i].SetActive(true);
            }
            else
            {
                BlasterScript.LegGrounders[i].SetActive(!inGravityZone);
                if (inGravityZone)
                {
                    BlasterScript.playerGrounded = false;
                }
            }
        }

        if (coconutActive)
        {
            time.rigidbody2D.velocity = Vector2.ClampMagnitude(time.rigidbody2D.velocity, coconutMaxVelocity);
        }

        Physics2D.IgnoreLayerCollision(8, 18, coconutActive);

        if (time.rigidbody2D.velocity.x > 0)
        {
            coconutDirection = 1;
        }
        else if (time.rigidbody2D.velocity.x < 0)
        {
            coconutDirection = -1;
        }

        if (BlasterScript.endingCutscene)
        {
            MovementJoystick.gameObject.SetActive(false);
            bananaPowerUpObject.SetActive(false);
            melonPowerupObject.SetActive(false);
            cherryPowerupObject.SetActive(false);
            pineapplePowerupObject.SetActive(false);
            dragonfruitPowerupObject.SetActive(false);
            coconutPowerupObject.SetActive(false);
            if (blasterUI != null)
            {
                blasterUI.SetActive(false);
            }

            GameObject[] menuButtons = GameObject.FindGameObjectsWithTag("MenuButtons");
            foreach (GameObject g in menuButtons)
            {
                g.SetActive(false);
            }
        }

        if (secondCheckpoint != null && secondCheckpointBlocker != null)
        {
            secondCheckpointBlocker.SetActive(!secondCheckpoint.activated);
        }

        if (bananaFrozen)
        {
            if (bananaFrozenVelocity.Equals(Vector2.zero))
            {
                bananaFrozenVelocity = time.rigidbody2D.velocity;
            }
            time.rigidbody2D.velocity = new Vector2(0, 0);
        }
        else if (!bananaFrozenVelocity.Equals(Vector2.zero))
        {
            time.rigidbody2D.velocity = bananaFrozenVelocity;
            bananaFrozenVelocity = Vector2.zero;
        }

        if (isPhoton)
        {
            if (transform.localScale.x > 0)
            {
                SetScale(nameText.transform, 0, 1);
            }
            else
            {
                SetScale(nameText.transform, 0, -1);
            }
            nameText.text = view.Owner.NickName;

            if (type.Equals("Coconut"))
            {
                if (groundPoundFX.transform.GetChild(0).transform.localScale.z == 1)
                {
                    coconutActive = false;
                    groundPoundFX.GetComponent<ParticleSystem>().Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
                    groundPoundFX.transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
                    animator.enabled = true;
                    invincible = false;
                }
                else
                {
                    coconutActive = true;
                    groundPoundFX.GetComponent<ParticleSystem>().Play();
                    groundPoundFX.transform.GetChild(0).GetComponent<Collider2D>().enabled = true;
                    animator.enabled = false;
                    invincible = true;
                    foreach (Transform ikt in ikTargets)
                    {
                        ikt.position = photonCenter.position;
                    }
                }
            }
        }
    }
}
