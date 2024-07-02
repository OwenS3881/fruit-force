using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragonfruitPowerupController : BaseBehaviour
{

    public float requiredOres = 4;
    public float currentOres;
    private Image background;
    private Button button;
    public float slowness = 25f;

    // Start is called before the first frame update
    void Start()
    {
        currentOres = requiredOres;
        button = GetComponent<Button>();
        background = GetComponent<Image>();
    }

    public void PhotonStart()
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement p in players)
        {
            if (p.view.IsMine)
            {
                p.DoDragonfruitPower();
                return;
            }
        }
        Debug.LogError("Player not found");
    }

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
