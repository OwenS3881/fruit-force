using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using UnityEngine.UI;
using TMPro;
using Cinemachine;

public class BananaPowerupController : BaseBehaviour
{

    //public float cooldownTime = 30f;
    //public float currentCooldown = 0f;
    public float requiredOres = 5;
    public float currentOres;
    private Toggle toggle;
    public Image background;
    private Vector3 camStartPos;
    public float slowness = 50f;

    // Start is called before the first frame update
    void Start()
    {
        currentOres = requiredOres;
        toggle = GetComponent<Toggle>();
        camStartPos = GameObject.FindGameObjectWithTag("MainCameraPositionerBanana").transform.localPosition;
    }

    /*
    IEnumerator Countdown()
    {
      while (true)
      {
        if (currentCooldown > 0)
        {
          yield return time.WaitForSeconds(1f);
          currentCooldown--;
        }
        yield return time.WaitForSeconds(0.00000000000000000000000001f);
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

    public void MoveCam()
    {
      Timekeeper.instance.Clock("Root").localTimeScale = 1f;
      FindObjectOfType<CinemachineConfiner>().m_ConfineScreenEdges = true;
      GameObject.FindWithTag("MainCameraPositionerBanana").transform.localPosition = camStartPos;
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
