using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Chronos;
using Cinemachine;
using Photon.Pun;
using UnityEngine.Experimental.Rendering.Universal;

public class BlasterPoint : BaseBehaviour
{

  public GameObject LeftArmTarget;
  public GameObject RightArmTarget;
  public Joystick BlasterJoystick;
  public GameObject player;
  [SerializeField] private float sensitivity = 0f;
  [SerializeField] private float pullDecrease = 1;
  [SerializeField] public float forceFactor = 100f;
  private Vector2 TargetStartPos;
  [SerializeField] private bool active = false;
  private Vector2 lastDirection;
  public GameObject[] LegGrounders;
  public bool playerGrounded;
  private int shots;
  [SerializeField] private int maxShots = 3;
  [SerializeField] private float reloadDelay = 1f;
  [SerializeField] private float resetRotateSpeed = 10f;
  public List<GameObject> LightIndicators = new List<GameObject>();
  public GameObject projectile;
  public GameObject muzzleFlash;
  [SerializeField] private float projectileForce = 10;
  public GameObject firePoint;
  public PhysicsMaterial2D BasicPlayer;
  public PhysicsMaterial2D AirPlayer;
  public bool firing = false;
  public Toggle powerUpToggle;
  public GameObject bananaProjectile;
  public GameObject bananaCanvas;
  private List<GameObject> projectiles = new List<GameObject>();
  public int maxProjectiles = 3;
  public CinemachineTargetGroup targetGroup;
  public string type;
  public GameObject MainCameraPositioner;
  public GameObject cherryBoomButton;
  public GameObject cherryProjectile;
  public Vector3 lastSafePos;
  public float camRadius = 3f;
    public OptionsMenu options;
    public LegGrounder headGrounder;
    public bool headGrounded;
    public PhysicsMaterial2D coconutMat;
    public bool endingCutscene = false;
    public PlayerTrigger endingTrigger;
    public GameObject[] uiLightIndicators;
    [Header("Trajectory Line Variables")]
    public GameObject trajectoryPoint;
    GameObject[] trajectoryPoints;
    public int numberOfPoints = 50;
    public float spaceBetweenPoints = 0.025f;
    public LayerMask stopTrajectoryLayer;
    [Header("Photon Stuff")]
    private bool isPhoton;
    public PhotonView view;
    public GameObject blasterUIMultiPrefab;
    private Collider2D[] playerColliders;
    public GameObject powerShield;


    public void PointBlaster(Vector3 Start, Vector3 Target)
  {
    Vector3 direction = Target - Start;
    double dX = direction.x;
    double dY = direction.y;
    double newDirection = Math.Atan2(dY, dX);
    newDirection = newDirection*(180/Math.PI);
    float floatNewDirection = (float) newDirection;
    this.transform.eulerAngles = new Vector3(0,0,floatNewDirection);
  }

