using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class ConnectToServer : MonoBehaviourPunCallbacks
{

    [Header("Start Scene Vars")]
    public GameObject clouds;
    public float cloudSpeed = 0.005f;
    private float cloudTime = 0;
    [Space]
    [Header("Connect To Server Vars")]
    public TMP_InputField usernameInput;
    public TMP_Text buttonText;

    public void OnClickConnect()
    {
        if (usernameInput.text.Length >= 1)
        {
            PhotonNetwork.NickName = ProfanityFilter.replaceProfanity(usernameInput.text);
            buttonText.text = "Connecting...";
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        FindObjectOfType<LevelLoader>().LoadSceneEffect("Lobby");
    }

    public void ClickSound()
    {
        AudioManager.instance.PlaySoundOneShot("Click");
    }

    // Update is called once per frame
    private void Update()
    {
        clouds.GetComponent<RectTransform>().anchoredPosition = new Vector2(Mathf.Lerp(0, 3000f, cloudTime), 0);
        cloudTime += cloudSpeed * Time.deltaTime;

        if (cloudTime > 1)
        {
            cloudTime = 0;
        }

    }
}
