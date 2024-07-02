using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using UnityEngine.UI;
using TMPro;

public class CherryPowerupController : BaseBehaviour
{

    //public float cooldownTime = 15f;
    //public float currentCooldown = 0f;
    public float requiredOres = 3;
    public float currentOres;
    private Toggle toggle;
    public Image background;
    public float slowness = 25f;

    // Start is called before the first frame update
    void Start()
    {
      currentOres = requiredOres;
      toggle = GetComponent<Toggle>();
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

    public void Boom()
    {
      GameObject[] bombs = GameObject.FindGameObjectsWithTag("CherryBomb");
      foreach (GameObject b in bombs)
      {
        b.GetComponent<CherryBomb>().Boom();
      }
        AudioManager.instance.PlaySound("CherryBoom");
      currentOres = 0;
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
            toggle.isOn = false;
            toggle.interactable = false;
      }
      else
      {
            background.fillAmount = 1;
            toggle.interactable = true;
            currentOres = Mathf.Clamp(currentOres, 0, requiredOres);
      }
    }
}
