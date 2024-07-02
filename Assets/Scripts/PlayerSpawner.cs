using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPoints;
    public GameObject oreSpawnerPrefab;
    public Transform[] oreSpawnerPositions;

    private void Awake()
    {
        int index;
        if (PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"] != null)
        {
            index = (int)PhotonNetwork.LocalPlayer.CustomProperties["playerAvatar"];
        }
        else
        {
            index = 0;
        }

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            foreach (Transform t in oreSpawnerPositions)
            {
                if (t == null)
                {
                    continue;
                }
                PhotonNetwork.Instantiate(oreSpawnerPrefab.name, t.position, Quaternion.identity);
            }
        }

        GameObject playerToSpawn = playerPrefabs[index];
        GameObject player = PhotonNetwork.Instantiate(playerToSpawn.name, Vector3.zero, Quaternion.identity);
        player.transform.position = spawnPoints[player.GetComponentInChildren<PlayerMovement>().view.Owner.ActorNumber - 1].position;
        player.GetComponentInChildren<PlayerMovement>().nameText.text = PhotonNetwork.LocalPlayer.NickName;
    }
}
