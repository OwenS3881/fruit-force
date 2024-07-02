using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Chronos;
using TMPro;

public class MelonPowerupController : BaseBehaviour
{

    //public float cooldownTime = 15f;
    //public float currentCooldown = 0f;
    public float requiredOres = 3;
    public float currentOres;
    public Image background;
    private Button button;
    public float slowness = 25f;


    // Start is called before the first frame update
    void Start()
    {
      currentOres = requiredOres;
      button = GetComponent<Button>();
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

    public void PhotonStart()
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement p in players)
        {
            if (p.view.IsMine)
            {
                p.DoMelonPower();
                return;
            }
        }
        Debug.LogError("Player not found");
    }

    public void Increase()
    {
        if (currentOres < requiredOres)
        {
            StartCoroutine(IncreaseCo(slowness));
        }
    }

    public void ClickSound()
    {
        AudioManager.instance.PlaySoundOneShot("Click");
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
