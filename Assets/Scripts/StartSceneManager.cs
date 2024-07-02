using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MyFunctions;
using TMPro;
//using LootLocker.Requests;

public class StartSceneManager : MonoBehaviour
{

    private DataCollector dataCollector;
    public GameObject clouds;
    public float cloudSpeed = 0.1f;
    private float cloudTime = 0;
    public GameObject playMenu;
    public TMP_Text[] timeLabels;
    public TMP_Text[] initialLabels;
    public GameObject[] updateNotices;

    // Start is called before the first frame update
    void Start()
    {
        dataCollector = FindObjectOfType<DataCollector>();
        SetupLootLocker();
        CheckForUpdate();
        EnterLeaderboard();
    }

    [ContextMenu("UpdateVersionNumber", false, 0)]
    public void UpdateVersionNumber()
    {
        /*
        SetupLootLocker();
        LootLockerSDKManager.SubmitScore(Application.version, (int)System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(), 588, (response) =>
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

    private void CheckForUpdate()
    {
        /*
        string output = "ERROR";
        LootLockerSDKManager.GetScoreList(588, 1, (response) =>
        {
            if (response.success)
            {
                foreach (GameObject g in updateNotices)
                {
                    g.SetActive(!Application.version.Equals(response.items[0].member_id));
                }
            }
            else
            {
                Debug.Log("Loot locker Fail");
            }
        });
        */
    }

    public void DeleteData()
    {
        SaveSystem.DeleteData();
    }

    public void ClickSound()
    {
        AudioManager.instance.PlaySoundOneShot("Click");
    }

    public void PlayButton()
    {
        CompareData();
        SaveSystem.SaveData(dataCollector);
        GameData gameData = SaveSystem.LoadData();
        Vector3 lsp;
        lsp.x = gameData.lastSafePos[0];
        lsp.y = gameData.lastSafePos[1];
        lsp.z = gameData.lastSafePos[2];
        if (lsp == Vector3.zero)
        {
            FindObjectOfType<RespawnManager>().RespawnCheckpoint();
        }
        else
        {
            playMenu.SetActive(true);
        }
    }

    private void CompareData()
    {
        GameData gameData = SaveSystem.LoadData();
        if (gameData == null)
        {
            //Debug.Log("No game data found, data will be set to default values");
            ResetOreIDList();
            return;
        }

        if (gameData.playerType != dataCollector.playerType)
        {
            dataCollector.playerType = gameData.playerType;
        }

        if (gameData.activatedIndex != dataCollector.activatedIndex)
        {
            dataCollector.activatedIndex = gameData.activatedIndex;
        }

        if (gameData.highestCheckpoint != dataCollector.highestCheckpoint)
        {
            dataCollector.highestCheckpoint = gameData.highestCheckpoint;
        }

        if (gameData.respawnType != dataCollector.respawnType)
        {
            dataCollector.respawnType = gameData.respawnType;
        }

        if (gameData.lastSafePos != dataCollector.lastSafePos)
        {
            dataCollector.lastSafePos = gameData.lastSafePos;
        }

        if (gameData.unlocked != dataCollector.unlocked)
        {
            dataCollector.unlocked = gameData.unlocked;
        }

        if (gameData.adCredits != dataCollector.adCredits)
        {
            dataCollector.adCredits = gameData.adCredits;
        }

        if (gameData.oreCount != dataCollector.oreCount)
        {
            dataCollector.oreCount = gameData.oreCount;
        }

        if (gameData.unCollectedOreIDs != dataCollector.unCollectedOreIDs)
        {
            dataCollector.unCollectedOreIDs = gameData.unCollectedOreIDs;
            if (dataCollector.unCollectedOreIDs.Length == 0)
            {
                ResetOreIDList();
            }
        }
        else
        {
            ResetOreIDList();
        }

        if (gameData.rawSpeedrunTime != dataCollector.rawSpeedrunTime)
        {
            dataCollector.rawSpeedrunTime = gameData.rawSpeedrunTime;
        }

        if (gameData.speedrunActive != dataCollector.speedrunActive)
        {
            dataCollector.speedrunActive = gameData.speedrunActive;
        }
    }

    private void ResetOreIDList()
    {
        dataCollector.unCollectedOreIDs = new string[1];
        dataCollector.unCollectedOreIDs[0] = "RESET";
    }

    public void ExitStart(string sceneToGoTo)
    {
        CompareData();
        SaveSystem.SaveData(dataCollector);
        FindObjectOfType<LevelLoader>().LoadSceneEffect(sceneToGoTo);
    }

    public Vector3 RawTimeToVector3(float raw)
    {
        float tempRaw = raw;
        float hours = (int)(tempRaw / 3600);
        tempRaw = tempRaw % 3600;

        float minutes = (int)(tempRaw / 60);
        tempRaw = tempRaw % 60;

        float seconds = (int)tempRaw;

        Vector3 st = new Vector4(hours, minutes, seconds);
        return st;
    }

    public string FormatVector3(Vector3 formattedSpeedrunTime)
    {
        string[] stringSpeedrunTime = new string[3];
        stringSpeedrunTime[0] = formattedSpeedrunTime.x.ToString();
        stringSpeedrunTime[1] = formattedSpeedrunTime.y.ToString();
        stringSpeedrunTime[2] = formattedSpeedrunTime.z.ToString();

        for (int i = 0; i < stringSpeedrunTime.Length; i++)
        {
            if (stringSpeedrunTime[i].Length == 1)
            {
                stringSpeedrunTime[i] = "0" + stringSpeedrunTime[i];
            }
        }

        string displayedSpeedrunTime = stringSpeedrunTime[0] + ":" + stringSpeedrunTime[1] + ":" + stringSpeedrunTime[2];
        return displayedSpeedrunTime;
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

    public void EnterLeaderboard()
    {
        /*
        LootLockerSDKManager.GetScoreList(573, timeLabels.Length, (response) =>
        {
            if (response.success)
            {
                LootLockerLeaderboardMember[] scores = response.items;

                for (int i = 0; i < scores.Length; i++)
                {
                    timeLabels[i].text = FormatVector3(RawTimeToVector3(scores[i].score));
                    initialLabels[i].text = scores[i].member_id.Substring(0, 3);
                }

                if (scores.Length < timeLabels.Length)
                {
                    for (int i = scores.Length; i < timeLabels.Length; i++)
                    {
                        timeLabels[i].text = "";
                        initialLabels[i].text = "";
                    }
                }
            }
            else
            {
                Debug.Log("Loot locker Fail");
            }
        });
        */
    }

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
