using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using System;
using TMPro;
//using LootLocker.Requests;

public class SpeedrunTimer : BaseBehaviour
{

    public float rawSpeedrunTime;
    public Vector4 formattedSpeedrunTime;
    public string[] stringSpeedrunTime;
    public string displayedSpeedrunTime;
    [Space]
    public TMP_Text displayText;
    public bool speedrunActive;
    public GameObject endSpeedrunParent;
    public TMP_Text finalDisplayText;
    public TMP_InputField initialInput;
    public bool gameFinished;

    // Start is called before the first frame update
    void Start()
    {
        stringSpeedrunTime = new string[4];
    }

    public Vector4 RawTimeToVector4(float raw)
    {
        float tempRaw = raw;
        float hours = (int)(tempRaw / 3600);
        tempRaw = tempRaw % 3600;

        float minutes = (int)(tempRaw / 60);
        tempRaw = tempRaw % 60;

        float seconds = (int)tempRaw;
        tempRaw = tempRaw - seconds;

        float decimalSeconds = ((float)Math.Round((decimal)tempRaw, 2)) * 100;

        Vector4 st = new Vector4(hours, minutes, seconds, decimalSeconds);
        return st;
    }

    public void StartSpeedrun()
    {
        if (speedrunActive)
        {
            return;
        }
        FindObjectOfType<PauseMenu>().warpIndex = 0;
        FindObjectOfType<PauseMenu>().WarpToCheckpoint();
        speedrunActive = true;
        FindObjectOfType<DataManager>().SaveData();
    }

    public void End()
    {
        if (!speedrunActive)
        {
            FindObjectOfType<DataManager>().SaveData();
            FindObjectOfType<LevelLoader>().LoadSceneEffect("StartScene");
            return;
        }

    }

    private void SetupLootLocker()
    {
        /*
        LootLockerSDKManager.StartSession("Player", (response) =>
        {
            if (response.success)
            {
                Debug.Log("Loot locker Success");
            }
            else
            {
                Debug.Log("Loot locker Fail");
            }
        });
        */
    }

    public void ExitWithInitials()
    {
        /*
        SetupLootLocker();
        string id = ProfanityFilter.replaceProfanity(initialInput.text) + "      ";
        LootLockerSDKManager.SubmitScore(id, (int)rawSpeedrunTime, 573, (response) =>
        {
            if (response.success)
            {
                Debug.Log("Loot locker Success");
            }
            else
            {
                Debug.Log("Loot locker Fail");
            }
        });
        ExitWithoutInitials();
        */
    }

    public void ExitWithoutInitials()
    {
        speedrunActive = false;
        FindObjectOfType<DataManager>().SaveData();
        FindObjectOfType<LevelLoader>().LoadSceneEffect("StartScene");  
    }
    // Update is called once per frame
    void Update()
    {
        endSpeedrunParent.SetActive(speedrunActive);
        if (speedrunActive)
        {
            if (!gameFinished)
            {
                rawSpeedrunTime += time.deltaTime;
            }
            formattedSpeedrunTime = RawTimeToVector4(rawSpeedrunTime);

            stringSpeedrunTime[0] = formattedSpeedrunTime.x.ToString();
            stringSpeedrunTime[1] = formattedSpeedrunTime.y.ToString();
            stringSpeedrunTime[2] = formattedSpeedrunTime.z.ToString();
            stringSpeedrunTime[3] = formattedSpeedrunTime.w.ToString();

            for (int i = 0; i < stringSpeedrunTime.Length; i++)
            {
                if (stringSpeedrunTime[i].Length == 1)
                {
                    stringSpeedrunTime[i] = "0" + stringSpeedrunTime[i];
                }
            }

            displayedSpeedrunTime = stringSpeedrunTime[0] + ":" + stringSpeedrunTime[1] + ":" + stringSpeedrunTime[2] + "." + stringSpeedrunTime[3];
            if (!gameFinished)
            {
                displayText.text = displayedSpeedrunTime;
            }
            else
            {
                displayText.text = "";
            }

            finalDisplayText.text = displayedSpeedrunTime;
        }
        else
        {
            rawSpeedrunTime = 0f;
            displayText.text = "";
        }
    }
}
