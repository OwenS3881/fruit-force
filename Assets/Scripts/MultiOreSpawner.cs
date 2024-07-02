using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static MyFunctions;

public class MultiOreSpawner : MonoBehaviourPunCallbacks
{
    public PhotonView view;
    public Transform spawnPoint;
    public Vector2 spawnDelayRange;
    public bool isSpawning;
    public GameObject currentOre;

    // Start is called before the first frame update
    void Start()
    {
        /*
        spawnPoint = transform.GetChild(0);
        if (PhotonNetwork.IsMasterClient)
        {
            isSpawning = true;
            StartCoroutine(SpawnOre(0.1f));
        }
        */
    }

    void ActivateNewOre()
    {
        SetScale(currentOre.transform, 2, 1);
        isSpawning = false;
    }

    /*
    IEnumerator SpawnOre(float delay)
    {
        yield return new WaitForSeconds(delay);
        photonView.RPC("InstantiateOre", RpcTarget.All);
        isSpawning = false;
    }

    [PunRPC] void InstantiateOre()
    {
        GameObject ore = Instantiate(orePrefab, spawnPoint.position, Quaternion.identity);
        ore.transform.parent = spawnPoint;
        ore.transform.localPosition = Vector3.zero;
        ore.GetComponent<Ore>().spawnParent = spawnPoint;
    }
    */

    // Update is called once per frame
    void Update()
    {
        /*
        if (PhotonNetwork.IsMasterClient && spawnPoint.childCount == 0 && !isSpawning)
        {
            StartCoroutine(SpawnOre(Random.Range(spawnDelayRange.x, spawnDelayRange.y)));
            isSpawning = true;
        }
        */
        if (currentOre.transform.localScale.z != 1 && !isSpawning)
        {
            Debug.Log("Ore was collected");
            isSpawning = true;
            Invoke("ActivateNewOre", Random.Range(spawnDelayRange.x, spawnDelayRange.y));
        }
    }
}
