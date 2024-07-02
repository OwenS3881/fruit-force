using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Chronos;
using TMPro;
using Photon.Pun;

public class PineapplePowerupController : BaseBehaviour
{
    //public float cooldownTime = 15f;
    //public float currentCooldown = 0f;
    public float requiredOres = 3;
    public float currentOres;
    public Image background;
    private Button button;
    public GameObject spike;
    public float slowness = 25f;
    public GameObject player;
    public bool isPhoton;
    private Collider2D[] playerColliders;

    // Start is called before the first frame update
    void Start()
    {
        currentOres = requiredOres;
        button = GetComponent<Button>();
        if (isPhoton)
        {
            PhotonStart();
        }
    }

    private void PhotonStart()
    {
        Physics2D.IgnoreLayerCollision(8, 12, false);
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement p in players)
        {
            if (p.view.IsMine)
            {
                player = p.gameObject;
                playerColliders = player.GetComponentsInChildren<Collider2D>();
                return;
            }
        }
        Debug.LogError("Player not found");
    }

    /*
    IEnumerator Countdown()
    {
        while (currentCooldown > 0)
        {
            yield return time.WaitForSeconds(1f);
            currentCooldown--;
        }
    }

    public void StartCountdown()
    {
        StartCoroutine(Countdown());
    }
    */

    IEnumerator IncreaseCo(float sl)
    {
        for (int i = 0; i < sl; i++)
        {
            currentOres += 1 / sl;
            yield return time.WaitForSeconds(0.01f);
        }
    }

    public void Increase()
    {
        if (currentOres < requiredOres)
        {
            StartCoroutine(IncreaseCo(slowness));
        }
    }

    public void DoPineapplePower()
    {
        StartCoroutine(PineapplePower());
    }

    IEnumerator PineapplePower()
    {
        AudioManager.instance.PlaySound("SpikeBurst");
        currentOres = 0;
        if (!isPhoton)
        {
            GameObject[] emitters = GameObject.FindGameObjectsWithTag("SpikeEmitter");
            foreach (GameObject e in emitters)
            {
                GameObject s = Instantiate(spike, e.transform.position, Quaternion.identity);
                s.transform.right = e.transform.right;            
            }
        }
        else
        {
            Transform[] emitters = player.GetComponentsInChildren<Transform>();
            foreach (Transform e in emitters)
            {
                if (e.CompareTag("SpikeEmitter"))
                {
                    GameObject s = PhotonNetwork.Instantiate(spike.name, e.position, Quaternion.identity);
                    s.transform.right = e.right;
                    Collider2D sc = s.GetComponent<Collider2D>();
                    foreach (Collider2D c in playerColliders)
                    {
                        Physics2D.IgnoreCollision(sc, c);
                    }
                }
            }
        }

        yield return null;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (requiredOres - currentOres <= 0.05f)
        {
            currentOres = requiredOres;
        }

        if (currentOres < requiredOres)
        {
            if (currentOres != 0)
            {
                background.fillAmount = currentOres / requiredOres;
            }
            else
            {
                background.fillAmount = 0;
            }
            button.interactable = false;
        }
        else
        {
            background.fillAmount = 1;
            button.interactable = true;
            currentOres = Mathf.Clamp(currentOres, 0, requiredOres);
        }
    }
}
