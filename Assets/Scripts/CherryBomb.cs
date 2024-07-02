using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using Cinemachine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using static MyFunctions;

public class CherryBomb : BaseBehaviour
{

    public GameObject cherryDeath;
    public float boomDistance = 30f;
    public Transform player;
    private bool boomed = false;
    public PhotonView view;
    private bool isPhoton;
    public GameObject photonExplosionEffect;

    private void Start()
    {
        isPhoton = view != null;
        if (!isPhoton)
        {
            player = FindObjectOfType<PlayerMovement>().gameObject.transform;
            if (SceneManager.GetActiveScene().name != "CherryTest")
            {
                FindObjectOfType<CinemachineConfiner>().m_ConfineScreenEdges = false;
            }
        }
    }

    bool IsMine()
    {
        return !isPhoton || view.IsMine;
    }

    public void Boom()
    {
        boomed = true;
        if (!isPhoton && SceneManager.GetActiveScene().name != "CherryTest")
        {
            FindObjectOfType<CinemachineConfiner>().m_ConfineScreenEdges = true;
        }
        if (!isPhoton)
        {
            GetComponent<Animator>().Play("Base Layer.CherryBomb-explode");
        }
        else if (IsMine())
        {
            GameObject effect = PhotonNetwork.Instantiate(photonExplosionEffect.name, transform.position, Quaternion.identity);          
        }

        GameObject death;
        if (isPhoton)
        {
            death = PhotonNetwork.Instantiate(cherryDeath.name, transform.position, Quaternion.identity);
        }
        else
        {
            death = Instantiate(cherryDeath, transform.position, Quaternion.identity);
            death.transform.parent = this.transform;
            death.transform.localScale = new Vector3(1, 1, 1);
        }
            
      GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        if (!isPhoton)
        {
            SetAllClocks(0.25f);
        }
        else if (IsMine())
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void SetAllClocks(float tscale)
    {
      Timekeeper.instance.Clock("Root").localTimeScale = tscale;
      Timekeeper.instance.Clock("Banana").localTimeScale = tscale;
      Timekeeper.instance.Clock("DeathEffects").localTimeScale = tscale;
    }

    IEnumerator DelayedDeath(float delay)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
      if (!isPhoton && GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Done"))
      {
            SetAllClocks(1f);
            Destroy(gameObject);
            
      }
        if (IsMine() && Vector2.Distance(transform.position, player.position) >= boomDistance && !boomed)
        {
            GameObject.FindWithTag("DetonateButton").SetActive(false);
            FindObjectOfType<CherryPowerupController>().Boom();
        }
    }
}
