using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ErrorPopup : MonoBehaviour
{

    private TMP_Text messageText;
    public string message;
    private Animator anim;
    public float timeAlive;
    public float duration = 5f;

    void Start()
    {
        messageText = GetComponentInChildren<TMP_Text>();
        anim = GetComponent<Animator>();
    }

    public void OnClick()
    {
        anim.SetTrigger("End");
        Destroy(gameObject, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        messageText.text = message;
        timeAlive += Time.deltaTime;
        if (FindObjectOfType<ErrorPopup>() != null && FindObjectOfType<ErrorPopup>().timeAlive < timeAlive)
        {
            OnClick();
        }

        if (timeAlive > duration)
        {
            OnClick();
        }
    }
}
