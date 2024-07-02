using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using static MyFunctions;

public class MotherShip : BaseBehaviour
{

    public float rotationFactor = 5f;
    public Transform target;
    public Transform end;
    public float speedCap = 10f;
    private bool sequenceStarted = false;
    public Animator fade;
    [Tooltip("end.positon.y - fadeHeight = the height at which fading will start")]
    public float fadeHeight = 20;
    private bool fading;

    private void Start()
    {
        SetChildrenActive(false);
        if (!PlayerPrefs.HasKey("Completed"))
        {
            PlayerPrefs.SetInt("Completed", 0);
        }
    }

    private void SetChildrenActive(bool state)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(state);
        }
    }

    [ContextMenu("StartMotherShip",false,0)]
    public void StartMotherShip(BlasterPoint blaster)
    {
        if (!sequenceStarted)
        {
            StartCoroutine(Sequence(blaster));
            sequenceStarted = true;
        }
    }

    IEnumerator AddCam(BlasterPoint blaster)
    {
        yield return time.WaitForSeconds(0.25f);
        blaster.targetGroup.AddMember(transform, 1f, 5f);
    }

    IEnumerator Sequence(BlasterPoint blaster)
    {
        SetChildrenActive(true);
        StartCoroutine(AddCam(blaster));
        blaster.lastSafePos = Vector3.zero;
        FindObjectOfType<SpeedrunTimer>().gameFinished = true;
        while (Vector2.Distance(transform.position, target.position) > 1f)
        {
            Vector2 direction = target.position - transform.position;
            time.rigidbody2D.AddForce(direction);
            yield return null;
        }
        GetComponentInChildren<Animator>().SetTrigger("Appear");
        while (!GetComponentInChildren<PlayerTrigger>().triggered)
        {
            yield return null;
        }
        GameObject player = FindObjectOfType<PlayerMovement>().gameObject;
        player.transform.parent = transform;
        player.GetComponent<Timeline>().rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        //player.transform.localPosition = new Vector2 (0, -0.25f);
        Destroy(GetComponentInChildren<AreaEffector2D>());
        yield return time.WaitForSeconds(0.25f);
        GetComponentInChildren<Animator>().SetTrigger("Disappear");
        yield return time.WaitForSeconds(0.25f);
        yield return time.WaitForSeconds(GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length);
        while (Vector2.Distance(transform.position, end.position) > 1f)
        {
            Vector2 direction = end.position - transform.position;
            time.rigidbody2D.AddForce(direction);
            yield return null;
        }
    }

    void Exit()
    {
        if (PlayerPrefs.GetInt("Completed") == 0)
        {
            Debug.Log("Loggly: " + "Id: " + SystemInfo.deviceUniqueIdentifier + " Beat the game");
            PlayerPrefs.SetInt("Completed", 1);
        }

        FindObjectOfType<SpeedrunTimer>().End();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0f, 0f, -time.rigidbody2D.velocity.x * rotationFactor);
        time.rigidbody2D.velocity = Vector2.ClampMagnitude(time.rigidbody2D.velocity, speedCap);
        SetPosition(target, 0, GameObject.FindWithTag("PlayerCenter").transform.position.x);
        SetPosition(end, 0, GameObject.FindWithTag("PlayerCenter").transform.position.x);
        if (transform.position.y > end.position.y - fadeHeight && fade.GetCurrentAnimatorStateInfo(0).IsName("Default"))
        {
            fade.SetTrigger("Fade");
        }
        if (fade.GetCurrentAnimatorStateInfo(0).IsName("FadeToBlack") && !fading)
        {
            Invoke("Exit", fade.GetCurrentAnimatorStateInfo(0).length + 1f);
            fading = true;
        }
    }
}
