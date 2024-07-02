using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System;
using Photon.Pun;
using static MyFunctions;

public class Ore : BaseBehaviour
{
    private Animator anim;
    public GameObject collectParticles;
    public Transform particlePos;
    public bool collected;
    public string myGuid;
    public PhotonView view;
    private bool isPhoton;
    private bool photonSpawned;
    public Transform spawnParent;
    public GameObject graphics;
    public SpriteRenderer sr;

    private void Start()
    {
        anim = GetComponent<Animator>();
        isPhoton = view != null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" && anim.GetCurrentAnimatorStateInfo(0).IsName("Ore-idle"))
        {
            if (FindObjectOfType<RespawnManager>() != null && SceneManager.GetActiveScene().name == "MainLevel" && !collected)
            {
                collected = true;
                FindObjectOfType<RespawnManager>().oreCount++;
                FindObjectOfType<DataManager>().SaveData();
            }
            anim.SetTrigger("Collect");
            AudioManager.instance.PlaySoundOneShot("Ore");
            if (!isPhoton)
            {
                Instantiate(collectParticles, particlePos.position, Quaternion.identity);
            }
            else if (!collected)
            {
                collected = true;
                Debug.Log("Calling PhotonCollect() on: " + collision.gameObject.GetPhotonView().Owner.NickName);
                collision.gameObject.GetComponentInParent<PlayerMovement>().PhotonCollect();
                PhotonNetwork.Instantiate(collectParticles.name, particlePos.position, Quaternion.identity);
                //view.TransferOwnership(collision.gameObject.GetPhotonView().Owner);
            }
        }
    }

    void Powerup()
    {
        if (FindObjectOfType<BananaPowerupController>() != null)
        {
            FindObjectOfType<BananaPowerupController>().Increase();
        }
        else if (FindObjectOfType<MelonPowerupController>() != null)
        {
            FindObjectOfType<MelonPowerupController>().Increase();
        }
        else if (FindObjectOfType<CherryPowerupController>() != null)
        {
            FindObjectOfType<CherryPowerupController>().Increase();
        }
        else if (FindObjectOfType<PineapplePowerupController>() != null)
        {
            FindObjectOfType<PineapplePowerupController>().Increase();
        }
        else if (FindObjectOfType<DragonfruitPowerupController>() != null)
        {
            FindObjectOfType<DragonfruitPowerupController>().Increase();
        }
        else if (FindObjectOfType<CoconutPowerupController>() != null)
        {
            FindObjectOfType<CoconutPowerupController>().Increase();
        }
    }

    void Collect()
    {    
        if (!isPhoton)
        {
            Powerup();
            Destroy(gameObject);
        }
        else if (view.IsMine)
        {
            SetScale(transform, 2, 2);
            anim.ResetTrigger("Collect");
            anim.Play("Base Layer.Ore-idle");
            graphics.SetActive(transform.localScale.z == 1);
        }
        collected = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Done"))
        {
            Collect();
        }
        if (collected)
        {
            Color newColor = sr.color;
            newColor.a = 0.5f;
            sr.color = newColor;
        }
        else
        {
            Color newColor = sr.color;
            newColor.a = 1f;
            sr.color = newColor;
        }

        /*
        if (isPhoton && !photonSpawned && spawnParent != null)
        {
            photonSpawned = true;
            transform.parent = spawnParent;
            transform.localPosition = Vector3.zero;
        }
        */

        if (isPhoton)
        {
            graphics.SetActive(transform.localScale.z == 1);
            if (transform.localScale.z != 1)
            {
                collected = false;
            }
        }
        
    }
}
