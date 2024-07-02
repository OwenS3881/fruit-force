using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviourPunCallbacks
{

    [Header("Start Scene Vars")]
    public GameObject clouds;
    public float cloudSpeed = 0.005f;
    private float cloudTime = 0;
    [Space]
    [Header("Lobby Manager Vars")]
    public TMP_InputField roomInputField;
    public GameObject lobbyParent;
    public GameObject roomParent;
    public TMP_Text roomName;

    public RectTransform roomListContent;
    public RoomItem roomItemPrefab;
    private List<RoomItem> roomItemList = new List<RoomItem>();

    public float timeBetweenUpdates = 1.5f;
    float nextUpdateTime;

    private List<PlayerItem> playerItemsList = new List<PlayerItem>();
    public PlayerItem playerItemPrefab;
    public Transform playerItemParent;

    public GameObject playButton;
    public int minPlayers;

    public TMP_InputField joinInputField;

    public GameObject errorPopup;
    public Transform mainCanvas;

    public TMP_Text waitingText;

    public LevelLoader levelLoader;
    public Button leaveButton;

    ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();

    private void Start()
    {
        PhotonNetwork.JoinLobby();
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
    }

    public void OnClickCreate()
    {
        if (roomInputField.text.Length >= 1)
        {
            PhotonNetwork.CreateRoom(ProfanityFilter.replaceProfanity(roomInputField.text.ToLower()), new RoomOptions() { MaxPlayers = 4, BroadcastPropsChangeToAll = true, PublishUserId = true });
        }
        else
        {
            CreateErrorPopup("Please enter a room name");
        }
    }

    public void OnClickJoin()
    {
        if (joinInputField.text.Length >= 1)
        {
            PhotonNetwork.JoinRoom(joinInputField.text.ToLower());
        }
        else
        {
            CreateErrorPopup("Please enter a room name");
        }
    }

    public void OnJoinRandom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        FindObjectOfType<LevelLoader>().LoadSceneEffect("StartScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);
        Debug.Log(message);
        CreateErrorPopup(message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log(message);
        CreateErrorPopup(message);
    }

    public void CreateErrorPopup(string message)
    {
        GameObject ep = Instantiate(errorPopup, mainCanvas);
        ep.GetComponent<ErrorPopup>().message = message;
    }

    public override void OnJoinedRoom()
    {
        lobbyParent.SetActive(false);
        roomParent.SetActive(true);
        roomName.text = "Room: " + PhotonNetwork.CurrentRoom.Name;
        UpdatePlayerList();
        if (PhotonNetwork.IsMasterClient)
        {
            roomProperties["isLoading"] = false;
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {

        if (Time.time >= nextUpdateTime)
        {
            Debug.Log("Room list updating");
            //UpdateRoomList(roomList);
            nextUpdateTime = Time.time + timeBetweenUpdates;
        }
        else
        {
            Debug.Log("Room list failed to update due to time");
        }
        
    }

    void UpdateRoomList(List<RoomInfo> list)
    {
        foreach(RoomItem item in roomItemList)
        {
            Destroy(item.gameObject);
        }
        roomItemList.Clear();

        foreach (RoomInfo room in list)
        {
            RoomItem newRoom = Instantiate(roomItemPrefab, roomListContent.gameObject.transform);
            newRoom.SetRoomName(room.Name);
            roomItemList.Add(newRoom);
        }
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        roomParent.SetActive(false);
        lobbyParent.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    void UpdatePlayerList()
    {
        foreach (PlayerItem item in playerItemsList)
        {
            Destroy(item.gameObject);
        }
        playerItemsList.Clear();

        if (PhotonNetwork.CurrentRoom == null)
        {
            return;
        }

        foreach (KeyValuePair<int, Player> player in PhotonNetwork.CurrentRoom.Players)
        {
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerItemParent);
            newPlayerItem.SetPlayerInfo(player.Value);

            
            if (player.Value == PhotonNetwork.LocalPlayer)
            {
                newPlayerItem.ApplyLocalChanges();
            }
            

            playerItemsList.Add(newPlayerItem);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
    }

    public void OnClickPlayButton()
    {
        roomProperties["isLoading"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        levelLoader.LoadPhotonLevel("MultiplayerLevel");
    }

    public void ClickSound()
    {
        AudioManager.instance.PlaySoundOneShot("Click");
    }

    // Update is called once per frame
    private void Update()
    {
        //Start Scene Stuff
        clouds.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(0, 3000f, cloudTime), 0);
        cloudTime += cloudSpeed * Time.deltaTime;

        if (cloudTime > 1)
        {
            cloudTime = 0;
        }

        if (roomListContent.gameObject.transform.childCount > 0)
        {
            roomListContent.sizeDelta = new Vector2(roomListContent.sizeDelta.x, roomListContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.top + roomListContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom + (roomListContent.gameObject.transform.childCount * (roomListContent.gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta.y + roomListContent.gameObject.GetComponent<VerticalLayoutGroup>().spacing)));
        }
        else
        {
            roomListContent.sizeDelta = new Vector2(roomListContent.sizeDelta.x, roomListContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.top + roomListContent.gameObject.GetComponent<VerticalLayoutGroup>().padding.bottom);
        }

        //Photon Stuff
        if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount >= minPlayers)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }

        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.CurrentRoom.CustomProperties["isLoading"] != null && (bool)PhotonNetwork.CurrentRoom.CustomProperties["isLoading"])
            {
                waitingText.text = "Loading match";
            }
            else if (PhotonNetwork.CurrentRoom.PlayerCount < minPlayers)
            {
                waitingText.text = "Waiting for more players to join";
            }
            else if (PhotonNetwork.IsMasterClient)
            {
                waitingText.text = "The game is ready to start";
            }
            else
            {
                waitingText.text = "Waiting for " + PhotonNetwork.MasterClient.NickName + " to start the game";
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("isLoading"))
            {
                leaveButton.interactable = !(bool)PhotonNetwork.CurrentRoom.CustomProperties["isLoading"];
            }
            else
            {
                roomProperties["isLoading"] = false;
                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            }
        }

        
    }

}