  public void PointBlasterVector(Vector2 direction)
  {
    double dX = direction.x;
    double dY = direction.y;
    double newDirection = Math.Atan2(dY, dX);
    newDirection = newDirection*(180/Math.PI);
    float floatNewDirection = (float) newDirection;
    this.transform.eulerAngles = new Vector3(0,0,floatNewDirection);
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

    private void Awake()
    {
        isPhoton = player.GetComponent<PlayerMovement>().isPhoton;
        if (playerColliders == null)
        {
            playerColliders = player.GetComponentsInChildren<Collider2D>();
        }
    }

    void Start()
  {
        TargetStartPos = LeftArmTarget.transform.localPosition;
        shots = maxShots;
        trajectoryPoints = new GameObject[numberOfPoints];
        for (int i = 0; i < numberOfPoints; i++)
        {
            trajectoryPoints[i] = Instantiate(trajectoryPoint, firePoint.transform.position, Quaternion.identity);
        }
  }

    public void PhotonSetup()
    {
        if (playerColliders == null)
        {
            playerColliders = player.GetComponentsInChildren<Collider2D>();
        }
        BlasterJoystick = GameObject.Find("BlasterJoystick").GetComponent<Joystick>();
        if (type.Equals("Banana"))
        {
            powerUpToggle = FindObjectOfType<BananaPowerupController>().gameObject.GetComponent<Toggle>();
        }
        else if (type.Equals("Cherry"))
        {
            powerUpToggle = FindObjectOfType<CherryPowerupController>().gameObject.GetComponent<Toggle>();
        }
        bananaCanvas = GameObject.FindGameObjectWithTag("BananaCanvas");
        bananaCanvas.SetActive(false);
        targetGroup = FindObjectOfType<CinemachineTargetGroup>();
        cherryBoomButton = GameObject.FindGameObjectWithTag("DetonateButton");
        cherryBoomButton.SetActive(false);
        options = FindObjectOfType<OptionsMenu>();
        options.gameObject.SetActive(false);
        GameObject blasterUI = Instantiate(blasterUIMultiPrefab, Vector3.zero, Quaternion.Euler(0,0,-90), GameObject.FindGameObjectWithTag("ButtonCanvas").transform);
        blasterUI.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
        blasterUI.transform.localScale = new Vector3(3, 3, 1);
        blasterUI.GetComponent<RectTransform>().anchoredPosition = new Vector3(50, -225, 0);
        if (powerShield != null)
        {
            Collider2D ps = powerShield.GetComponent<Collider2D>();
            foreach (Collider2D c in playerColliders)
            {
                Physics2D.IgnoreCollision(ps, c);
            }
        }
        foreach (Transform t in blasterUI.GetComponentsInChildren<Transform>())
        {
            if (t.gameObject.CompareTag("BUIL"))
            {
                uiLightIndicators[0] = t.gameObject;
            }
            else if (t.gameObject.CompareTag("BUIM"))
            {
                uiLightIndicators[1] = t.gameObject;
            }
            else if (t.gameObject.CompareTag("BUIR"))
            {
                uiLightIndicators[2] = t.gameObject;
            }
        }
    }

  void OnEnable()
  {
        DoReload();
        active = false;
        if (IsMine())
        {
            targetGroup.AddMember(MainCameraPositioner.transform, 1f, camRadius);
            options.InitializeZoom();
        }    
  }

  void OnDisable()
  {
        if (IsMine())
        {
            targetGroup.RemoveMember(MainCameraPositioner.transform);
        }
  }

    bool IsMine()
    {
        return !isPhoton || view.IsMine;
    }

    public void UpdateCamRadius()
    {
        if (IsMine())
        {
            targetGroup.RemoveMember(MainCameraPositioner.transform);
            targetGroup.AddMember(MainCameraPositioner.transform, 1f, camRadius);
        }
    }


  public void DoReload()
  {
    StartCoroutine(Reload());
  }

  IEnumerator Reload()
  {
    while (true)
    {
      while ((playerGrounded || headGrounded || player.GetComponent<PlayerMovement>().notMoving) && (shots < maxShots))
      {
        yield return time.WaitForSeconds(reloadDelay);
        if ((playerGrounded || headGrounded || player.GetComponent<PlayerMovement>().notMoving) && (shots < maxShots))
        {
          shots++;
        }
      }
      yield return time.WaitForSeconds(0.0000000000001f);
    }
  }

  public void AddShots(int s)
  {
        shots += s;
  }

  IEnumerator Active()
  {
    while (BlasterJoystick.Direction.magnitude != 0)
    {
      lastDirection = BlasterJoystick.Direction;
            UpdateTrajectoryLine();
      if (BlasterJoystick.Direction.magnitude < sensitivity && BlasterJoystick.Direction.magnitude > -sensitivity && (BlasterJoystick.Direction.magnitude > 0.01f || BlasterJoystick.Direction.magnitude < -0.01f))
      {
        active = false;
        yield break;
      }
      if (Timekeeper.instance.Clock("Root").localTimeScale == 0 || player.GetComponent<PlayerMovement>().coconutActive || endingCutscene)
      {
                active = false;
                yield break;
      }
      yield return time.WaitForSeconds(0.0000000000001f);
    }
    if (shots > 0)
    {
      if ((-lastDirection.y > 0 && player.GetComponent<Timeline>().rigidbody2D.velocity.y < 0)||(-lastDirection.y < 0 && player.GetComponent<Timeline>().rigidbody2D.velocity.y > 0))
      {
        player.GetComponent<Timeline>().rigidbody2D.velocity = new Vector2 (player.GetComponent<Timeline>().rigidbody2D.velocity.x, 0);
      }
      if (playerGrounded || headGrounded)
      {
        player.GetComponent<Timeline>().rigidbody2D.velocity = new Vector2 (0, player.GetComponent<Timeline>().rigidbody2D.velocity.y);
        firing = true;
        playerGrounded = false;
        headGrounded = false;
      }
      player.GetComponent<Timeline>().rigidbody2D.AddForce(-lastDirection.normalized*forceFactor, ForceMode2D.Impulse);
      if ((type != "Banana" && type != "Cherry") || powerUpToggle.isOn == false)
      {
                GameObject proj;
                if (isPhoton)
                {
                    proj = PhotonNetwork.Instantiate(projectile.name, firePoint.transform.position, Quaternion.identity);
                    Collider2D[] projCs = proj.GetComponents<Collider2D>();
                    foreach(Collider2D c in playerColliders)
                    {
                        foreach(Collider2D p in projCs)
                        {
                            Physics2D.IgnoreCollision(p, c);
                        }
                    }
                }
                else
                {
                    proj = Instantiate(projectile, firePoint.transform.position, Quaternion.identity);
                }
                proj.GetComponent<ProjectileScript>().isPhoton = isPhoton;
                projectiles.Add(proj);
                proj.GetComponent<Rigidbody2D>().AddForce(lastDirection.normalized*projectileForce, ForceMode2D.Impulse);
                //targetGroup.AddMember(proj.transform, 1f, 1f);
      }
      else if (type == "Banana")
      {
                BananaPowerupController bpc = player.GetComponent<PlayerMovement>().bananaPowerUpObject.GetComponent<BananaPowerupController>();
                bpc.currentOres = 0;
                bananaCanvas.SetActive(true);
                GameObject proj;
                if (isPhoton)
                {
                    powerShield.transform.localScale = new Vector3(powerShield.transform.localScale.x, powerShield.transform.localScale.y, 2);
                    proj = PhotonNetwork.Instantiate(bananaProjectile.name, firePoint.transform.position, Quaternion.identity);
                    proj.GetComponent<BananaProjectile>().powerShield = powerShield;
                    proj.GetComponent<BananaProjectile>().bananaPlayer = player.GetComponent<PlayerMovement>();
                    Collider2D projC = proj.GetComponent<Collider2D>();
                    Physics2D.IgnoreCollision(projC, powerShield.GetComponent<Collider2D>());
                    player.GetComponent<PlayerMovement>().bananaFrozen = true;         
                    foreach (Collider2D c in playerColliders)
                    {
                        Physics2D.IgnoreCollision(projC, c);
                    }
                }
                else
                {
                    proj = Instantiate(bananaProjectile, firePoint.transform.position, Quaternion.identity);
                }
                proj.GetComponent<BananaProjectile>().LastDirection = lastDirection;
                proj.transform.eulerAngles = new Vector3 (0, 0, VectorToFloat(lastDirection)+80f);
                powerUpToggle.isOn = false;
                if (!isPhoton)
                {
                    FindObjectOfType<PostProcessingManager>().startSlowEffect = true;
                }
                else
                {
                    proj.GetComponent<BananaProjectile>().PhotonAwake();
                }
      }
      else if (type == "Cherry")
      {
                FindObjectOfType<CherryPowerupController>().currentOres = 0;
                cherryBoomButton.SetActive(true);
                GameObject proj;
                if (isPhoton)
                {
                    proj = PhotonNetwork.Instantiate(cherryProjectile.name, firePoint.transform.position, transform.rotation);
                    proj.GetComponent<CherryBomb>().player = player.transform;
                }
                else
                {
                    proj = Instantiate(cherryProjectile, firePoint.transform.position, transform.rotation);
                }
                proj.GetComponent<Rigidbody2D>().AddForce(lastDirection.normalized*projectileForce, ForceMode2D.Impulse);
                targetGroup.AddMember(proj.transform, 1f, 1f);
      }
            GameObject muzzle;
            if (isPhoton)
            {
                firePoint.transform.GetChild(0).gameObject.GetComponent<MuzzleMulti>().Burst();
                /*
                muzzle = firePoint.transform.GetChild(0).gameObject;
                Animator mAnim = muzzle.GetComponent<Animator>();
                if (!mAnim.enabled)
                {
                    mAnim.enabled = true;
                }
                else
                {
                    mAnim.Play("Base Layer.Burst");
                }
                */
            }
            else
            {
                muzzle = Instantiate(muzzleFlash, firePoint.transform.position, Quaternion.identity);
                muzzle.transform.parent = firePoint.transform;
            }
            
            active = false;
            AudioManager.instance.PlaySoundOneShot("Blaster");
            shots--;
    }
    else
    {
      active = false;
    }
    yield return time.WaitForSeconds(0.1f);
    firing = false;
  }

    void BlasterRotationStuff()
    {
        if ((BlasterJoystick.Direction.magnitude >= sensitivity || BlasterJoystick.Direction.magnitude <= -sensitivity) && !active)
        {
            active = true;
            StartCoroutine(Active());
        }
        RightArmTarget.transform.position = LeftArmTarget.transform.position;
        if (player.transform.localScale.x > 0 && time.timeScale > 0)
        {
            if (BlasterJoystick.Direction.magnitude >= sensitivity || BlasterJoystick.Direction.magnitude <= -sensitivity)
            {
                PointBlasterVector(BlasterJoystick.Direction);
                LeftArmTarget.transform.localPosition = TargetStartPos + (BlasterJoystick.Direction / pullDecrease);
            }
            else
            {
                if (transform.rotation.eulerAngles.z < resetRotateSpeed || transform.rotation.eulerAngles.z > 360f - resetRotateSpeed)
                {
                    transform.eulerAngles = Vector3.zero;
                }
                else if (transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z <= 180)
                {
                    transform.Rotate(0, 0, -resetRotateSpeed);
                }
                else if (transform.rotation.eulerAngles.z > 180)
                {
                    transform.Rotate(0, 0, resetRotateSpeed);
                }
                LeftArmTarget.transform.localPosition = TargetStartPos;
            }
        }
        else if (time.timeScale > 0)
        {
            if ((BlasterJoystick.Horizontal >= sensitivity || BlasterJoystick.Horizontal <= -sensitivity) || (BlasterJoystick.Vertical >= sensitivity || BlasterJoystick.Vertical <= -sensitivity))
            {
                PointBlasterVector(-BlasterJoystick.Direction);
                Vector2 newDirection = new Vector2(-BlasterJoystick.Horizontal, BlasterJoystick.Vertical);
                LeftArmTarget.transform.localPosition = TargetStartPos + (newDirection / pullDecrease);
            }
            else
            {
                if (transform.localRotation.eulerAngles.z < resetRotateSpeed || transform.localRotation.eulerAngles.z > 360f - resetRotateSpeed)
                {
                    transform.eulerAngles = Vector3.zero;
                }
                else if (transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z <= 180)
                {
                    transform.Rotate(0, 0, resetRotateSpeed);
                }
                else if (transform.rotation.eulerAngles.z > 180)
                {
                    transform.Rotate(0, 0, -resetRotateSpeed);
                }
                LeftArmTarget.transform.localPosition = TargetStartPos;
            }
        }
    }

    private Vector2 PointPosition(float t)
    {
        Vector2 position = (Vector2)firePoint.transform.position + (BlasterJoystick.Direction.normalized * projectileForce * t) + 0.5f * Physics2D.gravity * (t * t);
        return position;
    }

    private bool CheckForEnd(Vector2 pos)
    {
        Collider2D circle = Physics2D.OverlapCircle(pos, spaceBetweenPoints, stopTrajectoryLayer);
        return circle != null;
    }

    void UpdateTrajectoryLine()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            if (type == "Banana" && powerUpToggle.isOn == true)
            {
                trajectoryPoints[i].transform.position = (Vector2)firePoint.transform.position + (BlasterJoystick.Direction.normalized * projectileForce * (i *spaceBetweenPoints));
            }
            else
            {
                trajectoryPoints[i].transform.position = PointPosition(i * spaceBetweenPoints);
            }

            if (CheckForEnd(trajectoryPoints[i].transform.position))
            {
                for (int j = i+1; j < numberOfPoints; j++)
                {
                    trajectoryPoints[j].transform.position = new Vector2(transform.position.x, transform.position.y - 100f);
                }
                return;
            }
        }
    }

    void TrajectoryLineSetAcive(bool a)
    {
        foreach (GameObject p in trajectoryPoints)
        {
            p.SetActive(a);
        }
    }

  void FixedUpdate()
  {

        TrajectoryLineSetAcive(active);

    if (endingCutscene)
        {
            BlasterJoystick.gameObject.SetActive(false);
            FindObjectOfType<MotherShip>().StartMotherShip(this);
        }

    playerGrounded = true;
    foreach (GameObject g in LegGrounders)
    {
      if (g.GetComponent<LegGrounder>().grounded == false)
      {
        playerGrounded = false;
        break;
      }
    }

     headGrounded = headGrounder.grounded;

        if (!player.GetComponent<PlayerMovement>().coconutActive)
        {
            if (IsMine())
            {
                BlasterRotationStuff();
            }
        }

        if (IsMine())
        {
            for (int i = 0; i < LightIndicators.Count; i++)
            {
                uiLightIndicators[i].SetActive(i + 1 <= shots);
                LightIndicators[i].SetActive(i + 1 <= shots);
            }
        }

    if (playerGrounded || headGrounded)
    {
      player.GetComponent<Rigidbody2D>().sharedMaterial = BasicPlayer;
    }
    else
    {
      player.GetComponent<Rigidbody2D>().sharedMaterial = AirPlayer;
    }

    if (player.GetComponent<PlayerMovement>().coconutActive)
        {
            player.GetComponent<Rigidbody2D>().sharedMaterial = coconutMat;
        }

    for (var i = 0; i < projectiles.Count; i++)
    {
      if (projectiles[i] == null)
      {
        projectiles.Remove(projectiles[i]);
      }
    }

    while (projectiles.Count > maxProjectiles)
    {
      projectiles[0].GetComponent<ProjectileScript>().Despawn();
      projectiles.Remove(projectiles[0]);
    }

    if ((playerGrounded || headGrounded) && !player.GetComponent<PlayerMovement>().dead && player.GetComponent<PlayerMovement>().safe)
    {
      lastSafePos = player.transform.position;
    }

        if (endingTrigger != null)
        {
            endingCutscene = endingTrigger.triggered;
        }
    }

}
