using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class EliminationMessage : MonoBehaviour
{

    private TMP_Text messageText;
    public string message;
    private Animator anim;
    public float timeAlive;
    public float duration = 5f;
    public PhotonView view;
    public string creatorUserId;

    void Start()
    {
        messageText = GetComponentInChildren<TMP_Text>();
        anim = GetComponent<Animator>();
        if (!view.IsMine || !view.Owner.UserId.Equals(creatorUserId))
        {
            Debug.Log("Disabling elim message");
            gameObject.SetActive(false);
        }
    }

    public void OnClick()
    {
        anim.SetTrigger("End");
        StartCoroutine(DelayedDeath());
    }

    IEnumerator DelayedDeath()
    {
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.Destroy(gameObject);
        //Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        messageText.text = message;
        timeAlive += Time.deltaTime;
        if (FindObjectOfType<EliminationMessage>() != null && FindObjectOfType<EliminationMessage>().timeAlive < timeAlive)
        {
            OnClick();
        }

        if (timeAlive > duration)
        {
            OnClick();
        }
    }
}
