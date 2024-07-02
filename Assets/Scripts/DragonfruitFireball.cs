using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DragonfruitFireball : BaseBehaviour
{

    public float speed = 20f;
    private int castDirection = 0;
    public float destroyDistance = 50f;
    private GameObject player;
    public bool isPhoton = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().gameObject;
        if (transform.eulerAngles.z == 0)
        {
            castDirection = 1;
        }
        else
        {
            castDirection = -1;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Shootable" && !isPhoton)
        {
            FindObjectOfType<PostProcessingManager>().startHitEffect = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        time.rigidbody2D.velocity = new Vector2(speed * castDirection, 0);
        if (Vector2.Distance(transform.position, player.transform.position) > destroyDistance)
        {
            if (!isPhoton)
            {
                Destroy(gameObject);
            }
            else if (gameObject.GetPhotonView().IsMine)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }
}
