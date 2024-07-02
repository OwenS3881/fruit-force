using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using static MyFunctions;

public class PhotonGameManager : MonoBehaviour
{
    public PlayerMovement[] unsortedPlayers;
    public PlayerMovement[] players;
    private Dictionary<int, Photon.Realtime.Player> photonPlayerList;
    private int totalPlayers;
    public bool allPlayersIn;
    public TMP_Text startingText;
    public Transform livesSection;
    public GameObject livesDisplayItem;
    public LivesDisplay[] playerDisplays;
    public bool gameStarted;
    public bool gameFinished;
    public PhotonView view;
    public int livingPlayers;
    //There is no variable for a player's lives, instead,
    //their lives are stored in their z scale

    private void Awake()
    {
        photonPlayerList = PhotonNetwork.CurrentRoom.Players;
        totalPlayers = PhotonNetwork.CurrentRoom.PlayerCount;
        livingPlayers = totalPlayers;
    }

    void OpenDoors()
    {
        GameObject[] startDoors = GameObject.FindGameObjectsWithTag("PhotonStartDoor");
        foreach (GameObject g in startDoors)
        {
            g.GetComponentInChildren<Animator>().Play("Base Layer.PhotonDoor-Open");
        }
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1.5f);
        startingText.text = "Get Ready!";
        yield return new WaitForSeconds(1.5f);
        startingText.text = "3";
        yield return new WaitForSeconds(1f);
        startingText.text = "2";
        yield return new WaitForSeconds(1f);
        startingText.text = "1";
        yield return new WaitForSeconds(1f);
        startingText.text = "GO!";
        OpenDoors();
        yield return new WaitForSeconds(1f);
        startingText.text = "";
        CreateLivesDisplay();
        gameStarted = true;
    }

    void CreateLivesDisplay()
    {
        playerDisplays = new LivesDisplay[totalPlayers];
        for (int i = 0; i < players.Length; i++)
        {
            PlayerMovement p = players[i];
            GameObject livesDisplayObject = PhotonNetwork.Instantiate(livesDisplayItem.name, livesSection.position, Quaternion.identity);
            livesDisplayObject.transform.SetParent(livesSection);
            livesDisplayObject.transform.localScale = new Vector3(1, 1, 1);
            LivesDisplay livesDisplay = livesDisplayObject.GetComponent<LivesDisplay>();
            livesDisplay.name = p.nameText.text;
            playerDisplays[i] = livesDisplay;
        }
        livesSection.gameObject.SetActive(true);
    }

    void EndRound(PlayerMovement player)
    {
        if (gameFinished)
        {
            return;
        }
        startingText.text = player.nameText.text + " has won the round!";
        gameFinished = true;
        StartCoroutine(DelayLeaveMatch(5f));
    }

    IEnumerator DelayLeaveMatch(float delay)
    {
        yield return new WaitForSeconds(delay);
        LeaveMatch();
    }

    public void LeaveMatch()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel("Lobby");
    }

    // Update is called once per frame
    void Update()
    {
        if (unsortedPlayers.Length == totalPlayers && players.Length == 0)
        {
            players = new PlayerMovement[totalPlayers];
            foreach (PlayerMovement p in unsortedPlayers)
            {
                players[p.view.Owner.ActorNumber - 1] = p;
            }
            allPlayersIn = true;
            StartCoroutine(StartGame());
        }
        else if (!allPlayersIn)
        {
            unsortedPlayers = FindObjectsOfType<PlayerMovement>();
        }

        if (gameStarted)
        {
            int tempLivingPlayers = 0;
            for (int i = 0; i < players.Length; i++)
            {
                if (players[i] == null)
                {
                    playerDisplays[i].lives = 0;
                    continue;
                }
                if (players[i].transform.localScale.z > 0)
                {
                    tempLivingPlayers++;
                }
                playerDisplays[i].lives = (int)players[i].transform.localScale.z;
            }
            livingPlayers = tempLivingPlayers;
            if (livingPlayers == 1)
            {
                foreach (PlayerMovement p in players)
                {
                    if (p != null && p.transform.localScale.z > 0)
                    {
                        EndRound(p);
                    }
                }
            }
        }
    }
}
